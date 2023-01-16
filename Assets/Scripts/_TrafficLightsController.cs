using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightsController : MonoBehaviour
{
    public GameObject red;
    public GameObject green;
    public GameObject yellow;
    public GameObject ScaleObject;

    void Start()
    {
        red.SetActive(true);
        yellow.SetActive(false);
        green.SetActive(false);
    }

    public void Update()
    {
        if (ScaleObject.transform.localScale.x == 0.7f)
        {
            red.SetActive(true);
            yellow.SetActive(false);
            green.SetActive(false);
        }

        else if (ScaleObject.transform.localScale.x == 0.8f)
        {
            red.SetActive(false);
            yellow.SetActive(true);
            green.SetActive(false);
        }

        else if (ScaleObject.transform.localScale.x == 0.9f)
        {
            red.SetActive(false);
            yellow.SetActive(false);
            green.SetActive(true);
        }
    }
}