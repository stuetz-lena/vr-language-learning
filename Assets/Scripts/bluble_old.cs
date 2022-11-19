using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Code from fist attempt
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
    }

    private void OnCollisionEnter(Collision other) {
        if(other.collider.tag == "Player") {
            Destroy(this.gameObject);
        }
    }
}
