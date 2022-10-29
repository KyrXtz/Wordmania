using System;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Samples.Purchasing.Core.BuyingConsumables
{
    public class BuyingConsumables : MonoBehaviour, IStoreListener
    {
        IStoreController m_StoreController; // The Unity Purchasing system.

        //Your products IDs. They should match the ids of your products in your store.
        public LevelManager levelManager;
        public string goldProductId = "com.kyrxtz.crosswordladdersus.gold";
        public string goldProductId2 = "com.kyrxtz.crosswordladdersus.gold2";
        public string goldProductId3 = "com.kyrxtz.crosswordladdersus.gold3";
        public string goldProductId4 = "com.kyrxtz.crosswordladdersus.gold4";
        public string goldProductId5 = "com.kyrxtz.crosswordladdersus.gold5";
        public string diamondProductId = "com.mycompany.mygame.diamond1";



        int m_GoldCount;
        int m_DiamondCount;

        void Start()
        {
            InitializePurchasing();
           // UpdateUI();
        }

        void InitializePurchasing()
        {
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            //Add products that will be purchasable and indicate its type.
            builder.AddProduct(goldProductId, ProductType.Consumable);
            builder.AddProduct(goldProductId2, ProductType.Consumable);
            builder.AddProduct(goldProductId3, ProductType.Consumable);
            builder.AddProduct(goldProductId4, ProductType.Consumable);
            builder.AddProduct(goldProductId5, ProductType.Consumable);
            //builder.AddProduct(diamondProductId, ProductType.Consumable);

            UnityPurchasing.Initialize(this, builder);
        }

        public void BuyGold(string id)
        {
            m_StoreController.InitiatePurchase(id);
        }

        public void BuyDiamond()
        {
            m_StoreController.InitiatePurchase(diamondProductId);
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            Debug.Log("In-App Purchasing successfully initialized");
            m_StoreController = controller;
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.Log($"In-App Purchasing initialize failed: {error}");
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            //Retrieve the purchased product
            var product = args.purchasedProduct;

            //Add the purchased product to the players inventory
            if (product.definition.id == goldProductId)
            {
                // AddGold();
                levelManager.updateCoins(50, false);
                // Debug.Log("NCICE");
            }
            else if (product.definition.id == goldProductId2)
            {
                levelManager.updateCoins(100, false);
            }
            else if (product.definition.id == goldProductId3)
            {
                levelManager.updateCoins(300, false);
            }
            else if (product.definition.id == goldProductId4)
            {
                levelManager.updateCoins(500, false);
            }
            else if (product.definition.id == goldProductId5)
            {
                levelManager.updateCoins(1000,false);
            }

            Debug.Log($"Purchase Complete - Product: {product.definition.id}");

            //We return Complete, informing IAP that the processing on our side is done and the transaction can be closed.
            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Debug.Log($"Purchase failed - Product: '{product.definition.id}', PurchaseFailureReason: {failureReason}");
        }

        

        
    }
}
