using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class ShowPanels : MonoBehaviour
{
    public GameObject optionsPanel;							//Store a reference to the Game Object OptionsPanel 
    public GameObject optionsTint;							//Store a reference to the Game Object OptionsTint 
    public GameObject menuPanel;							//Store a reference to the Game Object MenuPanel 
    public GameObject pausePanel;                           //Store a reference to the Game Object PausePanel
    public GameObject modalPanel;                           //Store a reference to the Game Object ModalPanel
    public GameObject modalTint;                            //Store a reference to the Game Object ModalTint
    public GameObject[] timerPanel;                         //Store references to the Game Object TimerPanel

    private GameObject activePanel;                         
    private MenuObject activePanelMenuObject;
    private EventSystem eventSystem;

    private TimerObject[] timerObjects;

    private int timerNumber;                                //Max: 3


    private void SetSelection( GameObject panelToSetSelected )
    {
        activePanel = panelToSetSelected;
        activePanelMenuObject = activePanel.GetComponent<MenuObject>();
        if ( activePanelMenuObject != null ) {
            activePanelMenuObject.SetFirstSelected();
        }
    }

    public void Start()
    {
        SetSelection( menuPanel );
        //TimerPanel = new GameObject[4];
        timerNumber = 0;

        timerObjects = new TimerObject[4];
        for ( int i = 0; i < 4; ++i ) {
            timerObjects[i] = timerPanel[i].GetComponent<TimerObject>();
        }
    }

    //Call this function to activate and display the Options panel during the main menu
    public void ShowOptionsPanel()
	{
        optionsPanel.SetActive( true );
        optionsTint.SetActive( true );
        menuPanel.SetActive( false );
        SetSelection( optionsPanel );

    }

	//Call this function to deactivate and hide the Options panel during the main menu
	public void HideOptionsPanel()
	{
        menuPanel.SetActive( true );
        optionsPanel.SetActive( false );
        optionsTint.SetActive( false );
	}

	//Call this function to activate and display the main menu panel during the main menu
	public void ShowMenu()
	{
        menuPanel.SetActive( true );
        SetSelection( menuPanel );
    }

	//Call this function to deactivate and hide the main menu panel during the main menu
	public void HideMenu()
	{
        menuPanel.SetActive( false );

	}
	
	//Call this function to activate and display the Pause panel during game play
	public void ShowPausePanel()
	{
        pausePanel.SetActive( true );
        optionsTint.SetActive( true );
        SetSelection(pausePanel);
    }

	//Call this function to deactivate and hide the Pause panel during game play
	public void HidePausePanel()
	{
        pausePanel.SetActive( false );
        optionsTint.SetActive( false );

	}

    public void ShowModalPanel()
    {
        modalPanel.transform.SetAsLastSibling();
        modalPanel.SetActive( true );
        modalTint.SetActive( true );

        // Disable the main menu panel
        CanvasGroup menuCanvasGroup = menuPanel.GetComponent<CanvasGroup>();
        menuCanvasGroup.interactable = false;
        menuCanvasGroup.blocksRaycasts = false;
    }

    public void HideModalPanel()
    {
        modalPanel.SetActive( false );
        modalTint.SetActive( false );

        // Enable the main menu panel
        CanvasGroup menuCanvasGroup = menuPanel.GetComponent<CanvasGroup>();
        menuCanvasGroup.interactable = true;
        menuCanvasGroup.blocksRaycasts = true;
    }

    public void ShowTimerPanel()
    {
        if ( timerNumber >= 4 )
            return;

        for ( int i = 0; i < 4; ++i ) {
            if ( !timerPanel[i].activeSelf ) {
                timerPanel[i].SetActive( true );
                break;
            }
        }

        ++timerNumber;
    }

    private void Update()
    {
        HideTimerPanel();
    }

    public void HideTimerPanel()
    {
        for ( int i = 0; i < timerObjects.Length; ++i ) {
            if ( timerObjects[i].disable ) {
                timerObjects[i].SetDisable( false );
                timerObjects[i].gameObject.SetActive( false );
                --timerNumber;
                Debug.Log( "Hide timer" );
            }
        }
    }
}
