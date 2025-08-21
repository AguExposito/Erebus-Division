using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : EntityInterface
{
    [SerializeField] Image playerHealthBar;
    [SerializeField] Image playerFear;
    Color color;
    float alphaColor;

    private void Start()
    {
        playerHealthBar=GameManager.instance.playerHealth;
        playerFear = GameManager.instance.playerFear;
        color = GameManager.instance.healthVignette.color;
    }
    public override void TakeDamage(float damage)
    {          
        health -= damage;
        Debug.Log($"{entityName} took {damage} damage. Remaining health: {health}");
        playerHealthBar.fillAmount = health / maxHealth;

        // El alpha será máximo 0.5 cuando la vida sea 0, y 0 cuando la vida sea máxima
        float alpha = 0.5f * (1f - (health / maxHealth));
        GameManager.instance.healthVignette.color = new Color(color.r, color.g, color.b, alpha);

        if (health < 0)
        {
            health = 0;
            playerHealthBar.fillAmount = 0;
        }    
    }
}
