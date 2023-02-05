using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FmodFormTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            FMODUnity.StudioEventEmitter emitter = GetComponent<FMODUnity.StudioEventEmitter>();
            emitter.SetParameter("FormValue", 0);
            Debug.Log(emitter);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            FMODUnity.StudioEventEmitter emitter = GetComponent<FMODUnity.StudioEventEmitter>();
            emitter.SetParameter("FormValue", 1);
            Debug.Log("Pressed W");
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            FMODUnity.StudioEventEmitter emitter = GetComponent<FMODUnity.StudioEventEmitter>();
            emitter.SetParameter("FormValue", 2);
            Debug.Log("Pressed E");
        }

    }
}