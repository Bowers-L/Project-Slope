using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityTimer;

public class AlternateAnimBeat : MonoBehaviour
{
    [SerializeField] float offset = 0;
    [SerializeField] GameObject[] trackBeats;

    Animator[] _animator = new Animator[4];
    Timer beatTimer;
    Timer offsetTimer;

    void Start()
    {
        GameStateManager.alternateBeatEvent.AddListener(BeatIt);

        for (int i = 0; i < 4; i++)
        {
            _animator[i] = trackBeats[i].GetComponent<Animator>();
            Debug.Log(_animator[i]);
        }
    }

    void BeatIt()
    {
        beatTimer = Timer.Register(0.54166666666f + offset, () => 
        {
            foreach (Animator anim in _animator)
            {
                anim.SetTrigger("bounce");
            }
        });
    }

    void OnDestroy()
    {
        Timer.CancelAllRegisteredTimers();
    }
}
