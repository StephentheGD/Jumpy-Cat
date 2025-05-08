using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using GooglePlayGames;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverDisplay;
    [SerializeField] private TMP_Text gameOverText;
    [SerializeField] private GameObject respawnPrompt;
    [SerializeField] private Image gameOverMedal;
    [SerializeField] private Sprite medal1;
    [SerializeField] private Sprite medal2;

    [SerializeField] private TextMeshProUGUI signInButtonText;
    [SerializeField] private TextMeshProUGUI authStatus;

    [SerializeField] private Button signInButton;
    [SerializeField] private Button signOutButton;
    [SerializeField] private Button achievementsButton;
    [SerializeField] private Button leaderboardButton;

    GameSession gs;

    public void ShowGameOverDisplay(int previousHighScore, int newHighScore)
    {
        gameOverDisplay.SetActive(true);
        if (newHighScore > previousHighScore)
            gameOverText.text = "New High Score:" + newHighScore.ToString();
        else
            gameOverText.text = "Game Over";

        if (newHighScore > 1000)
        {
            gameOverMedal.gameObject.SetActive(true);
            gameOverMedal.sprite = medal2;
        }
        else if (newHighScore > 500)
        {
            gameOverMedal.gameObject.SetActive(true);
            gameOverMedal.sprite = medal1;
        }
    }

    public void HideGameOverDisplay()
    {
        gameOverDisplay.SetActive(false);
    }

    public void SetRespawnPrompt(bool option)
    {
        respawnPrompt.SetActive(option);
    }

    public void SignInButton()
    {
        if (!gs)
            gs = FindObjectOfType<GameSession>();

        gs.SignInToGooglePlayServices(this);
    }

    public void UpdateSignInUIElements(string signIn, string auth)
    {
        signInButtonText.text = signIn;
        authStatus.text = auth;

        signInButton.gameObject.SetActive(!Social.localUser.authenticated);
        signOutButton.gameObject.SetActive(Social.localUser.authenticated);
        achievementsButton.gameObject.SetActive(Social.localUser.authenticated);
        leaderboardButton.gameObject.SetActive(Social.localUser.authenticated);
    }

    public void ShowAchievements()
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated)
            PlayGamesPlatform.Instance.ShowAchievementsUI();
        else
            Debug.Log("Cannot show achievements, not logged in");
    }

    public void ShowLeaderboard()
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated)
            PlayGamesPlatform.Instance.ShowLeaderboardUI();
        else
            Debug.Log("Cannot show leaderboards, not logged in");
    }
}
