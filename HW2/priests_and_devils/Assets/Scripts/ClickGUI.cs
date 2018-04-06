using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameBase;

public class ClickGUI : MonoBehaviour {

    IUserAction userAction;
    GameBase.CharacterController thisCharacter;

	// Use this for initialization
	void Start()
    {
        userAction = Director.GetInstance().currentSceneController as IUserAction;
	}
	
	private void OnMouseDown()
    {
        if ( Movement.IsMoving() )
            return;

        if ( thisCharacter != null && thisCharacter.name == "character" ) {
            userAction.MoveCharacter( thisCharacter );
        } else {
            userAction.MoveBoat();
        }
    }

    public void SetCharacter( GameBase.CharacterController cha )
    {
        thisCharacter = cha;
    }
}
