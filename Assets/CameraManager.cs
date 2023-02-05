using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    [SerializeField] private GameObject leftNearCam;
    [SerializeField] private GameObject leftMidCam;
    [SerializeField] private GameObject leftFarCam;
    [SerializeField] private GameObject rightNearCam;
    [SerializeField] private GameObject rightMidCam;
    [SerializeField] private GameObject rightFarCam;

    [SerializeField] private TrackMover trackMover; 

    // Start is called before the first frame update
    void Start()
    {
        leftNearCam.SetActive(false);
        leftMidCam.SetActive(false);
        rightNearCam.SetActive(false);
        rightMidCam.SetActive(false);
        
        leftFarCam.SetActive(true);
        rightFarCam.SetActive(true);
    }

    public void TransitionNear() {
        leftNearCam.SetActive(true);
        rightNearCam.SetActive(true);

        leftFarCam.SetActive(false);
        leftMidCam.SetActive(false);
        rightFarCam.SetActive(false);
        rightMidCam.SetActive(false);
    }

    public void TransitionMid() {
        leftMidCam.SetActive(true);
        rightMidCam.SetActive(true);

        leftFarCam.SetActive(false);
        leftNearCam.SetActive(false);
        rightFarCam.SetActive(false);
        rightNearCam.SetActive(false);
    }

    public void TransitionFar() {
        leftFarCam.SetActive(true);
        rightFarCam.SetActive(true);

        leftMidCam.SetActive(false);
        leftNearCam.SetActive(false);
        rightMidCam.SetActive(false);
        rightNearCam.SetActive(false);
    }

    public void StartScene() {
        StartCoroutine(StartSceneCo());
    }

    IEnumerator StartSceneCo() {

            GameObject.FindObjectOfType<FadeCanvas>().FadeDown();

            yield return new WaitForSeconds(2);
            TransitionMid();

            yield return new WaitForSeconds(2);
            trackMover.PutOnScreen();


            yield return new WaitForSeconds(2);
            TransitionNear();


            yield return new WaitForSeconds(1);
            GameObject.FindObjectOfType<GameStateManager>().StartFMODEvent();


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
