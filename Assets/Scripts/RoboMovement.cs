using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoboMovement : MonoBehaviour
{
    [Tooltip("Enable/Disable arms movement")]
    public bool armMovement = true;
    [Tooltip("Enable/Disable whole robo floating")]
    public bool floating = true;
    [Tooltip("Enable/Disable body movement")]
    public bool bodyMovement = true;
    [Tooltip("Enable/Disable robo moving in circles")]
    public bool circleMovement = false;
    [Tooltip("Enable/Disable head movement")]
    public bool headMovement = true;

    private float ctime; //for ciclular
    private float ftime; //for floating
    private float hcounter; //for headmovement
    private float bcounter; //for bodymovement
    private bool hdirection; //for headmovement
    private bool bdirection; //for bodymovement
    private float acounter; //for armmovement
    private bool adirection; //for armmovement

    [Tooltip("Movement speed factor of head")]
    public float headMovementSpeed = 3;
    [Tooltip("Movement speed factor of body")]
    public float bodyMovementSpeed = 3;
    [Tooltip("Body movement from degree (+/-)")]
    public float bfrom = -20.0f;
    [Tooltip("Body movement to degree (+/-)")]
    public float bto = 20.0f;
    [Tooltip("Head movement from degree (+/-)")]
    public float hfrom = -90.0f;
    [Tooltip("Head movement to degree (+/-)")]
    public float hto = -75.0f;
    [Tooltip("Whole body floating speed factor")]
    public float floatingSpeed = 0.25f;
    [Tooltip("Circluar movement speed factor")]
    public float cspeed = 0.5f;
    [Tooltip("Arms movement speed factor")]
    public float aspeed = 0.5f;
    [Tooltip("Circluar movement radius")]
    public float cradius = 2.0f;
    [Tooltip("Arms movement from degree (+/-)")]
    public float afrom = -39.0f;
    [Tooltip("Arms movement to dregree (+/-)")]
    public float ato = -12.0f;

    private float initialY; 
    private float initialX; 
    private float initialZ;
    Transform arms; 
    Transform body;
    Transform head;

    // Start is called before the first frame update
    void Start()
    {
        ftime = 0.0f;
        ctime = 0.0f;
        hdirection = true;
        hcounter = 0.0f;
        bdirection = true;
        bcounter = 0.0f;
        acounter = 0.0f;
        adirection = true;

        initialY = this.transform.position.y; 
        initialX = this.transform.position.x;
        initialZ = this.transform.position.z;

        arms = this.transform.Find("arms");
        body = this.transform.Find("body");
        head = this.transform.Find("Cube");

        /*//to = new Vector3(8.359f, 5.586f+33.399f, 174.944f); //new Vector3(5.868f,6.012f,174.579f);
        ato = new Vector3(-24.662f, 9.053f+33.399f, 174.064f); //new Vector3(5.868f,6.012f,174.579f);
        afrom = new Vector3(-39.015f,10.955f,173.053f); // arms.transform.eulerAngles;*/
    }

    // Update is called once per frame
    void Update()
    {
        if(armMovement){
            if(acounter <= 0.09){
                adirection = true;
            } else if(acounter >= 1){
                adirection = false;
            }

            if(adirection == false){
                acounter -= Time.deltaTime*aspeed;
            } else
            {
                acounter += Time.deltaTime*aspeed;
            }
            arms.transform.rotation = Quaternion.Euler(Mathf.Lerp(afrom, ato, acounter),arms.transform.eulerAngles.y,arms.transform.eulerAngles.z);
            //arms.transform.eulerAngles = Vector3.Lerp(afrom, ato, acounter); //Mathf.Abs(Mathf.Sin(armstime)
        }
        if(headMovement){
            if(hcounter <= 0.09){
                hdirection = true;
            } else if(hcounter >= 1){
                hdirection = false;
            }

            if(hdirection == false){
                hcounter -= 0.1f*Time.deltaTime*headMovementSpeed;
            } else
            {
                hcounter += 0.1f*Time.deltaTime*headMovementSpeed;
            }

            head.transform.rotation = Quaternion.Euler(Mathf.Lerp(hfrom, hto, hcounter),head.transform.eulerAngles.y,head.transform.eulerAngles.z);
            //head.transform.eulerAngles = new Vector3(hfrom + (hto-hfrom)*hcounter, head.transform.eulerAngles.y, head.transform.eulerAngles.z);
        }
        if(circleMovement && floating) {
            ctime += Time.deltaTime*cspeed;
            ftime += 0.02f;
            float x = Mathf.Cos(ctime)* cradius + (initialX+cradius);
            float z = Mathf.Sin(ctime) * cradius + (initialZ+cradius);
            float y = Mathf.Sin(ftime)*floatingSpeed + initialY;
            this.transform.position = new Vector3(x, y, z);
 
        } else {
            if(floating){
                ftime += 0.02f;
                float y = Mathf.Sin(ftime)*floatingSpeed + initialY;
                transform.position = new Vector3(initialX, y,  initialZ); 
            }
            if(circleMovement){
                ctime += Time.deltaTime*cspeed;
                float x = Mathf.Cos(ctime)* cradius + (initialX+cradius);
                float z = Mathf.Sin(ctime) * cradius + (initialZ+cradius);
                float y = initialY;
                this.transform.position = new Vector3(x, y, z);
            }
        }
        
        if(bodyMovement){
            if(bcounter <= 0.09){
                bdirection = true;
            } else if(bcounter >= 1){
                bdirection = false;
            }

            if(bdirection == false){
                bcounter -= 0.1f*Time.deltaTime*bodyMovementSpeed;
            } else
            {
                bcounter += 0.1f*Time.deltaTime*bodyMovementSpeed;
            }
            body.transform.rotation = Quaternion.Euler(body.transform.eulerAngles.x,this.transform.eulerAngles.y,Mathf.Lerp(bfrom, bto, bcounter));
        }  
    }
}
