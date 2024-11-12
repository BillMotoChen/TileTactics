using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileChanger : MonoBehaviour
{
    private Camera mainCamera;

    private LevelLoader levelLoader;
    private GameUIController gameUIController;
    private AudioSource audioSource;
    public GameObject itemMenuTrigger;

    public Sprite[] tileSprites;

    [HideInInspector] public int remainSteps;
    [HideInInspector] public int maxSteps;
    private int activeAnimations;

    public Image itemImage;

    [HideInInspector] public bool canClick;

    [HideInInspector] public bool usingItem;
    [HideInInspector] public bool usingPencil;
    [HideInInspector] public bool usingBomb;
    [HideInInspector] public bool usingRocket;
    [HideInInspector] public bool usingAnvil;

    private void Start()
    {
        levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
        gameUIController = GameObject.Find("GameUIController").GetComponent<GameUIController>();
        maxSteps = (int)(levelLoader.levelData.steps * 1.5f) < 5 ? 5 : (int)(levelLoader.levelData.steps * 1.5f);
        remainSteps = maxSteps;
        gameUIController.UpdateStepText(remainSteps, maxSteps);
        
        audioSource = GetComponent<AudioSource>();
        mainCamera = Camera.main;
        canClick = true;
        SetItemBoolFalse();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!usingItem)
            {
                DetectClick(levelLoader.levelData.pattern, levelLoader.levelData.mapSize);
            }
            else
            {
                if (usingPencil)
                {
                    Pencil();
                }
                else if (usingBomb)
                {
                    Bomb(levelLoader.levelData.mapSize);
                }
                else if (usingRocket)
                {
                    Rocket(levelLoader.levelData.mapSize);
                }
                else if (usingAnvil)
                {
                    Anvil(levelLoader.levelData.mapSize);
                }
            }
        }
    }

    private void DetectClick(int pattern, int size)
    {
        Vector2 worldPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        if (hit.collider != null && canClick && remainSteps > 0)
        {
            canClick = false;
            remainSteps -= 1;
            gameUIController.UpdateStepText(remainSteps, maxSteps);
            if (PlayerPrefs.GetInt("SoundSetting") == 1) audioSource.Play();
            StartCoroutine("Clickable");
            GameObject clickedObject = hit.collider.gameObject;
            string[] position = clickedObject.name.Split('_');
            int x = int.Parse(position[0]);
            int y = int.Parse(position[1]);

            List<GameObject> targetGameObjects = new List<GameObject>();

            for (int i = x - 1; i <= x + 1; i++)
            {
                for (int j = y - 1; j <= y + 1; j++)
                {
                    if ((i == x || j == y) && (i >= 0 && i < size && j >= 0 && j < size))
                    {
                        string tileName = i + "_" + j;
                        GameObject neighborTile = GameObject.Find("TileMapSize" + size.ToString() + "/" + tileName);
                        if (neighborTile != null)
                        {
                            targetGameObjects.Add(neighborTile);
                        }
                    }
                }
            }

            activeAnimations = targetGameObjects.Count;

            foreach (GameObject target in targetGameObjects)
            {
                SpriteRenderer spriteRenderer = target.GetComponent<SpriteRenderer>();
                Animator animator = target.GetComponent<Animator>();

                if (spriteRenderer != null && tileSprites.Length > 0)
                {
                    Sprite currentSprite = spriteRenderer.sprite;
                    int currentIndex = System.Array.IndexOf(tileSprites, currentSprite);
                    int nextIndex = (currentIndex + 1) % pattern;

                    StartCoroutine(AnimatorOn(animator, spriteRenderer, nextIndex));
                }
            }
        }
    }

    private bool CheckClear()
    {
        string objectName = "TileMapSize" + levelLoader.levelData.mapSize.ToString();

        for (int i = 0; i < levelLoader.levelData.mapSize; i++)
        {
            for (int j = 0; j < levelLoader.levelData.mapSize; j++)
            {
                string tileName = i.ToString() + "_" + j.ToString();
                GameObject target = GameObject.Find(objectName + "/" + tileName);
                if (target != null)
                {
                    if (target.GetComponent<SpriteRenderer>().sprite != levelLoader.levelTarget.sprite)
                    {
                        return false;
                    }
                }
                else
                {
                    Debug.LogWarning("Tile not found: " + tileName);
                    return false;
                }
            }
        }
        return true;
    }

    IEnumerator AnimatorOn(Animator animator, SpriteRenderer spriteRenderer, int nextIndex)
    {
        animator.Rebind();
        animator.enabled = true;

        GameObject tempSpriteObj = new GameObject("TempSprite");
        SpriteRenderer tempSpriteRenderer = tempSpriteObj.AddComponent<SpriteRenderer>();
        if (levelLoader.levelData.mapSize == 4)
        {
            tempSpriteObj.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
        }
        if (levelLoader.levelData.mapSize == 5)
        {
            tempSpriteObj.transform.localScale = new Vector3(0.75f, 0.75f, 1f);
        }
        tempSpriteRenderer.sprite = tileSprites[nextIndex];
        tempSpriteRenderer.color = new Color(1, 1, 1, 0);
        tempSpriteRenderer.sortingOrder = spriteRenderer.sortingOrder - 1;
        tempSpriteRenderer.transform.position = spriteRenderer.transform.position;

        tempSpriteObj.transform.SetParent(spriteRenderer.transform);

        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float alpha = elapsed / duration;
            tempSpriteRenderer.color = new Color(1, 1, 1, alpha);
            spriteRenderer.color = new Color(1, 1, 1, 1 - alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }
        spriteRenderer.sprite = tileSprites[nextIndex];
        spriteRenderer.color = new Color(1, 1, 1, 1);
        animator.enabled = false;
        Destroy(tempSpriteObj);

        activeAnimations--;

        if (activeAnimations == 0)
        {
            if (CheckClear())
            {
                gameUIController.LevelClear();
            }
            else if (remainSteps <= 0)
            {
                gameUIController.GameOver();
            }
        }
    }

    IEnumerator Clickable()
    {
        yield return new WaitForSeconds(1f);
        canClick = true;
    }

    // items

    // change one tile
    private void Pencil()
    {
        Vector2 worldPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
        activeAnimations = 1;
        if (hit.collider != null && canClick)
        {
            if (PlayerPrefs.GetInt("SoundSetting") == 1) audioSource.Play();
            SetFalseAfterItem();
            usingPencil = false;
            StartCoroutine("Clickable");
            GameObject clickedObject = hit.collider.gameObject;
            SpriteRenderer spriteRenderer = clickedObject.GetComponent<SpriteRenderer>();
            Animator animator = clickedObject.GetComponent<Animator>();
            StartCoroutine(AnimatorOn(animator, spriteRenderer, int.Parse(levelLoader.levelData.targetColor)));
        }
    }

    // change row
    private void Rocket(int size)
    {
        Vector2 worldPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        if (hit.collider != null && canClick)
        {
            if (PlayerPrefs.GetInt("SoundSetting") == 1) audioSource.Play();
            SetFalseAfterItem();
            usingRocket = false;
            StartCoroutine("Clickable");
            GameObject clickedObject = hit.collider.gameObject;
            string[] position = clickedObject.name.Split('_');
            int x = int.Parse(position[0]);
            List<GameObject> targetGameObjects = new List<GameObject>();

            for (int i = 0; i < size; i++)
            {
                if (i >= 0 && i < size && x >= 0 && x < size)
                {
                    string tileName = x + "_" + i;
                    GameObject neighborTile = GameObject.Find("TileMapSize" + size.ToString() + "/" + tileName);
                    if (neighborTile != null)
                    {
                        targetGameObjects.Add(neighborTile);
                    }
                }
            }

            activeAnimations = targetGameObjects.Count;

            foreach (GameObject target in targetGameObjects)
            {
                SpriteRenderer spriteRenderer = target.GetComponent<SpriteRenderer>();
                Animator animator = target.GetComponent<Animator>();
                if (spriteRenderer != null && tileSprites.Length > 0)
                {
                    StartCoroutine(AnimatorOn(animator, spriteRenderer, int.Parse(levelLoader.levelData.targetColor)));
                }
            }
        }
    }

    // change column
    private void Anvil(int size)
    {
        Vector2 worldPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        if (hit.collider != null && canClick)
        {
            if (PlayerPrefs.GetInt("SoundSetting") == 1) audioSource.Play();
            SetFalseAfterItem();
            usingAnvil = false;
            StartCoroutine("Clickable");
            GameObject clickedObject = hit.collider.gameObject;
            string[] position = clickedObject.name.Split('_');
            int y = int.Parse(position[1]);

            List<GameObject> targetGameObjects = new List<GameObject>();

            for (int i = 0; i < size; i++)
            {
                if (i >= 0 && i < size && y >= 0 && y < size)
                {
                    string tileName = i + "_" + y;
                    GameObject neighborTile = GameObject.Find("TileMapSize" + size.ToString() + "/" + tileName);
                    if (neighborTile != null)
                    {
                        targetGameObjects.Add(neighborTile);
                    }
                }
            }

            activeAnimations = targetGameObjects.Count;

            foreach (GameObject target in targetGameObjects)
            {
                SpriteRenderer spriteRenderer = target.GetComponent<SpriteRenderer>();
                Animator animator = target.GetComponent<Animator>();
                if (spriteRenderer != null && tileSprites.Length > 0)
                {
                    StartCoroutine(AnimatorOn(animator, spriteRenderer, int.Parse(levelLoader.levelData.targetColor)));
                }
            }
        }
    }

    // change 3x3 
    private void Bomb(int size)
    {
        Vector2 worldPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        if (hit.collider != null && canClick)
        {
            if (PlayerPrefs.GetInt("SoundSetting") == 1) audioSource.Play();
            SetFalseAfterItem();
            usingBomb = false;
            StartCoroutine("Clickable");
            GameObject clickedObject = hit.collider.gameObject;
            string[] position = clickedObject.name.Split('_');
            int x = int.Parse(position[0]);
            int y = int.Parse(position[1]);

            List<GameObject> targetGameObjects = new List<GameObject>();

            for (int i = x - 1; i <= x + 1; i++)
            {
                for (int j = y - 1; j <= y + 1; j++)
                {
                    if (i >= 0 && i < size && j >= 0 && j < size)
                    {
                        string tileName = i + "_" + j;
                        GameObject neighborTile = GameObject.Find("TileMapSize" + size.ToString() + "/" + tileName);
                        if (neighborTile != null)
                        {
                            targetGameObjects.Add(neighborTile);
                        }
                    }
                }
            }

            activeAnimations = targetGameObjects.Count;

            foreach (GameObject target in targetGameObjects)
            {
                SpriteRenderer spriteRenderer = target.GetComponent<SpriteRenderer>();
                Animator animator = target.GetComponent<Animator>();
                if (spriteRenderer != null && tileSprites.Length > 0)
                {
                    StartCoroutine(AnimatorOn(animator, spriteRenderer, int.Parse(levelLoader.levelData.targetColor)));
                }
            }
        }
    }

    private void SetItemBoolFalse()
    {
        usingItem = false;
        usingPencil = false;
        usingBomb = false;
        usingRocket = false;
        usingAnvil = false;
        itemImage.enabled = false;
    }

    private void SetFalseAfterItem()
    {
        canClick = false;
        usingItem = false;
        itemImage.enabled = false;
        itemMenuTrigger.SetActive(true);
    }
}
