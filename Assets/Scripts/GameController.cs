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

    TextMeshPro roboText; //text element on robo
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
        //update the time if a text element is available, the start time was set, the game is not finished yet or currently paused
        if(timeText != null && startTime != 0 && (destroyedBlubles <  words.GetLength(0)) && !NetworkManager.Instance.GetIsPaused()){
            TimeSpan t = TimeSpan.FromSeconds(Time.time - startTime - pauseTime); //substract start time (after pushing the button) and pause time from the current time
            if(timeText.text != string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds)) //change in UI if necessary
                timeText.text = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
        }
    }

    public int GetScore() {
        return score;
    }

    public String GetTranslation(String basis){
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

    public void SetWordStore(){ //adjust word amout to player amout
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
                {"Sache","die",null,"Thing"},
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
                {"Sache","die",null,"Thing"},
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
                {"Sache","die",null,"Thing"},
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
        int index = UnityEngine.Random.Range(0, words.GetLength(0)); //Randomize word order
        if(words[index,2] == null) { //if the bubble does not exist yet
            //Create new bubble
            float deviationX = UnityEngine.Random.Range(-1 * deviationBaseX * PhotonNetwork.CurrentRoom.PlayerCount, deviationBaseX*PhotonNetwork.CurrentRoom.PlayerCount); //spawn breadth depends on player amount
            float initalY = Camera.main.transform.position.y + UnityEngine.Random.Range(deviationYFrom, deviationYTo);
            Vector3 position = new Vector3(Camera.main.transform.position.x + deviationX, initalY, Camera.main.transform.position.z + blubleDeviationZ);
            BlubleDraggable currentBluble = PhotonNetwork.InstantiateSceneObject("bluble", position, Camera.main.transform.rotation, 0).GetComponent<BlubleDraggable>();
            photonView.RPC("SetUpBuble", RpcTarget.All, currentBluble.GetComponent<PhotonView>().ViewID, index, initalY, deviationX);
            
            //Initiate next bluble
            blubleRoutine = StartCoroutine(BlubleCreator(emergingBaseSpeed/PhotonNetwork.CurrentRoom.PlayerCount)); //time to wait for a new bubble is depends on player amount
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
        if(PhotonNetwork.IsMasterClient){
            score += points;
            photonView.RPC("UpdateScore", RpcTarget.All, score);
        }
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
            float x = resultStartPosX - PhotonNetwork.CurrentRoom.PlayerCount * 2 - xSteps;
            float ySteps = 2/(words.GetLength(0)/perRow);
            float y = resultStartPosY;
            float rowCounter = 0; 
            for (int i = 0; i < words.GetLength(0); i++){
                if(rowCounter == perRow) {
                    y = y - ySteps;
                    rowCounter = 0;
                    x = resultStartPosX - PhotonNetwork.CurrentRoom.PlayerCount * 2 - xSteps;
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
        /*if(!PhotonNetwork.IsMasterClient)
            words = word;*/
        BlubleDraggable currentBluble = PhotonView.Find(viewID).gameObject.GetComponent<BlubleDraggable>(); //get the current bubble
        currentBluble.transform.parent = this.transform;
        currentBluble.gameObject.name = "Bluble " + word0;

        //Show the right result and change bubble color based on correct/wrong sorting
        currentBluble.GetComponentInChildren<TextMeshPro>().text = word1 + " " + word0;
        
        currentBluble.GetComponentInChildren<Canvas>().gameObject.SetActive(false);
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

    public void LeaveGame(){
        if(PhotonNetwork.IsMasterClient){
            int counter = 0;
            foreach(object[] word in words){
                photonView.RPC("UpdateWords", RpcTarget.Others, counter, word[0], word[1], word[2], word[3], words.GetLength(0));
                counter++;
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
