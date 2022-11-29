using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.CoreUtils;
using UnityEngine.XR;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;

//added due to positioning problem with Photon, not solved yet, Buckets and Score should spawn per player and be positioned correctly
//unfortunately the rig position is adjusted by RigPosition script and thereby changed after Bucket Spawn, copy of NetwordPlayer
public class BucketPositioner : MonoBehaviourPun
{
    public Transform Head;
    private PhotonView photonView;

    private Transform headRig;
    public Vector3 CameraOffset = new Vector3(0, 0, 0);


    void Start()
    {
        photonView = GetComponent<PhotonView>();
        XROrigin rig = FindObjectOfType<XROrigin>();
        headRig = rig.transform.Find("ViveCameraRig/Camera");
        if (photonView.IsMine)
        {
            ChangeMyName(this.GetComponent<Collider>().tag);
        }
        if (photonView.IsMine){
            CameraOffset.y = -2;
            StartCoroutine(MapPosition(Head, headRig, CameraOffset));
        }
    }

    void Update()
    {
        
    }

    IEnumerator MapPosition(Transform target, Transform rigTransform, Vector3 cameraoffset)
    {
        yield return new WaitForSeconds(5);
        target.position = rigTransform.position + cameraoffset;
        target.position = new Vector3(target.position.x-0.5f, GetComponent<Rigidbody>().position.y, target.position.z+1);

        target.rotation = rigTransform.rotation;
        target.rotation = Quaternion.Euler(0.0f, target.eulerAngles.y, 0.0f);
    }

    void ChangeMyName(string name)
    {
        this.gameObject.name = name;
        transform.Find("NameUI").gameObject.GetComponent<TextMesh>().text = name;
    }

    /*public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
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
    }*/
}