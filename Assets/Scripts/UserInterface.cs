using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UserInterface : MonoBehaviour
{
    [Tooltip("AudioSource to be played when a UI button is clicked")]
    public AudioSource buttonSound;

    // Start is called before the first frame update
    void Start()
    {}

    // Update is called once per frame
    void Update()
    {
    }

    public void ButtonSound(){
        if(buttonSound)
            buttonSound.Play();
    }
}
