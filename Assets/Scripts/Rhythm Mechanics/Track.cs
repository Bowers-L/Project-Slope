using MyBox;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

    [SerializeField, Tooltip("It's Rewind Time!")] private float rewindTime;


    //private Queue<NoteData> noteInstantiateQueue = new Queue<NoteData>();

    public float TrackLengthYDelta => spawnBar.transform.position.y - beatBar.transform.position.y;
    public float BeatsPerTL => beatsPerTL;
    public GameObject SpawnBar => spawnBar;
    public GameObject BeatBar => beatBar;

    public List<TrackNote> ActiveNotes => _activeNoteList;
    private List<TrackNote> _activeNoteList = new List<TrackNote>();
    private HashSet<NoteData> _activeNoteSet = new HashSet<NoteData>();
    private ChartData _chart;
    private bool _trackActive = false;
    private bool _isRewinding = false;
    private int _trackMoment;
    private int _trackRewindLengthCU;

    public int Direction => _isRewinding ? -1 : 1;

    private void Awake()
    {
        InitializeSingleton(false);
        _chart = null;
        _trackActive = false;
        _isRewinding = false;
        _trackMoment = 0;
    }

    private void OnEnable()
    {
        Conductor.OnPlay += OnConductorPlay;
    }

    private void OnDisable()
    {
        Conductor.OnPlay -= OnConductorPlay;
    }

    private void Update()
    {   

        if (_trackActive)
        {
            //Update Moment
            if (_isRewinding)
            {
                _trackMoment -= (int)(_trackRewindLengthCU/rewindTime * Time.deltaTime);
                _trackMoment = Mathf.Max(_trackMoment, 0);
            }
            else
            {
                _trackMoment = Conductor.Instance.CurrMomentCU;
            }

            //Check Spawning Notes
            for (int i = 0; i < _chart.notes.Count; i++)
            {
                if (!_activeNoteSet.Contains(_chart.notes[i]) && ShouldSpawnNote(_chart.notes[i]))
                {
                    //Debug.Log($"SPAWNING NOTE: {_chart.notes[i].moment}");
                    SpawnNote(_chart.notes[i]);
                }
            }

            //Check Despawning Notes
            for (int i = 0; i < _activeNoteList.Count; i++)
            {
                if (ShouldDespawnNote(_activeNoteList[i].NoteData))
                {
                    DespawnNote(_activeNoteList[i]);
                    _activeNoteSet.Remove(_activeNoteList[i].NoteData);
                    _activeNoteList.RemoveAt(i);
                    i--;
                }
            }
        }
    }

    private bool ShouldDespawnNote(NoteData note)
    {
        int deltaCU = NoteMomentDeltaCU(note);
        return deltaCU <= DespawnMomentCU() || deltaCU > SpawnMomentCU();
    }

    private bool ShouldSpawnNote(NoteData note)
    {
        if (note == null)
        {
            return false;
        }

        int deltaCU = NoteMomentDeltaCU(note);
        bool upperBoundCheck = deltaCU <= SpawnMomentCU();
        bool lowerBoundCheck = deltaCU > DespawnMomentCU();
        return note != null && upperBoundCheck && lowerBoundCheck;
    }

    private void SpawnNote(NoteData noteData)
    {
        GameObject noteObj = Instantiate(notePrefab, GetNotePos(noteData), transform.rotation);
        TrackNote tNote = noteObj.GetComponent<TrackNote>();
        tNote.NoteData = noteData;

        _activeNoteList.Add(tNote);
        _activeNoteSet.Add(noteData);
    }

    private void DespawnNote(TrackNote note)
    {
        if (!_isRewinding)
        {
            PlayerPerformanceManager.Instance.HandleNoteMissed(note);
        }

        Destroy(note.gameObject);
    }

    private void ClearTrackData()
    {
        foreach (TrackNote n in _activeNoteList) {
            Destroy(n.gameObject);
        }
        _activeNoteList.Clear();
        _activeNoteSet.Clear();
    }

    private void OnConductorPlay(ChartData chart)
    {
        ClearTrackData();

        _chart = chart;
        _trackActive = true;
        _isRewinding = false;
    }

    public int NoteMomentDeltaCU(NoteData note)
    {
        return (int) (note.moment - _trackMoment);
    }

    //The moment a note should spawn on the track
    public int SpawnMomentCU()
    {
        return (int) (beatsPerTL * _chart.unitsPerBeat);
    }

    public int DespawnMomentCU()
    {
        return (int) (-despawnBeats * _chart.unitsPerBeat);
    }

    public void Rewind(System.Action callback = null)
    {
        StartCoroutine(RewindCorout(callback));
    }

    private IEnumerator RewindCorout(System.Action callback = null)
    {
        _trackActive = true;
        _isRewinding = true;
        _trackRewindLengthCU = _trackMoment;

        yield return new WaitUntil(() =>
        {
            return _trackMoment <= 0;
        });

        _isRewinding = false;
        callback();
    }

    public Vector3 GetNotePos(NoteData note)
    {
        //Assumes center of note is 0, 0
        float x = GetNoteXPos(note.pitch);
        float y = GetNoteYPos(NoteMomentDeltaCU(note));

        Vector3 localNotePos = new Vector3(x, y);
        Vector3 globalNotePos = transform.TransformPoint(localNotePos);
        return globalNotePos;
    }

    private float GetNoteXPos(int pitch)
    {
        return noteXOffset + pitch * noteXSpacing;
    }

    private float GetNoteYPos(int momentDiff)
    {
        float y = BeatBar.transform.position.y;
        float trackLengthsFromBeatBar = (float) momentDiff / _chart.unitsPerBeat / BeatsPerTL;
        y += trackLengthsFromBeatBar * TrackLengthYDelta;
        return y;
    }

    private float GetNoteYPosBeat(float momentDiffBeats)
    {
        float y = BeatBar.transform.position.y;
        float trackLengthsFromBeatBar = momentDiffBeats / BeatsPerTL;
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
                    GetNoteYPosBeat(beat),
                    spawnBar.transform.position.z), 0.2f);
            }

            //Spawn Point
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(new Vector3(
                x,
                GetNoteYPosBeat(BeatsPerTL),
                spawnBar.transform.position.z), 0.2f);

            //Despawn Point
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(new Vector3(
                x,
                GetNoteYPosBeat(-despawnBeats),
                spawnBar.transform.position.z), 0.2f);
        }

        Gizmos.color = Color.cyan;
        //Gizmos.DrawSphere(new Vector3(BeatBar.transform.position.x, BeatBar.transform.position.y, BeatBar.transform.position.z), 1f);
    }
}