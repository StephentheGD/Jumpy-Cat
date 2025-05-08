using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static GameManager;

public class Cloud : MonoBehaviour
{
    [SerializeField] private Vector2 moveSpeedRange;
    private float moveSpeed;
    private bool isMoving = true;

    [SerializeField] private Sprite flippedCloud;

    private void Start()
    {
        moveSpeed = Random.Range(moveSpeedRange.x, moveSpeedRange.y);
        transform.localScale = new Vector3(moveSpeed / 2, moveSpeed / 2, 1);
        if (Random.Range(0, 2) >= 1)
        {
            GetComponent<SpriteRenderer>().sprite = flippedCloud;
        }

        OnPaused += ToggleMoving;
    }

    void Update()
    {
        if (isMoving)
            transform.position = new Vector2(transform.position.x - (moveSpeed * Time.deltaTime), transform.position.y);
    }

    private void ToggleMoving(bool paused)
    {
        isMoving = !paused;
    }
}
