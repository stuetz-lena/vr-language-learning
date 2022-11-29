using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    static string GAME_VERSION = "Ver.1";
    PhotonView photonView;

    static RoomOptions ROOM_OPTIONS = new RoomOptions()
    {
        MaxPlayers = 20,
        IsOpen = true,
        IsVisible = true
    };

    [SerializeField]
    GameObject networkPlayer;
    [SerializeField]
    TextMeshPro scoreText;

    //added to provide player
    public GameController gameController;

    [SerializeField]
    Transform cameraRig;

    List<string> modelList = new List<string>() {
            "audioboy", "bighead", "unitychan"
        };

    void Start()
    {
        Debug.Log("PhotonLogin: Verbindung zum Server wird hergestellt...");
        PhotonNetwork.GameVersion = GAME_VERSION;
        PhotonNetwork.ConnectUsingSettings();
        photonView = GetComponent<PhotonView>();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Verbunden zum Server.");
        PhotonNetwork.JoinOrCreateRoom("bluble", ROOM_OPTIONS, null);
    }

    public override void OnJoinedRoom()
    {
        CreateAvatar();
        StartCoroutine(gameController.BlubleCreator(0)); //create first bluble
    }

    void CreateAvatar()
    {
        Debug.Log("Ein neuer Avatar hat den Raum betreten.");
        int index = Random.Range(0, modelList.Count);
        networkPlayer = PhotonNetwork.Instantiate(modelList[index], new Vector3(0, 0, 0), Quaternion.identity, 0);
        networkPlayer.transform.parent = transform;
        cameraRig.transform.parent = networkPlayer.transform;
        /*if(photonView.IsMine){
            networkPlayer.SetActive(false);
        }*/

        //added to hand over player details
        gameController.player = networkPlayer;
        gameController.playerNr = PhotonNetwork.LocalPlayer.ActorNumber;
       
        //create buckets and score per player, problem with positioning
        GameObject bucketDer = PhotonNetwork.Instantiate("Bucket_der",  new Vector3(cameraRig.transform.position.x-0.5f, 0.3f, -13.6f), Quaternion.identity, 1);
        bucketDer.transform.parent = transform;
        GameObject bucketDie = PhotonNetwork.Instantiate("Bucket_die", new Vector3(this.transform.position.x, 0.3f, -13.6f), Quaternion.identity, 1);
        bucketDie.transform.parent = transform;
        GameObject bucketDas = PhotonNetwork.Instantiate("Bucket_das", new Vector3(this.transform.position.x, 0.3f, -13.6f), Quaternion.identity, 1);
        bucketDas.transform.parent = transform;
        scoreText = PhotonNetwork.Instantiate("Score", new Vector3(0, 0, 0), Quaternion.identity, 0).GetComponent<TextMeshPro>();
    }
}
