using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class TrafficLightsControllerTrigger : MonoBehaviourPun
{
    public GameObject TrafficLights;

    public void ChangeSize()
    {
        photonView.RequestOwnership();
        StartCoroutine(waiter());
    }

    IEnumerator waiter()
    {
        TrafficLights.transform.localScale = new Vector3(0.8f, 0.7f, 0.7f);
        yield return new WaitForSecondsRealtime(4);
        TrafficLights.transform.localScale = new Vector3(0.9f, 0.7f, 0.7f);
        yield return new WaitForSecondsRealtime(10);
        TrafficLights.transform.localScale = new Vector3(0.8f, 0.7f, 0.7f);
        yield return new WaitForSecondsRealtime(4);
        TrafficLights.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
    }
}
