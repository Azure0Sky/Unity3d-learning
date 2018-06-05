using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerObject : MonoBehaviour
{
    Transform timerTransform;
    Text timerText;
    float timerNum;

    public bool disable;

    // Use this for initialization
    void Start()
    {
        timerTransform = transform.Find( "Timer" );
        if ( timerTransform == null ) {
            throw new MissingReferenceException( "No timer child" );
        } else {
            timerText = timerTransform.gameObject.GetComponent<Text>();
        }

        timerNum = 0;
        disable = false;
    }

    // Update is called once per frame
    void Update()
    {
        if ( timerTransform != null ) {
            timerNum += Time.smoothDeltaTime;
            timerText.text = timerNum.ToString();
        }

        if ( timerNum >= 10 ) {
            timerNum = 0;
            disable = true;
            //transform.gameObject.SetActive( false );
        }
    }

    public void SetDisable( bool val )
    {
        disable = val;
    }
}
