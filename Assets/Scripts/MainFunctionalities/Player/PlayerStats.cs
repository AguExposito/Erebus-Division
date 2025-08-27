using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : EntityInterface
{
    [SerializeField] FPSController fPSController;
    [SerializeField] Image playerHealthBar;
    [SerializeField] Image playerFear;
    Color color;
    float alphaColor;
    public float fleeChance = 100;

    private void Start()
    {
        fPSController = gameObject.GetComponent<FPSController>();
        playerHealthBar =GameManagerDD.instance.playerHealth;
        playerFear = GameManagerDD.instance.playerFear;
        color = GameManagerDD.instance.healthVignette.color;
    }
    public override void TakeDamage(float damage)
    {          
        health -= damage;
        Debug.Log($"{entityName} took {damage} damage. Remaining health: {health}");
        playerHealthBar.fillAmount = health / maxHealth;

        // El alpha será máximo 0.5 cuando la vida sea 0, y 0 cuando la vida sea máxima
        float alpha = 0.5f * (1f - (health / maxHealth));
        GameManagerDD.instance.healthVignette.color = new Color(color.r, color.g, color.b, alpha);

        if (health < 0)
        {
            health = 0;
            playerHealthBar.fillAmount = 0;
        }
        CalculateFleePercentage();
    }

    public void CalculateFleePercentage() {
        float baseFleeChance = (health / maxHealth) * 100;
        float threathLevel = TurnManager.instance.encounterThreathLevel;
        fleeChance = (float)Math.Round(fleeChance > 0 ? baseFleeChance - threathLevel : 0,1);
        GameManagerDD.instance.fleePercentage.text = $"{fleeChance}%";
    }
    public void Flee()
    {
        float fleeRandom = UnityEngine.Random.Range(0, 100f);
        CalculateFleePercentage();
        if (fleeRandom < fleeChance)
        {
            // 1. Eliminar todos los enemigos del turnero
            List<EntityInterface> toRemove = new List<EntityInterface>(TurnManager.instance.entitiesTurns);

            foreach (EntityInterface entity in toRemove)
            {
                // Puedes hacer que los enemigos reaccionen si querés
                if (entity.TryGetComponent<EnemyAI>(out EnemyAI enemy))
                {
                    enemy.OnPlayerFled(); // O destruye al enemigo, etc.
                    TurnManager.instance.RemoveTurn(entity);
                }               
            }

            // 2. Eliminar al jugador del turnero
            //TurnManager.instance.RemoveTurn(this);
            this.isItsTurn = false;

            //Restaurar control al jugador
            fPSController.GiveBackControlToPlayer();

            Debug.Log("Player fled the battle!");
        }
        else 
        {
            TurnManager.instance.EndTurn(this);
            Debug.LogWarning("Flee Failed!"); 
        }
    }
}
