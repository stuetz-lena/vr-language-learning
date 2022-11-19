using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

//responsible for Bluble spawn and score management
public class GameController : MonoBehaviourPunCallbacks
{
    //public
    public TextMeshPro scoreText; 
    public BlubleDraggable bluble;
    public GameObject player;
    [HideInInspector]
    public int playerNr = 0;
    public int variant = 1;
    public int deviationZ = 20;
    public int blublesPerRow = 5;
    public GameObject bucket_der;
    public GameObject bucket_die;
    public GameObject bucket_das;
    public AudioSource final;
    
    //private
    private int score = 0;
    private object[,] words;
    private int blubleCounter = 0;
    private int destroyedBlubles = 0;
    private int numberOfBlubleCoroutines = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        //Start bluble Generation > now in NetworkManager
        //StartCoroutine(BlubleCreator(0));

        //Set words to one of the three variations
        switch(variant) {
            case 1: words = new object[13,3] {
                {"Internet","das",null},
                {"Post","die",null},
                {"Computer","der",null},
                {"Film","der",null},
                {"Banane","die",null},
                {"Entschuldigung","die",null},
                {"Comic","der",null},
                {"Person","die",null},
                {"Name","der",null},
                {"Beispiel","das",null},
                {"Familienname","der",null},
                {"Teil","das",null},
                {"Handy","das",null}
            };
            break;
            case 2: words = new object[13,3] {
                {"Antwort","die",null},
                {"Familie","die",null},
                {"Musik","die",null},
                {"Quiz","das",null},
                {"Punkt","der",null},
                {"Schauspieler","der",null},
                {"Sprache","die",null},
                {"Ausland","das",null},
                {"Sache","die",null},
                {"E-Mail","die",null},
                {"Tag","der",null},
                {"Fernseher","der",null},
                {"Problem","das",null}
            };
            break; 
            case 3: words = new object[13,3] {
                {"Blume","die",null},
                {"Fahrrad","das",null},
                {"Hose","die",null},
                {"Klavier","das",null},
                {"Kühlschrank","der",null},
                {"Schrank","der",null},
                {"Spiel","das",null},
                {"Ding","das",null},
                {"Briefmarke","die",null},
                {"Lebensmittel","das",null},
                {"Urlaub","der",null},
                {"Sport","der",null},
                {"Wohnung","die",null}
            };
            break;
        }
        //Set PlayerNr > now in NetworkManager
        //StartCoroutine(GetPlayerNr(2));
    }

    // Update is called once per frame
    void Update(){ }

    public void Fail(string word = "false", string bucket = "false"){
        //if wrong sorting, save in array
        if(!String.Equals(word, "false")){
            destroyedBlubles++;
            for(int i = 0; i < words.GetLength(0); i++) {
                if(String.Equals(words[i,0],word)){
                    words[i,2] = bucket;
                    break;
                }
            }
            CheckForTie();
        } else {
            //otherwise just increase counter
            destroyedBlubles++;
            CheckForTie();
        }
    }

    //Check if all blubles are gone, so we need to create a new one or if game is finished
    public void CheckForTie(){
        if(destroyedBlubles ==  words.GetLength(0)){
            Debug.Log("End");
            ShowResults();
        } else if(blubleCounter == destroyedBlubles){
            StartCoroutine(BlubleCreator(0));
        }
    }

    //Increase score and write to array
    public void Congrats(int points, string word){
        score += points;
        destroyedBlubles++;
        UpdateScore();
        for(int i = 0; i < words.GetLength(0); i++) {
            if(String.Equals(words[i,0],word)){
                words[i,2] = true;
                break;
            }
        }
        CheckForTie();
    }

    public void ShowResults(){
        //Hide buckets & play sound
        if(final)
            final.Play();
        bucket_der.SetActive(false);
        bucket_die.SetActive(false);
        bucket_das.SetActive(false);

        //Reposition Counter
        scoreText.transform.position = new Vector3(player.transform.position.x-0.49f, scoreText.transform.position.y-0.07f, scoreText.transform.position.z);

        //Show Blubles again
        float xSteps = 6/blublesPerRow;
        float x = -1.5f - xSteps;
        float ySteps = 2/(words.GetLength(0)/blublesPerRow);
        float y = 2.2f;
        float rowCounter = 0; 
        for (int i = 0; i < words.GetLength(0); i++){
            //float x = UnityEngine.Random.Range(-3, 3);
            //float initalY = player.transform.position.y+UnityEngine.Random.Range(0.4f,1);
            //float z = UnityEngine.Random.Range(2, 7);
            if(rowCounter == blublesPerRow) {
                y = y - ySteps;
                rowCounter = 0;
                x = -1.5f - xSteps;
            }
            x = x + xSteps;
            rowCounter++;
            Vector3 position = new Vector3(player.transform.position.x+(x-xSteps/2), player.transform.position.y + (y+ySteps/2), player.transform.position.z+5.5f);
            BlubleDraggable currentBluble = Instantiate(bluble, position, player.transform.rotation, this.transform);
            currentBluble.gameObject.name = "Bluble " + (string)words[i,0];
            currentBluble.GetComponentInChildren<TextMeshPro>().text = (string)words[i,1] + " " + (string)words[i,0];
            currentBluble.isHit = true;
            //Debug.Log(words[i,2].ToString());
            if(String.Equals(words[i,2].ToString(), "True")){
                currentBluble.GetComponent<Renderer>().material = currentBluble.green;
            } else if (!String.Equals(words[i,2].ToString(), "False")) {
                currentBluble.GetComponent<Renderer>().material = currentBluble.red;
            }   
        }
    }

    private void UpdateScore(){
        scoreText.text = score.ToString("D3");
    }

    public float GetPlayerX() {
        return player.transform.position.x;
    }

    public IEnumerator BlubleCreator(int time){
        numberOfBlubleCoroutines++; //trying to have only one Coroutine at a time
        yield return new WaitForSeconds(time);
        CreateBluble();
    }
    /*IEnumerator GetPlayerNr(int time){
        yield return new WaitForSeconds(time);
        if (PhotonNetwork.InRoom)
            playerNr = PhotonNetwork.LocalPlayer.ActorNumber;
            //Debug.Log("Player-Nr: " + playerNr);
    }*/
    private void CreateBluble() {
        //Randomize Order
        int index = UnityEngine.Random.Range(0, words.GetLength(0));
        if(words[index,2] == null) {
            blubleCounter++;
            words[index,2] = false;
            //Create new bluble
            float x = UnityEngine.Random.Range(-0.5f, 0.5f);
            float initalY = player.transform.position.y+UnityEngine.Random.Range(0.7f,1);
            Vector3 position = new Vector3(player.transform.position.x+x, initalY, player.transform.position.z+deviationZ);
            BlubleDraggable currentBluble = Instantiate(bluble, position, player.transform.rotation, this.transform);
            currentBluble.gameObject.name = "Bluble " + (string)words[index,0];
            currentBluble.initialY = initalY; //make sure the random factor stays constant
            currentBluble.deviationX = x;
            currentBluble.emergingBluble.Play();
            currentBluble.GetComponentInChildren<TextMeshPro>().text = (string)words[index,0];
            currentBluble.SetGameController(this);
            currentBluble.tag = (string)words[index,1];
            //Debug.Log(words[index,0] + ", " + words[index,1]);
            //Initiate next bluble
            numberOfBlubleCoroutines--;
            if(numberOfBlubleCoroutines == 0){ //trying to avoid two parallel coroutines
                StartCoroutine(BlubleCreator(5));
            }
        } else if(blubleCounter < words.GetLength(0)) { //if bluble already created start again, until all are finished
            CreateBluble();
        }
    } 
}
