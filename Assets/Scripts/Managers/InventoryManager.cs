using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ResourceSprite
{
    public string resourceName;
    public Sprite icon;
}

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public BiscuitScroller scroller;
    public GameObject resourceItemPrefab;

    public List<ResourceSprite> resourceIconsList;
    private Dictionary<string, Sprite> resourceIcons = new Dictionary<string, Sprite>();

    public Dictionary<string, int> resources = new Dictionary<string, int>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);


            foreach (var pair in resourceIconsList)
            {
                if (!resourceIcons.ContainsKey(pair.resourceName))
                    resourceIcons[pair.resourceName] = pair.icon;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        RefreshResourceDisplay();
    }
    public void RefreshResourceDisplay()
    {
        scroller.ClearAllItems();

        foreach (var kvp in resources)
        {
            string resourceName = kvp.Key;
            int quantity = kvp.Value;

            for (int i = 0; i < quantity; i++)
            {
                GameObject newItem = Instantiate(resourceItemPrefab);
                Image image = newItem.GetComponentInChildren<Image>();

                if (image != null && resourceIcons.ContainsKey(resourceName))
                {
                    image.sprite = resourceIcons[resourceName];
                }
                else
                {
                    Debug.LogWarning($"No icon assigned for {resourceName}");
                }

                RectTransform rect = newItem.GetComponent<RectTransform>();
                if (rect != null)
                    scroller.AddItem(rect);
                else
                    Debug.LogError("Prefab is missing RectTransform.");
            }
        }
    }


    public void AddResource(string resourceName, int amount)
    {
        if (resources.ContainsKey(resourceName))
        {
            resources[resourceName] += amount;
        }
        else
        {
            resources[resourceName] = amount;
        }

        RefreshResourceDisplay();
    }

    public void RemoveResource(string resourceName, int amount)
    {
        if (!resources.ContainsKey(resourceName))
            return;

        resources[resourceName] -= amount;

        if (resources[resourceName] <= 0)
            resources.Remove(resourceName);

        RefreshResourceDisplay();
    }


}
