using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "Chart", menuName = "ScriptableObjects/Chart", order = 1)]
public class ChartData : ScriptableObject
{

    public List<Note> notes;
    public float bpm;
    public int timeSignatureNum;   //Implied 4 in denominator
    public int unitsPerBeat = Note.QUARTER; //Always 4 in time signature

    public ChartData(List<Note> notes, float bpm, int timeSignatureNum, int unitsPerBeat = Note.QUARTER)
    {
        this.bpm = bpm;
        this.timeSignatureNum = timeSignatureNum;
        this.notes = notes;
        this.unitsPerBeat = unitsPerBeat;
    }
}
