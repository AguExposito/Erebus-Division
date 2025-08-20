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

    [Space]
    public float critChance;
    public float hitChance;

    [Space]
    public TextMeshProUGUI entityNameTMP;
    public string entityName;

    public GameObject encounterHUD;
    public List<Image> enemyHealthBar = new List<Image>();

    private void Start()
    {
        encounterHUD = GameManager.instance.encounterHUD;
        enemyHealthBar = GameManager.instance.enemyHealthBar;
        entityNameTMP = GameManager.instance.entityNameTMP;
    }
    private void OnEnable()
    {
        maxHealth = Random.Range(randomMaxHealth.x, randomMaxHealth.y);
        health = maxHealth;
    }
    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log($"{entityName} took {damage} damage. Remaining health: {health}");
        for (int i = 0; i < enemyHealthBar.Count; i++)
        {
            enemyHealthBar[i].fillAmount = health / maxHealth;
        }
        if (health < 0)
        {
            health = 0;
            for (int i = 0; i < enemyHealthBar.Count; i++)
            {
                enemyHealthBar[i].fillAmount = 0;
            }
            //Death logic can go here
        }
    }
    public void Attack(EntityInterface target)
    {
        float randHitChance = Random.Range(0f, 100f);
        if (randHitChance <= hitChance)
        {
            float dodgeChance = Random.Range(0f, 100f);
            if (dodgeChance > target.baseDodgeChance)
            {
                float attackPower = Random.Range(randomAttackPower.x, randomAttackPower.y);
                target.TakeDamage(attackPower);
            }
            else
            {
                Debug.LogWarning("Attack Dodged!");
            }
        }
        else
        {
            Debug.LogWarning("Attack Missed!");
        }
    }


    public void OnRaycastEnter()
    {
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
