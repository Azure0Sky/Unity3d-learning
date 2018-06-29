using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TankNetworkController : NetworkBehaviour {

    public GameObject cameraRigPrefab;
    public Material localColor;

    private CameraFollow cameraFollow;

	public override void OnStartLocalPlayer()
    {
        // Create and Set camera
        cameraFollow = Instantiate( cameraRigPrefab ).GetComponent<CameraFollow>();
        cameraFollow.SetTarget( transform );

        // Change the color of local player
        foreach ( Transform child in transform ) {
            if ( child.tag == "TankRenderer" ) {
                for ( int i = 0; i < child.childCount; ++i ) {
                    child.GetChild( i ).GetComponent<MeshRenderer>().materials[0].color = localColor.color;
                }
                break;
            }
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
