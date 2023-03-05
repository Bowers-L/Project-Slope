using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TrackMover : MonoBehaviour
{
    enum MoveType {snap, scroll}
    [SerializeField] MoveType moveType;

    private Vector3 onScreenPos;
    private Vector3 offScreenPos;

    [SerializeField] private float adjustment = 10;
    [SerializeField] private float moveTime = 1;
    [SerializeField] [Range(0, 1)] private float speed = 0.05f;
    [SerializeField] private float endVerticalPos = 34.7f;
    [SerializeField] private float delayBeforeEndScroll = 0;

    // Start is called before the first frame update
    void Start()
    {   
        onScreenPos = transform.position;
        offScreenPos = onScreenPos + adjustment * Vector3.up;
        transform.position = offScreenPos;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PutOnScreen() {
        if (moveType == MoveType.snap)
        {
            StartCoroutine(SnapDown());
        } else if (moveType == MoveType.scroll)
        {
            StartCoroutine(MoveDown());
        }
    }

    IEnumerator MoveDown() {
        // yield return new WaitForSeconds(5);
        
        //float timer = 0;

        // while (timer < moveTime) {
            
        //     float timeProgress = timer / moveTime;

        //     transform.position = Vector3.Lerp(offScreenPos, onScreenPos, Mathf.Clamp01(timeProgress));
            
        //     timer += Time.deltaTime;
        //     yield return null;
        // }

        while (transform.position.y < endVerticalPos)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y+speed, transform.position.z);
            yield return new WaitForSecondsRealtime(0.016f);
        }

        transform.position = onScreenPos;
        yield return new WaitForSeconds(delayBeforeEndScroll);
        GameStateManager.Instance.DimOverTimePublic(1, true, 1);
        if (OnEndScroll.GetPersistentEventCount() > 0)
        {
            UnityTimer.Timer.Register(2f, () => OnEndScroll.Invoke());
        }
        yield return null;
    }

    IEnumerator SnapDown()
    {
        float step;
        //Debug.Log("Initial distance between character and location: " + Vector3.Distance(charTransform.position, loc));
        while (Vector3.Distance(transform.position, onScreenPos) > 0.001f) {
            step = Vector3.Distance(transform.position, onScreenPos) * 0.17f;
            transform.position = Vector3.MoveTowards(transform.position, onScreenPos, step);
            yield return new WaitForSeconds(0.016f);
        }
        transform.position = onScreenPos;
        yield return new WaitForSeconds(delayBeforeEndScroll);
        if (OnEndScroll.GetPersistentEventCount() > 0) OnEndScroll.Invoke();
        yield return null;
    }

    [SerializeField] private UnityEvent OnEndScroll;
}
