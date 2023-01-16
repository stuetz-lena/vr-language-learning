using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;

public class ButtonController : MonoBehaviour
{
    public XRNode node;

    // Update is called once per frame
    void Update()
    {

        UnityEngine.XR.Interaction.Toolkit.InputHelpers.IsPressed(InputDevices.GetDeviceAtXRNode(node), UnityEngine.XR.Interaction.Toolkit.InputHelpers.Button.Trigger, out bool triggerValue, 0.5f);
        if (triggerValue)
        {
            Debug.Log("trigger");
        }

        UnityEngine.XR.Interaction.Toolkit.InputHelpers.IsPressed(InputDevices.GetDeviceAtXRNode(node), UnityEngine.XR.Interaction.Toolkit.InputHelpers.Button.Grip, out bool gripPressed, 0.5f);
        if (gripPressed)
        {
            Debug.Log("grip");
        }
        UnityEngine.XR.Interaction.Toolkit.InputHelpers.IsPressed(InputDevices.GetDeviceAtXRNode(node), UnityEngine.XR.Interaction.Toolkit.InputHelpers.Button.PrimaryAxis2DUp, out bool touchpadValue, 0.5f);
        if (touchpadValue)
        {
            Debug.Log("touchpad");
        }

        UnityEngine.XR.Interaction.Toolkit.InputHelpers.IsPressed(InputDevices.GetDeviceAtXRNode(node), UnityEngine.XR.Interaction.Toolkit.InputHelpers.Button.PrimaryButton, out bool primaryButton, 0.5f);
        if (primaryButton)
        {
            Debug.Log("primary");
        }

        UnityEngine.XR.Interaction.Toolkit.InputHelpers.IsPressed(InputDevices.GetDeviceAtXRNode(node), UnityEngine.XR.Interaction.Toolkit.InputHelpers.Button.SecondaryButton, out bool secondaryButton, 0.5f);
        if (secondaryButton)
        {
            Debug.Log("secondary");
            //new function in update if is dragged & secondary > show hint
        }
    }
}