using FMOD;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class TrackNote : MonoBehaviour
{
    private Note note;
    private Track track;
    private Conductor conductor;

    private void Update()
    {
        if (note != null)
        {
            transform.position = track.GetNotePos(note);
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