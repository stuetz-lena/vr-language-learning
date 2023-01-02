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
    //public TextMeshPro scoreText; 
    public BlubleDraggable bluble;
    //public GameObject player;
    [HideInInspector]
    public int variant = 1;
    public int deviationZ = 20;
    public int blublesPerRow = 5;
    public AudioSource final;
    
    //private
    private int score = 0;
    private object[,] words;
    private int blubleCounter = 0;
    private int destroyedBlubles = 0;
    private Coroutine blubleRoutine;
    private GameObject bucket_der;
    private GameObject bucket_die;
    private GameObject bucket_das;
    public Transform XRig; 
    
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
    }

    // Update is called once per frame
    void Update(){ }

    /*private void UpdateScore(){
        scoreText.text = score.ToString("D3");
    }*/

    public Quaternion GetRotation() {
        return XRig.rotation;
    }
    
    public float getXRigX() {
        return XRig.position.x;
    }

    public int GetScore() {
        return score;
    }

    public void SetBucketDer(GameObject bucket) {
       bucket_der = bucket;
    }

    public void SetBucketDie(GameObject bucket) {
       bucket_die = bucket;
    }

    public void SetBucketDas(GameObject bucket) {
       bucket_das = bucket;
    }

    public void SetTransform(Transform rig) {
       XRig = rig;
    }

    public void FirstBluble(){
        blubleRoutine = StartCoroutine(BlubleCreator(0));
    }

    public IEnumerator BlubleCreator(int time){
        yield return new WaitForSeconds(time);
        //PhotonView photonView = this.GetComponent<PhotonView>();
        //photonView.RPC("CreateBluble", RpcTarget.All);
        CreateBluble();
    }

    [PunRPC]
    private void CreateBluble() {
        //Randomize Order
        int index = UnityEngine.Random.Range(0, words.GetLength(0));
        if(words[index,2] == null) {
            blubleCounter++;
            words[index,2] = false;
            //Create new bluble
            float x = UnityEngine.Random.Range(-0.5f, 0.5f);
            float initalY = XRig.transform.position.y+UnityEngine.Random.Range(0.7f,1);
            Vector3 position = new Vector3(XRig.transform.position.x+x, initalY, XRig.transform.position.z+deviationZ);
            BlubleDraggable currentBluble = PhotonNetwork.InstantiateSceneObject("bluble", position, XRig.transform.rotation, 0).GetComponent<BlubleDraggable>();
            //Event mitteilen PhotonView.RPC > RaiseEvent
            currentBluble.transform.parent = this.transform;
            currentBluble.gameObject.name = "Bluble " + (string)words[index,0];
            currentBluble.SetInitialY(initalY); //make sure the random factor stays constant
            currentBluble.SetDeviationX(x);
            currentBluble.emergingBluble.Play();
            currentBluble.GetComponentInChildren<TextMeshPro>().text = (string)words[index,0];
            currentBluble.SetGameController(this);
            currentBluble.tag = (string)words[index,1];
            GameObject go = currentBluble.transform.gameObject;
            AudioSource audio = go.AddComponent<AudioSource>() as AudioSource;
            audio.clip = Resources.Load("Audio/" + (string)words[index,0]) as AudioClip;
            audio.playOnAwake = false;
            audio.priority = 0;
            //Debug.Log(words[index,0] + ", " + words[index,1]);
            //Initiate next bluble
            blubleRoutine = StartCoroutine(BlubleCreator(5));
        } else if(blubleCounter < words.GetLength(0)) { //if bluble already created start again, until all are finished
            CreateBluble();
            //photonView.RPC("CreateBluble", RpcTarget.All);
        }
    } 

    public void Fail(string word = "false", string bucket = "false"){
        //if wrong sorting, save in array
        if(!String.Equals(word, "false") && !String.Equals(bucket, "false")){
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
            //destroyedBlubles++;
            blubleCounter--;
            for(int i = 0; i < words.GetLength(0); i++) {
                if(String.Equals(words[i,0],word)){
                    words[i,2] = null;
                    break;
                }
            }
            CheckForTie();
        }
    }

    //Check if all blubles are gone, so we need to create a new one or if game is finished
    public void CheckForTie(){
        if(destroyedBlubles ==  words.GetLength(0)){
            Debug.Log("End");
            PhotonView photonView = this.GetComponent<PhotonView>();
            photonView.RPC("ShowResults", RpcTarget.All);
            //ShowResults();
        } else if(blubleCounter == destroyedBlubles){
            StopCoroutine(blubleRoutine); //have only one Coroutine at a time
            blubleRoutine = StartCoroutine(BlubleCreator(0));
        }
    }

    //Increase score and write to array
    [PunRPC]
    public void Congrats(int points, string word){
        score += points;
        destroyedBlubles++;
        //UpdateScore();
        for(int i = 0; i < words.GetLength(0); i++) {
            if(String.Equals(words[i,0],word)){
                words[i,2] = true;
                break;
            }
        }
        CheckForTie();
    }

    [PunRPC]
    public void ShowResults(){
        //Hide buckets & play sound
        if(final)
            final.Play();
        bucket_der.SetActive(false);
        bucket_die.SetActive(false);
        bucket_das.SetActive(false);

        //Reposition Counter
        //scoreText.transform.position = new Vector3(player.transform.position.x-0.49f, scoreText.transform.position.y-0.07f, scoreText.transform.position.z);

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
            Vector3 position = new Vector3(XRig.position.x+(x-xSteps/2), XRig.position.y + (y+ySteps/2), XRig.position.z+5.5f);
            BlubleDraggable currentBluble = PhotonNetwork.Instantiate("bluble", position, XRig.rotation, 0).GetComponent<BlubleDraggable>();
            currentBluble.transform.parent = this.transform;
            currentBluble.gameObject.name = "Bluble " + (string)words[i,0];
            currentBluble.GetComponentInChildren<TextMeshPro>().text = (string)words[i,1] + " " + (string)words[i,0];
            currentBluble.isHit = true;
            Rigidbody rb = currentBluble.GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
            currentBluble.GetComponent<BlubleDraggable>().enabled = false;
            //Debug.Log(words[i,2].ToString());
            if(String.Equals(words[i,2].ToString(), "True")){
                currentBluble.GetComponent<Renderer>().material = currentBluble.green;
            } else if (!String.Equals(words[i,2].ToString(), "False")) {
                currentBluble.GetComponent<Renderer>().material = currentBluble.red;
            }   
        }
    }
}
