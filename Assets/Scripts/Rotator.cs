using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    private float rot;

    // Start is called before the first frame update
    void Start()
    {
        rot = 100.0f;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(0.0f, rot * Time.deltaTime, 0.0f);
    }
}
