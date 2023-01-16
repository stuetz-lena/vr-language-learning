using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerName : MonoBehaviourPun, IPunObservable
{
    void Start()
    {
        if (photonView.IsMine)
        {
            ChangeMyName("Player " + PhotonNetwork.LocalPlayer.ActorNumber);
        }
    }

    void Update(){
        this.gameObject.transform.rotation = Camera.main.transform.rotation;
    }

    void ChangeMyName(string name)
    {
        this.gameObject.name = name;
        transform.Find("NameUI").gameObject.GetComponent<TextMesh>().text = name;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            string myName = this.gameObject.name;
            stream.SendNext(myName);
        }
        else
        {
            string otherName = (string)stream.ReceiveNext();
            this.gameObject.name = otherName;
            transform.Find("NameUI").gameObject.GetComponent<TextMesh>().text = otherName;
        }
    }
}
