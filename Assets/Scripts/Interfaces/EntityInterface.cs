using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static EnemyPart;

public abstract class EntityInterface : MonoBehaviour
{
    public float health;
    public float maxHealth;
    public Vector2 randomMaxHealth;
    public Vector2 randomAttackPower;
    public float baseHitChance;
    public float baseDodgeChance;
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
        if (health < 0)
        {
            health = 0;
            //Death logic can go here
        }
    }
    public void Attack(EntityInterface target)
    {
        float hitChance = Random.Range(0f, 1f);
        if (hitChance <= baseHitChance)
        {
            float dodgeChance = Random.Range(0f, 1f);
            if (dodgeChance > target.baseDodgeChance)
            {
                float attackPower = Random.Range(randomAttackPower.x, randomAttackPower.y);
                target.TakeDamage(attackPower);
            }
            else
            {
                // Target dodged the attack
            }
        }
        else
        {
            // Attack missed
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
