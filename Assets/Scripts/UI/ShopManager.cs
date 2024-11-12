using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
using TMPro;

public class ShopManager : MonoBehaviour
{
    private List<GemPurchaseInfo> gemPurchaseOptions;
    private List<EnergyPurchaseInfo> energyPurchaseOptions;

    private EnergyManager energyManager;
    private CommonUIController commonUIController;
    private HomeUIButtonController homeUIButtonController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeEnergyItems();
        InitializeGemItems();
        energyManager = GameObject.Find("EnergyManager").GetComponent<EnergyManager>();
        commonUIController = GameObject.Find("CommonUIController").GetComponent<CommonUIController>();
        homeUIButtonController = GameObject.Find("HomeUIButtonController").GetComponent<HomeUIButtonController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void gemPurchase(int id)
    {
        GemPurchaseInfo item = gemPurchaseOptions.Find(x => x.gemPurchaseId == id);
        UserData.diamond += item.quantity;
        commonUIController.UpdateGemText();
        UserData.instance.SaveData();
    }

    public void energyPurchase(int id)
    {
        EnergyPurchaseInfo item = energyPurchaseOptions.Find(x => x.energyPurchaseId == id);

        if (UserData.diamond < item.price)
        {
            homeUIButtonController.ShopOn(1);
            homeUIButtonController.WarningScreen("No Enough Gem!");
        }

        else if (UserData.diamond >= item.price)
        {
            UserData.diamond -= item.price;
            commonUIController.UpdateGemText();
            energyManager.GainEnergy(item.quantity);            
        }
    }

    private void InitializeGemItems()
    {
        gemPurchaseOptions = new List<GemPurchaseInfo>
        {
            new GemPurchaseInfo { gemPurchaseId = 0, price = 0, quantity = 2 },
            new GemPurchaseInfo { gemPurchaseId = 1, price = 0.99f, quantity = 100 },
            new GemPurchaseInfo { gemPurchaseId = 2, price = 4.99f, quantity = 500 },
            new GemPurchaseInfo { gemPurchaseId = 3, price = 9.99f, quantity = 1100 },
            new GemPurchaseInfo { gemPurchaseId = 4, price = 19.99f, quantity = 2300 },
            new GemPurchaseInfo { gemPurchaseId = 5, price = 49.99f, quantity = 6000 },
        };
    }

    private void InitializeEnergyItems()
    {
        energyPurchaseOptions = new List<EnergyPurchaseInfo>
        {
            new EnergyPurchaseInfo { energyPurchaseId = 1, price = 0, quantity = 2 },
            new EnergyPurchaseInfo { energyPurchaseId = 2, price = 5, quantity = 5 },
            new EnergyPurchaseInfo { energyPurchaseId = 3, price = 8, quantity = 10 },
        };
    }
}

[System.Serializable]
public class GemPurchaseInfo
{
    public int gemPurchaseId;
    public int quantity;
    public float price;
}

[System.Serializable]
public class EnergyPurchaseInfo
{
    public int energyPurchaseId;
    public int quantity;
    public int price;
}