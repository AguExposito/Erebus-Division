using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class PlayerStats : EntityInterface
{
    [SerializeField] public float fear=0;
    [SerializeField] public float maxFear=100;
    [SerializeField] FPSController fPSController;
    [SerializeField] Image playerHealthBar;
    [SerializeField] Image playerFear;
    [SerializeField] AudioClip takeDamage;
    Color color;
    float alphaColor;
    public float fleeChance = 100;

    private void Start()
    {
        fPSController = gameObject.GetComponent<FPSController>();
        playerHealthBar =GameManagerDD.instance.playerHealth;
        playerFear = GameManagerDD.instance.playerFear;
        color = GameManagerDD.instance.healthVignette.color;
        audioSource = GetComponent<AudioSource>();
    }
    public override void TakeDamage(float damage)
    {
        audioSource.PlayOneShot(takeDamage);
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
        if (health <= 0)
        {
            Debug.Log($"{entityName} has been defeated!");
            // Aquí puedes agregar lógica adicional para cuando el jugador muere, como reiniciar el nivel o mostrar una pantalla de "Game Over".
        }
        CalculateFleePercentage();
        IncreaseFear(damage * 2); // Aumenta el miedo al recibir daño, ajusta el divisor según sea necesario
    }
    public float GetFearMult ()
    {
        fear = Mathf.Clamp(fear, 0f, 100f);
        float t = fear / 100f;
        float result = Mathf.Lerp(0.5f, 1.0f, 1f - t);

        return result;
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
            //this.isItsTurn = false;

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

    public void Heal(float healAmount)
    {
        health += healAmount;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        playerHealthBar.fillAmount = health / maxHealth;
        float alpha = 0.5f * (1f - (health / maxHealth));
        GameManagerDD.instance.healthVignette.color = new Color(color.r, color.g, color.b, alpha);
        Debug.Log($"{entityName} healed {healAmount}. Current health: {health}");
        CalculateFleePercentage();
    }

    public void IncreaseFear(float fearAmount)
    {
        fear += fearAmount;
        if (fear > maxFear)
        {
            fear = maxFear;
        }
        playerFear.fillAmount = fear / maxFear;
        Debug.Log($"{entityName} increased fear by {fearAmount}. Current fear: {fear}");
    }

    public void DecreaseFear(float fearAmount)
    {
        fear -= fearAmount;
        if (fear < 0)
        {
            fear = 0;
        }
        playerFear.fillAmount = fear / maxFear;
        Debug.Log($"{entityName} decreased fear by {fearAmount}. Current fear: {fear}");
    }

    public void UpdateBaseCritChance(float chance) 
    { 
        baseCritChance = chance;
    }
}
