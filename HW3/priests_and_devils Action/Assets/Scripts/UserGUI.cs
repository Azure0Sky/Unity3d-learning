using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameBase;

public class UserGUI : MonoBehaviour {

    IUserAction userAction;

    int gameStatus;             // 0: continue, 1: win, 2: game over;
    GUIStyle infoStyle;
    GUIStyle buttionStyle;

	// Use this for initialization
	void Start () {
        userAction = Director.GetInstance().currentSceneController as IUserAction;
        gameStatus = 0;

        infoStyle = new GUIStyle
        {
            fontSize = 40,
            alignment = TextAnchor.MiddleCenter
        };

        buttionStyle = new GUIStyle( "button" )
        {
            fontSize = 30
        };
    }

    public void SetStatus( int _status )
    {
        gameStatus = _status;
    }

    public int GetStatus()
    {
        return gameStatus;
    }

    private void OnGUI()
    {
        if ( GUI.Button( new Rect( Screen.width / 2 - 75, Screen.height / 2 + 70, 150, 50 ), "Restart", buttionStyle ) ) {
            gameStatus = 0;
            userAction.Restart();
        }

        if ( gameStatus == 1 ) {            // win
            GUI.Label( new Rect( Screen.width / 2 - 50, Screen.height / 2 - 180, 100, 50 ), "You Win!", infoStyle );
        } else if ( gameStatus == 2 ) {     // game over
            GUI.Label( new Rect( Screen.width / 2 - 50, Screen.height / 2 - 180, 100, 50 ), "Game Over", infoStyle );
        }
    }
}
