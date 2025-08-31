using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverController : MonoBehaviour
{
    public List<string> tips = new List<string>();
    public TextMeshProUGUI tipText;
    public GameObject gameOverContent;
    public GameObject gameOverBG;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public void GameOver() 
    {
        gameOverBG.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        Invoke("EnableContent", 2);
        Invoke("PlayGameOverSFX", 1.5f);
        int random = Random.Range(0, tips.Count);
        tipText.text = tips[random];
        FindFirstObjectByType<FPSController>().gameObject.SetActive(false);
        foreach (var enemy in EnemyAI.enemies)
        {
            enemy.agent.speed = 0;
        }
        Debug.Log("GAME OVER");
        MusicManager.Instance.audioSource.Stop();
        MusicManager.Instance.hearthAudioSource.Stop();
    }
    public void EnableContent() {
        gameOverContent.SetActive(true);
    }
    public void PlayGameOverSFX() { 
        GetComponent<AudioSource>().Play();
    }
}
