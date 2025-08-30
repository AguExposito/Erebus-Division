using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


public class ManThing : EntityInterface
{
    public EntityInterface playerEntityInterface;
    public Vector2 hitChanceMinMax;
    public Vector2 critChanceMinMax;
    public Animator animator;
    public AudioClip takeDamage;

    private void Start()
    {
        playerEntityInterface = GameObject.FindGameObjectWithTag("Player").GetComponent<EntityInterface>();
        hitChance = (float)Math.Round(UnityEngine.Random.Range(hitChanceMinMax.x, hitChanceMinMax.y), 1);
        critChance = (float)Math.Round(UnityEngine.Random.Range(critChanceMinMax.x, critChanceMinMax.y), 1);
        animator = GetComponent<Animator>();
    }
    public void CheckTurn()
    {
        if (isItsTurn)
        {
            animator.Play("Attack");
        }
    }

    public void AnimationEvent_Attack()
    {
        Attack(playerEntityInterface);
    }

    public override void TakeDamage(float damage)
    {
        AudioSource.PlayClipAtPoint(takeDamage, transform.position);
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

            Death();
        }        
    }

    void Death()
    {
        TurnManager.instance.RemoveTurn(this);
        GetComponent<EnemyAI>().EndEncounter();
    }
}
