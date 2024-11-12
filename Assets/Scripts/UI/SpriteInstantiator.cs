using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SpriteInstantiator : MonoBehaviour
{
    public GameObject diamondPrefab;
    public GameObject energyPrefab;
    public Transform diamondTarget;
    public Transform energyTarget;
    public Transform parent;
    public Canvas canvas;
    public float spriteSize = 50f;      
    public float moveSpeed = 200f;
    public float spawnInterval = 0.1f;

    
    public void SpawnDiamond(int numberOfSprites)
    {
        Vector3 clickPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
            canvas.worldCamera,
            out Vector2 localPoint);
        clickPosition = canvas.transform.TransformPoint(localPoint);
        StartCoroutine(InstantiateSprites(diamondPrefab, clickPosition, numberOfSprites, diamondTarget));
    }

    public void SpawnEnergy(int numberOfSprites)
    {
        Vector3 clickPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
            canvas.worldCamera,
            out Vector2 localPoint);
        clickPosition = canvas.transform.TransformPoint(localPoint);
        StartCoroutine(InstantiateSprites(energyPrefab, clickPosition, numberOfSprites, energyTarget));
    }

    private IEnumerator InstantiateSprites(GameObject prefab, Vector3 startPosition, int numberOfSprites, Transform target)
    {
        Time.timeScale = 1f;
        for (int i = 0; i < numberOfSprites; i++)
        {
            
            GameObject sprite = Instantiate(prefab, parent);

            
            RectTransform rectTransform = sprite.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(spriteSize, spriteSize);
            rectTransform.position = startPosition;

            
            StartCoroutine(MoveAndDestroy(sprite, target));
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private IEnumerator MoveAndDestroy(GameObject sprite, Transform target)
    {
        float arrivalThreshold = 0.1f;
        float maxLifetime = 0.5f;
        float timer = 0f;

        while (sprite != null)
        {
            float distance = Vector3.Distance(sprite.transform.position, target.position);
            if (distance <= arrivalThreshold)
            {
                Destroy(sprite);
                yield break;
            }

            Vector3 direction = (target.position - sprite.transform.position).normalized;
            sprite.transform.position += direction * moveSpeed * Time.deltaTime;

            timer += Time.deltaTime;
            if (timer >= maxLifetime)
            {
                Destroy(sprite);
                yield break;
            }

            yield return null;
        }
    }
}