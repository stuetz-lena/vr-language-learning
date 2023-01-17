using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UfoMovement : MonoBehaviour
{
    [Tooltip("Enable/disable floating")][SerializeField]
    bool floating = true;
    [Tooltip("Enable/disable moving in circles")][SerializeField]
    bool circleMovement = false;
   
    [Tooltip("Floating speed factor")][SerializeField]
    float fSpeed = 1f;
    [Tooltip("Circluar movement speed factor")][SerializeField]
    float cspeed = 0.5f;
    [Tooltip("Circluar movement radius")][SerializeField]
    float cradius = 5.0f;

    float ctime; //for circlular movement
    float ftime; //for floating
    
    float initialY; 
    float initialX; 
    float initialZ;

    // Start is called before the first frame update
    void Start(){
        ftime = 0.0f;
        ctime = 0.0f;
        initialY = this.transform.position.y; 
        initialX = this.transform.position.x;
        initialZ = this.transform.position.z;
    }

    // Update is called once per frame
    void Update(){
        if(circleMovement && floating) { //Move up and down in a circle depending on the radius
            ctime += Time.deltaTime*cspeed;
            ftime += 0.02f;
            float x = Mathf.Cos(ctime)*cradius + (initialX+cradius);
            float z = Mathf.Sin(ctime)*cradius + (initialZ+cradius);
            float y = Mathf.Sin(ftime)*fSpeed + initialY;
            this.transform.position = new Vector3(x, y, z);
        } else {
            if(circleMovement){ //Move in a circle depending on radius
                ctime += Time.deltaTime*cspeed;
                float x = Mathf.Cos(ctime)*cradius + (initialX+cradius);
                float z = Mathf.Sin(ctime)*cradius + (initialZ+cradius);
                this.transform.position = new Vector3(x, initialY, z);
            }
            if(floating){ //Move up and down over time
                ftime += 0.02f;
                float y = Mathf.Sin(ftime)*fSpeed + initialY;
                this.transform.position = new Vector3(initialX, y, initialZ); 
            }
        }
    }
}