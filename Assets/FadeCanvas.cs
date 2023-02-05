using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeCanvas : MonoBehaviour
{

    public float fadeTime;

    CanvasGroup cg;

    // Start is called before the first frame update
    void Start()
    {
        cg = GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FadeDown() {
        StartCoroutine(FadeDownCo());
    }

    IEnumerator FadeDownCo() {
        float timer = 0;

        while (timer < fadeTime) {
            
            float timeProgress = timer / fadeTime;

            cg.alpha = 1-timeProgress;
            
            timer += Time.deltaTime;
            yield return null;
        }

        cg.alpha = 0;

    }

    public void FadeUp() {
        StartCoroutine(FadeUpCo());
    }

    IEnumerator FadeUpCo() {
        float timer = 0;

        while (timer < fadeTime) {
            
            float timeProgress = timer / fadeTime;

            cg.alpha = timeProgress;
            
            timer += Time.deltaTime;
            yield return null;
        }

        cg.alpha = 1;

    }
}
