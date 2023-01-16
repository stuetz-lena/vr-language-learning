using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UserInterface : MonoBehaviour
{
    public static UserInterface Instance;

    [Tooltip("Start canvas to be shown initially.")][SerializeField]
    GameObject startScreen;
    [Tooltip("Canvas with the start button for the master.")][SerializeField]
    GameObject startButtonMaster;
    [Tooltip("Canvas with the waiting info for clients.")][SerializeField]
    GameObject waitForMaster;
    [Tooltip("The canvas shown while the game is paused.")][SerializeField]
    GameObject pauseMenu;
    [Tooltip("The canvas with the head up display.")][SerializeField]
    GameObject HUD;
    [Tooltip("The pause button within the HUD.")][SerializeField]
    GameObject HUDPauseButton;
    [Tooltip("The next button within the HUD.")][SerializeField]
    GameObject HUDNextButton;
    [Tooltip("The first instruction canvas.")][SerializeField]
    GameObject instructions;
    [Tooltip("The second instruction canvas.")][SerializeField]
    GameObject instructions2;
    
    [Tooltip("GameObject with the AudioSource played during the menu.")][SerializeField]
    GameObject menuSound;
    [Tooltip("AudioSource to be played when a UI button is clicked.")][SerializeField]
    AudioSource buttonSound;

    // Start is called before the first frame update
    void Start(){
        Instance = this;
        startScreen.SetActive(true);
    }

    // Update is called once per frame
    void Update(){}

    public void TriggerStartButtonMaster(bool value){
        startButtonMaster.SetActive(value);
    }

    public void TriggerWaitForMaster(bool value){
        waitForMaster.SetActive(value);
    }

    public void TriggerPauseMenu(bool value){
        pauseMenu.SetActive(value);
    }

    public void TriggerHUD(bool value){
        HUD.SetActive(value);
    }

    public void TriggerPauseButton(bool value){
        HUDPauseButton.SetActive(value);
    }

    public void TriggerNextButton(bool value){
        HUDNextButton.SetActive(value);
    }

    public void TriggerInstructions(bool value){
        instructions.SetActive(value);
        instructions2.SetActive(value);
    }

    public void ButtonSound(){
        if(buttonSound)
            buttonSound.Play();
    }

    public void RepositionCanvas(){
        //positioning canvas based on camera x
        this.transform.position = new Vector3(Camera.main.transform.position.x, this.transform.position.y, this.transform.position.z); 
    }

    public void QuitGame(){ 
        //managing the UI needs to be done via script to be synced on all clients through a single click
        menuSound.SetActive(true);
        startScreen.SetActive(true);
        //using the existing functions in case they are extended later  
        TriggerPauseMenu(false); 
        TriggerHUD(false);
        TriggerPauseButton(true);
        TriggerNextButton(false);
    }
}