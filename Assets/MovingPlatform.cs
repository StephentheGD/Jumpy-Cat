using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Vector2 movementBounds;
    private float originPositionX;
    private float randomOffset;
    private bool isMoving = true;

    private void Start()
    {
        originPositionX = transform.position.x;
        randomOffset = Random.Range(0.0f, 1.0f);

        OnPaused += ToggleMoving;
    }

    void Update()
    {
        if (isMoving)
        {
            float normalisedSin = (Mathf.Sin(Time.time + randomOffset) + 1) / 2;
            float newPositionX = Mathf.Lerp(movementBounds.x, movementBounds.y, normalisedSin);
            transform.position = new Vector2(originPositionX + newPositionX, transform.position.y);
        }
    }

    private void ToggleMoving(bool paused)
    {
        isMoving = !paused;
    }
}
