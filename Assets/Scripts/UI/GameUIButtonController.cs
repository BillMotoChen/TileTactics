using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public class GameUIButtonController : MonoBehaviour
{
    public GameObject pauseButton;
    public GameObject pauseMenu;
    public GameObject itemMenu;
    public GameObject itemMenuTrigger;
    public GameObject itemIntro;
    public GameObject warningScreen;

    private TileChanger tileChanger;
    private GameUIController UIController;

    private bool diamondGained;

    void Start()
    {
        UIController = GameObject.Find("GameCanvas/GameUIController").GetComponent<GameUIController>();
        tileChanger = GameObject.Find("TileChanger").GetComponent<TileChanger>();
        warningScreen.SetActive(false);
        diamondGained = false;
    }

    public void PauseMenuOn()
    {
        pauseButton.SetActive(false);
        itemMenuTrigger.SetActive(false);
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        tileChanger.canClick = false;
    }

    public void PauseMenuOff()
    {
        pauseButton.SetActive(true);
        itemMenuTrigger.SetActive(true);
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        tileChanger.canClick = true;
    }

    public void GoHome()
    {
        SceneManager.LoadScene("Home");
    }

    public void Restart()
    {
        if (UserData.energy > 0)
        {
            UserData.energy -= 1;
            UserData.instance.SaveData();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            StartCoroutine(WarningGone());
        }
    }

    public void NextStage()
    {
        if (!diamondGained)
        {
            UserData.diamond += 1;
            UserData.instance.SaveData();
            if (UserData.energy > 0)
            {
                UserData.energy -= 1;
                UserData.instance.SaveData();
                StartCoroutine(WaitOneSecond(SceneManager.GetActiveScene().name));
            }
            else
            {
                StartCoroutine(WarningGone());
            }
            diamondGained = true;
        }
    }

    public void NextStageDoubleGem()
    {
        if (!diamondGained)
        {
            UserData.diamond += 2;
            UserData.instance.SaveData();
            if (UserData.energy > 0)
            {
                UserData.energy -= 1;
                UserData.instance.SaveData();
                StartCoroutine(WaitOneSecond(SceneManager.GetActiveScene().name));
            }
            else
            {
                StartCoroutine(WarningGone());
            }
            diamondGained = true;
        }
    }


    public void HomeAfterClear()
    {
        if (!diamondGained)
        {
            StartCoroutine(WaitOneSecond("Home"));
            UserData.diamond += 1;
            UserData.instance.SaveData();
            diamondGained = true;
        }
    }

    IEnumerator WaitOneSecond(string sceneName)
    {
        yield return new WaitForSecondsRealtime(1f);
        SceneManager.LoadScene(sceneName);
    }

    public void ItemMenuOn()
    {
        itemMenu.SetActive(true);
        tileChanger.canClick = false;
        itemMenuTrigger.SetActive(false);
        pauseButton.SetActive(false);
        Time.timeScale = 0f;
    }

    public void ItemMenuOff()
    {
        itemMenu.SetActive(false);
        tileChanger.canClick = true;
        itemMenuTrigger.SetActive(true);
        pauseButton.SetActive(true);
        Time.timeScale = 1f;
    }

    public void ItemIntroOn()
    {
        itemIntro.SetActive(true);
    }

    public void ItemIntroOff()
    {
        itemIntro.SetActive(false);
    }

    IEnumerator WarningGone()
    {
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

    public void Revive5Steps()
    {
        tileChanger.remainSteps += 5;
        UIController.gameOverMenu.SetActive(false);
        UIController.UpdateStepText(tileChanger.remainSteps, tileChanger.maxSteps);
        UIController.pauseButton.SetActive(true);
        Time.timeScale = 1f;
    }
}
