using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigPosition : MonoBehaviour
{
    public Transform XRRigPosition;

    void Start()
    {
        //XRRigPosition.transform.position = new Vector3(Random.Range(-6, 6), 0.1f, Random.Range(-6, 6));
        XRRigPosition.transform.position = new Vector3(Random.Range(-6, 6), 0.25f,-14.5f);
    }
}
