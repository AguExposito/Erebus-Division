using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public bool isRedy = false;
    void Start()
    {
        StartCoroutine(LoadSceneAsync());

    }

    IEnumerator LoadSceneAsync() { 
        AsyncOperation operation = SceneManager.LoadSceneAsync(1);
        operation.allowSceneActivation = false;
        StartCoroutine(WaitForLogo());  

        while (!operation.isDone)
        {
            if (isRedy == true)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }

    }

    IEnumerator WaitForLogo()
    {
        AnimatorClipInfo[] clipInfo = FindFirstObjectByType<Animator>().GetCurrentAnimatorClipInfo(0);
        yield return new WaitForSeconds(clipInfo[0].clip.length);
        isRedy = true;
    }

}

