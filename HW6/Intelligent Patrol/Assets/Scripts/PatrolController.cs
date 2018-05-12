using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof( Animator ) )]
[RequireComponent( typeof( CapsuleCollider ) )]
[RequireComponent( typeof( Rigidbody ) )]

public class PatrolController : MonoBehaviour
{
    //private CapsuleCollider col;
    private Rigidbody rb;
    private Animator anim;
    private AnimatorStateInfo currentBaseState;

    private Queue<Vector3> nextPos;
    private bool idle;                              // 到达一个目的地时，停下来一段时间
    private bool isChasing;
    private readonly float chaseRange = 12f;

    private UnityChanControl unityChanControl;

    [SerializeField] private Vector3 np;
    [SerializeField] private int count;

    // 前进速度
    private float forwardSpeed = 4.0f;
    // 旋转速度
    private float rotateSpeed = 8.0f;

    // 委托
    public delegate void hitPlayer();
    public delegate void losePlayer();
    // 事件
    public static event hitPlayer hitPlayerEvent;
    public static event losePlayer losePlayerEvent;

    void Start ()
    {
        anim = GetComponent<Animator>();
        //col = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();

        nextPos = new Queue<Vector3>();
        idle = false;
        isChasing = false;

        unityChanControl = Singleton<UnityChanControl>.Instance;

        losePlayerEvent += ToPatrol;
    }
	
	void FixedUpdate ()
    {   
        if ( !( Director.GetInstance().CurrentSceneController as FirstController ).GetGameStatus() )    // 游戏未开始
            return;

        count = nextPos.Count;
        currentBaseState = anim.GetCurrentAnimatorStateInfo( 0 );

        float distance = Vector3.Distance( transform.position, unityChanControl.transform.position );
        if ( distance <= chaseRange ) {

            forwardSpeed = 6f;
            GoNext( unityChanControl.transform.position );

            anim.SetBool( "toRun", true );
            isChasing = true;

        } else {

            if ( isChasing ) {
                losePlayerEvent();
            }

            if ( nextPos.Count == 0 ) {

                GetNextPositions();

            } else {

                if ( Vector3.Distance( transform.position, nextPos.Peek() ) > 0.1 && !idle ) {

                    GoNext( nextPos.Peek() );

                } else {
                    if ( !idle )
                        nextPos.Dequeue();

                    idle = true;
                }

                if ( idle ) {
                    anim.SetBool( "toRun", false );
                }

            }

        }

        AnimatorProcess();
	}

    void OnCollisionEnter( Collision collision )
    {
        if ( collision.gameObject.tag == "Player" ) {
            hitPlayerEvent();
        } else {
            nextPos.Dequeue();                      // 向下一个目的地前进
        }
    }

    private void OnCollisionStay( Collision collision )
    {
        nextPos.Dequeue();
    }

    private void GetNextPositions()
    {
        float width = Random.Range( 3, 5 );
        float height = Random.Range( 3, 5 );

        int sideNum = Random.Range( 3, 5 );
        float currX = transform.position.x;
        float currZ = transform.position.z;

        Vector3 newPos = Vector3.zero;
        for ( int i = 0; i < sideNum; ++i ) {
            switch ( i ) {
                case 0:
                    newPos = new Vector3( Random.Range( currX - width, currX + width ), 0, currZ + height );
                    break;

                case 1:
                    newPos = new Vector3( currX + width, 0, Random.Range( currZ - height, currZ + height ) );
                    break;

                case 2:
                    newPos = new Vector3( Random.Range( currX - width, currX + width ), 0, currZ - height );
                    break;

                case 3:
                    newPos = new Vector3( currX - width, 0, Random.Range( currZ - height, currZ + height ) );
                    break;

                default:
                    break;
            }

            if ( i < 4 )
                nextPos.Enqueue( newPos );
        }

        if ( sideNum == 5 ) {
            nextPos.Enqueue( transform.position );
        }
    }

    private void AnimatorProcess()
    {
        if ( currentBaseState.fullPathHash == Animator.StringToHash( "Base Layer.Idle" ) ) {
            if ( !anim.IsInTransition( 0 ) ) {
                rb.velocity = Vector3.zero;
                idle = false;
                anim.SetBool( "toRun", true );
            }
        }
    }

    private void ToPatrol()
    {
        isChasing = false;
        forwardSpeed = 7f;
    }

    private void GoNext( Vector3 next )
    {
        np = next;
        Vector3 direction = next - transform.position;
        rb.velocity = direction.normalized * forwardSpeed;

        rb.rotation = Quaternion.Slerp( transform.rotation, Quaternion.LookRotation( direction ), rotateSpeed * Time.fixedDeltaTime );
    }
}
