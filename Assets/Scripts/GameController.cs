using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.XR;
//using UnityEngine.InputSystem;

//responsible for Bluble spawn and score management
public class GameController : MonoBehaviourPunCallbacks
{
    public static GameController Instance;
    //public
    public TextMeshProUGUI scoreText; 
    public TextMeshProUGUI timeText;
    public BlubleDraggable bluble;
    //public GameObject player;
    //public int variant = 1;
    public int deviationZ = 20;
    public int blublesPerRow = 5;
    public AudioSource final;
    public AudioSource gameSound;
    public GameObject pauseButton;
    public GameObject nextButton;
    public GameObject ufo;
    
    //private
    private TextMeshPro roboText;
    private int score = 0;
    private float startTime = 0;
    private float pauseTime = 0;
    private float pauseStart;
    private object[,] words;
    private int blubleCounter = 0;
    private int destroyedBlubles = 0;
    private Coroutine blubleRoutine;
    private GameObject bucketDer;
    private GameObject bucketDie;
    private GameObject bucketDas;
    private GameObject robo;
    public Transform XRig; 
    [SerializeField]
    private bool isPaused = false;
    private float deviationYPlayers = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    public void SetWordStore(){
        switch(PhotonNetwork.CurrentRoom.PlayerCount) {
            case 1: words = new object[13,3] {
                {"Internet","das",null},
                {"Post","die",null},
                {"Com-puter","der",null},
                {"Film","der",null},
                {"Banane","die",null},
                {"Entschul-digung","die",null},
                {"Comic","der",null},
                {"Person","die",null},
                {"Name","der",null},
                {"Beispiel","das",null},
                {"Familien-name","der",null},
                {"Teil","das",null},
                {"Handy","das",null}
            };
            break;
            case 2: words = new object[26,3] {
                {"Internet","das",null},
                {"Post","die",null},
                {"Com-puter","der",null},
                {"Film","der",null},
                {"Banane","die",null},
                {"Entschul-digung","die",null},
                {"Comic","der",null},
                {"Person","die",null},
                {"Name","der",null},
                {"Beispiel","das",null},
                {"Familien-name","der",null},
                {"Teil","das",null},
                {"Handy","das",null},
                {"Antwort","die",null},
                {"Familie","die",null},
                {"Musik","die",null},
                {"Quiz","das",null},
                {"Punkt","der",null},
                {"Schau-spieler","der",null},
                {"Sprache","die",null},
                {"Ausland","das",null},
                {"Sache","die",null},
                {"E-Mail","die",null},
                {"Tag","der",null},
                {"Fern-seher","der",null},
                {"Problem","das",null}
            };
            break; 
            case 3: words = new object[39,3] {
                {"Internet","das",null},
                {"Post","die",null},
                {"Com-puter","der",null},
                {"Film","der",null},
                {"Banane","die",null},
                {"Entschul-digung","die",null},
                {"Comic","der",null},
                {"Person","die",null},
                {"Name","der",null},
                {"Beispiel","das",null},
                {"Familien-name","der",null},
                {"Teil","das",null},
                {"Handy","das",null},
                {"Antwort","die",null},
                {"Familie","die",null},
                {"Musik","die",null},
                {"Quiz","das",null},
                {"Punkt","der",null},
                {"Schau-spieler","der",null},
                {"Sprache","die",null},
                {"Ausland","das",null},
                {"Sache","die",null},
                {"E-Mail","die",null},
                {"Tag","der",null},
                {"Fern-seher","der",null},
                {"Problem","das",null},
                {"Blume","die",null},
                {"Fahrrad","das",null},
                {"Hose","die",null},
                {"Klavier","das",null},
                {"Kühl-schrank","der",null},
                {"Schrank","der",null},
                {"Spiel","das",null},
                {"Ding","das",null},
                {"Brief-marke","die",null},
                {"Lebens-mittel","das",null},
                {"Urlaub","der",null},
                {"Sport","der",null},
                {"Wohn-ung","die",null}
            };
            break;
        }
    }

    // Update is called once per frame
    void Update(){ 
        if(timeText != null && startTime != 0 && (destroyedBlubles <  words.GetLength(0)) && !isPaused){
            //float t = Time.time - startTime;
            TimeSpan t = TimeSpan.FromSeconds(Time.time - startTime - pauseTime);
            //if(timeText.text != TimeSpan.FromSeconds(t).ToString("mm:ss"))
                timeText.text = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds); //.ToString("D3")
        }
    }

    private void UpdateScore(){
        scoreText.text = score.ToString("D2");
        roboText.text = score.ToString("D2");
    }

    public bool GetIsPaused(){
        return isPaused;
    }

    public int GetScore() {
        return score;
    }

    public GameObject GetRobo(){
        return robo;
    }

    public void SetBucketDer(GameObject bucket) {
       bucketDer = bucket;
    }

    public void SetBucketDie(GameObject bucket) {
       bucketDie = bucket;
    }

    public void SetBucketDas(GameObject bucket) {
       bucketDas = bucket;
    }

    public void SetRobo(TextMeshPro tmp, GameObject rob){
        roboText = tmp;
        robo = rob;
    }

    public void SetStartTime(float t){
        startTime = t;
    }

    public void MakeSound(){
        gameSound.Play();
    }

    public void ReviewTutorial(){
        bucketDer.SetActive(false);
        bucketDas.SetActive(false);
        bucketDie.SetActive(false);
        robo.SetActive(false);
    }

    public void PauseGame(){
        if(isPaused){
            if(pauseStart != 0)
                pauseTime += Time.time - pauseStart;
            pauseStart = 0;
            isPaused = false;
            blubleRoutine = StartCoroutine(BlubleCreator(5));
            ufo.GetComponent<UfoMovement>().enabled = true;
            robo.GetComponent<RoboMovement>().enabled = true;
            gameSound.Play();

            //Disable buckets and show bubbles
            Rigidbody rb = bucketDer.GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
            rb = bucketDie.GetComponent<Rigidbody>();
            int LayerIgnoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
            bucketDer.layer = LayerIgnoreRaycast;
            rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
            rb = bucketDas.GetComponent<Rigidbody>();
            bucketDie.layer = LayerIgnoreRaycast;
            rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
            bucketDas.layer = LayerIgnoreRaycast;
            bucketDer.GetComponent<Draggable>().enabled = false;
            bucketDie.GetComponent<Draggable>().enabled = false;
            bucketDas.GetComponent<Draggable>().enabled = false;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
        } else {
            isPaused = true;
            pauseStart = Time.time;
            StopCoroutine(blubleRoutine);
            ufo.GetComponent<UfoMovement>().enabled = false;
            robo.GetComponent<RoboMovement>().enabled = false;
            gameSound.Stop();

            //Enable buckets and hide bubbles
            Rigidbody rb = bucketDer.GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            int LayerIgnoreRaycast = LayerMask.NameToLayer("Default");
            bucketDer.layer = LayerIgnoreRaycast;
            rb = bucketDie.GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            bucketDie.layer = LayerIgnoreRaycast;
            rb = bucketDas.GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            bucketDas.layer = LayerIgnoreRaycast;
            bucketDer.GetComponent<Draggable>().enabled = true;
            bucketDie.GetComponent<Draggable>().enabled = true;
            bucketDas.GetComponent<Draggable>().enabled = true;
            if(PhotonNetwork.IsMasterClient){
                foreach (Transform child in transform)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }
    }

    public void FirstBluble(){
        blubleRoutine = StartCoroutine(BlubleCreator(0));
    }

    public IEnumerator BlubleCreator(float time){
        yield return new WaitForSeconds(time);
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
            float x = UnityEngine.Random.Range(-0.5f*PhotonNetwork.CurrentRoom.PlayerCount, 0.5f*PhotonNetwork.CurrentRoom.PlayerCount);
            float initalY = Camera.main.transform.position.y+UnityEngine.Random.Range(0.7f,1);
            Vector3 position = new Vector3(Camera.main.transform.position.x+x, initalY, Camera.main.transform.position.z+deviationZ);
            BlubleDraggable currentBluble = PhotonNetwork.InstantiateSceneObject("bluble", position, Camera.main.transform.rotation, 0).GetComponent<BlubleDraggable>();
            //Event mitteilen PhotonView.RPC > RaiseEvent
            currentBluble.transform.parent = this.transform;
            currentBluble.gameObject.name = "Bluble " + (string)words[index,0];
            currentBluble.SetInitialY(initalY); //make sure the random factor stays constant
            currentBluble.SetDeviationX(x);
            currentBluble.emergingBluble.Play();
            currentBluble.GetComponentInChildren<TextMeshPro>().text = (string)words[index,0];
            currentBluble.tag = (string)words[index,1];
            GameObject go = currentBluble.transform.gameObject;
            AudioSource audio = go.AddComponent<AudioSource>() as AudioSource;
            audio.clip = Resources.Load("Audio/" + (string)words[index,0]) as AudioClip;
            audio.playOnAwake = false;
            audio.priority = 0;
            //Debug.Log(words[index,0] + ", " + words[index,1]);
            //Initiate next bluble
            blubleRoutine = StartCoroutine(BlubleCreator(5.0f/PhotonNetwork.CurrentRoom.PlayerCount));
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
            //PhotonView photonView = this.GetComponent<PhotonView>();
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
        UpdateScore();
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
        bucketDer.SetActive(false);
        bucketDie.SetActive(false);
        bucketDas.SetActive(false);
        pauseButton.SetActive(false);
        //quitButton.transform.position = new Vector3(-0.5f, 0.267f, -10.68f);
        nextButton.SetActive(true);
        robo.SetActive(false);
       
        //Reposition Counter
        //scoreText.transform.position = new Vector3(player.transform.position.x-0.49f, scoreText.transform.position.y-0.07f, scoreText.transform.position.z);

        //Show Blubles again
        float xSteps = 6/blublesPerRow;
        float x = -1.5f - xSteps;
        float ySteps = 2/(words.GetLength(0)/blublesPerRow);
        float y = 1.4f;
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
            Vector3 position = new Vector3(Camera.main.transform.position.x+(x-xSteps/2), Camera.main.transform.position.y + (y+ySteps/2), Camera.main.transform.position.z+6.5f);
            BlubleDraggable currentBluble = PhotonNetwork.Instantiate("bluble", position, Quaternion.identity, 0).GetComponent<BlubleDraggable>();
            currentBluble.transform.parent = this.transform;
            currentBluble.gameObject.name = "Bluble " + (string)words[i,0];
            currentBluble.GetComponentInChildren<TextMeshPro>().text = (string)words[i,1] + " " + (string)words[i,0];
            int LayerIgnoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
            currentBluble.gameObject.layer = LayerIgnoreRaycast;
            //currentBluble.SetIsHit(true);
            currentBluble.SetInitialY(currentBluble.transform.position.y);
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

    public void QuitGame(){
        //Stop bluble generation & sound
        StopCoroutine(blubleRoutine);
        gameSound.Stop();  

        //Remove all blubles
        if(PhotonNetwork.IsMasterClient){
            foreach (Transform child in transform)
            {
                PhotonNetwork.Destroy(child.gameObject);
            }
        }

        //Reset values
        score = 0;
        UpdateScore();
        startTime = 0.0f;
        pauseTime = 0.0f;
        pauseStart = 0;
        blubleCounter = 0;
        destroyedBlubles = 0;
        isPaused = false;
        words = null;
    }
}
