using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoboMovement : MonoBehaviour
{
    [Tooltip("Enable/disable arms movement")][SerializeField]
    bool armMovement = true;
    [Tooltip("Enable/disable robo floating")][SerializeField]
    bool floating = true;
    [Tooltip("Enable/disable body movement")][SerializeField]
    bool bodyMovement = true;
    [Tooltip("Enable/disable robo moving in circles")][SerializeField]
    bool circleMovement = false;
    [Tooltip("Enable/disable head movement")][SerializeField]
    bool headMovement = true;

    [Tooltip("Movement speed factor of body")][SerializeField]
    float bspeed = 0.4f;
    [Tooltip("Body movement from degree (+/-)")][SerializeField]
    float bfrom = -20.0f;
    [Tooltip("Body movement to degree (+/-)")][SerializeField]
    float bto = 20.0f;
    [Tooltip("Movement speed factor of head")][SerializeField]
    float hspeed = 0.3f;
    [Tooltip("Head movement from degree (+/-)")][SerializeField]
    float hfrom = -90.0f;
    [Tooltip("Head movement to degree (+/-)")][SerializeField]
    float hto = -75.0f;
    [Tooltip("Floating speed factor")][SerializeField]
    float fSpeed = 0.25f;
    [Tooltip("Circluar movement speed factor")][SerializeField]
    float cspeed = 0.5f;
    [Tooltip("Circluar movement radius")][SerializeField]
    float cradius = 2.0f;
    [Tooltip("Movement speed factor of arms")][SerializeField]
    float aspeed = 0.5f;
    [Tooltip("Arms movement from degree (+/-)")][SerializeField]
    float afrom = -39.0f;
    [Tooltip("Arms movement to degree (+/-)")][SerializeField]
    float ato = -12.0f;

    float ctime; //for circlular movement
    float ftime; //for floating
    float hcounter; //for head movement
    bool hdirection; //for head movement
    float bcounter; //for body movement
    bool bdirection; //for body movement
    float acounter; //for armm ovement
    bool adirection; //for arm movement

    float initialX; 
    float initialY; 
    float initialZ;

    Transform arms; 
    Transform body;
    Transform head;

    // Start is called before the first frame update
    void Start(){
        ctime = 0.0f;
        ftime = 0.0f;
        hcounter = 0.0f;
        hdirection = true;
        bcounter = 0.0f;
        bdirection = true;
        acounter = 0.0f;
        adirection = true;

        initialX = this.transform.position.x;
        initialY = this.transform.position.y; 
        initialZ = this.transform.position.z;

        arms = this.transform.Find("arms");
        body = this.transform.Find("body");
        head = this.transform.Find("head");
    }

    // Update is called once per frame
    void Update(){
        if(armMovement){ //Arms moving from afrom to ato and reverse
            //Check if direction change is necessary at the borders 0 and 1 for Math Lerp interpolation
            if(acounter <= 0.09){ //0.0 was never reached, prevention of negative values
                adirection = true;
            } else if(acounter >= 1){
                adirection = false;
            }
            //Increase or decrease counter depending on direction
            if(adirection == false){
                acounter -= Time.deltaTime*aspeed;
            } else{
                acounter += Time.deltaTime*aspeed;
            }
            //Apply
            arms.transform.rotation = Quaternion.Euler(Mathf.Lerp(afrom, ato, acounter),arms.transform.eulerAngles.y,arms.transform.eulerAngles.z);
        }

        if(headMovement){ //Head moving from hfrom to hto and reverse
            //Check if direction change is necessary at the borders 0 and 1 for Math Lerp interpolation
            if(hcounter <= 0.09){ //0.0 was never reached, prevention of negative values
                hdirection = true;
            } else if(hcounter >= 1){
                hdirection = false;
            }
            //Increase or decrease counter depending on direction
            if(hdirection == false){
                hcounter -= Time.deltaTime*hspeed;
            } else{
                hcounter += Time.deltaTime*hspeed;
            }
            //Apply
            head.transform.rotation = Quaternion.Euler(Mathf.Lerp(hfrom, hto, hcounter),head.transform.eulerAngles.y,head.transform.eulerAngles.z);
        }

        if(bodyMovement){ //Body moving from bfrom to bto and reverse
            //Check if direction change is necessary at the borders 0 and 1 for Math Lerp interpolation
            if(bcounter <= 0.09){ //0.0 was never reached, prevention of negative values
                bdirection = true;
            } else if(bcounter >= 1){
                bdirection = false;
            }
            //Increase or decrease counter depending on direction
            if(bdirection == false){
                bcounter -= Time.deltaTime*bspeed;
            } else{
                bcounter += Time.deltaTime*bspeed;
            }
            //Apply
            body.transform.rotation = Quaternion.Euler(body.transform.eulerAngles.x,this.transform.eulerAngles.y,Mathf.Lerp(bfrom, bto, bcounter));
        }  

        if(circleMovement && floating) { //Moving up and down in a circle depending on the radius
            ctime += Time.deltaTime*cspeed;
            ftime += 0.02f;
            float x = Mathf.Cos(ctime)*cradius + (initialX+cradius);
            float z = Mathf.Sin(ctime)*cradius + (initialZ+cradius);
            float y = Mathf.Sin(ftime)*fSpeed + initialY;
            this.transform.position = new Vector3(x, y, z);
        } else {
            if(floating){ //Moving up and down over time
                ftime += 0.02f;
                float y = Mathf.Sin(ftime)*fSpeed + initialY;
                transform.position = new Vector3(initialX, y, initialZ); 
            }
            if(circleMovement){ //Moving in a circle depending on the radius
                ctime += Time.deltaTime*cspeed;
                float x = Mathf.Cos(ctime)*cradius + (initialX+cradius);
                float z = Mathf.Sin(ctime)*cradius + (initialZ+cradius);
                this.transform.position = new Vector3(x, initialY, z);
            }
        }
    }
}