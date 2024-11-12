using UnityEngine;

public class PlayerPrefData : MonoBehaviour
{
    void Start()
    {
        PlayerPrefInit();
    }

    public void PlayerPrefInit()
    {
        if (!PlayerPrefs.HasKey("FirstTime"))
        {
            PlayerPrefs.SetInt("FirstTime", 1);
            PlayerPrefs.SetInt("SoundSetting", 1);
            PlayerPrefs.SetInt("NotificationSetting", 1);
            PlayerPrefs.SetInt("Mode", 3);
        }
    }

    public void ResetAllData()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("All player data reset.");
    }
}
