using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject titleScreen;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private TMP_Text scoreDisplay;
    [SerializeField] private TMP_Text highScoreDisplay;
    [SerializeField] private GameObject medal1;
    [SerializeField] private GameObject medal2;
    [SerializeField] private GameObject pauseButton;
    private PlayerController player;

    [SerializeField] public int score;
    [SerializeField] private int levelTargetScore;
    private GameSession gs;
    private MusicPlayerController musicPlayer;
    private UIManager uiManager;

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject pauseMenuBox;
    public float startPosition;
    public float endPosition;
    public bool isPaused;
    public delegate void Paused(bool paused);
    public static event Paused OnPaused;

    private bool readyToPlay;

    [Header("Settings")]
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject settingsMenuBox;
    [SerializeField] private Image musicIcon;
    [SerializeField] private Sprite musicOn;
    [SerializeField] private Sprite musicOff;
    [SerializeField] private Image soundIcon;
    [SerializeField] private Sprite soundOn;
    [SerializeField] private Sprite soundOff;

    [Header("Credits")]
    [SerializeField] private GameObject creditsScreen;
    [SerializeField] private GameObject creditsScreenBox;

    [Header("Skin Select")]
    [SerializeField] private GameObject skinSelectScreen;
    [SerializeField] private GameObject skinSelectScreenBox;

    [Header("Background")]
    [SerializeField] private SpriteRenderer background;
    [SerializeField] private Gradient backgroundGradient;

    [SerializeField] public GameObject previousBest;
    [SerializeField] private ParticleSystem confettiEffect;

    [Header("Mines")]
    [SerializeField] private GameObject mine;
    [SerializeField] private GameObject mineContainer;
    private float nextMineDistance;
    [SerializeField] private float mineSpawnOffset;
    private float previousMineHeight;
    [SerializeField] Vector2 minMineOffset;
    [SerializeField] Vector2 maxMineOffset;

    [Header("Clouds")]
    [SerializeField] private GameObject cloud;
    [SerializeField] private GameObject cloudContainer;
    private float nextCloudDistance;
    [SerializeField] private float cloudSpawnOffset;
    private float previousCloudHeight;
    [SerializeField] Vector2 minCloudOffset;
    [SerializeField] Vector2 maxCloudOffset;

    [Header("Platforms")]
    [SerializeField] GameObject platformContainer;
    [SerializeField] GameObject platform;
    [SerializeField] GameObject platformShort;
    [SerializeField] GameObject platformMoving;
    private float nextPlatformDistance;
    [SerializeField] private float platformSpawnOffset;
    private float previousPlatformHeight;

    [SerializeField] Vector2 minPlatformOffset;
    [SerializeField] Vector2 maxPlatformOffset;

    private void Start()
    {
        // Get references
        player = FindObjectOfType<PlayerController>();
        gs = FindObjectOfType<GameSession>();
        musicPlayer = FindObjectOfType<MusicPlayerController>();
        uiManager = FindObjectOfType<UIManager>();

        scoreDisplay.text = "0";
        nextPlatformDistance = 0;
        UpdateHighScoreDisplay();

        // Show medal
        if (gs.highScore > 500)
            medal1.SetActive(true);

        // Show medal 2
        if (gs.highScore > 1000)
            medal2.SetActive(true);

        // Set settings icons
        if (gs.isMusicMuted)
            musicIcon.sprite = musicOff;

        if (gs.isSoundMuted)
            soundIcon.sprite = soundOff;

        if (gs.highScore > 10)
            previousBest.gameObject.SetActive(true);
        previousBest.transform.position = new Vector3(0, gs.highScore, 0);
    }

    public void ReleaseConfetti()
    {
        if (previousBest.activeSelf)
            confettiEffect.Play();
    }

    public void HideTitleScreen()
    {
        titleScreen.SetActive(false);
    }

    public void DisplayPauseButton()
    {
        pauseButton.SetActive(true);
    }

    public void DisplayGameOverScreen()
    {
        uiManager.ShowGameOverDisplay(gs.highScore, score);
    }

    public void HideGameOverScreen()
    {
        uiManager.HideGameOverDisplay();
    }

    public void Respawn()
    {
        uiManager.SetRespawnPrompt(false);
        player.Respawn();
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void UpdateScoreDisplay(int scoreDifference)
    {
        score += scoreDifference;
        scoreDisplay.text = score.ToString();
    }

    public void UpdateHighScoreDisplay()
    {
        highScoreDisplay.text = "Best: " + gs.highScore.ToString();
    }

    private void Update()
    {
        if (player.transform.position.y > previousPlatformHeight + platformSpawnOffset)
            SpawnNewPlatform();

        if (player.transform.position.y > previousCloudHeight + cloudSpawnOffset)
            SpawnNewCloud();

        if (player.transform.position.y > previousMineHeight + mineSpawnOffset)
            SpawnNewMine();

        // Handle Background Color
        background.color = backgroundGradient.Evaluate((float)score / levelTargetScore);
    }

    private void SpawnNewPlatform()
    {
        float newPlatformRandomOffsetX = Random.Range(minPlatformOffset.x, maxPlatformOffset.x);
        float newPlatformRandomOffsetY = Random.Range(minPlatformOffset.y, maxPlatformOffset.y);

        Vector2 newPlaformPosition = new Vector2(newPlatformRandomOffsetX, previousPlatformHeight + newPlatformRandomOffsetY);

        float randomNumber = Random.Range(0.0f, 1.0f);

        //Debug.Log("Random Number: " + Random.Range(0.0f, 1.0f) + ", Scaling value: " + (float)score / (float)levelTargetScore);
        if (randomNumber > (float)score / (float)levelTargetScore)
        {
            Instantiate(platform, newPlaformPosition, Quaternion.identity, platformContainer.transform);
        }
        else if (randomNumber > (float)score / (float)(levelTargetScore * 2))
        {
            Instantiate(platformShort, newPlaformPosition, Quaternion.identity, platformContainer.transform);
        }
        else
        {
            Instantiate(platformMoving, newPlaformPosition, Quaternion.identity, platformContainer.transform);
        }


        previousPlatformHeight += newPlatformRandomOffsetY;
    }

    private void SpawnNewMine()
    {
        float newMineRandomOffsetX = Random.Range(minMineOffset.x, maxMineOffset.x);
        float newMineRandomOffsetY = Random.Range(minMineOffset.y, maxMineOffset.y);

        Vector2 newMinePosition = new Vector2(newMineRandomOffsetX, previousMineHeight + newMineRandomOffsetY);

        // Spawn mine as difficulty increases
        if (Random.Range(0.0f, 1.0f) < (float)score / (float)levelTargetScore)
        {
            Instantiate(mine, newMinePosition, Quaternion.identity, mineContainer.transform);
        }

        previousMineHeight += newMineRandomOffsetY;
    }

    private void SpawnNewCloud()
    {
        if (score <= 500)
        {
            float newCloudRandomOffsetX = Random.Range(minCloudOffset.x, maxCloudOffset.x);
            float newCloudRandomOffsetY = Random.Range(minCloudOffset.y, maxCloudOffset.y);

            Vector2 newCloudPosition = new Vector2(newCloudRandomOffsetX, previousCloudHeight + newCloudRandomOffsetY);
            Instantiate(cloud, newCloudPosition, Quaternion.identity, cloudContainer.transform);

            previousCloudHeight += newCloudRandomOffsetY;
        }
    }

    public void Pause()
    {
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
        pauseMenu.SetActive(true);
        isPaused = true;

        if (OnPaused != null)
            OnPaused(true);

        StartCoroutine(AnimateMenu(pauseMenuBox.transform, Screen.height, Screen.height / 2));
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        isPaused = false;

        if (OnPaused != null)
            OnPaused(false);
    }

    public void RecentreGyro()
    {
        player.UpdateGyroOffset(Input.gyro.attitude.y);
    }

    public void Quit()
    {
        ReloadScene();
    }

    public void OpenSettingsMenu()
    {
        settingsMenu.SetActive(true);
        StartCoroutine(AnimateMenu(settingsMenuBox.transform, Screen.height, Screen.height / 2));
    }

    public void CloseSettingsMenu()
    {
        settingsMenu.SetActive(false);
    }

    public void ToggleMuteMusic()
    {
        if (gs.isMusicMuted)
        {
            gs.UpdateMusicMuted(false);
            musicIcon.sprite = musicOn;
            musicPlayer.SetMusicMute(false);
        }
        else
        {
            gs.UpdateMusicMuted(true);
            musicIcon.sprite = musicOff;
            musicPlayer.SetMusicMute(true);
        }
    }

    public void ToggleMuteSounds()
    {
        if (gs.isSoundMuted)
        {
            gs.UpdateSoundMuted(false);
            soundIcon.sprite = soundOn;
            player.jumpAudioSource.mute = false;
        }
        else
        {
            gs.UpdateSoundMuted(true);
            soundIcon.sprite = soundOff;
            player.jumpAudioSource.mute = true;
        }
    }

    public void ClearHighscore()
    {
        gs.ClearHighscore();
        UpdateHighScoreDisplay();
    }

    public void OpenCreditsScreen()
    {
        creditsScreen.SetActive(true);
        StartCoroutine(AnimateMenu(creditsScreenBox.transform, Screen.height, Screen.height / 2));
    }

    public void CloseCreditsScreen()
    {
        creditsScreen.SetActive(false);
    }

    public void OpenSkinSelectScreen()
    {
        skinSelectScreen.SetActive(true);
        StartCoroutine(AnimateMenu(skinSelectScreenBox.transform, Screen.height, Screen.height / 2));
    }

    public void CloseSkinSelectScreen()
    {
        skinSelectScreen.SetActive(false);
    }

    public IEnumerator AnimateMenu(Transform menuTransform, float startPosition, float endPosition)
    {
        float animationLength = 1f;
        float startTime = Time.time;
        float endTime = startTime + animationLength;

        if (!gs)
            gs = FindObjectOfType<GameSession>();
        AnimationCurve animationCurve = gs.menuAnimationCurve;

        while (endTime > Time.time)
        {
            float curveValue = animationCurve.Evaluate(Time.time - startTime);
            Debug.Log("Curve Value: " + curveValue);
            float newYPos = Mathf.LerpUnclamped(startPosition, endPosition, curveValue);
            menuTransform.position = new Vector3(menuTransform.position.x, newYPos, menuTransform.position.z);
            yield return new WaitForEndOfFrame();
        }

        menuTransform.position = new Vector3(menuTransform.position.x, endPosition, menuTransform.position.z);
    }
}
