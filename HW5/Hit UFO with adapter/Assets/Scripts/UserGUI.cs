using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserGUI : MonoBehaviour {

    IUserAction userAction;

    bool start;                 // 游戏是否已经开始
    int life;                   // 当前生命
    GUIStyle titleStyle;
    GUIStyle infoStyle;
    GUIStyle buttonStyle;
    GUIStyle scoreStyle;
    GUIStyle roundStyle;
    GUIStyle heartStyle;
    GUIStyle resultStyle;

    private void Start()
    {
        userAction = Director.GetInstance().CurrentSceneController as IUserAction;
        start = false;
        life = 5;

        titleStyle = new GUIStyle
        {
            fontSize = 60,
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold
        };

        infoStyle = new GUIStyle
        {
            fontSize = 25,
            alignment = TextAnchor.MiddleCenter
        };

        buttonStyle = new GUIStyle( "button" )
        {
            fontSize = 25,
            alignment = TextAnchor.MiddleCenter
        };

        scoreStyle = new GUIStyle
        {
            fontSize = 28,
            fontStyle = FontStyle.Bold
        };
        scoreStyle.normal.textColor = Color.red;

        roundStyle = new GUIStyle
        {
            fontSize = 25,
            fontStyle = FontStyle.Bold
        };
        roundStyle.normal.textColor = Color.white;

        heartStyle = new GUIStyle
        {
            fontSize = 20
        };
        heartStyle.normal.textColor = Color.red;

        resultStyle = new GUIStyle
        {
            fontSize = 35
        };
        resultStyle.normal.textColor = Color.white;
    }

    private void OnGUI()
    {
        if ( start ) {                        // 游戏已开始

            int score = Singleton<ScoreController>.Instance.GetScore();
            int round = ( Director.GetInstance().CurrentSceneController as FirstController ).GetRound();

            // 显示分数
            GUI.Label( new Rect( 10, 10, 100, 40 ), "Score: ", infoStyle );
            GUI.Label( new Rect( 110, 15, 100, 40 ), score.ToString(), scoreStyle );

            // 显示回合数
            GUI.Label( new Rect( 10, 70, 100, 40 ), "Round: ", infoStyle );
            GUI.Label( new Rect( 110, 75, 100, 40 ), round.ToString(), roundStyle );

            // 显示生命值
            GUI.Label( new Rect( Screen.width - 100, 10, 30, 40 ), "Life: ", infoStyle );
            for ( int i = 0; i < life; ++i ) {
                GUI.Label( new Rect( Screen.width - 110 + i * 20, 55, 10, 40 ), "❤", heartStyle );
            }

            // 游戏结束
            if ( life == 0 ) {
                GUI.Label( new Rect( Screen.width / 2 - 75, Screen.height / 2 - 50, 150, 100 ), "Game Over", titleStyle );
                GUI.Label( new Rect( Screen.width / 2 - 75, Screen.height / 2 + 70, 75, 50 ), "Your score: ", infoStyle );
                GUI.Label( new Rect( Screen.width / 2 + 50, Screen.height / 2 + 75, 75, 50 ), score.ToString(), resultStyle );

                userAction.GameOver();

                if ( GUI.Button( new Rect( Screen.width / 2 - 180, Screen.height / 2 + 150, 360, 50 ), "Restart in Kinematics Mode", buttonStyle ) ) {

                    life = 5;
                    userAction.Restart( Fly.Mode.Kinematics );
                    return;

                } else if ( GUI.Button( new Rect( Screen.width / 2 - 180, Screen.height / 2 + 210, 360, 50 ), "Restart in Physics Mode", buttonStyle ) ) {

                    life = 5;
                    userAction.Restart( Fly.Mode.Physics );
                    return;

                }

            }

            // 用户点击射击
            if ( Input.GetButtonDown( "Fire1" ) ) {
                if ( start )
                    userAction.Shoot( Input.mousePosition );
            }

        } else {                            // 游戏未开始

            GUI.Label( new Rect( Screen.width / 2 - 75, Screen.height / 2 - 50, 150, 100 ), "Hit UFO !", titleStyle );

            if ( GUI.Button( new Rect( Screen.width / 2 - 180, Screen.height / 2 + 50, 360, 50 ), "Start in Kinematics Mode", buttonStyle ) ) {

                start = true;
                userAction.StartGame( Fly.Mode.Kinematics );

            } else if ( GUI.Button( new Rect( Screen.width / 2 - 180, Screen.height / 2 + 110, 360, 50 ), "Start in Physics Mode", buttonStyle ) ) {

                start = true;
                userAction.StartGame( Fly.Mode.Physics );

            }

        }
    }

    public void ReduceLife()
    {
        --life;
    }
}
