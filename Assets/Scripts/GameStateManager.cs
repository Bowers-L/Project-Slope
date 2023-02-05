using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * This class is the main entry point into the game. 
 *
 * The Game State Manager keeps track of deaths, current section, and
 * manages starting Fmod and the Conductor (rhythm game logic).
 *
 * It also handles restarting the conductor / fmod upon a level failure. 
 */
public class GameStateManager : MonoBehaviour
{
    public enum GameState
    {
        Tutorial,
        FirstChorus,
        Level1,
        Response1,
        Level2,
        Response2,
        Level3,
        Response3,
        Ending
    }

    private GameState gameState;
    public List<ChartData> charts;
    private int deaths = 0;

    FMOD.Studio.EVENT_CALLBACK _musicFmodCallback;
    FMOD.Studio.EventInstance _musicEventInstance;

    [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
    static FMOD.RESULT FMODEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, System.IntPtr _event, System.IntPtr parameterPtr)
    {
        if (type == FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER)
        {
            var parameter = (FMOD.Studio.TIMELINE_MARKER_PROPERTIES)System.Runtime.InteropServices.Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_MARKER_PROPERTIES));
            UnityEngine.Debug.LogFormat("Marker: {0}", (string)parameter.name);
        }

        return FMOD.RESULT.OK;
    }

    // Start is called before the first frame update
    void Start()
    {
        gameState = GameState.Tutorial;
        
        FMOD.Studio.EventDescription desc = FMODUnity.RuntimeManager.GetEventDescription("event:/Parent");
        desc.createInstance(out _musicEventInstance);
        
        _musicFmodCallback = new FMOD.Studio.EVENT_CALLBACK(FMODEventCallback);

        _musicEventInstance.setCallback(_musicFmodCallback, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT | FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER);

        _musicEventInstance.start();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            UpdateGameState();
        }
    }

    /**
     * CALLED BY FMOD reaching the end of a section.
     *
     * Progresses the current state of the game. Calls responsible managers.
     */
    void UpdateGameState()
    {
        gameState++;
        gameState = 0;
        switch (gameState)
        {
            case GameState.Tutorial: // game
                // *** Start Chart 1 
                Conductor.Instance.Play(charts[0]);
                // *** start Tutorial part of Fmod Timeline

                FMODUnity.StudioEventEmitter emitter = GetComponent<FMODUnity.StudioEventEmitter>();
                emitter.Play();

                break;
            case GameState.FirstChorus: // dialog
                // *** Start Dialog 1
                // *** start First Chorus part of Fmod Timeline
                break;
            case GameState.Level1: // game
                // *** Start Chart 2
                Conductor.Instance.Play(charts[1]);
                // *** start Tutorial part of Fmod Timeline
                //FMODUnity.StudioEventEmitter emitter = GetComponent<FMODUnity.StudioEventEmitter>();
                //emitter.Play();
                break;
            case GameState.Response1: // dialog
                // *** Start Dialog 2
                break;
            case GameState.Level2: // game
                // *** Start Chart 3
                break;
            case GameState.Response2: // dialog
                // *** Start Dialog 3
                break;
            case GameState.Level3: // game 
                // *** Start Chart 4
                break;
            case GameState.Response3: // dialog
                // *** Start Dialog 4
                break;
            case GameState.Ending: // dialog!
                // *** Start Dialog 5
                break;
        }
        
        void HaltCurrentLevel()
        {
            deaths++;
            // *** Halt current instance of Fmod Timeline, also stop the current instance of game
            gameState--;
            UpdateGameState();
        }

    }
}

// fmod just continues unless update game state explicitly stops it
