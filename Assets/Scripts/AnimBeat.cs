using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityTimer;

public class AnimBeat : MonoBehaviour
{
    [SerializeField] int beatCap = 1;
    [SerializeField] float offset = 0;

    int beatCount = 0;
    Animator _animator;
    Timer offsetTimer;

    void Start()
    {
        GameStateManager.beatEvent.AddListener(EatIt);

        _animator = GetComponent<Animator>();
    }

    void EatIt()
    {
        beatCount++;
        if (beatCount >= beatCap)
        {
            offsetTimer = Timer.Register(offset, () => BeatIt());
        }
    }

    void BeatIt()
    {
        _animator.SetTrigger("bounce");
        beatCount = 0;
    }
}
