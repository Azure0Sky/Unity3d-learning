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
        uGUI.SetStatus( CheckStatus() );
    }

    public void MoveCharacter( GameBase.CharacterController character )
    {
        if ( uGUI.GetStatus() != 0 )
            return;

        BankController bank;
        if ( character.IsOnBoat() ) {

            if ( boat.WhichSide() == -1 ) {      // from side
                bank = fromBank;
            } else {                             // to side
                bank = toBank;
            }

            character.GetOnBank( bank );
            //character.MoveToPosition( bank.GetEmptyPosition() );
            bank.GetOnBank( character );
            boat.GetOffBoat( character.GetId() );

            Vector3 middle = character.GetGameObject().transform.position;
            middle.y = bank.GetEmptyPosition().y;

            actionManager.MoveCharacter( character.GetGameObject(), middle );
            actionManager.MoveCharacter( character.GetGameObject(), bank.GetEmptyPosition() );

        } else {

            if ( boat.IsFull() )
                return;

            if ( boat.WhichSide() != character.GetBankController().WhichSide() )
                return;

            bank = character.GetBankController();

            character.GetOnBoat( boat );
            //character.MoveToPosition( boat.GetEmptyPos() );
            bank.GetOffBank( character.GetId() );
            boat.GetOnBoat( character );

            Vector3 middle = 

        }

        uGUI.SetStatus( CheckStatus() );
    }

    private int CheckStatus()       // 0: continue, 1: win, 2: game over;
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

    public void Restart()
    {
        boat.Reset();
        fromBank.Reset();
        toBank.Reset();

        for ( int i = 0; i < characters.Length; ++i ) {
            characters[i].Reset();
        }
    }

    public BankController GetBank_from()
    {
        return fromBank;
    }
}
