using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UserInterface : MonoBehaviour
{
    public static UserInterface Instance;

    [Tooltip("The startscreen to be shown initially.")]
    public GameObject startScreen;
    [Tooltip("Canvas with the start button.")]
    public GameObject startButtonCanvas;
    [Tooltip("The screen shown while the game is paused.")]
    public GameObject pauseMenu;
    [Tooltip("The canvas with the head up display.")]
    public GameObject HUD;
    [Tooltip("The pause button within the HUD.")]
    public GameObject HUDPauseButton;
    [Tooltip("The next button within the HUD.")]
    public GameObject HUDNextButton;
    
    [Tooltip("GameObject with the AudioSource for the menu.")]
    public GameObject menuSound;
    [Tooltip("AudioSource to be played when a UI button is clicked.")]
    public AudioSource buttonSound;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        startScreen.SetActive(true);
    }

    // Update is called once per frame
    void Update(){}

    public void TriggerStartButton(bool value){
        //startButtonCanvas.transform.position =  new Vector3(Camera.main.transform.position.x,startButtonCanvas.transform.position.y,startButtonCanvas.transform.position.z);
        startButtonCanvas.SetActive(value);
    }

    public void TriggerHUD(bool value){
        HUD.SetActive(value);
    }

    public void TriggerPauseMenu(bool value){
        pauseMenu.SetActive(value);
    }

    public void TriggerPauseButton(bool value){
        HUDPauseButton.SetActive(value);
    }

    public void TriggerNextButton(bool value){
        HUDNextButton.SetActive(value);
    }

    public void ButtonSound(){
        if(buttonSound)
            buttonSound.Play();
    }

    public void RepositionCanvas(){
        this.transform.position = new Vector3(Camera.main.transform.position.x, this.transform.position.y, this.transform.position.z); //position canvas based on camera x
    }

    public void QuitGame(){ //managing the UI needs to be done via script to be synced on all clients
        menuSound.SetActive(true);
        startScreen.SetActive(true);
        pauseMenu.SetActive(false);
        HUD.SetActive(false);
        HUDPauseButton.SetActive(true);
        HUDNextButton.SetActive(false);
    }
}
