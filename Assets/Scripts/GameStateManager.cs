using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using FMOD.Studio;
using Unity.VisualScripting;
using System;
using UnityEngine.Events;

/**
 * This class is the main entry point into the game. 
 *
 * The Game State Manager keeps track of deaths, current section, and
 * manages starting Fmod and the Conductor (rhythm game logic).
 *
 * It also handles restarting the conductor / fmod upon a level failure. 
 */
public class GameStateManager : MyBox.Singleton<GameStateManager>
{
    public enum GameState
    {
        None,
        Prechorus,
        Chorus,
        Verse1,
        Response1,
        Verse2,
        Response2,
        Verse3,
        Response3,
        Ending
    }

    [SerializeField] [ReadOnly] private GameState gameState;
    public List<ChartData> charts;
    public int NumFails = 0;
    [SerializeField] GameObject dialogueManagerObj;

    //FMOD JANK
    private FMOD.Studio.EVENT_CALLBACK _musicFmodCallback;
    private FMOD.Studio.EventInstance _musicEventInstance;
    //FMOD.Studio.TIMELINE_MARKER_PROPERTIES? _prevMarker;
    private FMOD.Studio.TIMELINE_MARKER_PROPERTIES? _currMarker;
    private HashSet<string> _labelsPassed = new HashSet<string>();
    public HashSet<string> LabelsPassed => _labelsPassed;


    private bool _inFailureState;

    DialogueTest _dialogueManager;

    private FMODUnity.StudioEventEmitter musicEmitter;
    private FMODUnity.StudioEventEmitter sfxEmitter;
    private FMODUnity.StudioEventEmitter ambienceEmitter;

    public static UnityEvent beatEvent = new UnityEvent();

    void Awake() {
        InitializeSingleton();

        gameState = GameState.None;
        _inFailureState = false;
    }

    private void OnEnable()
    {
        PlayerPerformanceManager.OnLevelFailed += OnLevelFailed;
    }

    private void OnDisable()
    {
        PlayerPerformanceManager.OnLevelFailed -= OnLevelFailed;
    }

    // Start is called before the first frame update
    void Start()
    {
        FMODUnity.StudioEventEmitter[] emitters = GetComponents<FMODUnity.StudioEventEmitter>();

        musicEmitter = emitters[0];
        sfxEmitter = emitters[1];
        ambienceEmitter = emitters[2];
        
        ambienceEmitter.Play();

        _musicFmodCallback = new FMOD.Studio.EVENT_CALLBACK(FMODEventCallback);

        //_musicEventInstance.start();

        _dialogueManager = dialogueManagerObj.GetComponent<DialogueTest>();
        _dialogueManager.endNodeSignal.AddListener(OnDialogueEnd);

        //StartFMODEvent();
    }

    public void StartFMODEvent()
    {
        musicEmitter.Play();
        _labelsPassed.Clear();
        _musicEventInstance = musicEmitter.EventInstance;
        _musicEventInstance.setCallback(_musicFmodCallback, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT | FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER);
    }

    [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
    static FMOD.RESULT FMODEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, System.IntPtr _event, System.IntPtr parameterPtr)
    {
        if (type == FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER)
        {
            var game = GameStateManager.Instance;
            var parameter = (FMOD.Studio.TIMELINE_MARKER_PROPERTIES)System.Runtime.InteropServices.Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_MARKER_PROPERTIES));
            //GameStateManager.Instance._prevMarker = GameStateManager.Instance._currMarker;
            game._currMarker = parameter;
            string labelName = (string) parameter.name;
            UnityEngine.Debug.LogFormat("<b><color=green>Marker: {0}</color></b>", (string)parameter.name);
            
            if (!game.LabelsPassed.Contains(labelName))
            {
                //First time passing label
                game.LabelsPassed.Add(labelName);
                game.NumFails = 0;
                game.musicEmitter.SetParameter("NumFails", game.NumFails);
                game.UpdateGameState(labelName);
                //Debug.Log($"Num Fails: {game.NumFails}");
            }
            //TODO: Identify why there is no callback on passing a label after fail state
            Debug.Log("<color=green>GameStateManager.FMODCallBack: Updating GameState with label: " + labelName + "</color>");
        }
        if (type == FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT)
        {
            beatEvent.Invoke();
        }

        return FMOD.RESULT.OK;
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    UpdateGameState();
        //}
    }

    [SerializeField] private TrackMover credits;

    /**
     * CALLED BY FMOD reaching the end of a section.
     *
     * Progresses the current state of the game. Calls responsible managers.
     */
    void UpdateGameState(string state)
    {
        Enum.TryParse(state, out gameState);
        Debug.Log($"SWITCH THE GAME STATE TO {gameState}");
        //gameState = 0;
        ProcessGameState();
    }

    void UpdateGameState(int i = 1) {
        gameState += i;
        Debug.Log($"SWITCH THE GAME STATE TO {gameState}");
        ProcessGameState();
    }

    void ProcessGameState()
    {
        switch (gameState)
        {
            case GameState.None: // this is the main menu
                // *** main menu witchcraft
                break;
            case GameState.Prechorus:
                if (ambienceEmitter.IsPlaying())
                {
                    ambienceEmitter.Stop();
                }
                _dialogueManager.StartNode("Tutorial");
                PlayChart(0);
                break;
            case GameState.Chorus:
                _dialogueManager.StartNode("Loop1");
                Conductor.Instance.Pause();
                break;
            case GameState.Verse1:
                PlayChart(1);
                break;
            case GameState.Response1:
                _dialogueManager.StartNode("Post1");
                Conductor.Instance.Pause();
                break;
            case GameState.Verse2:
                PlayChart(2);
                break;
            case GameState.Response2:
                _dialogueManager.StartNode("Post2");
                Conductor.Instance.Pause();
                break;
            case GameState.Verse3:
                PlayChart(3);
                break;
            case GameState.Response3:
                _dialogueManager.StartNode("Post3");
                break;
            case GameState.Ending:
                credits.PutOnScreen();
                break;
            default:
                Debug.LogError("GameStateManager.UpdateGameState(): Could not find Gamestate.");
                break;
        }
    }

    void PlayFMODFromLastMarker()
    {
        Debug.Log("GameStateManager.NextFmodSection(): Fired.");
        // If in fail state, resume
        if (!musicEmitter.IsPlaying())
        {
            musicEmitter.Play();
            _musicEventInstance = musicEmitter.EventInstance;
            _musicEventInstance.setTimelinePosition(_currMarker == null ? 0 : _currMarker.Value.position);
            _musicEventInstance.setCallback(_musicFmodCallback, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT | FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER);
            ambienceEmitter.Stop();
            UpdateGameState(0);
        }
    }

    private void PlayChart(int index)
    {
        if (index >= charts.Count)
        {
            Debug.LogError($"Chart not found {index}");
            return;
        }

        Conductor.Instance.Play(charts[index]);
    }

    /**
     * Called by PlayerPerformanceManager when the user fails. If the user fails, restart fmod sequence, start game chart, and play fail sfx.
     *
     */
    public void OnLevelFailed()
    {
        NumFails++;
        switch(NumFails){
            case 1:
                _dialogueManager.StartNode("Strike1");
                break;
            case 2:
                _dialogueManager.StartNode("Strike2");
                break;
            case 3:
                _dialogueManager.StartNode("Strike3");
                //gameover logic, return to start of game instead of restarting
                StartCoroutine(GameOver());
                break;
            default:
                break;
        }

        Conductor.Instance.Pause();

        //Update FMOD timeline position
        sfxEmitter.Play();
        musicEmitter.SetParameter("NumFails", NumFails);
        musicEmitter.Stop();
        _inFailureState = true;

        //ambienceEmitter.Play();
    }

    private void OnDialogueEnd()
    {
        //THIS IS SCUFFED PLZ FIX
        if (_inFailureState)
        {
            RestartLevel();
        }
    }

    public void RestartLevel()
    {
        Track.Instance.Rewind(() =>
        {
            _inFailureState = false;
            PlayFMODFromLastMarker();
        });
    }

    private IEnumerator GameOver() {
        
        GameObject.Find("Game Over").SetActive(true);
        yield return new WaitForSeconds(3);
        //Debug.Log("test");
        GameObject.FindObjectOfType<AppManager>().ReloadMainScene();
    }

}