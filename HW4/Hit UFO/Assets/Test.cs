using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        Camera cam = GetComponent<Camera>();
        if ( Input.GetButtonDown( "Fire1" ) ) {

            Debug.Log( Input.mousePosition );

            Ray ray = cam.ScreenPointToRay( Input.mousePosition );
            //RaycastHit hit;
            Debug.DrawLine( ray.origin, ray.direction, Color.red, 2.0f );

        }

	}
}
