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

    List<string> modelList = new List<string>() {
            "audioboy", "bighead", "unitychan"
        };
    
    void Awake(){
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        UserTriggersStart();
    }

    public void UserTriggersStart(){ //call via UI later
        Debug.Log("PhotonLogin: Verbindung zum Server wird hergestellt...");
        PhotonNetwork.GameVersion = GAME_VERSION;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Verbunden zum Server.");
        PhotonNetwork.JoinOrCreateRoom("bluble1", ROOM_OPTIONS, null);
        //PhotonNetwork.LoadLevel("MultiUserVR");
    }

    public override void OnJoinedRoom()
    {
        CreateAvatar();
        if(PhotonNetwork.IsMasterClient){
            //CreateGameController();
            //PhotonView photonView = GetComponent<PhotonView>();
            //photonView.RPC("CreateBuckets", RpcTarget.All);
            CreateBuckets();
            startButtonCanvas.transform.position =  new Vector3(XRRigPosition.transform.position.x,startButtonCanvas.transform.position.y,startButtonCanvas.transform.position.z);
            startButtonCanvas.SetActive(true);
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
        XRRigPosition.transform.position = new Vector3(0 + PhotonNetwork.LocalPlayer.ActorNumber * 2, 0.25f,-14.5f);
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
        startButtonCanvas.SetActive(false);
    }

    void CreateBuckets(){
        //create buckets and score per player
        GameObject bucketDer = PhotonNetwork.InstantiateSceneObject("Bucket_der",  new Vector3(1.4f, 0.3f, -13f), Quaternion.identity, 0);
        bucketDer.transform.parent = transform;
        bucketDer.tag = "Bucket_der";
        GameObject bucketDie = PhotonNetwork.InstantiateSceneObject("Bucket_die", new Vector3(2.3f, 0.3f, -12.3f), Quaternion.identity, 0);
        bucketDie.transform.parent = transform;
        bucketDie.tag = "Bucket_die";
        GameObject bucketDas = PhotonNetwork.InstantiateSceneObject("Bucket_das", new Vector3(2.94f, 0.3f, -12.95f), Quaternion.identity, 0);
        bucketDas.transform.parent = transform;
        bucketDas.tag = "Bucket_das";
        gameController.SetBucketDer(bucketDer);
        gameController.SetBucketDie(bucketDie);
        gameController.SetBucketDas(bucketDas);
        //scoreText = PhotonNetwork.Instantiate("Score", new Vector3(0, 0, 0), Quaternion.identity, 0).GetComponent<TextMeshPro>();
    }

    public int GetScore(){
        return gameController.GetScore();
    }
}
