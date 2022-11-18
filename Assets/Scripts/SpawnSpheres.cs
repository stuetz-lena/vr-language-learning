using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSpheres : MonoBehaviour
{
    public GameObject [] words;
    public float xRange = 4;
    public float yRange = 10;
    public float startDelay = 2;
    public float spawnIntervall = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnSphere", startDelay, spawnIntervall);
    }

    // Update is called once per frame
    void SpawnSphere ()
    {
        int randomIndex = Random.Range(0, words.Length);
        float randomXPosition = Random.Range(-xRange, xRange);
        float randomYPosition = Random.Range(2, yRange);
        Instantiate(words[randomIndex], new Vector3(randomXPosition, randomYPosition, 20), words[0].transform.rotation);
    }
}