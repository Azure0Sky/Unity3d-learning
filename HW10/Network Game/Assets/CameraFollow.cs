using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform followingTarget;       // The target this camera follows
    public float dampTime = 0.2f;           // Approximate time for the camera to refocus

    private Vector3 movingVelocity = Vector3.zero;

    void FixedUpdate ()
    {
        if ( followingTarget != null ) {
            transform.position = Vector3.SmoothDamp( transform.position, followingTarget.position, ref movingVelocity, dampTime );
        }
	}

    public void SetTarget( Transform target )
    {
        followingTarget = target;
    }
}
