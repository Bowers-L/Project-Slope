using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPerformanceManager : Singleton<PlayerPerformanceManager>
{
    [SerializeField] private float flashSpeed;
    [SerializeField] private float earlyTimingWindowBeats;
    [SerializeField] private float lateTimingWindowBeats;
    //[SerializeField, Range(0f, 1f)] private float failurePercentage;
    [SerializeField] private int playerMaxHealthPerSection;

    //Hit and Miss FX
    [SerializeField] private GameObject hitFXPrefab;
    [SerializeField] private GameObject missFXPrefab;
    [SerializeField] private GameObject[] tracks;

    [SerializeField] private GameObject brainMeterObject;
    Animator brainMeterAnimator;

    private int hitNotesInSection;
    private int missedNotesInSection;    //Letting the note pass without hitting it.
    private int missHits;    //Pressing button at wrong time

    private int playerHealth;

    private bool failed = false;


    private HashSet<SpriteRenderer> flashing = new HashSet<SpriteRenderer>();

    public delegate void OnLevelFailedDel(); 
    public static event OnLevelFailedDel OnLevelFailed;

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        brainMeterObject = GameObject.Find("Track and buffer").transform.GetChild(0).gameObject;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        brainMeterObject = GameObject.Find("Track and buffer").transform.GetChild(0).gameObject;
    }

    private void OnEnable()
    {
        Conductor.OnPlay += OnConductorPlay;
        brainMeterAnimator = brainMeterObject.GetComponent<Animator>();
    }

    private void OnDisable()
    {
        Conductor.OnPlay -= OnConductorPlay;
    }

    private void OnParticleSystemStopped()
    {
        failed = false;
        playerHealth = playerMaxHealthPerSection;
    }

    private void Update()
    {
        if (!Conductor.Instance.Paused)
        {
            if (CheckFailure() && !failed)
            {
                failed = true;
                Debug.Log("You Failed n00b");
                GameStateManager.Instance.OnLevelFailed();
            }

            if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Alpha1))
            {
                HandlePlayedNote(0);
            }

            if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Alpha2))
            {
                HandlePlayedNote(1);
            }

            if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.Alpha3))
            {
                HandlePlayedNote(2);
            }

            if (Input.GetKeyDown(KeyCode.V) || Input.GetKeyDown(KeyCode.Alpha4))
            {
                HandlePlayedNote(3);
            }
        }
    }

    public bool CheckFailure()
    {
        return playerHealth <= 0;
    }

    private void OnConductorPlay(ChartData chart)
    {
        failed = false;
        hitNotesInSection = 0;
        missedNotesInSection = 0;
        playerHealth = playerMaxHealthPerSection;
        if (brainMeterObject.activeSelf) brainMeterAnimator.SetInteger("brainJuice", playerMaxHealthPerSection);
    }

    private void HandlePlayedNote(int pitch)
    {
        TrackNote note = CheckHitNote(pitch);
        //Debug.Log($"Player Pressed Note: {pitch}");

        if (note == null)
        {
            //MISSED
            HandleMissHit();

        } else
        {
            //HIT
            HandleNoteHit(note);
        }
    }

    public void HandleNoteHit(TrackNote note)
    {
        //Do VFX Things. Keep Track of Pass/Fail, etc.
        hitNotesInSection++;
        playerHealth++;
        if (brainMeterObject.activeSelf) brainMeterAnimator.SetInteger("brainJuice", playerHealth);

        Track.Instance.ActiveNotes.Remove(note);

        GameObject track = tracks[note.NoteData.pitch];
        GameObject fxInstance = Instantiate(hitFXPrefab, track.transform.position, Track.Instance.transform.rotation);
        Destroy(note.gameObject);

        Animator anim = fxInstance.GetComponent<Animator>();
        Destroy(fxInstance, anim.GetCurrentAnimatorStateInfo(0).length);

        //StartCoroutine(FlashColor(note.GetComponent<SpriteRenderer>(), Color.green, flashSpeed, () =>
        //{
        //    Destroy(note.gameObject);
        //}));
    }

    public void HandleMissHit()
    {
        missHits++;
        playerHealth -= 5;
        if (brainMeterObject.activeSelf) brainMeterAnimator.SetInteger("brainJuice", playerHealth);
        StartCoroutine(FlashColor(Track.Instance.BeatBar.GetComponent<SpriteRenderer>(), Color.red, flashSpeed));
    }

    public void HandleNoteMissed(TrackNote note)
    {
        Debug.Log($"MISSED NOTE: {note}");
        //Do VFX Things. Keep Track of Pass/Fail, etc.
        missedNotesInSection++;
        playerHealth -= 5;
        if (brainMeterObject.activeSelf) brainMeterAnimator.SetInteger("brainJuice", playerHealth);

        //GameObject track = tracks[note.NoteData.pitch];
        GameObject fxInstance = Instantiate(missFXPrefab, note.transform.position, Track.Instance.transform.rotation);
        Animator anim = fxInstance.GetComponent<Animator>();
        Destroy(fxInstance, anim.GetCurrentAnimatorStateInfo(0).length);
    }

    private IEnumerator FlashColor(SpriteRenderer sr, Color c, float duration, System.Action callback = null)
    {
        if (!flashing.Contains(sr))
        {
            flashing.Add(sr);
            Color ogColor = sr.color;
            sr.color = c;
            yield return new WaitForSeconds(duration);
            sr.color = ogColor;
            flashing.Remove(sr);
        }

        if (callback != null)
        {
            callback();
        }
    }

    private TrackNote CheckHitNote(int pitch)
    {
        List<TrackNote> activeNotes = Track.Instance.ActiveNotes;

        if (activeNotes == null) {
            return null;
        }

        TrackNote hitNote = null;
        foreach (TrackNote note in activeNotes)
        {
            if (pitch == note.NoteData.pitch)
            {
                int noteMoment = Track.Instance.NoteMomentDeltaCU(note.NoteData);

                int earlyTimingWindowCU = Conductor.Instance.BeatsToCU(earlyTimingWindowBeats);
                int lateTimingWindowCU = Conductor.Instance.BeatsToCU(lateTimingWindowBeats);

                if (noteMoment > earlyTimingWindowCU) {
                    //We've reached the point where notes haven't reached the early window yet and can't be hit.
                    break;
                }

                if (noteMoment <= earlyTimingWindowCU && noteMoment >= -lateTimingWindowCU)
                {
                    hitNote = note;
                    break;
                }
            }

        }

        return hitNote;
    }
}