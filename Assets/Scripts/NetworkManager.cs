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

    [Tooltip("Canvas with the Start button.")]
    [SerializeField]
    GameObject startButtonCanvas;

    //added to provide player and access methods
    public GameController gameController;

    [SerializeField] //overtaken from RigPosition
    Transform XRRigPosition;

    [SerializeField]
    Transform cameraRig;

    [SerializeField]
    Transform UICanvas;

    private GameObject bucketDer;
    private GameObject bucketDie;
    private GameObject bucketDas;
    private GameObject robo;
    public GameObject pauseScreen;

    private bool gameQuitted = false;

    List<string> modelList = new List<string>() {
            "audioboy", "bighead", "unitychan"
        };
    
    void Awake(){
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        Debug.Log("PhotonLogin: Verbindung zum Server wird hergestellt...");
        PhotonNetwork.GameVersion = GAME_VERSION;
        PhotonNetwork.ConnectUsingSettings();
        XRRigPosition.transform.position = new Vector3(0 + PhotonNetwork.LocalPlayer.ActorNumber * 2, 0.25f,-14.5f);
        UICanvas.transform.position = new Vector3(XRRigPosition.transform.position.x,UICanvas.transform.position.y,UICanvas.transform.position.z);
    }

    void Update(){
    }

    public void UserTriggersStart(){
        gameQuitted = false;
        if(!gameController.GetIsPaused()){
            if(!PhotonNetwork.InRoom)
            PhotonNetwork.JoinOrCreateRoom("bluble", ROOM_OPTIONS, null);
            StartCoroutine(StartGame());
        } else {
            //gameController.PauseGame();
            bucketDer.SetActive(true);
            bucketDas.SetActive(true);
            bucketDie.SetActive(true);
            pauseScreen.SetActive(true);
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Verbunden zum Server.");
        if(!gameQuitted)
            PhotonNetwork.JoinOrCreateRoom("bluble", ROOM_OPTIONS, null);
        //PhotonNetwork.LoadLevel("MultiUserVR");
    }

    public override void OnJoinedRoom()
    {
        /*CreateAvatar();
        if(PhotonNetwork.IsMasterClient){
            //CreateGameController();
            //PhotonView photonView = GetComponent<PhotonView>();
            //photonView.RPC("CreateBuckets", RpcTarget.All);
            CreateBuckets();
            startButtonCanvas.transform.position =  new Vector3(XRRigPosition.transform.position.x,startButtonCanvas.transform.position.y,startButtonCanvas.transform.position.z);
            startButtonCanvas.SetActive(true);
        }*/      
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
    }

    public IEnumerator StartGame(){
        yield return new WaitForSeconds(1.0f);
        CreateAvatar();
        if(PhotonNetwork.IsMasterClient){
            //CreateGameController();
            //PhotonView photonView = GetComponent<PhotonView>();
            //photonView.RPC("CreateBuckets", RpcTarget.All);
            CreateBuckets();
            startButtonCanvas.transform.position =  new Vector3(XRRigPosition.transform.position.x,startButtonCanvas.transform.position.y,startButtonCanvas.transform.position.z);
            startButtonCanvas.SetActive(true);
            gameController.MakeSound();
        }    
    }

    void CreateAvatar()
    {
        Debug.Log("Ein neuer Avatar hat den Raum betreten.");
        int index = Random.Range(0, modelList.Count);
        networkPlayer = PhotonNetwork.Instantiate(modelList[index], new Vector3(0, 2, 0), Quaternion.identity, 0);
        networkPlayer.transform.parent = transform;
        cameraRig.transform.parent = networkPlayer.transform;
        //XRRigPosition.transform.position = new Vector3(Random.Range(-6, 6), 0.25f,-14.5f);
        //XRRigPosition.transform.position = new Vector3(0 + PhotonNetwork.LocalPlayer.ActorNumber * 2, 0.25f,-14.5f);
        //gameController.SetTransform(XRRigPosition); //added to hand over positioning details XRRigPosition

        //Hide own player
        PhotonView photonView = networkPlayer.GetComponent<PhotonView>();
        if(photonView.IsMine){
            Component[] renderers;
            renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer rend in renderers)
                rend.enabled = false;
        }
        
        //gameController.playerNr = PhotonNetwork.LocalPlayer.ActorNumber;  
    }

    [PunRPC]
    public void CreateGameController(){
        //gameController =  PhotonNetwork.InstantiateSceneObject("GameController", new Vector3(0,0.25f,13.67f), this.transform.rotation, 3).GetComponent<GameController>();
        //gameController.transform.parent = this.transform; //Initiated GameController does not produce blubles anymore
        if(PhotonNetwork.IsMasterClient) {
            gameController.FirstBluble(); //create first bluble
        }
        Rigidbody rb = bucketDer.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
        rb = bucketDie.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
        rb = bucketDas.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
        bucketDer.GetComponent<Draggable>().enabled = false;
        bucketDie.GetComponent<Draggable>().enabled = false;
        bucketDas.GetComponent<Draggable>().enabled = false;
        startButtonCanvas.SetActive(false);
        gameController.SetStartTime(Time.time);
        robo.GetComponent<RoboMovement>().enabled = true;
    }

    void CreateBuckets(){
        //create buckets and score per player
        bucketDer = PhotonNetwork.InstantiateSceneObject("Bucket_der",  new Vector3(-2.69f, 0.3f, -12.95f),new Quaternion(0,-0.3f,0,1), 0); //1.4
        bucketDer.transform.parent = transform;
        Canvas can = bucketDer.GetComponentInChildren<Canvas>();
        can.worldCamera = Camera.main;
        bucketDer.tag = "Bucket_der";
        bucketDie = PhotonNetwork.InstantiateSceneObject("Bucket_die", new Vector3(-2.05f, 0.3f, -12.3f), Quaternion.identity, 0); //2.3
        bucketDie.transform.parent = transform;
        can = bucketDie
        .GetComponentInChildren<Canvas>();
        can.worldCamera = Camera.main;
        bucketDie.tag = "Bucket_die";
        bucketDas = PhotonNetwork.InstantiateSceneObject("Bucket_das",  new Vector3(-1.15f, 0.3f, -13f), new Quaternion(0,0.3f,0,1), 0); //2.94
        bucketDas.transform.parent = transform;
        can = bucketDas.GetComponentInChildren<Canvas>();
        can.worldCamera = Camera.main;
        bucketDas.tag = "Bucket_das";
        gameController.SetBucketDer(bucketDer);
        gameController.SetBucketDie(bucketDie);
        gameController.SetBucketDas(bucketDas);
        //scoreText = PhotonNetwork.Instantiate("Score", new Vector3(0, 0, 0), Quaternion.identity, 0).GetComponent<TextMeshPro>();
        //createRobo
        robo = PhotonNetwork.InstantiateSceneObject("robo",  new Vector3(1.29f, 2.21f, -5.52f), new Quaternion(0,0.3f,0,1), 0);
        robo.transform.parent = transform;
        gameController.SetRobo(robo.GetComponentInChildren<TextMeshPro>(), robo.transform.Find("body").gameObject.GetComponent<MeshRenderer>(), robo);
    }

    public int GetScore(){
        return gameController.GetScore();
    }

    public void QuitGame(){
        gameQuitted = true;
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Destroy(networkPlayer);
        PhotonNetwork.Destroy(robo);
        if(PhotonNetwork.IsMasterClient){
            PhotonNetwork.Destroy(bucketDas);
            PhotonNetwork.Destroy(bucketDie);
            PhotonNetwork.Destroy(bucketDer);
        }
        gameController.QuitGame();
    }

    public void ShutDown(){
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Destroy(networkPlayer);
        Application.Quit();
    }
}
