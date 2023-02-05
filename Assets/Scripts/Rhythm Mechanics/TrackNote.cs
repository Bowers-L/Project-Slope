using UnityEngine;

public class TrackNote : MonoBehaviour
{
    private Track track;
    private Conductor conductor;

    public Note NoteData { get; set; }

    private void Update()
    {
        if (NoteData != null)
        {
            Debug.Log($"Setting note pos to {transform.position}");
            transform.position = track.GetNotePos(NoteData);
        }
    }

    public void SetTrack(Track track)
    {
        this.track = track;
        this.conductor = track.Conduct;
    }
}