using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UfoMovement : MonoBehaviour
{
    [Tooltip("Enable/Disable whole robo floating")]
    public bool floating = true;
    [Tooltip("Enable/Disable robo moving in circles")]
    public bool circleMovement = false;
   
    [Tooltip("Whole body floating speed factor")]
    public float fSpeed = 1f;
    [Tooltip("Circluar movement speed factor")]
    public float cspeed = 0.5f;
    [Tooltip("Circluar movement radius")]
    public float cradius = 5.0f;

    private float ctime; //for circlular movement
    private float ftime; //for floating
    
    private float initialY; 
    private float initialX; 
    private float initialZ;

    // Start is called before the first frame update
    void Start()
    {
        ftime = 0.0f;
        ctime = 0.0f;
        initialY = this.transform.position.y; 
        initialX = this.transform.position.x;
        initialZ = this.transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        if(circleMovement && floating) { //Move up and down in a circle depending on the radius
            ctime += Time.deltaTime*cspeed;
            ftime += 0.02f;
            float x = Mathf.Cos(ctime)* cradius + (initialX+cradius);
            float z = Mathf.Sin(ctime)* cradius + (initialZ+cradius);
            float y = Mathf.Sin(ftime)*fSpeed + initialY;
            this.transform.position = new Vector3(x, y, z);
        } else {
            if(circleMovement){ //Move in a circle depending on radius
                ctime += Time.deltaTime*cspeed;
                float x = Mathf.Cos(ctime)* cradius + (initialX+cradius);
                float z = Mathf.Sin(ctime)* cradius + (initialZ+cradius);
                float y = initialY;
                this.transform.position = new Vector3(x, y, z);
            }
            if(floating){ //Move up and down over time
                ftime += 0.02f;
                float y = Mathf.Sin(ftime)*fSpeed + initialY;
                transform.position = new Vector3(initialX, y, initialZ); 
            }
        }
    }
}