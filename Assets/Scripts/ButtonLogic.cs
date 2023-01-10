using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonLogic : MonoBehaviour
{
    public GameObject openWindow;
    public GameObject closeWindow;
    int n;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
        openWindow.SetActive(true);
        closeWindow.SetActive(false);
    }


}
