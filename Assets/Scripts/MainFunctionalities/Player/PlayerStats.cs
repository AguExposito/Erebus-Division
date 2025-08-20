using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : EntityInterface
{
    [SerializeField] Image playerHealthBar;
    [SerializeField] Image playerFear;

    private void Start()
    {
        playerHealthBar=GameManager.instance.playerHealth;
        playerFear = GameManager.instance.playerFear;
    }
    public override void TakeDamage(float damage)
    {          
        health -= damage;
        Debug.Log($"{entityName} took {damage} damage. Remaining health: {health}");
        playerHealthBar.fillAmount = health / maxHealth;

        if (health < 0)
        {
            health = 0;
            playerHealthBar.fillAmount = 0;
        }    
    }
}
