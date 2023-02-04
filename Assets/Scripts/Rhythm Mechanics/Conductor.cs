using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;

/*IMPORTANT TERMINOLOGY:
 * -I'm using the term "moment" instead of "position" to represent a note's placement in the track,
 * this is in order to avoid confusion between actual spacial unity positions and temporal positions.
 * 
 * Chart Units (or CU) is used to represent the moment of a note. 192 CU = 1 Quater Note Beat.
 */

public class Conductor : MonoBehaviour
{

    [SerializeField] ChartData chart;
    [SerializeField] private int firstBeatOffsetSeconds;

    private float currMomentSeconds;    //How much time has elapsed in the song
    private bool isPaused;   //

    //Chart specific parameters
    private float beatsPerSecond;
    private float unitsPerBeat;
    private float startTime;

    //FMOD Dsp Clock Things
    private FMOD.System fmodCore;
    private FMOD.ChannelGroup masterChannel;
    private int sampleRateHertz;



    private float CurrMomentBeats => currMomentSeconds * beatsPerSecond;
    private float CurrMomentCU => currMomentSeconds * beatsPerSecond * unitsPerBeat;

    private float TimeSinceStart => GetCurrentTimeDSP() - startTime;

    private void Awake()
    {
        isPaused = true;
    }

    private void Start()
    {
        beatsPerSecond = chart.bpm / 60f;
        unitsPerBeat = chart.unitsPerBeat;
        startTime = GetCurrentTimeDSP();

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
        }
    }

    public void Play()
    {
        currMomentSeconds = -firstBeatOffsetSeconds;
        isPaused = false;
    }

    public void Pause()
    {
        isPaused = true;
    }

    public void Resume()
    {
        isPaused = false;
    }

    private float GetCurrentTimeDSP()
    {
        ulong numSamples;
        masterChannel.getDSPClock(out numSamples, out _);

        double dspTime = (double) numSamples / sampleRateHertz;
        return (float) dspTime;
    }
}
