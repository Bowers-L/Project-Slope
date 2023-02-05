using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitManager : Singleton<HitManager>
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            HandlePlayedNote(0);
        } 
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            HandlePlayedNote(1);
        } 
        
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            HandlePlayedNote(2);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            HandlePlayedNote(3);
        }
    }

    private void HandlePlayedNote(int pitch)
    {
        TrackNote note = CheckHitNote(pitch);
        Debug.Log($"Player Pressed Note: {pitch}");

        if (note == null)
        {
            //MISSED
            StartCoroutine(FlashColor(Track.Instance.BeatBar.GetComponent<SpriteRenderer>(), Color.red, 0.3f));
        } else
        {
            //HIT
            StartCoroutine(FlashColor(note.GetComponent<SpriteRenderer>(), Color.green, 0.3f));
        }
    }

    private IEnumerator FlashColor(SpriteRenderer sr, Color c, float duration)
    {
        Color ogColor = sr.color;
        sr.color = c;
        yield return new WaitForSeconds(duration);
        sr.color = ogColor;
    }

    private TrackNote CheckHitNote(int pitch)
    {
        List<TrackNote> activeNotes = Track.Instance.ActiveNotes;


        return null;
    }
}
