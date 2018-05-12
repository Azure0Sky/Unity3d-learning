using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FirstController : MonoBehaviour, ISceneController, IUserAction
{
    private PatrolFactory patrolFactory;
    private UnityChanControl unityChan;

    private bool start;

    private List<GameObject> patrols;

    void Awake()
    {
        Director director = Director.GetInstance();
        director.CurrentSceneController = this;

        patrolFactory = Singleton<PatrolFactory>.Instance;
        unityChan = Singleton<UnityChanControl>.Instance;

        PatrolController.hitPlayerEvent += GameOver;

        start = false;
	}
	
	void FixedUpdate()
    {
        if ( !start )
            return;

        MoveInput();
	}

    public void GameOver()
    {
        start = false;
    }

    public void LoadResources()
    {
        patrols = patrolFactory.GetPatrols();
    }

    public void MoveInput()
    {
        float h = Input.GetAxis( "Horizontal" );                    // 水平轴设定为h
        float v = Input.GetAxis( "Vertical" );                      // 竖直轴设定为v

        if ( Input.GetButtonDown( "Jump" ) ) {
            unityChan.Jump();
        }

        unityChan.Move( h, v );
    }

    public void Restart()
    {
        SceneManager.LoadScene( 0 );
        StartGame();
    }

    public void StartGame()
    {
        LoadResources();
        start = true;
    }

    public bool GetGameStatus()
    {
        return start;
    }
}
