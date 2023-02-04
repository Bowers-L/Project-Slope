using System.Collections.Generic;

using UnityEngine;

public class ChartData : ScriptableObject
{
    public int bpm;
    public int timeSignatureNum;   //Implied 4 in denominator
    public List<Note> notes;

    public ChartData(int bpm, int timeSignatureNum, List<Note> notes)
    {
        this.bpm = bpm;
        this.timeSignatureNum = timeSignatureNum;
        this.notes = notes;
    }
}
