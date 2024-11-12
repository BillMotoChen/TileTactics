using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;

[DefaultExecutionOrder(-100)]
public class UserData : MonoBehaviour
{
    const string savedFilePath = "/save.dat";

    public static UserData instance;

    // stage data
    public static int mode3Stage = 1;
    public static int mode4Stage = 1;
    public static int mode5Stage = 1;
    public static int devilStage = 1;

    // currency
    public static int diamond = 0;
    public static int energy = 10;
    public static DateTime lastEnergyGenerated = DateTime.Now;


    // saved data
    // saved stage data
    public int mode3StageSaved = 1;
    public int mode4StageSaved = 1;
    public int mode5StageSaved = 1;
    public int devilStageSaved = 1;

    // saved currency
    public int diamondSaved = 0;
    public int energySaved = 10;

    // Use long for saving timestamp instead of DateTime
    public long lastEnergyGeneratedSaved;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        LoadData();
    }

    void dataInit()
    {
        mode3Stage = 1;
        mode4Stage = 1;
        mode5Stage = 1;
        devilStage = 1;
        diamond = 0;
        energy = 10;
        lastEnergyGenerated = DateTime.Now;
        SaveData();
        LoadData();
    }

    public void SaveData()
    {
        string fileLocation = Application.persistentDataPath + savedFilePath;

        mode3StageSaved = mode3Stage;
        mode4StageSaved = mode4Stage;
        mode5StageSaved = mode5Stage;
        devilStageSaved = devilStage;
        diamondSaved = diamond;
        energySaved = energy;
        lastEnergyGeneratedSaved = lastEnergyGenerated.ToUnixTimestamp();

        string jsonData = JsonUtility.ToJson(this);

        string encryptedData = EncryptionUtility.Encrypt(jsonData);
        //string encryptedData = jsonData;

        try
        {
            StreamWriter writer = new StreamWriter(fileLocation);
            writer.Write(encryptedData);
            writer.Close();
        }
        catch (IOException e)
        {
            Debug.LogError("Failed to save data: " + e.Message);
        }
    }

    public void LoadData()
    {
        string fileLocation = Application.persistentDataPath + savedFilePath;

        if (File.Exists(fileLocation))
        {
            try
            {
                StreamReader reader = new StreamReader(fileLocation);
                string encryptedData = reader.ReadToEnd();
                reader.Close();

                string decryptedData = EncryptionUtility.Decrypt(encryptedData);
                //string decryptedData = encryptedData;
                JsonUtility.FromJsonOverwrite(decryptedData, this);
                mode3Stage = mode3StageSaved;
                mode4Stage = mode4StageSaved;
                mode5Stage = mode5StageSaved;
                devilStage = devilStageSaved;
                diamond = diamondSaved;
                energy = energySaved;
                lastEnergyGenerated = DateTimeExtensions.FromUnixTimestamp(lastEnergyGeneratedSaved);
            }
            catch (IOException e)
            {
                Debug.LogError("Failed to load data: " + e.Message);
            }
        }
        else
        {
            Debug.Log("Save file does not exist, initializing new data.");
            dataInit();
        }
    }
}

public static class DateTimeExtensions
{
    public static long ToUnixTimestamp(this DateTime dateTime)
    {
        return (long)(dateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
    }

    public static DateTime FromUnixTimestamp(long timestamp)
    {
        return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timestamp);
    }
}
