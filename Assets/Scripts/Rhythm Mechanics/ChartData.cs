using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "Chart", menuName = "ScriptableObjects/Chart", order = 1)]
public class ChartData : ScriptableObject
{
    public const int beatLength = Note.QUARTER; //Always 4 in time signature
    public float bpm;
    public int timeSignatureNum;   //Implied 4 in denominator
    public List<Note> notes;

    public ChartData(float bpm, int timeSignatureNum, List<Note> notes)
    {
        this.bpm = bpm;
        this.timeSignatureNum = timeSignatureNum;
        this.notes = notes;
    }
}
