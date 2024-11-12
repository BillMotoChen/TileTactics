using UnityEngine;
using TMPro;

public class CommonUIController : MonoBehaviour
{
    public TMP_Text gemText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateGemText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateGemText()
    {
        gemText.text = UserData.diamond.ToString();
    }
}
