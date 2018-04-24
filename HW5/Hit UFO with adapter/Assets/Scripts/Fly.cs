using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fly : MonoBehaviour {

    float power;
    float verticalAngle;                    // 竖直方向角度
    float horizontalAngle;                  // 水平方向角度
    float gravity;

    Vector3 moveSpeed;
    Vector3 gravitySpeed;
    Vector3 currentAngle;

    float time;                         // 已经过去的时间

    private void Start()
    {
        Reset();
    }

    private void FixedUpdate () {

        time += Time.fixedDeltaTime;
        gravitySpeed.y = gravity * time;

        transform.position += ( moveSpeed + gravitySpeed ) * Time.fixedDeltaTime;
        currentAngle.x = -Mathf.Atan( ( moveSpeed.y + gravitySpeed.y ) / moveSpeed.z ) * Mathf.Rad2Deg;
        transform.eulerAngles = currentAngle;

    }

    public void Reset()
    {
        if ( gameObject.GetComponent<DiskData>().type == DiskData.DiskType.fast ) {
            power = Random.Range( 9f, 18f );
        } else {
            power = Random.Range( 5f, 10f );
        }

        verticalAngle = Random.Range( 30f, 40f );

        if ( gameObject.transform.position.x < 0 ) {
            horizontalAngle = Random.Range( -5f, 40f );
        } else {
            horizontalAngle = Random.Range( -40f, 5f );
        }

        if ( gameObject.GetComponent<DiskData>().type == DiskData.DiskType.strong ) {
            gravity = -10;
        } else {
            gravity = -5;
        }

        moveSpeed = Quaternion.Euler( new Vector3( -verticalAngle, horizontalAngle, 0 ) ) * Vector3.forward * power;

        gravitySpeed = Vector3.zero;
        currentAngle = Vector3.zero;

        time = 0;
    }
}
