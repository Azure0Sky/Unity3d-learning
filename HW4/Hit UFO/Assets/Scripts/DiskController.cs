using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiskController : MonoBehaviour {

    DiskFactory diskFactory;

    private void Awake()
    {
        diskFactory = Singleton<DiskFactory>.Instance;
    }

    public void Explode( DiskData disk )
    {
        Transform explode = disk.gameObject.transform.GetChild( 0 );

        disk.gameObject.GetComponent<MeshRenderer>().enabled = false;               // 隐藏飞碟
        disk.gameObject.GetComponent<CapsuleCollider>().enabled = false;

        explode.GetComponent<ParticleSystem>().Play();                              // 爆炸效果

        StartCoroutine( WaitForExplode( disk ) );                                   // 协程，等待爆炸效果结束
    }

    public void Launch( DiskData disk )
    {
        float pos_x = Random.Range( -6f, 6f );
        float pos_y = Random.Range( -1.5f, -1f );

        disk.transform.position = new Vector3( pos_x, pos_y, -3 );                   // 随机位置

        if ( disk.gameObject.GetComponent<Fly>() == null ) {
            disk.gameObject.AddComponent<Fly>();
        } else {
            disk.gameObject.GetComponent<Fly>().enabled = true;
        }
    }

    public void Retrieve( DiskData disk )
    {
        diskFactory.FreeDisk( disk );

        if ( disk.gameObject.GetComponent<Fly>() != null ) {
            disk.gameObject.GetComponent<Fly>().enabled = false;
            disk.gameObject.GetComponent<Fly>().Reset();
        }

        disk.gameObject.transform.position = new Vector3( 0, -10.0f, 0 );           // 使该飞碟离开视野（待定）
    }

    IEnumerator WaitForExplode( DiskData disk )
    {
        yield return new WaitForSeconds( 0.6f );                                    // 0.6s后回收
        Retrieve( disk );
    }

}
