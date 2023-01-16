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

    [Tooltip("TextMeshProUGUI element for the score in HUD.")][SerializeField]
    TextMeshProUGUI scoreText; 
    [Tooltip("TextMeshProUGUI element for the time in HUD.")][SerializeField]
    TextMeshProUGUI timeText;

    [Tooltip("Distance to camera for bubbles to spawn initally.")][SerializeField]
    int blubleDeviationZ = 20;
    [Tooltip("Time difference between two bubbles for single player.")][SerializeField]
    float emergingBaseSpeed = 5.0f; 
    [Tooltip("Half range of horizontal deviation of new bubbles for single player.")][SerializeField]
    float deviationBaseX = 0.5f;
    [Tooltip("Lower position of vertical deviation for new bubbles.")][SerializeField]
    float deviationYFrom = 0.7f;
    [Tooltip("Upper position of vertical deviation for new bubbles.")][SerializeField]
    float deviationYTo = 1.0f;
    [Tooltip("Number of bubbles per row during result view for single player.")][SerializeField]
    int blublesPerRow = 5;
    [Tooltip("Bubble deviation from camera in result view.")][SerializeField]
    float resultDeviationZ = 6.5f;
    [Tooltip("Horizontal start position for bubble grid in result view for single player.")][SerializeField]
    float resultStartPosX = 1.3f;
    [Tooltip("Vertical start position for bubble grid in result view for single player.")][SerializeField]
    float resultStartPosY = 0.9f;

    [Tooltip("AudioSource to be played during game mode.")][SerializeField]
    AudioSource gameSound;
    [Tooltip("AudioSource to be played during result view.")][SerializeField]
    AudioSource final;

    TextMeshPro roboText; //text element on robo, set via NetworkManager
    int score = 0;
    float startTime = 0;
    float pauseStart = 0;
    float pauseTime = 0;

    object[,] words; //array with vocabularies
    int blubleCounter = 0; //number of created bubbles
    int destroyedBlubles = 0; //number of destroyed bubbles
    Coroutine blubleRoutine; //to ensure only one coroutine is running at a time 
    
    // Start is called before the first frame update
    void Start(){
        Instance = this;
    }

    // Update is called once per frame
    void Update(){ 
        //update the timer if the text element is available, the start time was set, the game is not finished or currently paused
        if(timeText != null && startTime != 0 && (destroyedBlubles <  words.GetLength(0)) && !NetworkManager.Instance.GetIsPaused()){
            TimeSpan t = TimeSpan.FromSeconds(Time.time - startTime - pauseTime); //substract start time (taken at pushing the button) and pause time from current time
            if(!timeText.text.Equals(string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds))) //change only in UI if necessary
                timeText.text = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
        }
    }

    public int GetScore() {
        return score;
    }

    public string GetTranslation(string basis){
        for(int i = 0; i < words.GetLength(0); i++) {
            if(basis.Equals((string)words[i,0])){
                return (string)words[i,3];
            }
        }
        return basis;
    }

    public void SetRoboText(TextMeshPro roboTmp){
        roboText = roboTmp;
    }

    public void SetStartTime(float t){
        startTime = t;
    }

    public void SetWordStore(){ //adjust word amount to player amount
        switch(PhotonNetwork.CurrentRoom.PlayerCount) {
            case 1: words = new object[13,4] {
                {"Antwort","die",null,"Answer"},
                {"Familie","die",null,"Family"},
                {"Musik","die",null,"Music"},
                {"Quiz","das",null,"Quiz"},
                {"Punkt","der",null,"Point"},
                {"Schau-spieler","der",null,"Actor"},
                {"Sprache","die",null,"Lang-uage"},
                {"Ausland","das",null,"Abroad"},
                {"Sache","die",null,"Thing"},
                {"E-Mail","die",null,"E-Mail"},
                {"Tag","der",null,"Day"},
                {"Fern-seher","der",null,"TV"},
                {"Problem","das",null,"Problem"}
            };
            break;
            case 2: words = new object[26,4] {
                {"Internet","das",null,"Internet"},
                {"Post","die",null,"Post"},
                {"Com-puter","der",null,"Com-puter"},
                {"Film","der",null,"Movie"},
                {"Banane","die",null,"Banana"},
                {"Entschul-digung","die",null,"Excuse"},
                {"Comic","der",null,"Comic"},
                {"Person","die",null,"Person"},
                {"Name","der",null,"Name"},
                {"Beispiel","das",null,"Example"},
                {"Familien-name","der",null,"Last name"},
                {"Teil","das",null,"Part"},
                {"Handy","das",null,"Mobile phone"},
                {"Antwort","die",null,"Answer"},
                {"Familie","die",null,"Family"},
                {"Musik","die",null,"Music"},
                {"Quiz","das",null,"Quiz"},
                {"Punkt","der",null,"Point"},
                {"Schau-spieler","der",null,"Actor"},
                {"Sprache","die",null,"Lang-uage"},
                {"Ausland","das",null,"Abroad"},
                {"Sache","die",null,"Matter"},
                {"E-Mail","die",null,"E-Mail"},
                {"Tag","der",null,"Day"},
                {"Fern-seher","der",null,"TV"},
                {"Problem","das",null,"Problem"}
            };
            break; 
            case 3: words = new object[39,4] {
                {"Internet","das",null,"Internet"},
                {"Post","die",null,"Post"},
                {"Com-puter","der",null,"Com-puter"},
                {"Film","der",null,"Movie"},
                {"Banane","die",null,"Banana"},
                {"Entschul-digung","die",null,"Excuse"},
                {"Comic","der",null,"Comic"},
                {"Person","die",null,"Person"},
                {"Name","der",null,"Name"},
                {"Beispiel","das",null,"Example"},
                {"Familien-name","der",null,"Last name"},
                {"Teil","das",null,"Part"},
                {"Handy","das",null,"Mobile phone"},
                {"Antwort","die",null,"Answer"},
                {"Familie","die",null,"Family"},
                {"Musik","die",null,"Music"},
                {"Quiz","das",null,"Quiz"},
                {"Punkt","der",null,"Point"},
                {"Schau-spieler","der",null,"Actor"},
                {"Sprache","die",null,"Language"},
                {"Ausland","das",null,"Abroad"},
                {"Sache","die",null,"Matter"},
                {"E-Mail","die",null,"E-Mail"},
                {"Tag","der",null,"Day"},
                {"Fern-seher","der",null,"TV"},
                {"Problem","das",null,"Problem"},
                {"Blume","die",null,"Flower"},
                {"Fahrrad","das",null,"Bike"},
                {"Hose","die",null,"Trousers"},
                {"Klavier","das",null,"Piano"},
                {"Kühl-schrank","der",null,"Fridge"},
                {"Schrank","der",null,"Wardrobe"},
                {"Spiel","das",null,"Game"},
                {"Ding","das",null,"Thing"},
                {"Brief-marke","die",null,"Stamp"},
                {"Lebens-mittel","das",null,"Groceries"},
                {"Urlaub","der",null,"Vacation"},
                {"Sport","der",null,"Sport"},
                {"Wohn-ung","die",null,"Flat"}
            };
            break;
            default: words = new object[39,4] {
                {"Internet","das",null,"Internet"},
                {"Post","die",null,"Post"},
                {"Com-puter","der",null,"Com-puter"},
                {"Film","der",null,"Movie"},
                {"Banane","die",null,"Banana"},
                {"Entschul-digung","die",null,"Excuse"},
                {"Comic","der",null,"Comic"},
                {"Person","die",null,"Person"},
                {"Name","der",null,"Name"},
                {"Beispiel","das",null,"Example"},
                {"Familien-name","der",null,"Last name"},
                {"Teil","das",null,"Part"},
                {"Handy","das",null,"Mobile phone"},
                {"Antwort","die",null,"Answer"},
                {"Familie","die",null,"Family"},
                {"Musik","die",null,"Music"},
                {"Quiz","das",null,"Quiz"},
                {"Punkt","der",null,"Point"},
                {"Schau-spieler","der",null,"Actor"},
                {"Sprache","die",null,"Lang-uage"},
                {"Ausland","das",null,"Abroad"},
                {"Sache","die",null,"Matter"},
                {"E-Mail","die",null,"E-Mail"},
                {"Tag","der",null,"Day"},
                {"Fern-seher","der",null,"TV"},
                {"Problem","das",null,"Problem"},
                {"Blume","die",null,"Flower"},
                {"Fahrrad","das",null,"Bike"},
                {"Hose","die",null,"Trousers"},
                {"Klavier","das",null,"Piano"},
                {"Kühl-schrank","der",null,"Fridge"},
                {"Schrank","der",null,"Wardrobe"},
                {"Spiel","das",null,"Game"},
                {"Ding","das",null,"Thing"},
                {"Brief-marke","die",null,"Stamp"},
                {"Lebens-mittel","das",null,"Groceries"},
                {"Urlaub","der",null,"Vacation"},
                {"Sport","der",null,"Sport"},
                {"Wohn-ung","die",null,"Flat"}
            };
            break;
        }
    }

    [PunRPC]
    void UpdateScore(int sc = 0){
        if(!PhotonNetwork.IsMasterClient)
            score = sc;
        scoreText.text = sc.ToString("D2"); //format: 00
        roboText.text = sc.ToString("D2");
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
        int index = UnityEngine.Random.Range(0, words.GetLength(0)); //randomize word order
        if(words[index,2] == null) { //if the bubble does not exist yet
            //Create new bubble
            float deviationX = UnityEngine.Random.Range(-1 * deviationBaseX * PhotonNetwork.CurrentRoom.PlayerCount, deviationBaseX * PhotonNetwork.CurrentRoom.PlayerCount); //spawn breadth depends on player amount
            float initalY = Camera.main.transform.position.y + UnityEngine.Random.Range(deviationYFrom, deviationYTo);
            Vector3 position = new Vector3(Camera.main.transform.position.x + deviationX, initalY, Camera.main.transform.position.z + blubleDeviationZ);
            BlubleDraggable currentBluble = PhotonNetwork.InstantiateSceneObject("bluble", position, Camera.main.transform.rotation, 0).GetComponent<BlubleDraggable>();
            photonView.RPC("SetUpBuble", RpcTarget.All, currentBluble.GetComponent<PhotonView>().ViewID, index, initalY, deviationX);
            
            //Initiate next bubble
            blubleRoutine = StartCoroutine(BlubleCreator(emergingBaseSpeed/PhotonNetwork.CurrentRoom.PlayerCount)); //time to wait for next bubble depends on player amount
        } else if(blubleCounter < words.GetLength(0)) { //if the bubble was already created, start a new try
            CreateBluble();
        }
    } 

    [PunRPC]
    void SetUpBuble(int viewID, int index, float y, float x){
        //save bubble generation
        blubleCounter++; 
        words[index,2] = false; //not sorted in yet
        //get the current bubble
        BlubleDraggable currentBluble = PhotonView.Find(viewID).gameObject.GetComponent<BlubleDraggable>(); 

        currentBluble.transform.parent = this.transform;
        currentBluble.gameObject.name = "Bluble " + (string)words[index,0]; //name in scene
        //make sure the random factors stay constant
        currentBluble.SetInitialY(y); 
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

    public void Congrats(int points, string word){ 
        //increase score
        /*if(PhotonNetwork.IsMasterClient){
            score += points;
            photonView.RPC("UpdateScore", RpcTarget.All, score);
        }*/
        score += points;
            UpdateScore(score);
        //save the correct sorting
        for(int i = 0; i < words.GetLength(0); i++) {
            if(word.Equals(words[i,0])){
                words[i,2] = true;
                break;
            }
        }
        destroyedBlubles++;
        CheckForTie();
    }

    public void Fail(string word = "false", string bucket = "false"){
        if(!word.Equals("false") && !bucket.Equals("false")){
            //save wrong sorting
            destroyedBlubles++;
            for(int i = 0; i < words.GetLength(0); i++) {
                if(word.Equals(words[i,0])){
                    words[i,2] = bucket; //saving the chosen bucket
                    break;
                }
            }
        } else { 
            //bubble left game floor and disappeared - needs to show up again - reset creation
            blubleCounter--;
            for(int i = 0; i < words.GetLength(0); i++) {
                if(word.Equals(words[i,0])){
                    words[i,2] = null;
                    break;
                }
            }
        }
        CheckForTie();
    }

    void CheckForTie(){
        //check if all blubles are gone and game is finished
        if(destroyedBlubles == words.GetLength(0)){
            Debug.Log("End");
            ShowResults();
        } else if(blubleCounter == destroyedBlubles && PhotonNetwork.IsMasterClient){ 
            //game is not finished yet but no more bubbles exist to sort, we need to generate a new one quickly
            if(blubleRoutine != null)
                //we only want one routine running at the same time
                StopCoroutine(blubleRoutine); 
            blubleRoutine = StartCoroutine(BlubleCreator(0));
        }
    }

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
            float x = resultStartPosX - PhotonNetwork.CurrentRoom.PlayerCount*2 - xSteps;
            float ySteps = 2/(words.GetLength(0)/perRow);
            float y = resultStartPosY;
            float rowCounter = 0; 
            for (int i = 0; i < words.GetLength(0); i++){
                if(rowCounter == perRow) {
                    y = y - ySteps;
                    rowCounter = 0;
                    x = resultStartPosX - PhotonNetwork.CurrentRoom.PlayerCount*2 - xSteps;
                }
                x = x + xSteps;
                rowCounter++;
                Vector3 position = new Vector3(Camera.main.transform.position.x + (x-xSteps/2), Camera.main.transform.position.y + (y+ySteps/2), Camera.main.transform.position.z + resultDeviationZ);
                BlubleDraggable currentBluble = PhotonNetwork.InstantiateSceneObject("bluble", position, Quaternion.identity, 0).GetComponent<BlubleDraggable>(); 
                photonView.RPC("SetUpResultBubbles", RpcTarget.All, currentBluble.GetComponent<PhotonView>().ViewID, (string)words[i,0], (string)words[i,1], words[i,2].ToString());
            }
        }
    }

    [PunRPC]
    void SetUpResultBubbles(int viewID, string word0, string word1, string word2){
        //get the current bubble
        BlubleDraggable currentBluble = PhotonView.Find(viewID).gameObject.GetComponent<BlubleDraggable>(); 

        currentBluble.transform.parent = this.transform;
        currentBluble.gameObject.name = "Bluble " + word0;

        //hide hint canvas
        if(currentBluble.GetComponentInChildren<Canvas>()) 
            currentBluble.GetComponentInChildren<Canvas>().gameObject.SetActive(false);

        //show the right solution & change bubble color based on correct/wrong sorting
        currentBluble.GetComponentInChildren<TextMeshPro>().text = word1 + " " + word0;
        if(String.Equals(word2, "True")){
            currentBluble.GetComponent<Renderer>().material = currentBluble.GetGreen();
        } else if (!String.Equals(word2, "False")) {
            currentBluble.GetComponent<Renderer>().material = currentBluble.GetRed();
        }  

        //disable movement & selection
        currentBluble.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        currentBluble.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
        currentBluble.GetComponent<BlubleDraggable>().enabled = false;
    }

    public void PauseGame(){
        if(!NetworkManager.Instance.GetIsPaused()){
            //calculate how long the pause has lasted and add it to the pause counter
            if(pauseStart != 0) {
                pauseTime += Time.time - pauseStart;
                pauseStart = 0;
            }
            //restart bubble generation & sound
            if(PhotonNetwork.IsMasterClient)
                blubleRoutine = StartCoroutine(BlubleCreator(emergingBaseSpeed/PhotonNetwork.CurrentRoom.PlayerCount)); 
            gameSound.Play();
            //Show bubbles again
            foreach (Transform child in transform){
                if(child.gameObject.GetComponent<BlubleDraggable>() != null)
                    child.gameObject.SetActive(true);
            }
        } else {
            //save pause start
            pauseStart = Time.time;
            //pause bubble generation & sound
            if(blubleRoutine != null)
                StopCoroutine(blubleRoutine);
            gameSound.Stop();
            //Hide all bubbles
            foreach (Transform child in transform){
                if(child.gameObject.GetComponent<BlubleDraggable>() != null)
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
                if(child.gameObject.GetComponent<BlubleDraggable>() != null);
                    PhotonNetwork.Destroy(child.gameObject);
            }
        }

        //Reset values
        score = 0;
        UpdateScore(score);
        startTime = 0;
        pauseStart = 0;
        pauseTime = 0;
        blubleCounter = 0;
        destroyedBlubles = 0;
        words = null;
    }

    public void LeaveGame(){
        if(PhotonNetwork.IsMasterClient){
            for(int i = 0; i < words.GetLength(0); i++){
                photonView.RPC("UpdateWords", RpcTarget.Others, i, words[i,0], words[i,1], words[i,2], words[i,3], words.GetLength(0));
            }
        }
    }

    [PunRPC]
    void UpdateWords(int row, object word0, object word1, object word2, object word3, int length){
        if(row == 0)
            words = new object[length,4];
        words[row, 0] = word0;
        words[row, 1] = word1;
        words[row, 2] = word2;
        words[row, 3] = word3;
    }
}
