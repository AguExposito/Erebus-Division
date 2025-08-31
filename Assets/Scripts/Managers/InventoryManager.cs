using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
public class ResourceSprite
{
    public string resourceName;
    public Sprite icon;
}

[Serializable]
public class ResourceAction
{
    public string resourceName;
    public UnityEvent action;
}

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public BiscuitScroller scroller;
    public GameObject resourceItemPrefab;

    public List<ResourceSprite> resourceIconsList;
    public List<ResourceAction> resourceActionsList;

    private Dictionary<string, Sprite> resourceIcons = new Dictionary<string, Sprite>(); 
    private Dictionary<string, UnityEvent> resourceActions = new Dictionary<string, UnityEvent>();

    public Dictionary<string, int> resources = new Dictionary<string, int>();

    [Space]
    public float baseAttackPower = 10;
    public float baseCritChance = 10;
    public float baseHitChance = 85;
    public float baseDodgeChance = 15;
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
            foreach (var pair in resourceActionsList)
            {
                if (!resourceActions.ContainsKey(pair.resourceName))
                    resourceActions[pair.resourceName] = pair.action;
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
                newItem.name = resourceName;
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

    public void UseResource(string resourceName)
    {
        // Check if the resource has an associated action and invoke it
        if (resourceActions.ContainsKey(resourceName) && resourceActions[resourceName] != null)
        {
            resourceActions[resourceName].Invoke();
        }
        else
        {
            Debug.LogWarning($"No action defined for {resourceName}.");
        }

        // Remove the resource after its action is triggered
        RemoveResource(resourceName, 1);
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

    public void UpdateReferences() 
    { 
        scroller = FindFirstObjectByType<BiscuitScroller>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateReferences();
        RefreshResourceDisplay();
    }

    public void GoldenBiscuit() 
    {
        scroller.BiscuitHeal(1);
        scroller.BiscuitFear(1);
    }
    public void RedBiscuit() 
    { 
        scroller.BiscuitHeal(0.5f);

    }
    public void BlueBiscuit() 
    { 
        scroller.BiscuitFear(0.5f);

    }
    public void PurpleBiscuit() 
    { 
        scroller.BiscuitHeal(0.25f);
        scroller.BiscuitFear(0.25f);

    }

}

