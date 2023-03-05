using FMODUnity;
using MyBox;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

/*IMPORTANT TERMINOLOGY:
 * -I'm using the term "moment" instead of "position" to represent a note's placement in the track and how much 
 * time has passed since the start of the track,
 * this is in order to avoid confusion between actual spacial unity positions and temporal positions.
 * 
 * Chart Units (or CU) is used to represent the moment of a note. 192 CU = 1 Quater Note Beat.
 */

public class Conductor : MyBox.Singleton<Conductor>
{
    public enum TimingMethod
    {
        UnityTime,  //This will be out of sync, but is useful for testing.
        FMODDsp,     //Actually use this in game since it will sync better, but there are delays and issues and stuff with event starts.
        FMODTimelinePos,
    }

    [SerializeField] private ChartData testChart;
    [SerializeField] private float delayPerFailAdjustment;
    [SerializeField] private GameObject brainMeterObject;
    [SerializeField] private TimingMethod timingMethod;
    Animator brainMeterAnimator;

    private float currMomentSeconds;    //How much time has elapsed in the song
    private bool isPaused;
    public bool Paused => isPaused;

    //Chart specific parameters
    private float _beatsPerSecond;
    private float _unitsPerBeat;
    private float _startTimeSec;

    //FMOD Dsp Clock Things
    private FMOD.System fmodCore;
    private FMOD.ChannelGroup masterChannel;
    private int sampleRateHertz;

    //FMOD Timeline Method

    public float CurrMomentBeats => currMomentSeconds * _beatsPerSecond;
    public int CurrMomentCU => (int) (currMomentSeconds * _beatsPerSecond * _unitsPerBeat);

    public float TimeSinceStart => GetCurrentTime() - _startTimeSec;

    private ChartData _currChart;
    public ChartData Chart => _currChart;

    public delegate void OnPlayDel(ChartData chart);
    public delegate void OnPauseDel();
    public static event OnPlayDel OnPlay;
    public static event OnPauseDel OnPause;

    private void Awake()
    {
        InitializeSingleton(false);
        isPaused = true;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        fmodCore = FMODUnity.RuntimeManager.CoreSystem;
        fmodCore.getMasterChannelGroup(out masterChannel);
        fmodCore.getSoftwareFormat(out sampleRateHertz, out _, out _);

        if (testChart != null)
        {
            //Play(testChart); //FOR TESTING (call from Game Manager Instead.)
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        brainMeterObject = GameObject.Find("Track and buffer").transform.GetChild(0).gameObject;
        brainMeterAnimator = brainMeterObject.GetComponent<Animator>();
    }

    private void Update()
    {
        if (!isPaused)
        {
            if (TimeSinceStart < 5f)
            {
                //Debug.Log($"Curr Timeline Pos: {TimeSinceStart+_startTimeSec}");
                //Debug.Log($"Time Diff: {TimeSinceStart}");
            }

            currMomentSeconds = TimeSinceStart - _currChart.firstBeatOffsetSeconds;

            //Debug.Log($"Current Moment Beat: {CurrMomentBeats}");
        }
    }

    public void Play(ChartData chart)
    {
        brainMeterObject = GameObject.Find("Track and buffer").transform.GetChild(0).gameObject;
        brainMeterAnimator = brainMeterObject.GetComponent<Animator>();
        
        Debug.Log($"Starting the Chart {chart.name}");
        _currChart = chart;

        //Set Chart Parameters
        _beatsPerSecond = (float)_currChart.bpm / 60f;
        _unitsPerBeat = _currChart.unitsPerBeat;

        //Set Internal State
        currMomentSeconds = -_currChart.firstBeatOffsetSeconds;
        isPaused = false;

        brainMeterObject.SetActive(true);

        OnPlay?.Invoke(chart);

        _startTimeSec = GetCurrentTime();
        //Debug.Log($"START TIME: {_startTimeSec}");
    }

    public void PlayFromTimelinePos(ChartData chart, int timelinePosMilliS)
    {
        Play(chart);
        _startTimeSec = (float) (timelinePosMilliS / 1000f);
    }

    public void Pause()
    {
        isPaused = true;
        OnPause?.Invoke();
    }

    public void Resume()
    {
        isPaused = false;
    }

    private float GetCurrentTime()
    {
        switch (timingMethod)
        {
            case TimingMethod.UnityTime:
                return Time.time;
            case TimingMethod.FMODDsp:
                return GetDSPTime();
            case TimingMethod.FMODTimelinePos:
            default:
                return GetTimelinePosSeconds();
        }

    }

    public int BeatsToCU(float beats)
    {
        return (int)(beats * _currChart.unitsPerBeat);
    }

    private float GetDSPTime()
    {
        ulong numSamples;
        masterChannel.getDSPClock(out numSamples, out _);

        double dspTime = (double) numSamples / sampleRateHertz;
        return (float) dspTime;
    }

    private float GetTimelinePosSeconds()
    {
        int posMilliseconds;
        GameStateManager.Instance.MusicEvent.getTimelinePosition(out posMilliseconds);
        return (float) posMilliseconds / 1000f;
    }
}