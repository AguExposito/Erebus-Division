using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerDD : MonoBehaviour
{
    public static GameManagerDD instance;

    [Header("HUD References")]
    public GameObject encounterHUD;
    public GameObject exitHUD;
    public List<Image> enemyHealthBar = new List<Image>();
    public Image healthVignette;
    public TextMeshProUGUI entityNameTMP;
    [Space]
    public Image playerHealth;
    public Image playerFear;
    public GameObject flashlight;

    [Space]
    public TextMeshProUGUI critChance;
    public TextMeshProUGUI hitChance;
    public TextMeshProUGUI bodyPart;
    public TextMeshProUGUI bodyPartState;
    public GameObject enemyInfo;

    [Space]
    public GameObject enemyDialogue;
    public TextMeshProUGUI dialogueNumber;

    [Space]
    public TextMeshProUGUI fleePercentage;


    private void Awake()
    {
        instance = this;
    }
}

