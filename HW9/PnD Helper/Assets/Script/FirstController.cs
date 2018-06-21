using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameBase;

public class FirstController : MonoBehaviour, ISceneController, IUserAction
{
    public UserGUI uGUI;

    BankController fromBank;
    BankController toBank;
    BoatController boat;
    MyCharacterController[] characters;

    readonly Vector3 waterPos = new Vector3( 0, 0.64f, 0 );

    StatusGraph statusGraph;

	void Awake ()
    {
        Director director = Director.GetInstance();
        director.CurrentSceneController = this;
        uGUI = gameObject.AddComponent( typeof( UserGUI ) ) as UserGUI;

        characters = new GameBase.MyCharacterController[6];
        statusGraph = new StatusGraph( 3, 3, true );                // Generate the status graph

        LoadResources();
	}

    public void LoadResources()
    {
        fromBank = new BankController( false );
        toBank = new BankController( true );
        boat = new BoatController();

        int cid = 0;
        for ( ; cid < 3; ++cid ) {
            characters[cid] = new MyCharacterController( MyCharacterController.CharacterType.priest );
            characters[cid].SetId( cid );
            characters[cid].GetOnBank( fromBank );
            characters[cid].SetPosition( fromBank.GetEmptyPosition() );

            fromBank.GetOnBank( characters[cid] );
        }

        for ( ; cid < 6; ++cid ) {
            characters[cid] = new MyCharacterController( MyCharacterController.CharacterType.devil );
            characters[cid].SetId( cid );
            characters[cid].GetOnBank( fromBank );
            characters[cid].SetPosition( fromBank.GetEmptyPosition() );

            fromBank.GetOnBank( characters[cid] );
        }

        GameObject water = GameObject.Instantiate( Resources.Load( "Prefabs/Water" ), waterPos, Quaternion.identity ) as GameObject;
    }

    public void MoveBoat()
    {
        if ( uGUI.GetStatus() != 0 && uGUI.GetStatus() != 3 )
            return;

        if ( boat.IsEmpty() )
            return;

        boat.MoveToOpposite();
        uGUI.SetStatus( CheckStatus() );
    }

    public void MoveCharacter( MyCharacterController character )
    {
        if ( ( uGUI.GetStatus() != 0 && uGUI.GetStatus() != 3 ) || character == null )
            return;

        BankController bank;
        if ( character.IsOnBoat() ) {

            if ( boat.WhichSide() == -1 ) {      // from side
                bank = fromBank;
            } else {                             // to side
                bank = toBank;
            }

            character.GetOnBank( bank );
            character.MoveToPosition( bank.GetEmptyPosition() );
            bank.GetOnBank( character );
            boat.GetOffBoat( character.GetId() );

        } else {

            if ( boat.IsFull() )
                return;

            if ( boat.WhichSide() != character.GetBankController().WhichSide() )
                return;

            bank = character.GetBankController();

            character.GetOnBoat( boat );
            character.MoveToPosition( boat.GetEmptyPos() );
            bank.GetOffBank( character.GetId() );
            boat.GetOnBoat( character );

        }

        //uGUI.SetStatus( CheckStatus() );
    }

    private int CheckStatus()       // 0: continue, 1: win, 2: game over;
    {
        int priestNum = 0, devilNum = 0;

        // from bank
        priestNum = fromBank.GetCharacterNum( MyCharacterController.CharacterType.priest );
        devilNum = fromBank.GetCharacterNum( MyCharacterController.CharacterType.devil );

        if ( boat.WhichSide() == -1 ) {
            priestNum += boat.GetCharacterNum( MyCharacterController.CharacterType.priest );
            devilNum += boat.GetCharacterNum( MyCharacterController.CharacterType.devil );
        }

        if ( devilNum > priestNum && priestNum > 0 )
            return 2;

        // to bank
        priestNum = toBank.GetCharacterNum( MyCharacterController.CharacterType.priest );
        devilNum = toBank.GetCharacterNum( MyCharacterController.CharacterType.devil );

        if ( boat.WhichSide() == 1 ) {
            priestNum += boat.GetCharacterNum( MyCharacterController.CharacterType.priest );
            devilNum += boat.GetCharacterNum( MyCharacterController.CharacterType.devil );
        }

        if ( priestNum + devilNum == 6 )
            return 1;

        if ( devilNum > priestNum && priestNum > 0 )
            return 2;

        return 0;
    }

    public void Restart()
    {
        if ( uGUI.GetStatus() == 3 )
            return;

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

    public void Help()
    {
        if ( Movement.IsMoving() ) {            // if is moving, wait
            StartCoroutine( WaitMoving() );
        }

        int currPriestNum = fromBank.GetCharacterNum( MyCharacterController.CharacterType.priest );
        int currDevilNum = fromBank.GetCharacterNum( MyCharacterController.CharacterType.devil );

        currPriestNum += boat.WhichSide() == -1 ? boat.GetCharacterNum( MyCharacterController.CharacterType.priest ) : 0;
        currDevilNum += boat.WhichSide() == -1 ? boat.GetCharacterNum( MyCharacterController.CharacterType.devil ) : 0;

        var curr = statusGraph.GetStatus( currPriestNum, currDevilNum, ( boat.WhichSide() == -1 ) ? true : false );
        var next = curr.nextVertice;

        int priestDiff = Mathf.Abs( next.priest - curr.priest ) - boat.GetCharacterNum( MyCharacterController.CharacterType.priest );
        int devilDiff = Mathf.Abs( next.devil - curr.devil ) - boat.GetCharacterNum( MyCharacterController.CharacterType.devil );
        BankController boatBank = ( boat.WhichSide() == -1 ) ? fromBank : toBank;

        StartCoroutine( AutoMoving( priestDiff, devilDiff, boatBank ) );
        
    }

    private IEnumerator AutoMoving( int priestDiff, int devilDiff, BankController theBank )
    {
        while ( priestDiff < 0 ) {
            yield return new WaitForSeconds( 0.5f );
            MoveCharacter( boat.GetOneCharacter( MyCharacterController.CharacterType.priest ) );
            ++priestDiff;
        }

        while ( devilDiff < 0 ) {
            yield return new WaitForSeconds( 0.5f );
            MoveCharacter( boat.GetOneCharacter( MyCharacterController.CharacterType.devil ) );
            ++devilDiff;
        }

        while ( priestDiff > 0 ) {
            yield return new WaitForSeconds( 0.5f );
            MoveCharacter( theBank.GetOneCharacter( MyCharacterController.CharacterType.priest ) );
            --priestDiff;
        }

        while ( devilDiff > 0 ) {
            yield return new WaitForSeconds( 0.5f );
            MoveCharacter( theBank.GetOneCharacter( MyCharacterController.CharacterType.devil ) );
            --devilDiff;
        }

        yield return new WaitForSeconds( 0.7f );
        MoveBoat();
    }

    private IEnumerator WaitMoving()
    {
        yield return new WaitWhile( Movement.IsMoving );
    }
}
