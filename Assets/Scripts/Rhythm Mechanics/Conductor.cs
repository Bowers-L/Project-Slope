using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private float currMomentSeconds;

    private float beatsPerSecond;
    private float unitsPerBeat;
    private float dspStartTime;

    private bool running = false;

    private float CurrMomentBeats => currMomentSeconds * beatsPerSecond;
    private float CurrMomentCU => currMomentSeconds * beatsPerSecond * unitsPerBeat;

    private float TimeSinceStart => GetDSP() - dspStartTime;

    private void Awake()
    {
        running = false;
    }

    private void Start()
    {
        beatsPerSecond = chart.bpm / 60f;
        unitsPerBeat = chart.unitsPerBeat;

        dspStartTime = GetDSP();

        Play(); //FOR TESTING (call from FMOD event instead)
    }

    private void Update()
    {
        if (running)
        {
            currMomentSeconds = TimeSinceStart - firstBeatOffsetSeconds;
        }
    }

    public void Play()
    {
        currMomentSeconds = -firstBeatOffsetSeconds;
        running = true;
    }

    public void Pause()
    {
        running = false;
    }

    public void Resume()
    {
        running = true;
    }

    private float GetDSP()
    {
        //Insert FMOD jank to get dsp value.
        return 0f;
    }
}
