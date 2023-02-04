using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Track : MonoBehaviour
{
    [SerializeField] private float noteXOffset;
    [SerializeField] private float noteXSpacing;

    [SerializeField] private float beatsPerTL;
    [SerializeField] private GameObject spawnBar;
    [SerializeField] private GameObject beatBar;
    [SerializeField] private Conductor conductor;
    [SerializeField] private GameObject notePrefab;

    private int _nextNote;

    public float TrackLengthYDelta => spawnBar.transform.position.y - beatBar.transform.position.y;
    public float BeatsPerTL => beatsPerTL;
    public Conductor Conduct => conductor;
    public GameObject SpawnBar => spawnBar;
    public GameObject BeatBar => beatBar;

    private void Awake()
    {
        conductor = GameObject.FindObjectOfType<Conductor>();
    }

    private void Start()
    {
    }

    private void Update()
    {   
    }

    public Vector3 GetNotePos(Note note)
    {
        //Assumes center of note is 0, 0
        float x = GetNoteXPos(note.pitch);
        float y = GetNoteYPos(note.moment - conductor.CurrMomentCU);


        return new Vector3(x, y);
    }

    private float GetNoteXPos(int pitch)
    {
        return noteXOffset + pitch * noteXSpacing;
    }

    private float GetNoteYPos(int momentDiff)
    {
        float y = BeatBar.transform.position.y;
        float trackLengthsFromBeatBar = momentDiff / conductor.Chart.unitsPerBeat / BeatsPerTL;
        y += trackLengthsFromBeatBar * TrackLengthYDelta;

        return y;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;

        for (int i = 0; i < 4; i++)
        {
            Gizmos.DrawSphere(new Vector3(
                GetNoteXPos(i), 
                spawnBar.transform.position.y, 
                spawnBar.transform.position.z), 0.2f);
            
            for (float beat = 0; beat < beatsPerTL + 1f; beat += 1f)
            {
                Gizmos.DrawSphere(new Vector3(
                    GetNoteXPos(i),
                    GetNoteYPos((int) (beat * conductor.Chart.unitsPerBeat)),
                    spawnBar.transform.position.z), 0.2f);
            }
        }
    }
}
