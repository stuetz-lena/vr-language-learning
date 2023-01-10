using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteMover : MonoBehaviour
{
    [Tooltip("Enable/Disable vertical movement (either or to horizontal)")]
    public bool vertical = false;
    [Tooltip("Enable/Disable horizontal movement (either or to vertical)")]
    public bool horizontal = true;

    private bool direction;
    private float counter;
    private float initialX;
    private float initialY;

    // Start is called before the first frame update
    void Start()
    {
        direction = true;
        counter = 0;
        initialX = this.transform.localPosition.x-25;
        initialY = this.transform.localPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        if(vertical){
            if(counter <= 0.09){ //0.0 was never reached, prevent negative values
                direction = true;
            } else if(counter >= 1){
                direction = false;
            }
            //Increase or decrease counter depending on direction
            if(direction == false){
                counter -= 0.1f*Time.deltaTime*5;
            } else{
                counter += 0.1f*Time.deltaTime*5;
            }
            this.transform.localPosition = new Vector3(this.transform.localPosition.x, initialY + (50.0f * counter), this.transform.localPosition.z);
        }
        if(horizontal){
            if(counter <= 0.09){ //0.0 was never reached, prevent negative values
                direction = true;
            } else if(counter >= 1){
                direction = false;
            }
            //Increase or decrease counter depending on direction
            if(direction == false){
                counter -= 0.1f*Time.deltaTime*5;
            } else{
                counter += 0.1f*Time.deltaTime*5;
            }
            this.transform.localPosition = new Vector3(initialX + (50.0f * counter),this.transform.localPosition.y,this.transform.localPosition.z);
        }
    }
}
