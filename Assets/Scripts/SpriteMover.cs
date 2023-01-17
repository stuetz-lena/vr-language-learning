using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteMover : MonoBehaviour
{
    [Tooltip("Enable/disable vertical movement (either or to horizontal)")][SerializeField]
    bool vertical = false;
    [Tooltip("Enable/disable horizontal movement (either or to vertical)")][SerializeField]
    bool horizontal = true;
    [Tooltip("Movement time factor")][SerializeField]
    float timeFactor = 5.0f;
    [Tooltip("Horizontal movement distance")][SerializeField]
    float distance = 50.0f;

    bool direction;
    float counter;
    float initialX;
    float initialY;

    // Start is called before the first frame update
    void Start(){
        direction = true;
        counter = 0;
        initialX = this.transform.localPosition.x-(distance/2);
        initialY = this.transform.localPosition.y;
    }

    // Update is called once per frame
    void Update(){
        if(vertical){
            //Check if direction change is necessary
            if(counter <= 0.09){ //0.0 was never reached, prevention of negative values
                direction = true;
            } else if(counter >= 1){
                direction = false;
            }
            //Increase or decrease counter depending on direction
            if(direction == false){
                counter -= 0.1f*Time.deltaTime*timeFactor;
            } else{
                counter += 0.1f*Time.deltaTime*timeFactor;
            }
            this.transform.localPosition = new Vector3(this.transform.localPosition.x, initialY + (distance * counter), this.transform.localPosition.z);
        } else if(horizontal){
            //Check if direction change is necessary
            if(counter <= 0.09){ //0.0 was never reached, prevention of negative values
                direction = true;
            } else if(counter >= 1){
                direction = false;
            }
            //Increase or decrease counter depending on direction
            if(direction == false){
                counter -= 0.1f*Time.deltaTime*timeFactor;
            } else{
                counter += 0.1f*Time.deltaTime*timeFactor;
            }
            this.transform.localPosition = new Vector3(initialX + (distance * counter), this.transform.localPosition.y, this.transform.localPosition.z);
        }
    }
}