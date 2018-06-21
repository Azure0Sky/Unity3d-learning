using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameBase;

public class UserGUI : MonoBehaviour {

    IUserAction userAction;

    int gameStatus;             // 0: continue, 1: win, 2: game over, 3: helping;
    GUIStyle infoStyle;
    GUIStyle buttionStyle;

	// Use this for initialization
	void Start ()
    {
        userAction = Director.GetInstance().CurrentSceneController as IUserAction;
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
        if ( gameStatus == 3 )
            return;

        if ( GUI.Button( new Rect( Screen.width / 2 - 75, Screen.height / 2 + 70, 150, 50 ), "Restart", buttionStyle ) ) {
            if ( Movement.IsMoving() )
                return;

            gameStatus = 0;
            userAction.Restart();
        }

        if ( GUI.Button( new Rect( Screen.width / 2 - 65, Screen.height / 2 + 140, 130, 50 ), "Help", buttionStyle ) ) {
            if ( gameStatus != 0 )
                return;

            gameStatus = 3;
            userAction.Help();
        }

        if ( gameStatus == 1 ) {            // win
            GUI.Label( new Rect( Screen.width / 2 - 50, Screen.height / 2 - 180, 100, 50 ), "You Win!", infoStyle );
        } else if ( gameStatus == 2 ) {     // game over
            GUI.Label( new Rect( Screen.width / 2 - 50, Screen.height / 2 - 180, 100, 50 ), "Game Over", infoStyle );
        }
    }
}
