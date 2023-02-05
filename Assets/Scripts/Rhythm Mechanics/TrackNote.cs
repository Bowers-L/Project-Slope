using UnityEngine;

public class TrackNote : MonoBehaviour
{
    public Note NoteData { get; set; }

    private void Update()
    {
        if (NoteData != null)
        {
            //Debug.Log($"Setting note pos to {transform.position}");
            transform.position = Track.Instance.GetNotePos(NoteData);
        }
    }
}