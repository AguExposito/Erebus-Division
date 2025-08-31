using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public UnityEvent unityEvent;
    public bool isloading = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex==1) 
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //loads scene 1
    public void Play()
    {
        unityEvent.Invoke();
    }

    public void LoadScene(int sceneIndex)
    {
        if (!isloading)
        {
            isloading = true;
            StartCoroutine(LoadSceneAsync(sceneIndex));
        }
    }
    IEnumerator LoadSceneAsync(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            operation.allowSceneActivation = true;   

            yield return null;
        }

    }
    public void GoBackToMainMenu()
    {
        SceneManager.LoadSceneAsync(1);
    }

    //self explanatory
    public void Quit()
    {
        Application.Quit();
    }

    public void TitleScreen(float time) {
        Invoke("GoBackToTitleScreen", time);
    }
    public void GoBackToTitleScreen()
    {
        SceneManager.LoadSceneAsync(0);
        ElevatorController.count = 0;
        EnemyAI.attackers = 0;
        EnemyAI.enemies.Clear();
        Destroy(InventoryManager.Instance);
        Destroy(EntityLogManager.Instance);
    }
}
