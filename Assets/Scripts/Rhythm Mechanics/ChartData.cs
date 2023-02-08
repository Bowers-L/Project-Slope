using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "Chart", menuName = "ScriptableObjects/Chart", order = 1)]
public class ChartData : ScriptableObject
{

    public List<NoteData> notes;
    public float bpm;
    public int timeSignatureNum;   //Implied 4 in denominator
    public int unitsPerBeat = NoteData.QUARTER; //Always 4 in time signature

    public float firstBeatOffsetSeconds = 0f;

    public ChartData(List<NoteData> notes, float bpm, int timeSignatureNum, int unitsPerBeat = NoteData.QUARTER)
    {
        this.bpm = bpm;
        this.timeSignatureNum = timeSignatureNum;
        this.notes = notes;
        this.unitsPerBeat = unitsPerBeat;
    }
}
