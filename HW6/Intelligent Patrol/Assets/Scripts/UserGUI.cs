using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserGUI : MonoBehaviour
{
    IUserAction userAction;

    bool start;
    bool over;
    GUIStyle buttonStyle;
    GUIStyle scoreStyle;
    GUIStyle titleStyle;
    GUIStyle infoStyle;
    GUIStyle resultStyle;

    void Start ()
    {
        userAction = Director.GetInstance().CurrentSceneController as IUserAction;
        start = false;
        over = false;

        PatrolController.hitPlayerEvent += GameOver;

        titleStyle = new GUIStyle
        {
            fontSize = 60,
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold
        };

        infoStyle = new GUIStyle
        {
            fontSize = 20,
            alignment = TextAnchor.MiddleCenter
        };

        buttonStyle = new GUIStyle( "button" )
        {
            fontSize = 25,
            alignment = TextAnchor.MiddleCenter
        };

        scoreStyle = new GUIStyle
        {
            fontSize = 23,
            fontStyle = FontStyle.Bold
        };
        scoreStyle.normal.textColor = Color.red;

        resultStyle = new GUIStyle
        {
            fontSize = 35
        };
        resultStyle.normal.textColor = Color.white;
    }

    void OnGUI()
    {
        if ( start ) {

            int score = Singleton<ScoreController>.Instance.GetScore();
            GUI.Label( new Rect( 10, 10, 100, 40 ), "Score: ", infoStyle );
            GUI.Label( new Rect( 110, 15, 100, 40 ), score.ToString(), scoreStyle );

        } else if ( over ) {

            int score = Singleton<ScoreController>.Instance.GetScore();
            GUI.Label( new Rect( Screen.width / 2 - 75, Screen.height / 2 - 50, 150, 100 ), "Game Over", titleStyle );
            GUI.Label( new Rect( Screen.width / 2 - 75, Screen.height / 2 + 70, 75, 50 ), "Your score: ", infoStyle );
            GUI.Label( new Rect( Screen.width / 2 + 50, Screen.height / 2 + 75, 75, 50 ), score.ToString(), resultStyle );

            if ( GUI.Button( new Rect( Screen.width / 2 - 50, Screen.height / 2 + 150, 100, 50 ), "Restart", buttonStyle ) ) {
                userAction.Restart();
                over = false;
                start = true;
            }

        } else {            // 第一次进入游戏

            if ( GUI.Button( new Rect( Screen.width / 2 - 90, Screen.height / 2 - 30, 180, 60 ), "Start Game", buttonStyle ) ) {
                over = false;
                start = true;
                userAction.StartGame();
            }

        }
    }

    public void GameOver()
    {
        over = true;
        start = false;
    }
}
