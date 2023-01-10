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

    //added code -- 
    [Tooltip("Transform of the XRig.")]
    [SerializeField]
    Transform XRRigPosition; //functionality overtaken from RigPosition script

    [Tooltip("Canvas with the UI.")]
    [SerializeField]
    Transform UICanvas;

    [Tooltip("Canvas with the start button.")]
    [SerializeField]
    GameObject startButtonCanvas;

    [Tooltip("The screen shown while the game is paused.")]
    [SerializeField]
    GameObject pauseScreen;

    private GameObject bucketDer;
    private GameObject bucketDie;
    private GameObject bucketDas;
    private GameObject robo;

    private bool gameQuitted = false;
    // -- addedCode

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
        XRRigPosition.transform.position = new Vector3(0 + PhotonNetwork.LocalPlayer.ActorNumber * 2, 0.25f,-14.5f); //position player based on player amoutn
        UICanvas.transform.position = new Vector3(Camera.main.transform.position.x,UICanvas.transform.position.y,UICanvas.transform.position.z); //position canvas based on camera position
    }

    void Update(){}

    public override void OnConnectedToMaster()
    {
        Debug.Log("Verbunden zum Server.");
        if(!gameQuitted) //do not join again if the game was quitted
            PhotonNetwork.JoinOrCreateRoom("bluble", ROOM_OPTIONS, null);
    }

    public override void OnJoinedRoom(){}

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
    }

    public void UserEntersGameMode(){
        gameQuitted = false;
        if(!GameController.Instance.GetIsPaused()){ //if we are not coming from reviewing the tutorial we are at the start, otherwise we need to go back to the pause screen
            if(!PhotonNetwork.InRoom)
                PhotonNetwork.JoinOrCreateRoom("bluble", ROOM_OPTIONS, null);
            StartCoroutine(SetUpGame()); 
        } else {
            bucketDer.SetActive(true);
            bucketDas.SetActive(true);
            bucketDie.SetActive(true);
            pauseScreen.SetActive(true);
        }
    }

    public IEnumerator SetUpGame(){
        yield return new WaitForSeconds(1.0f); //inserted a delay due to the quick switch of the music and effects
        CreateAvatar();
        if(PhotonNetwork.IsMasterClient){
            CreateBucketsAndRobo();
            startButtonCanvas.transform.position =  new Vector3(Camera.main.transform.position.x,startButtonCanvas.transform.position.y,startButtonCanvas.transform.position.z);
            startButtonCanvas.SetActive(true); //only show for the master
            GameController.Instance.MakeSound();
        }    
    }

    void CreateAvatar()
    {
        Debug.Log("Ein neuer Avatar hat den Raum betreten.");
        int index = Random.Range(0, modelList.Count);
        networkPlayer = PhotonNetwork.Instantiate(modelList[index], new Vector3(0, 2, 0), Quaternion.identity, 0);
        networkPlayer.transform.parent = transform;
        cameraRig.transform.parent = networkPlayer.transform;

        //Hide own player
        PhotonView photonView = networkPlayer.GetComponent<PhotonView>();
        if(photonView.IsMine){
            Component[] renderers;
            renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer rend in renderers)
                rend.enabled = false;
        }
    }

    [PunRPC]
    public void StartGame(){
       if(PhotonNetwork.IsMasterClient) {
            GameController.Instance.FirstBluble(); //create first bluble if master client
        }
        //Freeze the buckets
        Rigidbody rb = bucketDer.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
        rb = bucketDie.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
        rb = bucketDas.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
        bucketDer.GetComponent<Draggable>().enabled = false;
        bucketDie.GetComponent<Draggable>().enabled = false;
        bucketDas.GetComponent<Draggable>().enabled = false;

        //start the game
        startButtonCanvas.SetActive(false);
        GameController.Instance.SetStartTime(Time.time);
        robo.GetComponent<RoboMovement>().enabled = true;
    }

    void CreateBucketsAndRobo(){
        //create buckets and score per player
        bucketDer = PhotonNetwork.InstantiateSceneObject("Bucket_der",  new Vector3(-2.69f, 0.3f, -12.95f),new Quaternion(0,-0.3f,0,1), 0); //1.4
        bucketDer.transform.parent = transform;
        Canvas can = bucketDer.GetComponentInChildren<Canvas>();
        can.worldCamera = Camera.main;
        bucketDer.tag = "Bucket_der";

        bucketDie = PhotonNetwork.InstantiateSceneObject("Bucket_die", new Vector3(-2.05f, 0.3f, -12.3f), Quaternion.identity, 0); //2.3
        bucketDie.transform.parent = transform;
        can = bucketDie.GetComponentInChildren<Canvas>();
        can.worldCamera = Camera.main;
        bucketDie.tag = "Bucket_die";

        bucketDas = PhotonNetwork.InstantiateSceneObject("Bucket_das",  new Vector3(-1.15f, 0.3f, -13f), new Quaternion(0,0.3f,0,1), 0); //2.94
        bucketDas.transform.parent = transform;
        can = bucketDas.GetComponentInChildren<Canvas>();
        can.worldCamera = Camera.main;
        bucketDas.tag = "Bucket_das";

        //pass to gamecontroller
        GameController.Instance.SetBucketDer(bucketDer);
        GameController.Instance.SetBucketDie(bucketDie);
        GameController.Instance.SetBucketDas(bucketDas);
        
        //Create Robo
        robo = PhotonNetwork.InstantiateSceneObject("robo",  new Vector3(1.29f, 2.21f, -5.52f), new Quaternion(0,0.3f,0,1), 0);
        robo.transform.parent = transform;
        GameController.Instance.SetRobo(robo.GetComponentInChildren<TextMeshPro>(), robo);
    }


    public void QuitGame(){
        gameQuitted = true;
        PhotonNetwork.Destroy(networkPlayer);
        PhotonNetwork.Destroy(robo);
        PhotonNetwork.LeaveRoom();
        if(PhotonNetwork.IsMasterClient){
            PhotonNetwork.Destroy(bucketDas);
            PhotonNetwork.Destroy(bucketDie);
            PhotonNetwork.Destroy(bucketDer);
        }
        GameController.Instance.QuitGame();
    }

    public void ShutDown(){
        PhotonNetwork.Destroy(networkPlayer);
        PhotonNetwork.LeaveRoom();
        Application.Quit();
    }
}
