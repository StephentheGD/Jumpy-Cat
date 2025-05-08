using System.Collections;
using System.Collections.Generic;
using System.Security;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkinSelectController : MonoBehaviour
{
    [SerializeField] private Sprite[] catSkins;

    [SerializeField] private Image catImage;
    [SerializeField] private GameObject leftButton;
    [SerializeField] private GameObject rightButton;
    [SerializeField] private Button selectSkinButton;
    [SerializeField] private Button purchaseSkinButton;
    [SerializeField] private GameObject skinLockedOverlay;
    [SerializeField] private TextMeshProUGUI skinTitle;

    [SerializeField] private bool[] isLocked;
    [SerializeField] private bool[] inAppPurchase;
    [SerializeField] private string[] names;
    [SerializeField] private string[] unlockConditions;

    private int selectedSkin;

    GameSession gs;

    private void Start()
    {
        if (!gs)
            gs = FindObjectOfType<GameSession>();
        selectedSkin = gs.currentSkin;
        catImage.sprite = catSkins[selectedSkin];
         
        LeftButtonCheck();
        RightButtonCheck();

        CheckLockedSkins();
        UpdateDisplay();
    }

    public void CheckLockedSkins()
    {
        if (gs.highScore > 250)
            isLocked[1] = false;

        if (gs.highScore > 500)
            isLocked[2] = false;

        if (gs.isGoldenUnlocked)
            isLocked[3] = false;
    }

    public bool CheckIfSkinIsLocked()
    {
        CheckLockedSkins();

        if (isLocked[selectedSkin])
        {
            skinLockedOverlay.SetActive(true);


            selectSkinButton.gameObject.SetActive(!inAppPurchase[selectedSkin]);
            purchaseSkinButton.gameObject.SetActive(inAppPurchase[selectedSkin]);

            selectSkinButton.enabled = false;
            return true;
        }
        else
        {
            skinLockedOverlay.SetActive(false);
            selectSkinButton.gameObject.SetActive(true);
            purchaseSkinButton.gameObject.SetActive(false);
            selectSkinButton.enabled = true;
            return false;
        }
    }

    public void UpdateDisplay()
    {
        if (CheckIfSkinIsLocked())
            skinTitle.text = unlockConditions[selectedSkin];
        else
            skinTitle.text = names[selectedSkin];
    }

    public void MoveLeft()
    {
        selectedSkin--;
        catImage.sprite = catSkins[selectedSkin];

        LeftButtonCheck();
        RightButtonCheck();

        UpdateDisplay();
    }

    public void MoveRight()
    {
        selectedSkin++;
        catImage.sprite = catSkins[selectedSkin];

        LeftButtonCheck();
        RightButtonCheck();

        UpdateDisplay();
    }

    private void LeftButtonCheck()
    {
        if (selectedSkin == 0)
            leftButton.SetActive(false);
        else
            leftButton.SetActive(true);
    }

    private void RightButtonCheck()
    {
        if (selectedSkin == catSkins.Length - 1)
            rightButton.SetActive(false);
        else
            rightButton.SetActive(true);
    }

    public void SelectSkin()
    {
        FindObjectOfType<PlayerController>().UpdateCurrentSkin(selectedSkin);
        FindObjectOfType<GameManager>().CloseSkinSelectScreen();
    }

    private void OnEnable()
    {
        if (!gs)
            gs = FindObjectOfType<GameSession>();

        if (gs.hasHighscoreBeenReset)
        {
            selectedSkin = 0;

            for (int i = 1; i < 3; i++)
                isLocked[i] = true;
        }

        UpdateDisplay();
    }

    public void RestoreDefaults()
    {
        selectedSkin = gs.currentSkin;

        for (int i = 1; i < 3; i++)
            isLocked[i] = true;
    }

    public void UnlockGolden()
    {
        FindObjectOfType<GameSession>().UnlockGolden();
    }
}
