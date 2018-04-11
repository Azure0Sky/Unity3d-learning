using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameBase;
using ActionController;

public class DActionManager : ActionManager, IActionCallback
{
    public FirstController sceneController;

    readonly float speed = 10.0f;

    private MoveToAction moveBoat;
    private MoveToAction moveCharacterToMiddle;
    private MoveToAction moveCharacterToTarget;

    Vector3 middle, dest;
    GameObject character;

    bool Moving;

    protected new void Update()
    {
        base.Update();
    }

    protected void Start()
    {
        sceneController = Director.GetInstance().currentSceneController as FirstController;
        sceneController.SetActionManager( this );
        Moving = false;
    }

    public bool IsMoving()
    {
        return Moving;
    }

    public void MoveCharacter( GameObject character, Vector3 target )
    {
        this.character = character;
        middle = target;
        dest = target;

        if ( character.transform.position.y > target.y ) {      // character moves from coast to boat
            middle.y = character.transform.position.y;
        } else {                                                // character moves from boat to coast
            middle.x = character.transform.position.x;
        }

        moveCharacterToMiddle = MoveToAction.GetAction( middle, speed );
        RunAction( character, moveCharacterToMiddle, this );

        Moving = true;
    }

    public void MoveBoat( BoatController boat )
    {
        GameObject boatObject = boat.GetGameObject();
        boat.MoveToOpposite();

        Vector3 oppositePos = new Vector3( boatObject.transform.position.x * -1.0f, boatObject.transform.position.y );
        moveBoat = MoveToAction.GetAction( oppositePos, speed );
        RunAction( boatObject, moveBoat, this );

        Moving = true;
    }

    public void ActionEvent( Action source, int intParam = 0, string strParam = null, object objectParam = null )
    {
        print( "callback!" );
        if ( source == moveBoat ) {

            Moving = false;

        } else if ( source == moveCharacterToMiddle ) {

            moveCharacterToTarget = MoveToAction.GetAction( dest, speed );
            RunAction( character, moveCharacterToTarget, this );

        } else if ( source == moveCharacterToTarget ) {

            print( "callback of arriving target" );
            Moving = false;

        }

        sceneController.SetGameStatus( sceneController.CheckStatus() );

    }
}

