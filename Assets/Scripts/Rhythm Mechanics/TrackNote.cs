using FMOD;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class TrackNote : MonoBehaviour
{
    private Track track;
    private Conductor conductor;

    public Note NoteData { get; set; }

    private void Update()
    {
        if (NoteData != null)
        {
            transform.position = track.GetNotePos(NoteData);
        }
    }

    public void SetTrack(Track track)
    {
        this.track = track;
        this.conductor = track.Conduct;
    }
}