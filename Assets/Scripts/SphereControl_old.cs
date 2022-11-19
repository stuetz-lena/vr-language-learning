using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Code from fist attempt
public class SphereControl : MonoBehaviour
{

    private Vector3 screenPoint;
    private Vector3 offset;
    private Collider thisObject;
    private int points = 0;

    public float movementSpeed = 0.1f;
    public float topBound = 30;
    public float bottomBound = -20;

     private float t;
    private float y;
    private float initialY;
    private float initialX;

    void Start() {
        thisObject = gameObject.GetComponent<Collider>();
         t = 0;
        initialY = transform.position.y;
        initialX = transform.position.x;
    }

    void Update()
    {
        //transform.Translate(Vector3.forward * Time.deltaTime * movementSpeed*-1);
         t += 0.02f;
        y = Mathf.Sin(t)*0.25f + initialY;
        transform.position =  new Vector3(initialX, y,  -1 * Time.deltaTime + transform.position.z);

        /*if(transform.position.z > topBound)
        {
            Destroy(gameObject);
        } else if (transform.position.z < bottomBound)
        {
            Destroy(gameObject);
        }*/
    }

    //Steuerung mit der Maus muss durch Steuerung mit den VR Controllern ersetzt werden
    void OnMouseDown()
    {
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
    
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
    
    }
    
    //Steuerung mit der Maus muss durch Steuerung mit den VR Controllern ersetzt werden
    void OnMouseDrag()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
    
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        transform.position = curPosition;
    
    }

     private void OnTriggerEnter(Collider other)
    {
        if ((other.CompareTag("Bucket_der") && thisObject.CompareTag("der")) || (other.CompareTag("Bucket_die") && thisObject.CompareTag("die")) || (other.CompareTag("Bucket_das") && thisObject.CompareTag("das")))
        {
            //Fehler bei der Punktezaehlung. Bleibt immer bei 1 haengen
            points++;
            Debug.Log("Richtig! Sie haben " + points + " Punkte.");
            gameObject.SetActive(false);
        }

        if ((other.CompareTag("Bucket_der") && thisObject.CompareTag("die")) || (other.CompareTag("Bucket_der") && thisObject.CompareTag("das")) || (other.CompareTag("Bucket_die") && thisObject.CompareTag("der")) || (other.CompareTag("Bucket_die") && thisObject.CompareTag("das")) || (other.CompareTag("Bucket_das") && thisObject.CompareTag("der")) || (other.CompareTag("Bucket_das") && thisObject.CompareTag("die")))
        {
            Debug.Log("Leider falsch");
        }
    }
}