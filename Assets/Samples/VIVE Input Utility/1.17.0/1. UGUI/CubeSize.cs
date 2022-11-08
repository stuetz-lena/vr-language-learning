using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSize : MonoBehaviour
{
    public GameObject cube;

    public void OnClickVRButton()
    {
        // Get the Renderer component from the new cube
        var cubeRenderer = cube.GetComponent<Renderer>();

        // Create a new RGBA color using the Color constructor and store it in a variable
        Color customColor = new Color(255, 234, 0, 255);

        // Call SetColor using the shader property name "_Color" and setting the color to red
        cubeRenderer.material.SetColor("_Color", customColor);
    }
}
