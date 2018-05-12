using UnityEngine;
using System.Collections;

// 必要的组件
[RequireComponent( typeof( Animator ) )]
[RequireComponent( typeof( CapsuleCollider ) )]
[RequireComponent( typeof( Rigidbody ) )]

public class UnityChanControl : MonoBehaviour
{
	public float animSpeed = 1.5f;
	//public float lookSmoother = 3.0f;			// a smoothing setting for camera motion
	public bool useCurves = true;

	public float useCurvesHeight = 0.5f;

	// 以下为角色用参数
	// 前进速度
	public float forwardSpeed = 7.0f;
    // 后退速度
    public float backwardSpeed = 2.0f;
    // 旋转速度
    public float rotateSpeed = 2.0f;
	// 跳跃力度
	public float jumpPower = 3.0f; 

	private CapsuleCollider col;
	private Rigidbody rb;

    private Vector3 velocity;

    private float orgColHight;
	private Vector3 orgVectColCenter;
	
	private Animator anim;
	private AnimatorStateInfo currentBaseState;

	private GameObject cameraObject;

    static readonly int idleState = Animator.StringToHash("Base Layer.Idle");
	static readonly int locoState = Animator.StringToHash("Base Layer.Locomotion");
	static readonly int jumpState = Animator.StringToHash("Base Layer.Jump");
	static readonly int restState = Animator.StringToHash("Base Layer.Rest");

    // initialization
    void Start ()
	{
		anim = GetComponent<Animator>();
		col = GetComponent<CapsuleCollider>();
		rb = GetComponent<Rigidbody>();

        cameraObject = GameObject.FindWithTag("MainCamera");

        orgColHight = col.height;
		orgVectColCenter = col.center;
    }

    void FixedUpdate()
    {
        currentBaseState = anim.GetCurrentAnimatorStateInfo( 0 );   // 将第0层的状态设为当前状态
        AnimatorProcess();
    }

    public void Jump()
    {
        if ( currentBaseState.fullPathHash == locoState ) {
            // 如果没有在状态转换中，则可以跳跃
            if ( !anim.IsInTransition( 0 ) ) {
                rb.AddForce( Vector3.up * jumpPower, ForceMode.VelocityChange );
                anim.SetBool( "Jump", true );
            }
        }
    }

    public void Move( float hor, float vir )
    {
        anim.SetFloat( "Speed", vir );                                // 将vir传递给Animator的Speed
        anim.SetFloat( "Direction", hor );                            // 将hor传递给Animator的Direction
        anim.speed = animSpeed;                                       // 将Animator的动画播放速度设置为animSpeed
        rb.useGravity = true;                                         // 在跳跃过程中取消重力影响，其他情况下则受重力影响

        // 角色动作处理
        velocity = new Vector3( 0, 0, vir );      // 从上下键输入获得Z轴方向的移动量

        // 转换角色的方向
        velocity = transform.TransformDirection( velocity );
        if ( vir > 0.1 ) {
            velocity *= forwardSpeed;
        } else if ( vir < -0.1 ) {
            velocity *= backwardSpeed;
        }

        // 输入上下键使角色移动
        transform.localPosition += velocity * Time.fixedDeltaTime;

        // 输入左右键使角色绕Y轴旋转
        transform.Rotate( 0, hor * rotateSpeed, 0 );
    }

    private void AnimatorProcess()
    {
        if ( currentBaseState.fullPathHash == locoState ) {

            if ( useCurves ) {
                ResetCollider();
            }

        } else if ( currentBaseState.fullPathHash == jumpState ) {                  // 跳跃处理
            cameraObject.SendMessage( "SetCameraPositionJumpView" );

            if ( !anim.IsInTransition( 0 ) ) {

                if ( useCurves ) {
                    float jumpHeight = anim.GetFloat( "JumpHeight" );
                    float gravityControl = anim.GetFloat( "GravityControl" );
                    if ( gravityControl > 0 )
                        rb.useGravity = false;                                      //在跳跃过程中取消重力影响

                    Ray ray = new Ray( transform.position + Vector3.up, -Vector3.up );
                    RaycastHit hitInfo = new RaycastHit();

                    if ( Physics.Raycast( ray, out hitInfo ) ) {
                        if ( hitInfo.distance > useCurvesHeight ) {

                            col.height = orgColHight - jumpHeight;
                            float adjCenterY = orgVectColCenter.y + jumpHeight;
                            col.center = new Vector3( 0, adjCenterY, 0 );

                        } else {

                            ResetCollider();

                        }
                    }
                }

                anim.SetBool( "Jump", false );
            }

        } else if ( currentBaseState.fullPathHash == idleState ) {                  // Idle处理

            if ( useCurves ) {
                ResetCollider();
            }
            // 输入空格键则进入Rest
            if ( Input.GetButtonDown( "Jump" ) ) {
                anim.SetBool( "Rest", true );
            }

        } else if ( currentBaseState.fullPathHash == restState ) {                  // Rest处理

            if ( !anim.IsInTransition( 0 ) ) {
                anim.SetBool( "Rest", false );
            }

        }
    }

	private void ResetCollider()
	{
		col.height = orgColHight;
		col.center = orgVectColCenter;
	}
}
