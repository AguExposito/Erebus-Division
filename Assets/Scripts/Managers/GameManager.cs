using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("HUD References")]
    public GameObject encounterHUD;
    public List<Image> enemyHealthBar = new List<Image>();
    public TextMeshProUGUI entityNameTMP;
    [Space]
    public Image playerHealth;
    public Image playerFear;

    [Space]
    public TextMeshProUGUI critChance;
    public TextMeshProUGUI hitChance;
    public TextMeshProUGUI bodyPart;
    public TextMeshProUGUI bodyPartState;
    public GameObject enemyInfo;


    private void Awake()
    {
        instance = this;
    }
}

