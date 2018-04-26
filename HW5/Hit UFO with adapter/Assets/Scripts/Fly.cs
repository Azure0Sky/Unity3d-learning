using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fly : MonoBehaviour {

    public enum Mode { Kinematics, Physics };

    float gravity;

    Vector3 moveSpeed;
    Vector3 gravitySpeed;
    Vector3 currentAngle;

    float time;                             // 已经过去的时间，用于运动学模式

    Mode mode;

    private void Start()
    {
        Debug.Log( "Fly Start" );
        Reset();
    }

    private void FixedUpdate ()
    {
        if ( mode == Mode.Physics ) {
            return;
        }

        time += Time.fixedDeltaTime;
        gravitySpeed.y = gravity * time;

        transform.position += ( moveSpeed + gravitySpeed ) * Time.fixedDeltaTime;
        currentAngle.x = -Mathf.Atan( ( moveSpeed.y + gravitySpeed.y ) / moveSpeed.z ) * Mathf.Rad2Deg;
        transform.eulerAngles = currentAngle;
    }

    public void Reset()
    {
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        gameObject.transform.rotation = Quaternion.Euler( Vector3.zero );

        if ( mode == Mode.Kinematics ) {
            SetToKinematics();
        } else {
            SetToPhysics();
        }
    }

    public void SetMode( Mode mode )
    {
        this.mode = mode;
    }

    private void SetToKinematics()
    {
        float power;
        float verticalAngle;                    // 竖直方向角度
        float horizontalAngle;                  // 水平方向角度

        gameObject.GetComponent<Rigidbody>().isKinematic = true;

        if ( gameObject.GetComponent<DiskData>().type == DiskData.DiskType.fast ) {
            power = Random.Range( 15f, 21f );
        } else {
            power = Random.Range( 12f, 15f );
        }

        verticalAngle = Random.Range( 20f, 30f );

        if ( gameObject.transform.position.x < 0 ) {
            horizontalAngle = Random.Range( -5f, 40f );
        } else {
            horizontalAngle = Random.Range( -40f, 5f );
        }

        if ( gameObject.GetComponent<DiskData>().type == DiskData.DiskType.heavy ) {
            gravity = -15;
        } else {
            gravity = -7;
        }

        moveSpeed = Quaternion.Euler( new Vector3( -verticalAngle, horizontalAngle, 0 ) ) * Vector3.forward * power;

        gravitySpeed = Vector3.zero;
        currentAngle = Vector3.zero;

        time = 0;
    }

    private void SetToPhysics()
    {
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
    }

    public void AddForeceToDisk()
    {
        Vector3 force;
        Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();

        if ( gameObject.GetComponent<DiskData>().type == DiskData.DiskType.fast ) {
            force = new Vector3( 0, Random.Range( 10f, 15f ), Random.Range( 17f, 28f ) );
        } else {
            force = new Vector3( 0, Random.Range( 7f, 10f ), Random.Range( 11f, 15f ) );
        }

        if ( gameObject.transform.position.x < 0 ) {
            force.x = force.z / Mathf.Tan( Random.Range( 50f, 100f ) * Mathf.Deg2Rad );
        } else {
            force.x = -force.z / Mathf.Tan( Random.Range( 50f, 100f ) * Mathf.Deg2Rad );
        }

        rigidbody.AddForce( force, ForceMode.Impulse );
        rigidbody.AddTorque( Vector3.right * 3 );

        Debug.Log( force );
    }
}
