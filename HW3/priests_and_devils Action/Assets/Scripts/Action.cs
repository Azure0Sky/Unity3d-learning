using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameBase;

//public enum ActionEventType:int { started, comp }
namespace ActionController
{
    public class Action : ScriptableObject
    {
        public bool enable = true;
        public bool destroy = false;

        public GameObject gameObjcect { get; set; }
        public Transform transform { get; set; }
        public IActionCallback callback { get; set; }

        protected Action() { }

        public virtual void Start()
        {
            throw new System.NotImplementedException();
        }

        public virtual void Update()
        {
            throw new System.NotImplementedException();
        }
    }

    public class MoveToAction : Action
    {
        public Vector3 target;
        public float speed;

        public static MoveToAction GetAction( Vector3 target, float speed )
        {
            MoveToAction action = ScriptableObject.CreateInstance<MoveToAction>();
            action.target = target;
            action.speed = speed;

            return action;
        }

        public override void Update()
        {
            transform.position = Vector3.MoveTowards( transform.position, target, speed * Time.deltaTime );
            Debug.Log( "move action update" );

            if ( transform.position == target ) {
                Debug.Log( "arrived" );
                destroy = true;
                callback.ActionEvent( this );
            }
        }

        public override void Start()
        {

        }
    }

    //public class SequenceAction : Action, IActionCallback
    //{
    //    public List<Action> sequence;
    //    public int repeat = -1;
    //    public int start = 0;

    //    public static SequenceAction GetSequenceAction( int repeat, int start, List<Action> sequence )
    //    {
    //        SequenceAction seqAction = ScriptableObject.CreateInstance<SequenceAction>();
    //        seqAction.repeat = repeat;
    //        seqAction.sequence = sequence;
    //        seqAction.start = start;

    //        return seqAction;
    //    }

    //    public override void Update()
    //    {
    //        if ( sequence.Count == 0 )
    //            return;

    //        if ( start < sequence.Count ) {
    //            sequence[start].Update();
    //        }
    //    }

    //    public void ActionEvent( Action source, int intParam = 0, string strParam = null, object objectParam = null )
    //    {
    //        source.destroy = false;

    //        ++start;
    //        if ( start >= sequence.Count ) {
    //            start = 0;

    //            if ( repeat > 0 )
    //                --repeat;
    //            if ( repeat == 0 ) {
    //                destroy = true;
    //                callback.ActionEvent( this );
    //            }
    //        }
    //    }

    //    public override void Start()
    //    {
    //        foreach ( Action action in sequence ) {
    //            action.gameObjcect = this.gameObjcect;
    //            action.transform = this.transform;
    //            action.callback = this;
    //            action.Start();
    //        }
    //    }

    //    private void OnDestroy()
    //    {
    //        //
    //    }
    //}

    public interface IActionCallback
    {
        void ActionEvent( Action source, int intParam = 0, string strParam = null, object objectParam = null );
    }

    public class ActionManager : MonoBehaviour
    {
        private Dictionary<int, Action> actions = new Dictionary<int, Action>();
        private List<Action> waitingAdd = new List<Action>();
        private List<int> waitingDelete = new List<int>();

        protected void Update()
        {
            foreach ( Action ac in waitingAdd ) {
                actions[ac.GetInstanceID()] = ac;
            }

            waitingAdd.Clear();

            foreach ( KeyValuePair<int, Action> kv in actions ) {
                Action ac = kv.Value;
                if ( ac.destroy ) {
                    Debug.Log( "add to destroy list" );
                    waitingDelete.Add( ac.GetInstanceID() );
                } else if ( ac.enable ) {
                    ac.Update();
                }
            }

            foreach ( int key in waitingDelete ) {
                Debug.Log( "Destorying" );
                Action ac = actions[key];
                actions.Remove( key );
                DestroyObject( ac );
            }
        }

        public void RunAction( GameObject gameObject, Action action, IActionCallback manager )
        {
            action.gameObjcect = gameObject;
            action.transform = gameObject.transform;
            action.callback = manager;

            waitingAdd.Add( action );
            action.Start();
        }
    }
}