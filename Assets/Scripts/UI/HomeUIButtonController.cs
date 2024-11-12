using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class HomeUIButtonController : MonoBehaviour
{
    // screen
    public GameObject tutorial;
    public GameObject energyToFly;
    public GameObject shopMenu;
    public GameObject energyShop;
    public GameObject gemShop;
    public GameObject warningScreen;
    public TMP_Text warningText;
    public GameObject devilStageButton;

    // level
    public GameObject levelLable;
    public TMP_Text modeText;
    public TMP_Text mainLevelText;

    // mode selection
    public GameObject modeSelectMenu;
    public TMP_Text levelText;
    public GameObject modeLable;
    public GameObject novice;
    public GameObject noviceCheck;
    public GameObject skilled;
    public GameObject skilledCheck;
    public GameObject master;
    public GameObject masterCheck;

    // settings
    public GameObject settingsMenu;
    public GameObject soundOn;
    public GameObject soundOff;
    [HideInInspector] public bool soundStatus;
    public GameObject notificationOn;
    public GameObject notificationOff;
    [HideInInspector] public bool notificationStatus;


    // colors
    private Color noviceColor = new Color(0.2509804f, 0.7450981f, 1f);
    private Color skilledColor = new Color(0.972549f, 0.5686275f, 0.2470588f);
    private Color masterColor = new Color(1f, 0.4445879f, 0.4386792f);
    private Color devilNotYetColor = new Color(0.3584906f, 0.3584906f, 0.3584906f);

    private EnergyManager energyManager;

    private void Start()
    {
        Time.timeScale = 1f;
        energyManager = GameObject.Find("EnergyManager").GetComponent<EnergyManager>();
        ChangeMainLevelText();
        soundStatus = PlayerPrefs.GetInt("SoundSetting") == 1;
        notificationStatus = PlayerPrefs.GetInt("NotificationSetting") == 1;
        UpdateNotificationState();
        UpdateSoundState();
        if (!DevilUnlock())
        {
            devilStageButton.GetComponent<Image>().color = devilNotYetColor;
        }
    }

    public void StartLevel()
    {
        if (UserData.energy > 0)
        {
            energyManager.GainEnergy(-1);
            UserData.instance.SaveData();
            StartCoroutine("SwitchToGame");
        }
        else
        {
            StartCoroutine(WarningGone("Out of Energy"));
        }
    }

    public void ModeSelectMenuOn()
    {
        ModeSelection(PlayerPrefs.GetInt("Mode"));
        modeSelectMenu.SetActive(true);
    }

    public void ModeSelectMenuOff()
    {
        ChangeMainLevelText();
        modeSelectMenu.SetActive(false);
    }

    public void ModeSelection(int mode)
    {
        TMP_Text modeText = modeLable.GetComponentInChildren<TMP_Text>();
        if (mode == 3)
        {
            PlayerPrefs.SetInt("Mode", 3);
            PlayerPrefs.Save();
            modeText.text = "Novice";
            modeLable.GetComponent<Image>().color = noviceColor;
            levelText.text = "LEVEL " + UserData.mode3Stage.ToString();
            noviceCheck.SetActive(true);
            skilledCheck.SetActive(false);
            masterCheck.SetActive(false);
        }
        else if (mode == 4)
        {
            PlayerPrefs.SetInt("Mode", 4);
            PlayerPrefs.Save();
            modeText.text = "Skilled";
            modeLable.GetComponent<Image>().color = skilledColor;
            levelText.text = "LEVEL " + UserData.mode4Stage.ToString();
            noviceCheck.SetActive(false);
            skilledCheck.SetActive(true);
            masterCheck.SetActive(false);
        }
        else if (mode == 5)
        {
            PlayerPrefs.SetInt("Mode", 5);
            PlayerPrefs.Save();
            modeText.text = "Master";
            modeLable.GetComponent<Image>().color = masterColor;
            levelText.text = "LEVEL " + UserData.mode5Stage.ToString();
            noviceCheck.SetActive(false);
            skilledCheck.SetActive(false);
            masterCheck.SetActive(true);
        }
    }

    public void ChangeMainLevelText()
    {
        int mode = PlayerPrefs.GetInt("Mode");
        if (mode == 3)
        {
            levelLable.GetComponent<Image>().color = noviceColor;
            modeText.text = "Novice";
            mainLevelText.text = "Lv. " + UserData.mode3Stage.ToString();
        }
        else if (mode == 4)
        {
            levelLable.GetComponent<Image>().color = skilledColor;
            modeText.text = "Skilled";
            mainLevelText.text = "Lv. " + UserData.mode4Stage.ToString();
        }
        else if (mode == 5)
        {
            levelLable.GetComponent<Image>().color = masterColor;
            modeText.text = "Master";
            mainLevelText.text = "Lv. " + UserData.mode5Stage.ToString();
        }
    }

    public void TutorialOn()
    {
        tutorial.SetActive(true);
    }

    public void TutorialOff()
    {
        tutorial.SetActive(false);
    }

    IEnumerator SwitchToGame()
    {
        energyToFly.SetActive(true);
        yield return new WaitForSeconds(1f);
        Transition.instance.TransitionToNextScene("GamePlay");
    }

    IEnumerator DevilGame()
    {
        energyToFly.SetActive(true);
        yield return new WaitForSeconds(1f);
        Transition.instance.TransitionToNextScene("DevilStage");
    }

    public void ToggleSound()
    {
        int currentSetting = PlayerPrefs.GetInt("SoundSetting", 0);
        PlayerPrefs.SetInt("SoundSetting", currentSetting == 1 ? 0 : 1);
        PlayerPrefs.Save();
        soundStatus = !soundStatus;
        UpdateSoundState();
    }

    public void ToggleNotification()
    {
        int currentSetting = PlayerPrefs.GetInt("NotificationSetting", 0);
        PlayerPrefs.SetInt("NotificationSetting", currentSetting == 1 ? 0 : 1);
        PlayerPrefs.Save();
        notificationStatus = !notificationStatus;
        UpdateNotificationState();
    }

    private void UpdateSoundState()
    {
        if (soundOn != null && soundOff != null)
        {
            soundOn.SetActive(soundStatus);
            soundOff.SetActive(!soundStatus);
        }
    }

    private void UpdateNotificationState()
    {
        if (notificationOn != null && notificationOff != null)
        {
            notificationOn.SetActive(notificationStatus);
            notificationOff.SetActive(!notificationStatus);
        }
    }

    public void SettingsOn()
    {
        settingsMenu.SetActive(true);
    }

    public void SettingsOff()
    {
        settingsMenu.SetActive(false);
    }

    public void ShopOn(int gemOrEnergy)
    {
        shopMenu.SetActive(true);
        if (gemOrEnergy == 1)
        {
            energyShop.SetActive(false);
            gemShop.SetActive(true);
        }
        else if (gemOrEnergy == 2)
        {
            energyShop.SetActive(true);
            gemShop.SetActive(false);
        }
    }

    public void ShopOff()
    {
        shopMenu.SetActive(false);
        energyShop.SetActive(false);
        gemShop.SetActive(false);
    }

    public void WarningScreen(string message)
    {
        StartCoroutine(WarningGone(message));
    }

    IEnumerator WarningGone(string message)
    {
        warningText.text = message;
        warningScreen.SetActive(true);

        CanvasGroup canvasGroup = warningScreen.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = warningScreen.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 1f;
        yield return new WaitForSecondsRealtime(1.5f);

        float fadeDuration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        warningScreen.SetActive(false);
    }

    public void DevilStage()
    {
        if (DevilUnlock())
        {
            if (UserData.energy > 0)
            {
                energyManager.GainEnergy(-1);
                UserData.instance.SaveData();
                StartCoroutine("DevilGame");
            }
            else
            {
                StartCoroutine(WarningGone("Out of Energy"));
            }
        }
        else
        {
            string devilNotYet = "Reach at least level 50 in all three modes to unlock!";
            WarningScreen(devilNotYet);
        }
    }

    private bool DevilUnlock()
    {
        return UserData.mode3Stage >= 50 && UserData.mode4Stage >= 50 && UserData.mode5Stage >= 50;
    }
}
