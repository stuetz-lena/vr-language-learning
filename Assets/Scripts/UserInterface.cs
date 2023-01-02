using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UserInterface : MonoBehaviour
{
    public GameObject startscreen;
    public GameObject chooseLanguage;
    public GameObject playmode;
    public GameObject level;
    public GameObject letsGo;
    public GameObject hud;
    public GameObject featureAvailableSoon_Lang;
    public GameObject featureAvailableSoon_Mode;
    public GameObject featureAvailableSoon_Level;

    // Start is called before the first frame update
    void Start()
    {
        startscreen.SetActive(true);
        chooseLanguage.SetActive(false);
        playmode.SetActive(false);
        level.SetActive(false);
        letsGo.SetActive(false);
        hud.SetActive(false);
        featureAvailableSoon_Lang.SetActive(false);
        featureAvailableSoon_Mode.SetActive(false);
        featureAvailableSoon_Level.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
