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
    public static GameController Instance;

    [Tooltip("TextMeshProUGUI element for the score in HUD.")]
    public TextMeshProUGUI scoreText; 
    [Tooltip("TextMeshProUGUI element for the time in HUD.")]
    public TextMeshProUGUI timeText;

    [Tooltip("Distance from camera for the bubbles to spawn initally.")]
    public int blubleDeviationZ = 20;
    [Tooltip("How long to wait between two bubbles for a single player.")]
    public float emergingBaseSpeed = 5.0f; 
    [Tooltip("Half range for horizontal deviation for new bubbles for a single player.")]
    public float deviationBaseX = 0.5f;
    [Tooltip("Lower position for vertical deviation for new bubbles.")]
    public float deviationYFrom = 0.7f;
    [Tooltip("Upper position for vertical deviation for new bubbles.")]
    public float deviationYTo = 1.0f;
    [Tooltip("Amount of bubbles per row during result view for single user.")]
    public int blublesPerRow = 5;
    [Tooltip("Bubble deviation from camera in result view.")]
    public float resultDeviationZ = 6.5f;
    [Tooltip("Start position for x for bubble grid in result view.")]
    public float resultStartPosX = 0.4f; //-0.4f 1.824 0
    [Tooltip("Start position for y for bubble grid in result view.")]
    public float resultStartPosY = 0.8f; //-0.245 0.25

    [Tooltip("AudioSource to be played during the game mode.")]
    public AudioSource gameSound;
    [Tooltip("AudioSource to be played during the result view.")]
    public AudioSource final;
    

    private TextMeshPro roboText;
    private int score = 0;
    private float startTime = 0;
    private float pauseStart = 0;
    private float pauseTime = 0;

    private object[,] words;
    private int blubleCounter = 0; //number of created bubbles
    private int destroyedBlubles = 0;
    private Coroutine blubleRoutine;
    
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update(){ 
        //if a text element is available, the start time was set, the game is not finished yet or currently paused, update the timer
        if(timeText != null && startTime != 0 && (destroyedBlubles <  words.GetLength(0)) && !NetworkManager.Instance.GetIsPaused()){
            TimeSpan t = TimeSpan.FromSeconds(Time.time - startTime - pauseTime); //substract start time (after pushing the button) and pause time from the current time
            if(timeText.text != string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds)) //change in UI if necessary
                timeText.text = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
        }
    }

    public int GetScore() {
        return score;
    }

    public void SetRoboText(TextMeshPro roboTmp){
        roboText = roboTmp;
    }

    public void SetStartTime(float t){
        startTime = t;
    }

    public void SetWordStore(){ //adjust word amout to player amout
        switch(PhotonNetwork.CurrentRoom.PlayerCount) {
            case 1: words = new object[13,3] {
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
            default: words = new object[39,3] {
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

    private void UpdateScore(){
        scoreText.text = score.ToString("D2"); //format: 00
        roboText.text = score.ToString("D2");
    }

    public void MakeSound(){
        gameSound.Play();
    }

    public void FirstBluble(){ //MasterONLYFunction
        blubleRoutine = StartCoroutine(BlubleCreator(0));
    }

    IEnumerator BlubleCreator(float time){ //MasterONLYFunction
        yield return new WaitForSeconds(time); //wait between bubbles
        CreateBluble();
    }

    void CreateBluble() { //MasterONLYFunction
        int index = UnityEngine.Random.Range(0, words.GetLength(0)); //Randomize word order
        if(words[index,2] == null) { //if the bubble does not exist yet
            //Create new bubble
            float deviationX = UnityEngine.Random.Range(-1 * deviationBaseX * PhotonNetwork.CurrentRoom.PlayerCount, deviationBaseX*PhotonNetwork.CurrentRoom.PlayerCount); //spawn breadth depends on player amount
            float initalY = Camera.main.transform.position.y + UnityEngine.Random.Range(deviationYFrom, deviationYTo);
            Vector3 position = new Vector3(Camera.main.transform.position.x + deviationX, initalY, Camera.main.transform.position.z + blubleDeviationZ);
            BlubleDraggable currentBluble = PhotonNetwork.InstantiateSceneObject("bluble", position, Camera.main.transform.rotation, 0).GetComponent<BlubleDraggable>();
            photonView.RPC("SetUpBuble", RpcTarget.All, currentBluble.GetComponent<PhotonView>().ViewID, index, initalY, deviationX);
            
            //Initiate next bluble
            blubleRoutine = StartCoroutine(BlubleCreator(emergingBaseSpeed/PhotonNetwork.CurrentRoom.PlayerCount); //time to wait for a new bubble is depends on player amount
        } else if(blubleCounter < words.GetLength(0)) { //if the bubble was already created, try a new one
            CreateBluble();
        }
    } 

    [PunRPC]
    void SetUpBuble(int viewID, int index, float y, float x){
        blubleCounter++; 
        words[index,2] = false; //not sorted yet

        BlubleDraggable currentBluble = PhotonView.Find(viewID).gameObject.GetComponent<BlubleDraggable>(); //get the current bubble

        currentBluble.transform.parent = this.transform;
        currentBluble.gameObject.name = "Bluble " + (string)words[index,0]; //name in scene

        currentBluble.SetInitialY(y); //make sure the random factors stay constant
        currentBluble.SetDeviationX(x);

        currentBluble.GetComponentInChildren<TextMeshPro>().text = (string)words[index,0]; //set text
        currentBluble.tag = (string)words[index,1]; //set tag

        //Set the word audio
        AudioSource audio = currentBluble.gameObject.AddComponent<AudioSource>() as AudioSource; 
        audio.clip = Resources.Load("Audio/" + (string)words[index,0]) as AudioClip;
        audio.playOnAwake = false;
        audio.priority = 0;
        currentBluble.SetWordSource(audio);
    }

    public void Congrats(int points, string word){ //correct sorting, increase score and write to array
        score += points;
        UpdateScore();
        destroyedBlubles++;
        for(int i = 0; i < words.GetLength(0); i++) {
            if(String.Equals(words[i,0], word)){
                words[i,2] = true; //save the correct sorting
                break;
            }
        }
        CheckForTie();
    }

    public void Fail(string word = "false", string bucket = "false"){ //wrong sorting or game floor overflow, save to array
        if(!String.Equals(word, "false") && !String.Equals(bucket, "false")){ //if wrong sorting
            destroyedBlubles++;
            for(int i = 0; i < words.GetLength(0); i++) {
                if(String.Equals(words[i,0],word)){
                    words[i,2] = bucket; //save the chosen bucket
                    break;
                }
            }
        } else { //bubble went out of game floor and disappeared - it needs to show up again
            blubleCounter--;
            for(int i = 0; i < words.GetLength(0); i++) {
                if(String.Equals(words[i,0],word)){
                    words[i,2] = null; //reset array for the item
                    break;
                }
            }
        }
        CheckForTie();
    }

    void CheckForTie(){
        //Check if all blubles are gone, so we need to create a new one now or if the game is finished
        if(destroyedBlubles == words.GetLength(0)){
            Debug.Log("End");
            ShowResults();
            //photonView.RPC("ShowResults", RpcTarget.All);
        } else if(blubleCounter == destroyedBlubles && PhotonNetwork.IsMasterClient){
            if(blubleRoutine != null)
                StopCoroutine(blubleRoutine); //have only one routine running at a time
            blubleRoutine = StartCoroutine(BlubleCreator(0)); //new routine with zero delay
        }
    }

    [PunRPC]
    void ShowResults(){
        //Hide buckets, adjust UI & play sound
        NetworkManager.Instance.TriggerGameObjects(false);
        UserInterface.Instance.TriggerPauseButton(false);
        UserInterface.Instance.TriggerNextButton(true);
        if(final)
            final.Play();
        
        //Show bubbles again, spawned only by master
        if(PhotonNetwork.IsMasterClient){
            //calculate grid
            int perRow = blublesPerRow*PhotonNetwork.CurrentRoom.PlayerCount;
            float xSteps = (perRow+PhotonNetwork.CurrentRoom.PlayerCount)/perRow;
            float x = resultStartPosX - PhotonNetwork.CurrentRoom.PlayerCount * 1.1f - xSteps;
            float ySteps = 2/(words.GetLength(0)/perRow);
            float y = resultStartPosY;
            float rowCounter = 0; 
            for (int i = 0; i < words.GetLength(0); i++){
                if(rowCounter == perRow) {
                    y = y - ySteps;
                    rowCounter = 0;
                    x = resultStartPosX - PhotonNetwork.CurrentRoom.PlayerCount * 1.1f - xSteps;
                }
                x = x + xSteps;
                rowCounter++;
                Vector3 position = new Vector3(Camera.main.transform.position.x + (x-xSteps/2), Camera.main.transform.position.y + (y+ySteps/2), Camera.main.transform.position.z + resultDeviationZ);
                BlubleDraggable currentBluble = PhotonNetwork.InstantiateSceneObject("bluble", position, Quaternion.identity, 0).GetComponent<BlubleDraggable>(); 
                photonView.RPC("SetUpResultBubbles", RpcTarget.All, currentBluble.GetComponent<PhotonView>().ViewID, i, (string)words[i,0], (string)words[i,1], words[i,2].ToString());
            }
        }
    }

    [PunRPC]
    void SetUpResultBubbles(int viewID, int i, string word0, string word1, string word2){
        /*if(!PhotonNetwork.IsMasterClient)
            words = word;*/
        BlubleDraggable currentBluble = PhotonView.Find(viewID).gameObject.GetComponent<BlubleDraggable>(); //get the current bubble
        currentBluble.transform.parent = this.transform;
        currentBluble.gameObject.name = "Bluble " + word0;

        //Show the right result and change bubble color based on correct/wrong sorting
        currentBluble.GetComponentInChildren<TextMeshPro>().text = word1 + " " + word0;
        Debug.Log(word2);
        if(String.Equals(word2, "True")){
            currentBluble.GetComponent<Renderer>().material = currentBluble.GetGreen();
        } else if (!String.Equals(word2, "False")) {
            currentBluble.GetComponent<Renderer>().material = currentBluble.GetRed();
        }  

        //Disable any movement or selection
        currentBluble.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        //currentBluble.SetInitialY(currentBluble.transform.position.y);
        currentBluble.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
        currentBluble.GetComponent<BlubleDraggable>().enabled = false;
    }

    public void PauseGame(){
        if(!NetworkManager.Instance.GetIsPaused()){
            if(pauseStart != 0) {//Calculate how long the pause has lasted and add it to the pause counter
                pauseTime += Time.time - pauseStart;
                pauseStart = 0;
            }
            if(PhotonNetwork.IsMasterClient)
                blubleRoutine = StartCoroutine(BlubleCreator(emergingBaseSpeed/PhotonNetwork.CurrentRoom.PlayerCount)); //restart bubble generation
            gameSound.Play();

            //Show bubbles again
            foreach (Transform child in transform){
                if(child.gameObject.GetComponent<BlubleDraggable>() != null);
                    child.gameObject.SetActive(true);
            }
        } else {
            pauseStart = Time.time;
            if(blubleRoutine != null)
                StopCoroutine(blubleRoutine); //pause bubble generation
            gameSound.Stop();
            
            //Hide all bubbles
            foreach (Transform child in transform){
                if(child.gameObject.GetComponent<BlubleDraggable>() != null);
                    child.gameObject.SetActive(false);
            }
        }
    }

    public void QuitGame(){
        //Stop bubble generation & sound
        if(blubleRoutine != null)
            StopCoroutine(blubleRoutine);
        gameSound.Stop();  

        //Remove all bubbles
        if(PhotonNetwork.IsMasterClient){
            foreach (Transform child in transform){
                PhotonNetwork.Destroy(child.gameObject);
            }
        }

        //Reset values
        score = 0;
        UpdateScore();
        startTime = 0;
        pauseTime = 0;
        pauseStart = 0;
        blubleCounter = 0;
        destroyedBlubles = 0;
        words = null;
    }
}
