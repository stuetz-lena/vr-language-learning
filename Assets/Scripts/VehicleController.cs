using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleController : MonoBehaviour
{
    public GameObject Vehicle1;
    public GameObject Vehicle2;
    public GameObject Vehicle3;
    public GameObject ScaleObject;
    public float speed = 10.0f;

    void Start()
    {
        Vehicle1.transform.position = new Vector3(110.0f, -0.5f, 11.5f);
        Vehicle2.transform.position = new Vector3(130.0f, 0.0f, 11.5f);
        Vehicle3.transform.position = new Vector3(-60.0f, 0.0f, 8.0f);
    }

    void Update()
    {  //Vehicle 1
        if (Vehicle1.transform.position.x  >= -45.0f)
        {
            if (Vehicle1.transform.position.x >= 12.0f && Vehicle1.transform.position.x < 14.0f  && ScaleObject.transform.localScale.x >= 0.8f)
            {
                speed = 0.0f;
                Vehicle1.transform.Translate(Vector3.left * speed * Time.deltaTime);
                
            }
            else
            {
                speed = 10.0f;
                Vehicle1.transform.Translate(Vector3.left * speed * Time.deltaTime);
            }
        }
        else
        {
            Vehicle1.transform.position = new Vector3(110.0f, -0.5f, 11.5f);
        }

        //Vehicle 2
        if (Vehicle2.transform.position.x >= -35.0f)
        {
            if (Vehicle2.transform.position.x >= 22.0f && Vehicle2.transform.position.x < 24.0f && ScaleObject.transform.localScale.x >= 0.8f)
            {
                speed = 0.0f;
                Vehicle2.transform.Translate(Vector3.left * speed * Time.deltaTime);

            }
            else
            {
                speed = 10.0f;
                Vehicle2.transform.Translate(Vector3.left * speed * Time.deltaTime);
            }
        }
        else
        {
            Vehicle2.transform.position = new Vector3(130.0f, 0.0f, 11.5f);
        }

        //Vehicle 3
        if (Vehicle3.transform.position.x <= 120.0f)
        {
            if (Vehicle3.transform.position.x < -13.0f && ScaleObject.transform.localScale.x >= 0.8f)
            {
                speed = 0.0f;
                Vehicle3.transform.Translate(Vector3.right * speed * Time.deltaTime);

            }
            else
            {
                speed = 10.0f;
                Vehicle3.transform.Translate(Vector3.right * speed * Time.deltaTime);
            }
        }
        else
        {
            Vehicle3.transform.position = new Vector3(-60.0f, 0.0f, 8.0f);
        }
    }
}
