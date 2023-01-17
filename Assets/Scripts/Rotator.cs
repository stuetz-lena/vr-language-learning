using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    float rotation;

    // Start is called before the first frame update
    void Start(){
        rotation = 100.0f;
    }

    // Update is called once per frame
    void Update(){
        this.transform.Rotate(0.0f, rotation * Time.deltaTime, 0.0f);
    }
}