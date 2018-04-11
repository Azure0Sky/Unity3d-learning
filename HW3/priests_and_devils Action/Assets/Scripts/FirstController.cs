using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameBase;

public class FirstController : MonoBehaviour, ISceneController, IUserAction
{
    UserGUI uGUI;

    BankController fromBank;
    BankController toBank;
    BoatController boat;
    GameBase.CharacterController[] characters;

    readonly Vector3 waterPos = new Vector3( 0, 0.64f, 0 );

    /*-------------- New --------------*/
    DActionManager actionManager;

	void Awake () {     // Start?
        Director director = Director.GetInstance();
        director.currentSceneController = this;
        uGUI = gameObject.AddComponent( typeof( UserGUI ) ) as UserGUI;

        characters = new GameBase.CharacterController[6];
        LoadResources();
	}

    public void LoadResources()
    {
        fromBank = new BankController( false );
        toBank = new BankController( true );
        boat = new BoatController();

        int cid = 0;
        for ( ; cid < 3; ++cid ) {
            characters[cid] = new GameBase.CharacterController( GameBase.CharacterController.CharacterType.priest );
            characters[cid].SetId( cid );
            characters[cid].GetOnBank( fromBank );
            characters[cid].SetPosition( fromBank.GetEmptyPosition() );

            fromBank.GetOnBank( characters[cid] );
        }

        for ( ; cid < 6; ++cid ) {
            characters[cid] = new GameBase.CharacterController( GameBase.CharacterController.CharacterType.devil );
            characters[cid].SetId( cid );
            characters[cid].GetOnBank( fromBank );
            characters[cid].SetPosition( fromBank.GetEmptyPosition() );

            fromBank.GetOnBank( characters[cid] );
        }

        GameObject water = GameObject.Instantiate( Resources.Load( "Prefabs/Water" ), waterPos, Quaternion.identity ) as GameObject;
    }

    public void SetActionManager( DActionManager _actionManager )
    {
        actionManager = _actionManager;
    }

    public DActionManager GetActionManager()
    {
        return actionManager;
    }

    public void MoveBoat()
    {
        if ( uGUI.GetStatus() != 0 )
            return;

        if ( boat.IsEmpty() )
            return;

        //boat.MoveToOpposite();
        actionManager.MoveBoat( boat );
        uGUI.SetStatus( CheckStatus() );
    }

    public void MoveCharacter( GameBase.CharacterController character )
    {
        if ( uGUI.GetStatus() != 0 )
            return;

        BankController bank;
        if ( character.IsOnBoat() ) {

            Debug.Log( "character on boat" );
            if ( boat.WhichSide() == -1 ) {      // from side
                bank = fromBank;
            } else {                             // to side
                bank = toBank;
            }

            Vector3 middle = bank.GetEmptyPosition();
            middle.x = character.GetGameObject().transform.position.x;

            //actionManager.MoveCharacter( character.GetGameObject(), middle );
            actionManager.MoveCharacter( character.GetGameObject(), bank.GetEmptyPosition() );

            character.GetOnBank( bank );
            //character.MoveToPosition( bank.GetEmptyPosition() );
            bank.GetOnBank( character );
            boat.GetOffBoat( character.GetId() );


        } else {

            if ( boat.IsFull() )
                return;

            if ( boat.WhichSide() != character.GetBankController().WhichSide() )
                return;

            bank = character.GetBankController();

            Vector3 middle = boat.GetEmptyPos();
            middle.y = character.GetGameObject().transform.position.y;

            //actionManager.MoveCharacter( character.GetGameObject(), middle );
            actionManager.MoveCharacter( character.GetGameObject(), boat.GetEmptyPos() );

            character.GetOnBoat( boat );
            //character.MoveToPosition( boat.GetEmptyPos() );
            bank.GetOffBank( character.GetId() );
            boat.GetOnBoat( character );


        }

        //uGUI.SetStatus( CheckStatus() );
    }

    public int CheckStatus()       // 0: continue, 1: win, 2: game over;
    {
        int priestNum = 0, devilNum = 0;

        // from bank
        priestNum = fromBank.GetCharacterNum( GameBase.CharacterController.CharacterType.priest );
        devilNum = fromBank.GetCharacterNum( GameBase.CharacterController.CharacterType.devil );

        if ( boat.WhichSide() == -1 ) {
            priestNum += boat.GetCharacterNum( GameBase.CharacterController.CharacterType.priest );
            devilNum += boat.GetCharacterNum( GameBase.CharacterController.CharacterType.devil );
        }

        if ( devilNum > priestNum && priestNum > 0 )
            return 2;

        // to bank
        priestNum = toBank.GetCharacterNum( GameBase.CharacterController.CharacterType.priest );
        devilNum = toBank.GetCharacterNum( GameBase.CharacterController.CharacterType.devil );

        Debug.Log( "Priest: " + priestNum );
        Debug.Log( "Devil" + devilNum );

        if ( priestNum + devilNum == 6 )
            return 1;

        if ( boat.WhichSide() == 1 ) {
            priestNum += boat.GetCharacterNum( GameBase.CharacterController.CharacterType.priest );
            devilNum += boat.GetCharacterNum( GameBase.CharacterController.CharacterType.devil );
        }

        if ( devilNum > priestNum && priestNum > 0 )
            return 2;

        return 0;
    }

    public void SetGameStatus( int status )
    {
        uGUI.SetStatus( status );
    }

    public void Restart()
    {
        //boat.Reset();
        //fromBank.Reset();
        //toBank.Reset();

        //if ( boat.WhichSide() == 1 )
        //    actionManager.MoveBoat( boat );

        //for ( int i = 0; i < characters.Length; ++i ) {
        //    actionManager.MoveCharacter( characters[i].GetGameObject(), fromBank.GetEmptyPosition() );
        //}

        UnityEngine.SceneManagement.SceneManager.LoadScene( 0 );
    }

    public BankController GetBank_from()
    {
        return fromBank;
    }
}
