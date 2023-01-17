using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigPosition : MonoBehaviour
{

    void Start(){
        //XRRigPosition.transform.position = new Vector3(Random.Range(-6, 6), 0.1f, Random.Range(-6, 6));
    }

    public void AdjustPosition(int playerNr){
        //Set start position in game mode depending on playerNr
        this.transform.position = new Vector3(-1 + playerNr, this.transform.position.y, this.transform.position.z);
    }
}