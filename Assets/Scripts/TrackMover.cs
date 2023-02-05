using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackMover : MonoBehaviour
{

    private Vector3 onScreenPos;
    private Vector3 offScreenPos;

    [SerializeField] private float adjustment = 10;
    [SerializeField] private float moveTime = 1;

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

        StartCoroutine(MoveDown());
    }

    IEnumerator MoveDown() {
        // yield return new WaitForSeconds(5);
        
        float timer = 0;

        while (timer < moveTime) {
            
            float timeProgress = timer / moveTime;

            transform.position = Vector3.Lerp(offScreenPos, onScreenPos, Mathf.Clamp01(timeProgress));
            
            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = onScreenPos;

    }
}
