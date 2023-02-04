using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackPositioner : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)] private float horizontalFraction;
    [SerializeField, Range(0f, 1f)] private float verticalFraction;
    [SerializeField, Range(0f, 1f)] private float beatBarOffset;
    [SerializeField, Range(0f, 1f)] private float spawnBarOffset;
    [SerializeField] private GameObject beatBar;
    [SerializeField] private GameObject spawnBar;
    private void Awake()
    {
        RefreshPosition();
    }

    public void RefreshPosition()
    {
        //Position the track
        Vector3 center = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 10));
        Vector3 bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, 10));
        Vector3 topRight = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f, 10));

        Vector3 size = topRight - bottomLeft;

        transform.position = new Vector3(center.x, center.y);
        transform.localScale = new Vector3(size.x * horizontalFraction, size.y * verticalFraction, 1f);
    }
}
