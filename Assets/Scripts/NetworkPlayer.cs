using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.CoreUtils;
using UnityEngine.XR;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkPlayer : MonoBehaviourPun
{
    public Transform Head;
    //public Transform LeftHand;
    //public Transform RightHand;
    private PhotonView photonView;

    private Transform headRig;
    //private Transform rightHandRig;
    //private Transform leftHandRig;
    public Vector3 CameraOffset = new Vector3(0, 0, 0);


    void Start()
    {
        photonView = GetComponent<PhotonView>();
        XROrigin rig = FindObjectOfType<XROrigin>();
        headRig = rig.transform.Find("ViveCameraRig/Camera");
        //leftHandRig = rig.transform.Find("ViveCameraRig/LeftHand");
        //rightHandRig = rig.transform.Find("ViveCameraRig/RightHand");
        if (photonView.IsMine)
        {
            ChangeMyName("Teilnehmer-Nr.:" + PhotonNetwork.LocalPlayer.ActorNumber);
        }

        /*if (photonView.IsMine)
        {
            foreach (var item in GetComponentsInChildren<Renderer>())
            {
                item.enabled = false;
            }
        }*/
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            //rightHand.gameObject.SetActive(false);
            //leftHand.gameObject.SetActive(false);
            //head.gameObject.SetActive(false);

            //MapPosition(Head, headRig);
            //MapPosition(LeftHand, leftHandRig);
            //MapPosition(RightHand, rightHandRig);
            //Head.transform.position = new Vector3(headRig.position.x, headRig.position.y, headRig.position.z);

            CameraOffset.y = -2;
            MapPosition(Head, headRig, CameraOffset);
        }
    }

    void MapPosition(Transform target, Transform rigTransform, Vector3 cameraoffset)
    {
        target.position = rigTransform.position + cameraoffset;
        target.position = new Vector3(target.position.x, GetComponent<Rigidbody>().position.y, target.position.z);

        target.rotation = rigTransform.rotation;
        target.rotation = Quaternion.Euler(0.0f, target.eulerAngles.y, 0.0f);
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