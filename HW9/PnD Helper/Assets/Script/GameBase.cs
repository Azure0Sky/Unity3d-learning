using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBase
{
    public class Director : System.Object
    {
        private static Director _instance;
        public ISceneController CurrentSceneController { get; set; }

        public static Director GetInstance()
        {
            if ( _instance == null ) {
                _instance = new Director();
            }

            return _instance;
        }
    }

    public class Movement : MonoBehaviour
    {
        public float speed = 10.0f;
        Vector3 dest;
        Vector3 middle;
        int status = 0;     // 0: no moving, 1: character's moving, 2: boat's moving
        bool middleArrived = false;

        static bool moving = false;

        private void Update()
        {
            if ( status == 1 ) {                                                    // character moves

                if ( transform.position == middle && !middleArrived ) {
                    middleArrived = true;
                } else if ( transform.position != middle && !middleArrived ) {      // moves to middle
                    moving = true;
                    transform.position = Vector3.MoveTowards( transform.position, middle, speed * Time.deltaTime );
                } else if ( transform.position == dest ) {                          // arrives at destination
                    status = 0;
                    middleArrived = false;
                    moving = false;
                } else {                                                            // moves from middle to destination
                    transform.position = Vector3.MoveTowards( transform.position, dest, speed * Time.deltaTime );
                }

            } else if ( status == 2 ) {                                             // boat moves

                if ( transform.position != dest ) {                                 // moves to destination
                    moving = true;
                    transform.position = Vector3.MoveTowards( transform.position, dest, speed * Time.deltaTime );
                }  else {                                                           // arrives at destination
                    status = 0;
                    moving = false;
                }

            }
        }

        public void SetDestination( Vector3 _dest )
        {
            dest = _dest;
            middle = _dest;

            if ( transform.position.y == _dest.y ) {            // boat
                status = 2;
            } else if ( transform.position.y > _dest.y ) {      // character moves from bank to boat
                middle.y = transform.position.y;
                status = 1;
            } else {                                            // character moves from boat to bank
                middle.x = transform.position.x;
                status = 1;
            }
        }

        public static bool IsMoving()
        {
            return moving;
        }

        public void Reset()
        {
            status = 0;
        }
    }

    /*------------------------------- CharacterController -------------------------------*/
    public class MyCharacterController
    {
        public enum CharacterType { priest, devil };
        static int currID = 0;

        GameObject character;
        CharacterType type;
        Movement moveScript;
        ClickGUI click;

        bool isOnBoat;
        BankController bank;

        int id;
        public string name;

        public MyCharacterController( CharacterType _type )
        {
            type = _type;
            id = currID++;
            name = "character";

            if ( type == CharacterType.priest ) {
                character = GameObject.Instantiate( Resources.Load( "Prefabs/Priest", typeof( GameObject ) ) ) as GameObject;
            } else {
                character = GameObject.Instantiate( Resources.Load( "Prefabs/Devil", typeof( GameObject ) ) ) as GameObject;
            }

            moveScript = character.AddComponent( typeof( Movement ) ) as Movement;
            click = character.AddComponent( typeof( ClickGUI ) ) as ClickGUI;
            click.SetCharacter( this );
        }

        public void SetId( int _id )
        {
            id = _id;
        }

        public int GetId()
        {
            return id;
        }

        public void SetPosition( Vector3 _pos )
        {
            character.transform.position = _pos;
        }

        public void MoveToPosition( Vector3 _pos )
        {
            moveScript.SetDestination( _pos );
        }

        public CharacterType GetTheType()
        {
            return type;
        }

        public bool IsOnBoat()
        {
            return isOnBoat;
        }

        public void GetOnBoat( BoatController theBoat )
        {
            bank = null;
            character.transform.parent = theBoat.GetObj().transform;
            isOnBoat = true;
        }

        public void GetOnBank( BankController theBank )
        {
            bank = theBank;
            character.transform.parent = null;
            isOnBoat = false;
        }

        public BankController GetBankController()
        {
            return bank;
        }

        public void Reset()
        {
            moveScript.Reset();
            isOnBoat = false;
            bank = ( Director.GetInstance().CurrentSceneController as FirstController ).GetBank_from();
            GetOnBank( bank );
            MoveToPosition( bank.GetEmptyPosition() );
            bank.GetOnBank( this );
        }
    }

    /*------------------------------- BankController -------------------------------*/
    public class BankController
    {
        GameObject bank;

        MyCharacterController[] characters;
        readonly Vector3[] Postion;              // characters' position on the bank
        int side;                                // -1: from side, 1: to side
            
        public BankController( bool opposite )
        {
            if ( opposite ) {
                side = 1;
                bank = GameObject.Instantiate( Resources.Load( "Prefabs/Bank" ), new Vector3( 5, 1, 0 ), Quaternion.identity ) as GameObject;
                Postion = new Vector3[] { new Vector3( 6.75f, 1.9f ), new Vector3( 6.05f, 1.9f ), new Vector3( 5.35f, 1.9f ),
                                            new Vector3( 4.65f, 1.9f ), new Vector3( 3.95f, 1.9f ), new Vector3( 3.25f, 1.9f )};
            } else {
                side = -1;
                bank = GameObject.Instantiate( Resources.Load( "Prefabs/Bank" ), new Vector3( -5, 1, 0 ), Quaternion.identity ) as GameObject;
                Postion = new Vector3[] { new Vector3( -6.75f, 1.9f ), new Vector3( -6.05f, 1.9f ), new Vector3( -5.35f, 1.9f ),
                                            new Vector3( -4.65f, 1.9f ), new Vector3( -3.95f, 1.9f ), new Vector3( -3.25f, 1.9f )};
            }

            characters = new MyCharacterController[6];
        }

        public int WhichSide()
        {
            return side;
        }

        public Vector3 GetEmptyPosition()
        {
            for ( int i = 0; i < 6; ++i ) {
                if ( characters[i] == null ) {
                    return Postion[i];
                }
            }

            return Vector3.zero;
        }

        public void GetOnBank( MyCharacterController character )
        {
            int emptyIdx;
            for ( emptyIdx = 0; emptyIdx < 6; ++emptyIdx ) {
                if ( characters[emptyIdx] == null )
                    break;
            }

            characters[emptyIdx] = character;
        }

        public MyCharacterController GetOffBank( int cid )
        {
            for ( int i = 0; i < characters.Length; ++i ) {
                if ( characters[i] != null && characters[i].GetId() == cid ) {
                    MyCharacterController temp = characters[i];
                    characters[i] = null;
                    return temp;
                }
            }

            return null;
        }

        public int GetCharacterNum( MyCharacterController.CharacterType _type )
        {
            int res = 0;
            for ( int i = 0; i < characters.Length; ++i ) {
                if ( characters[i] != null && characters[i].GetTheType() == _type )
                    ++res;
            }

            return res;
        }

        public MyCharacterController GetOneCharacter( MyCharacterController.CharacterType _type )
        {
            MyCharacterController one = null;
            for ( int i = 0; i < characters.Length; ++i ) {
                if( characters[i] != null && characters[i].GetTheType() == _type ) {
                    one = characters[i];
                }
            }

            return one;
        }

        public void Reset()
        {
            characters = new MyCharacterController[6];
        }
    }

    /*------------------------------- BoatController -------------------------------*/
    public class BoatController
    {
        GameObject boat;
        Movement moveScript;
        ClickGUI click;

        MyCharacterController[] passengers;
        Vector3[] boatPos_from;
        Vector3[] boatPos_to;

        int side;       // -1: from, 1: to

        public BoatController()
        {
            boat = GameObject.Instantiate( Resources.Load( "Prefabs/Boat" ), new Vector3( -1.8f, 1, 0 ), Quaternion.identity ) as GameObject;
            moveScript = boat.AddComponent( typeof( Movement ) ) as Movement;
            click = boat.AddComponent( typeof( ClickGUI ) ) as ClickGUI;

            passengers = new MyCharacterController[2];
            boatPos_from = new Vector3[] { new Vector3( -1.25f, 1.47f ), new Vector3( -2.35f, 1.47f ) };
            boatPos_to = new Vector3[] { new Vector3( 2.35f, 1.47f ), new Vector3( 1.25f, 1.47f ) };

            side = -1;
        }

        public bool IsEmpty()
        {
            return passengers[0] == null && passengers[1] == null;
        }

        public bool IsFull()
        {
            return passengers[0] != null && passengers[1] != null;
        }

        public int WhichSide()
        {
            return side;
        }

        public void MoveToOpposite()
        {
            moveScript.SetDestination( new Vector3( boat.transform.position.x * -1.0f, boat.transform.position.y ) );
            side *= -1;
        }

        public int GetEmptyIdx()
        {
            int emptyIdx = -1;
            if ( passengers[0] == null ) {
                emptyIdx = 0;
            } else if ( passengers[1] == null ) {
                emptyIdx = 1;
            }

            return emptyIdx;
        }

        public Vector3 GetEmptyPos()
        {
            int emptyIdx = GetEmptyIdx();
            if ( side == -1 )
                return boatPos_from[emptyIdx];
            else
                return boatPos_to[emptyIdx];
        }

        public int GetCharacterNum( MyCharacterController.CharacterType _type )
        {
            int res = 0;
            if ( passengers[0] != null && passengers[0].GetTheType() == _type )
                ++res;
            if ( passengers[1] != null && passengers[1].GetTheType() == _type )
                ++res;

            return res;
        }

        public MyCharacterController GetOneCharacter( MyCharacterController.CharacterType _type )
        {
            MyCharacterController one = null;
            if ( passengers[0] != null && passengers[0].GetTheType() == _type ) {
                one = passengers[0];
            } else if ( passengers[1] != null && passengers[1].GetTheType() == _type ) {
                one = passengers[1];
            }

            return one;
        }

        public void GetOnBoat( MyCharacterController cha )
        {
            passengers[GetEmptyIdx()] = cha;
        }

        public MyCharacterController GetOffBoat( int cid )
        {
            MyCharacterController cha = null;
            if ( passengers[0] != null && passengers[0].GetId() == cid ) {
                cha = passengers[0];
                passengers[0] = null;
            } else if ( passengers[1] != null && passengers[1].GetId() == cid ) {
                cha = passengers[1];
                passengers[1] = null;
            }

            return cha;
        }

        public GameObject GetObj()
        {
            return boat;
        }

        public void Reset()
        {
            moveScript.Reset();
            if ( side == 1 )
                MoveToOpposite();

            passengers = new MyCharacterController[2];
        }

    }
}
