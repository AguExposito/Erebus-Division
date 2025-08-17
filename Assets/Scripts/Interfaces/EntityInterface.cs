using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EntityInterface : MonoBehaviour
{
    public float health;
    public float maxHealth;
    public Vector2 randomMaxHealth;
    public Vector2 randomAttackPower;
    public float baseHitChance;
    public float baseDodgeChance;
    public TextMeshProUGUI entityNameTMP;
    public string entityName;

    private GameObject encounterHUD;
    private List<Image> enemyHealthBar = new List<Image>();

    private void Awake()
    {
        FindEncounterHUD();
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
    public void FindEncounterHUD()
    {
        encounterHUD = GameObject.FindGameObjectWithTag("EncounterHUD");
        if (encounterHUD != null)
        {
            for (int i = 0; i < encounterHUD.transform.GetChild(0).childCount; i++)
            {
                if (encounterHUD.transform.GetChild(0).GetChild(i).GetComponent<Image>() != null)
                {
                    enemyHealthBar.Add(encounterHUD.transform.GetChild(0).GetChild(i).GetComponent<Image>());
                }
                else
                {
                    if (entityNameTMP == null)
                    {
                        entityNameTMP = encounterHUD.transform.GetChild(0).GetChild(i).GetComponent<TextMeshProUGUI>();
                    }
                }
            }
        }
    }
    public void OnRaycastEnter()
    {
        if (encounterHUD != null)
        {
            encounterHUD.SetActive(true);
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

}
