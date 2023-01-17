using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoboMaterialController : MonoBehaviour
{
    [Tooltip("Material for robo body in case of correct sorting.")][SerializeField]
    Material green;
    
    MeshRenderer myRenderer;
    Material orgMaterial; 
    Coroutine resetMaterial; //to handle sortings with short time differences, only one coroutine is allowed at a time

    // Start is called before the first frame update
    void Start(){
        myRenderer = this.GetComponent<MeshRenderer>();
        orgMaterial = myRenderer.material;
    }

    // Update is called once per frame
    void Update(){}

    public void RoboChangeMaterial(){
        myRenderer.material = green;
        if(resetMaterial != null) //if a coroutine exists, we stop it before starting a new one
            StopCoroutine(resetMaterial);
        resetMaterial = StartCoroutine(ResetMaterial(1));
    }

    IEnumerator ResetMaterial(int time){
        yield return new WaitForSeconds(time);
        myRenderer.material = orgMaterial;
    }
}