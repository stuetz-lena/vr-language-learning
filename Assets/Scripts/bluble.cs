using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bluble : MonoBehaviour
{
    private float t;
    private float y;
    private float initialY;
    private float initialX;
    
    // Start is called before the first frame update
    void Start()
    {
        t = 0;
        initialY = transform.position.y;
        initialX = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        t += 0.02f;
        y = Mathf.Sin(t)*0.25f + initialY;
        transform.position =  new Vector3(initialX, y,  -1 * Time.deltaTime + transform.position.z);

        /*if (Keyboard.current[x].wasPressedThisFrame)
        {
            Debug.Log("up arrow key is held down");
        }

        if (Keyboard.current[d].wasPressedThisFrame)
        {
            Debug.Log("down arrow key is held down");
        }*/
    }

    private void OnCollisionEnter(Collision other) {
        if(other.collider.tag == "Player") {
            Destroy(this.gameObject);
        }
    }
}
