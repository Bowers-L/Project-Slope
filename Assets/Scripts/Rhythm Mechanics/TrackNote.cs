using FMOD;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class TrackNote : MonoBehaviour
{
    [SerializeField] private float xOffset;
    [SerializeField] private float xSize;
    [SerializeField] private float xPadding;

    private Note note;
    private Track track;
    private Conductor conductor;

    private void Update()
    {
        if (note != null)
        {
            float x = xOffset + note.pitch * (xSize + xPadding);
            float y = track.BeatBar.transform.position.y;
            float noteMomentDiff = note.moment - conductor.CurrMomentCU;

            float trackLengthsFromBeatBar = noteMomentDiff / conductor.Chart.unitsPerBeat / track.BeatsPerTL;
            y += trackLengthsFromBeatBar * track.TrackLengthYDelta;

            transform.position = new Vector3(x, y, 0);
        }
    }

    public void SetTrack(Track track)
    {
        this.track = track;
        this.conductor = track.Conduct;
    }
    public void SetData(Note noteData)
    {
        this.note = noteData;
    }
}