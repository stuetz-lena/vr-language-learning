using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoboMaterialController : MonoBehaviour
{
    [Tooltip("Material for robo to be set in case of correct sorting")]
    public Material green;
    
    private MeshRenderer myRenderer;
    private Material orgMaterial; 
    private Coroutine resetMaterial; //to handle sortings with a short time difference only one coroutine is allowed at a time

    // Start is called before the first frame update
    void Start()
    {
        myRenderer = this.GetComponent<MeshRenderer>();
        orgMaterial = myRenderer.material;
    }

    // Update is called once per frame
    void Update(){}

    public void RoboChangeMaterial(){
        myRenderer.material = green;
        if(resetMaterial != null) //if another coroutine exists already, we stop it before starting the new one
            StopCoroutine(resetMaterial);
        StartCoroutine(ResetMaterial(1));
    }

    private IEnumerator ResetMaterial(int time){
        yield return new WaitForSeconds(time);
        myRenderer.material = orgMaterial;
    }
}
