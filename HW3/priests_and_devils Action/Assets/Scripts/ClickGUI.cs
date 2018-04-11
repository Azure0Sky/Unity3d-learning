using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameBase;

public class ClickGUI : MonoBehaviour {

    IUserAction userAction;
    DActionManager actionManager;
    GameBase.CharacterController thisCharacter;

	// Use this for initialization
	void Start()
    {
        userAction = Director.GetInstance().currentSceneController as IUserAction;
        actionManager = ( Director.GetInstance().currentSceneController as FirstController ).GetActionManager();
	}

    private void OnMouseDown()
    {
        if ( actionManager.IsMoving() )
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
