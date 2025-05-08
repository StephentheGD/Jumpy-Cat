using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPManager : MonoBehaviour, IStoreListener
{
    static string golden_skin = "golden_skin";

    private IStoreController controller;
    private IExtensionProvider extensions;

    ConfigurationBuilder builder;

    private void Awake()
    {
        builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
    }

    public IAPManager()
    {
        /*
        builder.AddProduct("golden_skin", ProductType.NonConsumable, new IDs
        {
            { "golden_skin_google", GooglePlay.Name },
            { "golden_skin_apple", AppleAppStore.Name }
        });

        UnityPurchasing.Initialize(this, builder);
        */
    }

    public void OnInitialized (IStoreController controller, IExtensionProvider extensions)
    {
        this.controller = controller;
        this.extensions = extensions;
    }

    public void OnInitializeFailed (InitializationFailureReason error)
    {

    }

    public PurchaseProcessingResult ProcessPurchase (PurchaseEventArgs e)
    {
        if (String.Equals(e.purchasedProduct.definition.id, golden_skin, StringComparison.Ordinal))
        {
            Debug.Log("The golden cat skin has been purchased.");
            FindObjectOfType<GameSession>().isGoldenUnlocked = true;
        }

        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed (Product i, PurchaseFailureReason p)
    {

    }



    public void RestorePurchases()
    {
        if (controller == null || extensions == null)
        {
            Debug.LogError("RestorePurchases fail. Not initialised");
        }

        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer)
        {
            var apple = extensions.GetExtension<IAppleExtensions>();
            apple.RestoreTransactions((result) =>
            {
                Debug.Log(result);
            });
        }
        else
            Debug.Log("Restoring puchases is not available on this platform");
    }
}
