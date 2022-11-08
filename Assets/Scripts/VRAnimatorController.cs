using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRAnimatorController : MonoBehaviour
{
    public Transform PlayerPosition;
    public float speedTreshold = 0.1f;
    public float smoothing = 0.3f;
    private Animator animator;
    private Vector3 previousPos;

    void Start()
    {
        animator = GetComponent<Animator>();
        previousPos = PlayerPosition.position;
    }

    void Update()
    {
        Vector3 MoveSpeed = (PlayerPosition.position-previousPos)/Time.deltaTime;
        Vector3 headsetLocalSpeed = transform.InverseTransformDirection(MoveSpeed);
        previousPos = PlayerPosition.position;

        float previousDirectionX = animator.GetFloat("DirectionX");
        float previousDirectionY = animator.GetFloat("DirectionY");

        animator.SetBool("IsMoving", headsetLocalSpeed.magnitude > speedTreshold);

        animator.SetFloat("DirectionX", Mathf.Lerp(previousDirectionX, Mathf.Clamp(headsetLocalSpeed.x, -1, 1), smoothing));
        animator.SetFloat("DirectionY", Mathf.Lerp(previousDirectionY, Mathf.Clamp(headsetLocalSpeed.z, -1, 1), smoothing));
    }
}
