using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Track : MonoBehaviour
{
    [SerializeField] private float beatsPerTL;
    [SerializeField] private GameObject spawnBar;
    [SerializeField] private GameObject beatBar;
    [SerializeField] private GameObject notePrefab;


    private Conductor _conductor;

    private int _nextNote;
    private ChartData _chart;

    public float TrackLengthYDelta => spawnBar.transform.position.y - beatBar.transform.position.y;
    public float BeatsPerTL => beatsPerTL;
    public Conductor Conduct => _conductor;
    public GameObject SpawnBar => spawnBar;
    public GameObject BeatBar => beatBar;

    private void Awake()
    {
        _conductor = GameObject.FindObjectOfType<Conductor>();
        _chart = _conductor.Chart;
    }

    private void Start()
    {
    }

    private void Update()
    {   
    }
}
