using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolFactory : MonoBehaviour
{
    public GameObject patrol;       // 预制
    
    public List<GameObject> GetPatrols()
    {
        List<GameObject> patrols = new List<GameObject>();

        GameObject newPatrol = Instantiate<GameObject>( patrol, new Vector3( 16, 0, -32 ), Quaternion.identity );
        patrols.Add( newPatrol );

        newPatrol = Instantiate<GameObject>( patrol, new Vector3( -28, 0, -32 ), Quaternion.identity );
        patrols.Add( newPatrol );

        newPatrol = Instantiate<GameObject>( patrol, new Vector3( -28, 0, 0 ), Quaternion.identity );
        patrols.Add( newPatrol );

        newPatrol = Instantiate<GameObject>( patrol, new Vector3( 28, 0, 0 ), Quaternion.identity );
        patrols.Add( newPatrol );

        newPatrol = Instantiate<GameObject>( patrol, new Vector3( 0, 0, 32 ), Quaternion.identity );
        patrols.Add( newPatrol );

        newPatrol = Instantiate<GameObject>( patrol, new Vector3( 0, 0, 16 ), Quaternion.identity );
        patrols.Add( newPatrol );

        return patrols;
    }
}
