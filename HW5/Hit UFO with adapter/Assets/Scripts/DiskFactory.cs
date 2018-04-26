using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiskFactory : MonoBehaviour {

    List<DiskData> usedDisk = new List<DiskData>();
    List<DiskData> freeDisk = new List<DiskData>();
    GameObject diskPrefab;

    private DiskFactory() { }

    public DiskData GetDisk( int round )
    {
        diskPrefab = null;
        DiskData.DiskType type = ( DiskData.DiskType )Random.Range( 0, round );         // 随机获取飞碟种类
        
        for ( int i = 0; i < freeDisk.Count; ++i ) {
            if ( freeDisk[i].type == type ) {

                freeDisk[i].gameObject.SetActive( true );
                diskPrefab = freeDisk[i].gameObject;
                freeDisk.Remove( freeDisk[i] );
                break;

            }
        }

        if ( diskPrefab == null ) {

            Vector3 oriPos = new Vector3( 0, -10.0f, 0 );                               // 飞碟起始位置

            switch ( type ) {

                case DiskData.DiskType.normal:
                    diskPrefab = GameObject.Instantiate( Resources.Load( "Prefabs/Normal" ), oriPos, Quaternion.identity ) as GameObject;
                    break;

                case DiskData.DiskType.heavy:
                    diskPrefab = GameObject.Instantiate( Resources.Load( "Prefabs/Heavy" ), oriPos, Quaternion.identity ) as GameObject;
                    break;

                case DiskData.DiskType.fast:
                    diskPrefab = GameObject.Instantiate( Resources.Load( "Prefabs/Fast" ), oriPos, Quaternion.identity ) as GameObject;
                    break;

            }

        }

        usedDisk.Add( diskPrefab.GetComponent<DiskData>() );
        return diskPrefab.GetComponent<DiskData>();

    }

    public void FreeDisk( DiskData disk )
    {
        for ( int i = 0; i < usedDisk.Count; ++i ) {
            if ( usedDisk[i].gameObject.GetInstanceID() == disk.gameObject.GetInstanceID() ) {
                usedDisk.Remove( disk );
                freeDisk.Add( disk );

                disk.gameObject.SetActive( false );
                disk.gameObject.GetComponent<MeshRenderer>().enabled = true;
                disk.gameObject.GetComponent<CapsuleCollider>().enabled = true;

                break;
            }
        }
    }

    public void ResetAllDisksToMode( Fly.Mode mode )
    {
        for ( int i = 0; i < freeDisk.Count; ++i ) {
            Fly flyScript = freeDisk[i].gameObject.GetComponent<Fly>();
            flyScript.SetMode( mode );
            flyScript.Reset();
        }
    }

}
