using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class CameraController : MonoBehaviour
{
    private PlayerController player;
    private float maxPlayerHeight;
    private bool isMoving = true;

    [SerializeField] private float cameraTrackingSpeed;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>();

        OnPaused += ToggleMoving;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isMoving)
        {
            float currPlayerPos = player.transform.position.y;
            if (currPlayerPos > maxPlayerHeight)
            {
                maxPlayerHeight = currPlayerPos;
            }

            float newCameraHeight = Mathf.Lerp(transform.position.y, maxPlayerHeight, cameraTrackingSpeed);

            transform.position = new Vector3(transform.position.x, newCameraHeight, transform.position.z);
        }
    }

    private void ToggleMoving(bool paused)
    {
        isMoving = !paused;
    }
}
