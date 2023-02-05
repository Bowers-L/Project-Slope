using MyBox;
using UnityEngine;

/*IMPORTANT TERMINOLOGY:
 * -I'm using the term "moment" instead of "position" to represent a note's placement in the track and how much 
 * time has passed since the start of the track,
 * this is in order to avoid confusion between actual spacial unity positions and temporal positions.
 * 
 * Chart Units (or CU) is used to represent the moment of a note. 192 CU = 1 Quater Note Beat.
 */

public class Conductor : Singleton<Conductor>
{
    public enum TimingMethod
    {
        UnityTime,  //This will be out of sync, but is useful for testing.
        FMODDsp     //Actually use this in game since it will sync better.
    }

    public const TimingMethod timingMethod = TimingMethod.FMODDsp;

    [SerializeField] private ChartData testChart;

    private float currMomentSeconds;    //How much time has elapsed in the song
    private bool isPaused;

    //Chart specific parameters
    private float beatsPerSecond;
    private float unitsPerBeat;
    private float startTime;

    //FMOD Dsp Clock Things
    private FMOD.System fmodCore;
    private FMOD.ChannelGroup masterChannel;
    private int sampleRateHertz;

    public float CurrMomentBeats => currMomentSeconds * beatsPerSecond;
    public int CurrMomentCU => (int) (currMomentSeconds * beatsPerSecond * unitsPerBeat);

    public float TimeSinceStart => GetCurrentTime() - startTime;

    private ChartData _currChart;
    public ChartData Chart => _currChart;

    private void Awake()
    {
        InitializeSingleton();
        isPaused = true;
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

    private void Update()
    {
        if (!isPaused)
        {
            currMomentSeconds = TimeSinceStart - _currChart.firstBeatOffsetSeconds;

            //Debug.Log($"Current Moment Beat: {CurrMomentBeats}");
        }
    }

    public void Play(ChartData chart)
    {
        Debug.Log("Starting the Track");
        _currChart = chart;

        //Set Chart Parameters
        beatsPerSecond = (float)_currChart.bpm / 60f;
        unitsPerBeat = _currChart.unitsPerBeat;

        //Set Internal State
        startTime = GetCurrentTime();
        currMomentSeconds = -_currChart.firstBeatOffsetSeconds;
        isPaused = false;

        Track.Instance.Init();
        PlayerPerformanceManager.Instance.StartNewSection();
    }

    public void Pause()
    {
        isPaused = true;
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
        }

    }

    public int BeatsToCU(float beats)
    {
        return (int)(beats * Chart.unitsPerBeat);
    }

    private float GetDSPTime()
    {
        ulong numSamples;
        masterChannel.getDSPClock(out numSamples, out _);

        double dspTime = (double) numSamples / sampleRateHertz;
        return (float) dspTime;
    }
}