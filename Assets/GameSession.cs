using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class GameSession : MonoBehaviour
{
    public AnimationCurve menuAnimationCurve;

    public int highScore;
    public bool isMusicMuted;
    public bool isSoundMuted;
    public int currentSkin;
    public bool isGoldenUnlocked;

    public bool hasHighscoreBeenReset;

    void Start()
    {
        GameSession[] gsList = FindObjectsOfType<GameSession>();
        //Debug.Log("Number of game sessions: " + gsList.Length);
        if (gsList.Length > 1)
            Destroy(gameObject);

        DontDestroyOnLoad(this);

        // Load player preferences
        if (PlayerPrefs.HasKey("highscore"))
            highScore = PlayerPrefs.GetInt("highscore");
        else
            highScore = 0;

        if (PlayerPrefs.HasKey("isMusicMuted"))
            if (PlayerPrefs.GetInt("isMusicMuted") > 0)
                isMusicMuted = true;

        if (PlayerPrefs.HasKey("isSoundMuted"))
            if (PlayerPrefs.GetInt("isSoundMuted") > 0)
                isSoundMuted = true;

        if (PlayerPrefs.HasKey("currentSkin"))
            currentSkin = PlayerPrefs.GetInt("currentSkin");

        if (PlayerPrefs.HasKey("isGoldenUnlocked"))
            isGoldenUnlocked = (PlayerPrefs.GetInt("isGoldenUnlocked") > 0);

        // Load Google Play Games
        PlayGamesClientConfiguration playGamesClientConfiguration = new PlayGamesClientConfiguration.Builder().EnableSavedGames().Build();

        PlayGamesPlatform.DebugLogEnabled = true;

        PlayGamesPlatform.InitializeInstance(playGamesClientConfiguration);
        PlayGamesPlatform.Activate();

        PlayGamesPlatform.Instance.Authenticate(SignInCallback, true);

        /*
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager)
            SignInToGooglePlayServices(uiManager);
        */
    }

    public void SignInToGooglePlayServices(UIManager uiManager)
    {
        Debug.Log("Signing in to Google Play Services");
        if (!PlayGamesPlatform.Instance.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.Authenticate(SignInCallback, false);
        }
        else
        {
            PlayGamesPlatform.Instance.SignOut();
            uiManager.UpdateSignInUIElements("Sign In", "");
        }
    }

    public void SignInCallback(bool success)
    {
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (success)
        {
            Debug.Log("Signed in");
            uiManager.UpdateSignInUIElements("Sign Out", "Signed in as: " + Social.localUser.userName);
        }
        else
        {
            Debug.Log("Signed in failed");
            uiManager.UpdateSignInUIElements("Sign In to Google Play Games", "");
        }
    }

    public void UpdateHighscore(int newHighscore)
    {
        highScore = newHighscore;

        PlayerPrefs.SetInt("highscore", highScore);
        PlayerPrefs.Save();

        CheckAchievements();
        UpdateLeaderboard();
    }

    private void CheckAchievements()
    {
        // Only do achievements if the user is signed in
        if (Social.localUser.authenticated)
        {
            if (highScore > 50)
            {
                PlayGamesPlatform.Instance.UnlockAchievement(
                    "CgkIkqPXz7IFEAIQAA",
                    (bool success) => { Debug.Log("A Good Start: " + success); }
                );
            }
            if (highScore > 100)
            {
                PlayGamesPlatform.Instance.UnlockAchievement(
                    "CgkIkqPXz7IFEAIQAQ",
                    (bool success) => { Debug.Log("Head in the Clouds: " + success); }
                );
            }
            if (highScore > 250)
            {
                PlayGamesPlatform.Instance.UnlockAchievement(
                    "CgkIkqPXz7IFEAIQAg",
                    (bool success) => { Debug.Log("Ozone Explorer: " + success); }
                );
            }
            if (highScore > 500)
            {
                PlayGamesPlatform.Instance.UnlockAchievement(
                    "CgkIkqPXz7IFEAIQAw",
                    (bool success) => { Debug.Log("Entering the Stratosphere: " + success); }
                );
            }
            if (highScore > 750)
            {
                PlayGamesPlatform.Instance.UnlockAchievement(
                    "CgkIkqPXz7IFEAIQBA",
                    (bool success) => { Debug.Log("Above the Clouds: " + success); }
                );
            }
            if (highScore > 1000)
            {
                PlayGamesPlatform.Instance.UnlockAchievement(
                    "CgkIkqPXz7IFEAIQBQ",
                    (bool success) => { Debug.Log("Master of the Sky: " + success); }
                );
            }
        }
    }

    private void UpdateLeaderboard()
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.ReportScore(
                highScore,
                "CgkIkqPXz7IFEAIQBw",
                (bool success) => { Debug.Log("Updated Leaderboard: " + success); }
                );
        }
    }

    public void ClearHighscore()
    {
        PlayerPrefs.DeleteKey("highscore");
        PlayerPrefs.Save();
        highScore = 0;

        currentSkin = 0;
        hasHighscoreBeenReset = true;

        PlayerController player = FindObjectOfType<PlayerController>();
        player.UpdateCurrentSkin(0);

    }

    public void UpdateMusicMuted(bool mute)
    {
        isMusicMuted = mute;
        if (mute)
            PlayerPrefs.SetInt("isMusicMuted", 1);
        else
            PlayerPrefs.SetInt("isMusicMuted", 0);

        PlayerPrefs.Save();
    }

    public void UpdateSoundMuted(bool mute)
    {
        isSoundMuted = mute;
        if (mute)
            PlayerPrefs.SetInt("isSoundMuted", 1);
        else
            PlayerPrefs.SetInt("isSoundMuted", 0);

        PlayerPrefs.Save();
    }

    public void UpdateCurrentSkin(int skin)
    {
        currentSkin = skin;
        PlayerPrefs.SetInt("currentSkin", skin);

        PlayerPrefs.Save();
    }

    public void UnlockGolden()
    {
        isGoldenUnlocked = true;
        PlayerPrefs.SetInt("isGoldenUnlocked", 1);

        PlayerPrefs.Save();

        Debug.Log("Golden has been unlocked");
    }
}
