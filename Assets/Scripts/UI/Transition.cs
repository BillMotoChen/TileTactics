using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class Transition : MonoBehaviour
{

    public static Transition instance;

    public GameObject transitionBackground;

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

    public void TransitionToNextScene(string sceneName)
    {
        StartCoroutine(playTransitionAnimation(sceneName));
    }

    IEnumerator playTransitionAnimation(string sceneName)
    {
        transitionBackground.SetActive(true);
        yield return new WaitForSecondsRealtime(0.5f);
        Animator animator = transitionBackground.GetComponent<Animator>();
        SceneManager.LoadScene(sceneName);
        animator.Play("TransitionOut");
        yield return new WaitForSecondsRealtime(0.5f);
        transitionBackground.SetActive(false);
    }

}
