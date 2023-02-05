using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteAlways]
public class CameraObliquifier : MonoBehaviour
{
    
    private Camera cam;

    public Vector2 obliqueness;
    
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void SetObliqueness(float horizObl, float vertObl) {
        cam.ResetProjectionMatrix();
        Matrix4x4 mat  = cam.projectionMatrix;
        mat[0, 2] = horizObl;
        mat[1, 2] = vertObl;
        cam.projectionMatrix = mat;

        // Debug.Log(mat);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        SetObliqueness(obliqueness.x, obliqueness.y);
    }
}
