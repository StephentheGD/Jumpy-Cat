using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.Serialization.Formatters;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Sprite[] idleSprites;
    [SerializeField] private Sprite[] risingSprites;
    [SerializeField] private Sprite[] fallingSprites;
    private int currentSkin;

    public AudioSource jumpAudioSource;
    public ParticleSystem particleSystem;

    [SerializeField] private bool hasStarted;
    [SerializeField] private bool isOver;
    [SerializeField] private bool hasConfettiBeenReleased;

    [SerializeField] private float playerPosX;
    [SerializeField] private float playerPosXScale;

    private float rotationOffset;
    private float rotationValue;
    private float rotationSpeed = 6.0f;
    private float rotationFilter = 7.0f;

    [SerializeField] private float playerSpeedY;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float gravity;

    private int lastRecordedScore;

    private Gyroscope gyro;
    private GameManager gm;
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private BoxCollider2D collider;
    private GameSession gs;

    void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        gm = FindObjectOfType<GameManager>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        collider.enabled = true;
        gyro = Input.gyro;
        gyro.enabled = true;

        playerSpeedY = 0;
        rotationValue = Input.acceleration.x;

        gs = FindObjectOfType<GameSession>();

        if (gs.isSoundMuted)
            jumpAudioSource.mute = true;

        UpdateCurrentSkin(gs.currentSkin);
    }

    public void UpdateCurrentSkin(int skin)
    {
        currentSkin = skin;
        sr.sprite = idleSprites[skin];

        if (!gs)
            gs = FindObjectOfType<GameSession>();
        gs.UpdateCurrentSkin(skin);

        particleSystem.gameObject.SetActive(currentSkin == 3);
    }

    void Update()
    {
        if (!gm.isPaused)
        {
            // Handle input
            /*
            if (Input.touchCount > 0 && !isOver && !hasStarted)
            {
                

                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    Jump();
                }
            }
            */

            // Handle gyro
            if (hasStarted)
            {
#if UNITY_ANDROID
                //rotationValue = -gyro.attitude.x;
                //rotationValue = Input.acceleration.x;
                rotationValue = Mathf.Lerp(rotationValue, Input.acceleration.x, rotationFilter * Time.deltaTime);

#elif UNITY_IOS
                rotationValue = gyro.attitude.y;
#endif

                playerPosX = Mathf.Clamp((rotationValue - rotationOffset) * playerPosXScale, -2.5f, 2.5f);
                if (playerPosX > transform.position.x + 0.05f)
                {
                    sr.flipX = true;
                }
                else if (playerPosX < transform.position.x - 0.05f)
                {
                    sr.flipX = false;
                }
            }



            // Handle Score
            int newScore = Mathf.FloorToInt(transform.position.y);
            if (newScore > lastRecordedScore)
            {
                int scoreDifference = newScore - lastRecordedScore;
                gm.UpdateScoreDisplay(scoreDifference);
                lastRecordedScore = newScore;
            }
            if (transform.position.y > gm.previousBest.transform.position.y && !hasConfettiBeenReleased)
            {
                gm.ReleaseConfetti();
                hasConfettiBeenReleased = true;
            }

            // Keep falling after losing
            if (isOver)
            {
                transform.position = new Vector2(transform.position.x, transform.position.y + playerSpeedY);
            }
        }
    }

    public void Respawn()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        collider.enabled = true;

        hasStarted = true;
        isOver = false;

        transform.position = new Vector2(transform.position.x, lastRecordedScore);
        playerSpeedY = 0.2f;
    }

    public void StartGame()
    {
        hasStarted = true;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        gm.HideTitleScreen();
        gm.DisplayPauseButton();

        Jump();
    }

    private void FixedUpdate()
    {
        if (!gm.isPaused)
        {
            // Handle gravity
            if (hasStarted)
            {
                playerSpeedY -= gravity * Time.deltaTime;
                transform.position = new Vector2(playerPosX, transform.position.y + playerSpeedY);

                if (playerSpeedY > 0)
                {
                    sr.sprite = risingSprites[currentSkin];
                }
                else
                {
                    sr.sprite = fallingSprites[currentSkin];
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isOver)
        {
            switch (collision.tag)
            {
                case "Platform":
                    if (playerSpeedY <= 0)
                        Jump();
                    break;

                case "Kill Zone":
                    EndGame();
                    break;

                case "Obstacle":
                    HitObstacle(collision.gameObject);
                    break;
            }
        }
    }

    private void HitObstacle(GameObject obstacleObject)
    {
        //playerSpeedY = 0;
        Obstacle obstacle = obstacleObject.GetComponent<Obstacle>();
        obstacle.Activate();
        if (obstacle.obstacleType == Obstacle.ObstacleType.Mine)
        {
            Debug.Log("Force");
            Vector3 difference = transform.position - obstacle.transform.position;
            //rb.AddForce(difference * obstacle.explosionForce, ForceMode2D.Impulse);
            //rb.AddForce(transform.up, ForceMode2D.Impulse);
            playerSpeedY = difference.y * obstacle.explosionForce;
        }
    }

    private void Jump()
    {
        playerSpeedY = jumpSpeed;
        jumpAudioSource.Play();
    }

    private void EndGame()
    {
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
        collider.enabled = false;

        hasStarted = false;
        isOver = true;

        gm.DisplayGameOverScreen();

        if (!gs)
            gs = FindObjectOfType<GameSession>();

        if (gs.highScore < gm.score)
        {
            gs.UpdateHighscore(gm.score);
        }
        gm.UpdateHighScoreDisplay();
    }

    public void UpdateGyroOffset(float offset)
    {
        rotationOffset = offset;
    }
}
