using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

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

    void Start()
    {
        Debug.Log("PhotonLogin: Verbindung zum Server wird hergestellt...");
        PhotonNetwork.GameVersion = GAME_VERSION;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Verbunden zum Server.");
        PhotonNetwork.JoinOrCreateRoom("VR-Room", ROOM_OPTIONS, null);
    }

    public override void OnJoinedRoom()
    {
        CreateAvatar();
    }

    void CreateAvatar()
    {
        Debug.Log("Ein neuer Avatar hat den Raum betreten.");
        int index = Random.Range(0, modelList.Count);
        networkPlayer = PhotonNetwork.Instantiate(modelList[index], new Vector3(0, 0, 0), Quaternion.identity, 0);
        networkPlayer.transform.parent = transform;
        cameraRig.transform.parent = networkPlayer.transform;
    }
}
