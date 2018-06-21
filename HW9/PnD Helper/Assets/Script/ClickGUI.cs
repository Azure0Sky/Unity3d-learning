using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameBase;

public class ClickGUI : MonoBehaviour {

    IUserAction userAction;
    MyCharacterController thisCharacter;

	// Use this for initialization
	void Start()
    {
        userAction = Director.GetInstance().CurrentSceneController as IUserAction;
	}
	
	private void OnMouseDown()
    {
        // User could do nothing when helping
        if ( ( Director.GetInstance().CurrentSceneController as FirstController ).uGUI.GetStatus() == 3 ) {
            return;
        }

        if ( Movement.IsMoving() )
            return;

        if ( thisCharacter != null && thisCharacter.name == "character" ) {
            userAction.MoveCharacter( thisCharacter );
        } else {
            userAction.MoveBoat();
        }
    }

    public void SetCharacter( GameBase.MyCharacterController cha )
    {
        thisCharacter = cha;
    }
}
