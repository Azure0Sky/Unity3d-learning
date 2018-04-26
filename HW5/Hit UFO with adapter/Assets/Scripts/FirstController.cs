using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstController : MonoBehaviour, ISceneController, IUserAction
{
    UserGUI uGUI;
    DiskController diskController;
    DiskFactory diskFactory;
    ScoreController scoreController;
    IActionManager actionManager;

    private int round = 1;                              // 回合数
    private float interval = 2.0f;                      // 发射一个飞碟的间隔时间

    readonly private int scoreToRound2 = 2;            // 进入Round2所需分数
    readonly private int scoreToRound3 = 5;            // 进入Round3所需分数

    private bool start;

    List<DiskData> diskLaunched;                        // 已经发射的飞碟，即显示在画面中的飞碟

    Fly.Mode mode;

    void Awake()
    {
        Director director = Director.GetInstance();
        director.CurrentSceneController = this;

        start = false;
        diskLaunched = new List<DiskData>();

        LoadResources();
    }

    void Update()
    {
        if ( !start )
            return;

        if ( scoreController.GetScore() >= scoreToRound2 && round < 2 ) {

            round = 2;
            interval -= 0.5f;

            CancelInvoke( "LaunchDisk" );
            InvokeRepeating( "LaunchDisk", interval, interval );
            Debug.Log( "To Round 2" );

        } else if ( scoreController.GetScore() >= scoreToRound3 && round < 3 ) {

            round = 3;
            interval -= 0.6f;

            CancelInvoke( "LaunchDisk" );
            InvokeRepeating( "LaunchDisk", interval, interval );
            Debug.Log( "To Round 3" );

        }

        for ( int i = diskLaunched.Count-1; i >= 0; --i ) {
            if ( diskLaunched[i].gameObject.transform.position.y < -6 ||
                 diskLaunched[i].gameObject.transform.position.z > 20 ) {

                diskController.Retrieve( diskLaunched[i] );                 // 飞出范围，回收飞碟
                uGUI.ReduceLife();
                diskLaunched.RemoveAt( i );

            }
        }

    }

    public void LoadResources()
    {
        uGUI = Singleton<UserGUI>.Instance;
        diskFactory = Singleton<DiskFactory>.Instance;
        diskController = Singleton<DiskController>.Instance;
        scoreController = Singleton<ScoreController>.Instance;
    }

    public void StartGame( Fly.Mode mode )
    {
        if ( mode == Fly.Mode.Kinematics )
            actionManager = Singleton<SimActionManager>.Instance;
        else
            actionManager = Singleton<PhysicsActionManager>.Instance;

        this.mode = mode;
        start = true;
        InvokeRepeating( "LaunchDisk", 1.0f, interval );        // 游戏开始1秒后发射第一个飞碟
    }

    public void Restart( Fly.Mode mode )
    {
        interval = 2.0f;
        round = 1;
        scoreController.Reset();
        diskLaunched = new List<DiskData>();

        diskFactory.ResetAllDisksToMode( mode );

        StartGame( mode );
    }

    public void Shoot( Vector3 pos )
    {
        Ray ray = Camera.main.ScreenPointToRay( pos );
        RaycastHit[] hits;
        hits = Physics.RaycastAll( ray );

        if ( hits.Length <= 0 )
            return;

        RaycastHit hit = hits[0];
        DiskData diskHit = hit.collider.gameObject.GetComponent<DiskData>();

        if ( start )
            scoreController.Record( diskHit );

        diskController.Explode( diskHit );
        diskLaunched.Remove( diskHit );
    }

    private void LaunchDisk()
    {
        DiskData disk = diskFactory.GetDisk( round );
        actionManager.PlayDisk( disk );
        diskLaunched.Add( disk );
    }

    public void GameOver()
    {
        start = false;
        for ( int i = 0; i < diskLaunched.Count; ++i ) {
            diskController.Retrieve( diskLaunched[i] );
        }

        CancelInvoke( "LaunchDisk" );
    }

    public int GetRound()
    {
        return round;
    }
}
