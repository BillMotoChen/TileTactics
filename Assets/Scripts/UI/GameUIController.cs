using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUIController : MonoBehaviour
{
    public GameObject clearMenu;
    [HideInInspector] public Animator clearMenuAnimator;
    public GameObject gameOverMenu;
    private EnergyManager energyManager;
    public TMP_Text stepText;

    // mode
    public GameObject levelLable;
    public TMP_Text modeText;
    public TMP_Text mainLevelText;

    // colors
    private Color noviceColor = new Color(0.2509804f, 0.7450981f, 1f);
    private Color skilledColor = new Color(0.972549f, 0.5686275f, 0.2470588f);
    private Color masterColor = new Color(1f, 0.4445879f, 0.4386792f);

    public GameObject pauseButton;

    // audio
    private AudioSource clearAudio;
    private AudioSource gameoverAudio;


    private void Start()
    {
        ChangeMainLevelText();

        pauseButton = GameObject.Find("GameCanvas/SafeArea/PauseButtonWhite");
        energyManager = GameObject.Find("EnergyManager").GetComponent<EnergyManager>();
        clearMenuAnimator = clearMenu.GetComponent<Animator>();
        gameoverAudio = gameOverMenu.GetComponent<AudioSource>();
        clearAudio = clearMenu.GetComponent<AudioSource>();
    }

    public void LevelClear()
    {
        clearMenu.SetActive(true);
        if (PlayerPrefs.GetInt("SoundSetting") == 1)
        {
            clearAudio.Play();
        }
        energyManager.GainEnergy(1);
        pauseButton.SetActive(false);
        if (PlayerPrefs.GetInt("Mode") == 3)
        {
            UserData.mode3Stage += 1;
        }
        else if (PlayerPrefs.GetInt("Mode") == 4)
        {
            UserData.mode4Stage += 1;
        }
        else if (PlayerPrefs.GetInt("Mode") == 5)
        {
            UserData.mode5Stage += 1;
        }
        UserData.instance.SaveData();
        Time.timeScale = 0f;
    }

    public void DevilClear()
    {
        clearMenu.SetActive(true);
        if (PlayerPrefs.GetInt("SoundSetting") == 1)
        {
            clearAudio.Play();
        }
        energyManager.GainEnergy(1);
        pauseButton.SetActive(false);
        UserData.devilStage += 1;
        UserData.instance.SaveData();
        Time.timeScale = 0f;
    }

    public void GameOver()
    {
        gameOverMenu.SetActive(true);
        if (PlayerPrefs.GetInt("SoundSetting") == 1)
        {
            gameoverAudio.Play();
        }
        pauseButton.SetActive(false);
        Time.timeScale = 0f;
    }

    public void ChangeMainLevelText()
    {
        if (levelLable.name == "DevilLevelText")
        {
            mainLevelText.text = "Lv. " + UserData.devilStage.ToString();
        }
        else
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
    }

    public void UpdateStepText(int remainSteps, int maxSteps)
    {
        stepText.text = "MOVES " + remainSteps + "/" + maxSteps;
    }
}
