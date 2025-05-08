using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

[RequireComponent(typeof(Button))]
public class RewardedAdsButton : MonoBehaviour, IUnityAdsListener
{
    Button button;

    #if UNITY_IOS
    string gameId = "3842076";
#elif UNITY_ANDROID
    string gameId = "3842077";
#endif

    string thisPlacementId = "rewardedVideo";
    bool testMode = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SetUpButton());
    }

    private IEnumerator SetUpButton()
    {
        yield return new WaitForSeconds(0.1f);
        button = GetComponent<Button>();
        button.interactable = Advertisement.IsReady(thisPlacementId);

        Advertisement.AddListener(this);
        Advertisement.Initialize(gameId, testMode);
        yield return null;
    }

    public void ShowRewardedVideo()
    {
        if (Advertisement.IsReady(thisPlacementId))
            Advertisement.Show(thisPlacementId);
        else
            Debug.Log("Video is not ready");
    }

    public void OnUnityAdsDidFinish (string placementID, ShowResult showResult)
    {
        UIManager uiManager = FindObjectOfType<UIManager>();

        switch (showResult)
        {
            case ShowResult.Finished:
                uiManager.HideGameOverDisplay();
                uiManager.SetRespawnPrompt(true);
                break;
            case ShowResult.Skipped:
                uiManager.HideGameOverDisplay();
                uiManager.SetRespawnPrompt(true);
                break;
            case ShowResult.Failed:
                Debug.LogWarning("Ad did not finish due to an error");
                break;
        }
    }

    public void OnUnityAdsReady(string placementId)
    {
        if (placementId == thisPlacementId)
        {
            button.interactable = true;
        }
    }

    public void OnUnityAdsDidError(string message)
    {
        Debug.LogError(message);
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        // Player started an ad
    }

    public void OnDestroy()
    {
        Advertisement.RemoveListener(this);
    }
}
