using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [Serialize]
    public List<ChestItem> items; // List of items contained in the chest

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GetComponent<Animator>().SetTrigger("Open");
            Debug.Log("Player entered the chest trigger area.");
            foreach (var item in items)
            {
                float roll = UnityEngine.Random.Range(0f, 100f);
                if (roll <= item.chanceToSpawn)
                {
                    int quantityToAdd = UnityEngine.Random.Range(item.quantityMinMax.x, item.quantityMinMax.y + 1);
                    InventoryManager.Instance.AddResource(item.itemName, quantityToAdd);
                    Debug.Log($"Added {quantityToAdd} of {item.itemName} to inventory.");
                }
                else
                {
                    Debug.Log($"{item.itemName} did not spawn (rolled {roll}, needed <= {item.chanceToSpawn}).");
                }
            }
        }
    }
}
[System.Serializable]
public class ChestItem 
{ 
    public string itemName;
    public Vector2Int quantityMinMax;
    [UnityEngine.Range(0,100)]
    public float chanceToSpawn;
}
