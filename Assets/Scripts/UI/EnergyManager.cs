using UnityEngine;
using System.IO;
using System;
using TMPro;

public class EnergyManager : MonoBehaviour
{
    public TMP_Text energyText;
    public GameObject energyRecover;
    public TMP_Text energyRecoverText;

    private TimeSpan energyRegenInterval = TimeSpan.FromMinutes(7);

    private DateTime lastEnergyGenerated;

    void Start()
    {
        lastEnergyGenerated = UserData.lastEnergyGenerated;
        RegenerateOfflineEnergy();
        UpdateEnergyUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsEnergyMax())
        {
            energyRecover.SetActive(false);
            UserData.lastEnergyGenerated = DateTime.Now;
            UserData.instance.SaveData();
        }
        else if (!IsEnergyMax())
        {
            energyRecover.SetActive(true);
            RegenerateEnergy();
        }
    }

    public void GainEnergy(int amount)
    {
        UserData.energy += amount;
        UpdateEnergyUI();
        UserData.instance.SaveData();
    }

    private void RegenerateEnergy()
    {
        DateTime currentTime = DateTime.Now;
        TimeSpan timeSinceLastGeneration = currentTime - lastEnergyGenerated;

        if (timeSinceLastGeneration >= energyRegenInterval)
        {
            GainEnergy(1);
            lastEnergyGenerated = lastEnergyGenerated.AddMinutes(energyRegenInterval.TotalMinutes);
            UserData.lastEnergyGenerated = lastEnergyGenerated;
        }

        TimeSpan timeToNextEnergy = energyRegenInterval - (currentTime - lastEnergyGenerated);
        energyRecoverText.text = $"Next Energy in: {timeToNextEnergy.Minutes:D2}:{timeToNextEnergy.Seconds:D2}";
    }

    private void RegenerateOfflineEnergy()
    {
        DateTime currentTime = DateTime.Now;
        TimeSpan offlineTime = currentTime - lastEnergyGenerated;

        if (offlineTime > TimeSpan.Zero && UserData.energy < 10)
        {
            int energyToGain = (int)(offlineTime.TotalMinutes / energyRegenInterval.TotalMinutes);
            if (energyToGain > 0)
            {
                energyToGain = (UserData.energy + energyToGain) > 10 ? 10 - UserData.energy : energyToGain;
                GainEnergy(energyToGain);
            }

            double remainderMinutes = offlineTime.TotalMinutes % energyRegenInterval.TotalMinutes;
            lastEnergyGenerated = currentTime - TimeSpan.FromMinutes(remainderMinutes);
            UserData.lastEnergyGenerated = lastEnergyGenerated;
        }
    }

    private bool IsEnergyMax()
    {
        return UserData.energy >= 10;
    }

    private void UpdateEnergyUI()
    {
        energyText.text = UserData.energy.ToString() + "/10";
    }
}
