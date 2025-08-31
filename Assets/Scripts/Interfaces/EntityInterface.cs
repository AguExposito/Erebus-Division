using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static EnemyPart;

public abstract class EntityInterface : MonoBehaviour
{
    public float health;
    public Vector2 randomMaxHealth;
    public float maxHealth;
    public Vector2 randomAttackPower;
    public float baseAttackPower;
    public float baseCritChance;
    public float baseHitChance;
    public float baseDodgeChance;
    public float threathLevel;
    public AudioSource audioSource;

    [Space]
    public float critChance;
    public float hitChance;
    public EnemyPart targetEnemyPart;

    [Space]
    public TextMeshProUGUI entityNameTMP;
    public string entityName;

    public GameObject encounterHUD;
    public List<Image> enemyHealthBar = new List<Image>();

    [Space]
    public bool isItsTurn = false;

    [Space]
    public AudioClip failAttack;

    private void Start()
    {
        encounterHUD = GameManagerDD.instance.encounterHUD;
        enemyHealthBar = GameManagerDD.instance.enemyHealthBar;
        entityNameTMP = GameManagerDD.instance.entityNameTMP;
        if (audioSource == null) { audioSource = GetComponent<AudioSource>(); }
    }
    private void OnEnable()
    {
        maxHealth = Random.Range(randomMaxHealth.x, randomMaxHealth.y);
        health = maxHealth;
    }
    public abstract void TakeDamage(float damage);
    public void Attack(EntityInterface target)
    {
        float randHitChance = Random.Range(0f, 100f);
        float attackPower = Random.Range(randomAttackPower.x, randomAttackPower.y);
        if (randHitChance <= hitChance)
        {
            float dodgeChance = Random.Range(0f, 100f);
            if (dodgeChance > target.baseDodgeChance)
            {

                if (targetEnemyPart != null) { 
                    baseAttackPower = attackPower * targetEnemyPart.damageMultiplier;
                    targetEnemyPart = null;
                }

                float critChanceRoll = Random.Range(0f, 100f);
                if (critChanceRoll <= critChance)
                {
                    Debug.Log("Critical Hit!");
                    baseAttackPower *= Random.Range(1.5f,2);
                }

                target.TakeDamage(attackPower);
            }
            else
            {
                Debug.LogWarning("Attack Dodged!");
                audioSource = GetComponent<AudioSource>();
                audioSource.PlayOneShot(failAttack);
            }
        }
        else
        {
            Debug.LogWarning("Attack Missed!");
            audioSource = GetComponent<AudioSource>();
            audioSource.PlayOneShot(failAttack);
        }
        TurnManager.instance.EndTurn(this);

        if (target is PlayerStats ps) { 
            ps.IncreaseFear(attackPower); // Aumenta el miedo al recibir daño, ajusta el divisor según sea necesario
        }
    }


    public void OnRaycastEnter()
    {
        if (encounterHUD==null || enemyHealthBar==null || entityNameTMP==null) {
            encounterHUD = GameManagerDD.instance.encounterHUD;
            enemyHealthBar = GameManagerDD.instance.enemyHealthBar;
            entityNameTMP = GameManagerDD.instance.entityNameTMP;
        }

        for (int i = 0; i < enemyHealthBar.Count; i++)
        {
            enemyHealthBar[i].fillAmount = health / maxHealth;
        }
        if (entityNameTMP != null)
        {
            entityNameTMP.text = entityName;
        }        
    }


}
