using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsActionManager : MonoBehaviour, IActionManager
{
    DiskController diskController;

    // Use this for initialization
    void Start ()
    {
        diskController = Singleton<DiskController>.Instance;	
	}

    public void PlayDisk( DiskData disk )
    {
        diskController.Launch( disk, Fly.Mode.Physics );
    }
}
