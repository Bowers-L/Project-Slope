using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;

/*IMPORTANT TERMINOLOGY:
 * -I'm using the term "moment" instead of "position" to represent a note's placement in the track and how much 
 * time has passed since the start of the track,
 * this is in order to avoid confusion between actual spacial unity positions and temporal positions.
 * 
 * Chart Units (or CU) is used to represent the moment of a note. 192 CU = 1 Quater Note Beat.
 */

public class Conductor : MonoBehaviour
{
    public enum TimingMethod
    {
        UnityTime,  //This will be out of sync, but is useful for testing.
        FMODDsp     //Actually use this in game since it will sync better.
    }

    public const TimingMethod timingMethod = TimingMethod.UnityTime;

    [SerializeField] ChartData chart;
    [SerializeField] private int firstBeatOffsetSeconds;
    [SerializeField] private Track track;

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

    public ChartData Chart => chart;

    private void Awake()
    {
        track = FindObjectOfType<Track>();
        isPaused = true;
    }

    private void Start()
    {
        beatsPerSecond = (float) chart.bpm / 60f;
        unitsPerBeat = chart.unitsPerBeat;

        fmodCore = FMODUnity.RuntimeManager.CoreSystem;
        fmodCore.getMasterChannelGroup(out masterChannel);
        fmodCore.getSoftwareFormat(out sampleRateHertz, out _, out _);

        Play(); //FOR TESTING (call from FMOD event instead)
    }

    private void Update()
    {
        if (!isPaused)
        {
            currMomentSeconds = TimeSinceStart - firstBeatOffsetSeconds;

            Debug.Log($"Current Moment Beat: {CurrMomentBeats}");
        }
    }

    public void Play()
    {
        Debug.Log("Starting the Track");
        startTime = GetCurrentTime();
        currMomentSeconds = -firstBeatOffsetSeconds;
        isPaused = false;
        track.Init(this);
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

    private float GetDSPTime()
    {
        ulong numSamples;
        masterChannel.getDSPClock(out numSamples, out _);

        double dspTime = (double)numSamples / sampleRateHertz;
        return (float) dspTime;
    }
}