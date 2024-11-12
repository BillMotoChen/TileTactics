using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class DevilItemManager : MonoBehaviour
{
    private DevilTileChanger tileChanger;
    private DevilGameUIButtonController gameUIButtonController;
    private CommonUIController commonUIController;

    public Sprite[] itemImages;
    public Image itemImage;
    public GameObject itemMenuTrigger;

    public GameObject warningScreen;
    public TMP_Text warningText;

    private void Start()
    {
        tileChanger = GetComponentInParent<DevilTileChanger>();
        gameUIButtonController = GameObject.Find("GameUIButtonController").GetComponent<DevilGameUIButtonController>();
        commonUIController = GameObject.Find("CommonUIController").GetComponent<CommonUIController>();
    }

    public enum ItemType
    {
        Pencil,
        Rocket,
        Anvil,
        Bomb
    }

    public void BuyItem(ItemType itemType, int price)
    {
        if (UserData.diamond >= price)
        {
            gameUIButtonController.ItemMenuOff();
            tileChanger.usingItem = true;

            switch (itemType)
            {
                case ItemType.Pencil:
                    tileChanger.usingPencil = true;
                    itemImage.sprite = itemImages[0];
                    break;
                case ItemType.Rocket:
                    tileChanger.usingRocket = true;
                    itemImage.sprite = itemImages[1];
                    break;
                case ItemType.Anvil:
                    tileChanger.usingAnvil = true;
                    itemImage.sprite = itemImages[2];
                    break;
                case ItemType.Bomb:
                    tileChanger.usingBomb = true;
                    itemImage.sprite = itemImages[3];
                    break;
            }

            UserData.diamond -= price;
            commonUIController.UpdateGemText();
            UserData.instance.SaveData();
            itemImage.enabled = true;
            itemMenuTrigger.SetActive(false);
        }
        else
        {
            NoEnoughGem();
        }
    }

    public void BuyPencil(int price) => BuyItem(ItemType.Pencil, price);
    public void BuyRocket(int price) => BuyItem(ItemType.Rocket, price);
    public void BuyAnvil(int price) => BuyItem(ItemType.Anvil, price);
    public void BuyBomb(int price) => BuyItem(ItemType.Bomb, price);

    private void NoEnoughGem()
    {
        WarningScreen("No Enough GEM");
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
}