using MyBox;
using System.Collections.Generic;
using UnityEngine;

public class Track : Singleton<Track>
{
    [SerializeField] private float noteXOffset;
    [SerializeField] private float noteXSpacing;

    [SerializeField] private float beatsPerTL;
    [SerializeField] private float despawnBeats;
    [SerializeField] private GameObject spawnBar;
    [SerializeField] private GameObject beatBar;
    [SerializeField] private GameObject notePrefab;


    private Queue<Note> noteInstantiateQueue;
    private List<TrackNote> activeNoteList;

    public float TrackLengthYDelta => spawnBar.transform.position.y - beatBar.transform.position.y;
    public float BeatsPerTL => beatsPerTL;
    public GameObject SpawnBar => spawnBar;
    public GameObject BeatBar => beatBar;

    public List<TrackNote> ActiveNotes => activeNoteList;

    private bool hasInit = false;

    private void Awake()
    {
        InitializeSingleton();
        hasInit = false;
    }

    private void Start()
    {
    }

    private void Update()
    {   
        if (hasInit)
        {
            Note noteData;
            noteInstantiateQueue.TryPeek(out noteData);

            //Once note's moment passes the spawn moment, spawn the note.
            if (noteData != null && NoteDiff(noteData) <= SpawnMomentCU())
            {
                //Spawn the Note!
                GameObject noteObj = Instantiate(notePrefab, GetNotePos(noteData), Quaternion.identity);
                TrackNote tNote = noteObj.GetComponent<TrackNote>();
                tNote.NoteData = noteData;

                activeNoteList.Add(tNote);
                noteInstantiateQueue.Dequeue();
            }

            if (activeNoteList.Count > 0)
            {
                Note closestNote = activeNoteList[0].NoteData;
                if (NoteDiff(closestNote) <= DespawnMomentCU())
                {
                    Destroy(activeNoteList[0].gameObject);
                    activeNoteList.RemoveAt(0);
                }
            }
        }
    }

    public void Init(Conductor conductor)
    {
        hasInit = true;
        conductor = GameObject.FindObjectOfType<Conductor>();
        noteInstantiateQueue = new Queue<Note>();
        activeNoteList = new List<TrackNote>();

        List<Note> notes = conductor.Chart.notes;
        for (int i = 0; i < notes.Count; i++)
        {
            noteInstantiateQueue.Enqueue(notes[i]);
        }
    }

    public int NoteDiff(Note note)
    {
        return (int) (note.moment - Conductor.Instance.CurrMomentCU);
    }

    public Vector3 GetNotePos(Note note)
    {
        //Assumes center of note is 0, 0
        float x = GetNoteXPos(note.pitch);
        float y = GetNoteYPos(NoteDiff(note));

        return new Vector3(x, y);
    }

    //The moment a note should spawn on the track
    public int SpawnMomentCU()
    {
        return (int) (beatsPerTL * Conductor.Instance.Chart.unitsPerBeat);
    }

    public int DespawnMomentCU()
    {
        return (int) (-despawnBeats * Conductor.Instance.Chart.unitsPerBeat);
    }

    private float GetNoteXPos(int pitch)
    {
        return noteXOffset + pitch * noteXSpacing;
    }

    private float GetNoteYPos(int momentDiff)
    {
        float y = BeatBar.transform.position.y;
        float trackLengthsFromBeatBar = (float) momentDiff / Conductor.Instance.Chart.unitsPerBeat / BeatsPerTL;
        y += trackLengthsFromBeatBar * TrackLengthYDelta;
        return y;
    }

    private void OnDrawGizmosSelected()
    {


        for (int i = 0; i < 4; i++)
        {
            float x = GetNoteXPos(i);

            Gizmos.color = Color.blue;
            for (float beat = 0; beat < beatsPerTL + 1f; beat += 1f)
            {
                Gizmos.DrawSphere(new Vector3(
                    x,
                    GetNoteYPos((int) (beat * Conductor.Instance.Chart.unitsPerBeat)),
                    spawnBar.transform.position.z), 0.2f);
            }

            //Spawn Point
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(new Vector3(
                x,
                GetNoteYPos(SpawnMomentCU()),
                spawnBar.transform.position.z), 0.2f);

            //Despawn Point
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(new Vector3(
                x,
                GetNoteYPos(DespawnMomentCU()),
                spawnBar.transform.position.z), 0.2f);
        }
    }
}
