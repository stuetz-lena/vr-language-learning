using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    static string GAME_VERSION = "Ver.1";
   
    static RoomOptions ROOM_OPTIONS = new RoomOptions()
    {
        MaxPlayers = 20,
        IsOpen = true,
        IsVisible = true
    };

    [SerializeField]
    GameObject networkPlayer;

    [SerializeField]
    Transform cameraRig;

    List<string> modelList = new List<string>() {
            "audioboy", "bighead", "unitychan"
        };

    //added code ++
    public static NetworkManager Instance; 

    [Tooltip("Transform of the XRig.")]
    [SerializeField]
    Transform XRRigPosition; //functionality overtaken from RigPosition script as now dependent on player amount

    [Tooltip("GameObject of the top ufo to disable its movement during pauses.")]
    public GameObject ufo;

    private GameObject bucketDer;
    private GameObject bucketDie;
    private GameObject bucketDas;
    private GameObject robo;

    private bool isPaused = false;
    private bool gameQuitted = false;
    private bool tutorialFinished = false;
    // -- addedCode
    
    void Awake(){
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        Instance = this;
        Debug.Log("PhotonLogin: Verbindung zum Server wird hergestellt...");
        PhotonNetwork.GameVersion = GAME_VERSION;
        PhotonNetwork.ConnectUsingSettings();

        XRRigPosition.transform.position = new Vector3(PhotonNetwork.LocalPlayer.ActorNumber * 2, 0.25f, -14.5f); //position player based on player number
        UserInterface.Instance.RepositionCanvas(); //now that the camera position is clear, adjust the UI canvas
    }

    void Update(){}

    public override void OnConnectedToMaster()
    {
        Debug.Log("Verbunden zum Server.");
        if(!gameQuitted) //do not join again if the game was quit
            PhotonNetwork.JoinOrCreateRoom("bluble", ROOM_OPTIONS, null);
    }

    public override void OnJoinedRoom(){}

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
    }

    public bool GetIsPaused(){
        return isPaused;
    }

    public void UserEntersGameMode(){
        tutorialFinished = true;
        gameQuitted = false; //in case it is a new try
        if(!isPaused){ //if we are not coming from reviewing the tutorial we are at the start
            if(!PhotonNetwork.InRoom)
                PhotonNetwork.JoinOrCreateRoom("bluble", ROOM_OPTIONS, null);
            StartCoroutine(SetUpGame()); 
        } else { //otherwise we need to go back to the pause screen
            TriggerGameObjects(true);
            UserInterface.Instance.TriggerPauseMenu(true);
        }
    }

    IEnumerator SetUpGame(){
        yield return new WaitForSeconds(1.0f); //inserted a delay due to the otherwise too quick switch of sound and visuals
        CreateAvatar();
        if(PhotonNetwork.IsMasterClient){ //if master client, instantiate the objects and show the start button
            CreateBucketsAndRobo();
            UserInterface.Instance.TriggerStartButton(true);
        } else if(tutorialFinished && bucketDer != null){
            TriggerGameObjects(true);
        }
        GameController.Instance.MakeSound(); //start the gameplay sound
    }

    void CreateAvatar()
    {
        Debug.Log("Ein neuer Avatar hat den Raum betreten.");
        int index = Random.Range(0, modelList.Count);
        networkPlayer = PhotonNetwork.Instantiate(modelList[index], new Vector3(0, 2, 0), Quaternion.identity, 0);
        photonView.RPC("MoveAvatarInScene", RpcTarget.All, networkPlayer.GetComponent<PhotonView>().ViewID);
        cameraRig.transform.parent = networkPlayer.transform;

        //Hide own player to avoid confusion
        PhotonView playerView = networkPlayer.GetComponent<PhotonView>();
        if(playerView.IsMine){
            Component[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>(); //disabling all Renderers
            foreach (SkinnedMeshRenderer rend in renderers)
                rend.enabled = false;
        }
    }

    [PunRPC]
    void MoveAvatarInScene(int viewId){
        PhotonView.Find(viewId).gameObject.transform.parent = transform;
    }

    void CreateBucketsAndRobo(){ //MasterONLYFunction
        bucketDer = PhotonNetwork.InstantiateSceneObject("Bucket_der",  new Vector3(-2.69f, 0.3f, -12.95f),new Quaternion(0,-0.3f,0,1), 0);
        bucketDie = PhotonNetwork.InstantiateSceneObject("Bucket_die", new Vector3(-2.05f, 0.3f, -12.3f), Quaternion.identity, 0);
        bucketDas = PhotonNetwork.InstantiateSceneObject("Bucket_das",  new Vector3(-1.15f, 0.3f, -13f), new Quaternion(0,0.3f,0,1), 0);
        robo = PhotonNetwork.InstantiateSceneObject("robo",  new Vector3(1.29f, 2.21f, -5.52f), new Quaternion(0,0.3f,0,1), 0); 
        photonView.RPC("SetUpGameObjects", RpcTarget.All, bucketDer.GetComponent<PhotonView>().ViewID, bucketDie.GetComponent<PhotonView>().ViewID, bucketDas.GetComponent<PhotonView>().ViewID, robo.GetComponent<PhotonView>().ViewID);
    }

    public void TriggerGameObjects(bool val){
        bucketDer.SetActive(val);
        bucketDie.SetActive(val);
        bucketDas.SetActive(val);
        robo.SetActive(val);
    }

    public void TriggerRobo(bool val){
        robo.SetActive(val);
    }

    [PunRPC]
    void SetUpGameObjects(int bucketDerId, int bucketDieId, int bucketDasId, int roboId){
        if(!PhotonNetwork.IsMasterClient){ //establish references based in the viewIds
            bucketDer = PhotonView.Find(bucketDerId).gameObject;
            bucketDie = PhotonView.Find(bucketDieId).gameObject;
            bucketDas = PhotonView.Find(bucketDasId).gameObject;
            robo = PhotonView.Find(roboId).gameObject;

            if(!tutorialFinished && bucketDer != null){ //hide for other players while they are still in the menu
                TriggerGameObjects(false);
            }
        }
         
        bucketDer.transform.parent = transform;
        bucketDer.GetComponentInChildren<Canvas>().worldCamera = Camera.main;
        bucketDer.tag = "Bucket_der";

        bucketDie.transform.parent = transform;
        bucketDie.GetComponentInChildren<Canvas>().worldCamera = Camera.main;
        bucketDie.tag = "Bucket_die";

        bucketDas.transform.parent = transform;
        bucketDas.GetComponentInChildren<Canvas>().worldCamera = Camera.main;
        bucketDas.tag = "Bucket_das"; 

        robo.transform.parent = transform;
        GameController.Instance.SetRobo(robo.GetComponentInChildren<TextMeshPro>()); //Pass to game controller, text needed for score
    }

    public void Starter(){ //MasterONLYFunction
        photonView.RPC("StartGame", RpcTarget.All, bucketDer.GetComponent<PhotonView>().ViewID, bucketDie.GetComponent<PhotonView>().ViewID, bucketDas.GetComponent<PhotonView>().ViewID, robo.GetComponent<PhotonView>().ViewID); //to inform the other clients
        GameController.Instance.FirstBluble(); //create first bluble
    }

    [PunRPC]
    void StartGame(int bucketDerId, int bucketDieId, int bucketDasId, int roboId){
        if(!PhotonNetwork.IsMasterClient && bucketDer == null){ //if the client joined late
            SetUpGameObjects(bucketDerId, bucketDieId, bucketDasId, roboId);
        }

        //Freeze the buckets to avoid handling problems 
        bucketDer.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
        bucketDie.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
        bucketDas.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
        
        //Disable the draggable script and the recognition as raycast target
        bucketDer.GetComponent<Draggable>().enabled = false;
        bucketDie.GetComponent<Draggable>().enabled = false;
        bucketDas.GetComponent<Draggable>().enabled = false;
        int LayerIgnoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
        bucketDer.layer = LayerIgnoreRaycast;
        bucketDie.layer = LayerIgnoreRaycast;
        bucketDas.layer = LayerIgnoreRaycast;

        //Start the game
        GameController.Instance.SetStartTime(Time.time);
        GameController.Instance.SetWordStore();
        robo.GetComponent<RoboMovement>().enabled = true; //make robo move
        UserInterface.Instance.TriggerHUD(true); //show HUD
    }

    public void Pauser(){
        photonView.RPC("PauseGame", RpcTarget.All); //to inform the other clients
    }

    [PunRPC]
    void PauseGame(){
        if(isPaused){
            isPaused = false;
            UserInterface.Instance.TriggerPauseMenu(false);
            UserInterface.Instance.TriggerPauseButton(true);

            //Enable ufo & robo movement
            ufo.GetComponent<UfoMovement>().enabled = true;
            robo.GetComponent<RoboMovement>().enabled = true;

            //Deactivate selection & repositioning for the buckets
            bucketDer.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
            bucketDie.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
            bucketDas.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;

            int LayerIgnoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
            bucketDer.layer = LayerIgnoreRaycast;
            bucketDie.layer = LayerIgnoreRaycast;
            bucketDas.layer = LayerIgnoreRaycast;

            bucketDer.GetComponent<Draggable>().enabled = false;
            bucketDie.GetComponent<Draggable>().enabled = false;
            bucketDas.GetComponent<Draggable>().enabled = false;
        } else {
            isPaused = true;
            UserInterface.Instance.TriggerPauseMenu(true);
            UserInterface.Instance.TriggerPauseButton(false);

            //Disable ufo & robo movement
            ufo.GetComponent<UfoMovement>().enabled = false;
            robo.GetComponent<RoboMovement>().enabled = false;

            //Activate selection & repositioning for the buckets
            bucketDer.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
            bucketDie.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
            bucketDas.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;

            int LayerIgnoreRaycast = LayerMask.NameToLayer("Default");
            bucketDer.layer = LayerIgnoreRaycast;
            bucketDie.layer = LayerIgnoreRaycast;
            bucketDas.layer = LayerIgnoreRaycast;
            
            bucketDer.GetComponent<Draggable>().enabled = true;
            bucketDie.GetComponent<Draggable>().enabled = true;
            bucketDas.GetComponent<Draggable>().enabled = true;
        }
        GameController.Instance.PauseGame();
    }

    public void Quitter(){ //to enable a new game leave the room and destroy all objects
        photonView.RPC("QuitGame", RpcTarget.All); //to inform the other clients
    }

    [PunRPC]
    void QuitGame(){ //to enable a new game leave the room and destroy all objects
        gameQuitted = true;
        isPaused = false;
        UserInterface.Instance.QuitGame();
        PhotonNetwork.Destroy(networkPlayer);
        if(PhotonNetwork.IsMasterClient){
            PhotonNetwork.Destroy(bucketDas);
            PhotonNetwork.Destroy(bucketDie);
            PhotonNetwork.Destroy(bucketDer);
            PhotonNetwork.Destroy(robo);
        }
        GameController.Instance.QuitGame();
        PhotonNetwork.LeaveRoom();
    }

    public void ShutDown(){ //completely closing the application
        PhotonNetwork.Destroy(networkPlayer);
        PhotonNetwork.LeaveRoom();
        Application.Quit();
    }
}
