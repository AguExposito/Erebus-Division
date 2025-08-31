using UnityEngine;
using System.Collections.Generic;

public class BiscuitScroller : MonoBehaviour
{
    public RectTransform container;
    public List<RectTransform> items = new List<RectTransform>();
    public float spacing = -30f;
    public bool centerHorizontally = true;
    public float lerpSpeed = 5f;
    public Animator animator;
    public bool isSatchelOpen = false;

    private int currentIndex = 0;
    private float currentScrollValue = 0f;
    private float targetScrollValue = 0f;
    private FPSController player;


    void Start()
    {
        currentScrollValue = IndexToT(currentIndex);
        targetScrollValue = currentScrollValue;
        UpdateLayout(currentScrollValue);
        player = FindFirstObjectByType<FPSController>();
    }

    void Update()
    {
        if (items.Count == 0) return;

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Q))
            Scroll(-1);
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.E))
            Scroll(1);

        currentScrollValue = Mathf.MoveTowards(currentScrollValue, targetScrollValue, Time.deltaTime * lerpSpeed);

        UpdateLayout(currentScrollValue);
    }

    void Scroll(int direction)
    {
        if (items.Count == 0) return;

        currentIndex = (currentIndex + direction + items.Count) % items.Count;
        targetScrollValue += (float)direction / items.Count;
    }


    float IndexToT(int index)
    {
        if (items.Count == 0) return 0f;
        return (float)index / items.Count;
    }

    void UpdateLayout(float t)
    {
        if (items.Count == 0) return;

        float totalHeight = items.Count * spacing;
        float offset = (t % 1f) * totalHeight;


        for (int i = 0; i < items.Count; i++)
        {
            float y = ((i * spacing - offset + totalHeight) % totalHeight) - totalHeight / 2f;

            Vector2 newPos = new Vector2(centerHorizontally ? 0f : items[i].anchoredPosition.x, y);
            items[i].anchoredPosition = newPos;

            float distance = Mathf.Abs(y);
            float scale = Mathf.Clamp01(1f - distance / 150f);
            items[i].localScale = Vector3.one * (0.7f + 0.3f * scale);
        }
    }

    public void AddItem(RectTransform newItem)
    {
        if (newItem == null) return;

        newItem.SetParent(container, false);
        items.Add(newItem);

        // Reajustar índice si era el único
        if (items.Count == 1)
        {
            currentIndex = 0;
            currentScrollValue = 0f;
            targetScrollValue = 0f;
        }

        targetScrollValue = IndexToT(currentIndex);
    }

    public void RemoveItem(RectTransform itemToRemove)
    {
        if (itemToRemove == null || !items.Contains(itemToRemove)) return;

        items.Remove(itemToRemove);
        Destroy(itemToRemove.gameObject); // opcional

        // Asegurar que el índice actual siga válido
        currentIndex = Mathf.Clamp(currentIndex, 0, items.Count - 1);
        targetScrollValue = IndexToT(currentIndex);
    }
    public void RemoveSelectedItem()
    {
        if (items.Count == 0) return;

        RectTransform selectedItem = items[currentIndex];
        RemoveItem(selectedItem);
        string resourceName = selectedItem.gameObject.name;
        InventoryManager.Instance.UseResource(resourceName);
    }

    public void ClearAllItems()
    {
        foreach (var item in items)
        {
            if (item != null)
                Destroy(item.gameObject);
        }

        items.Clear();
        currentIndex = 0;
        currentScrollValue = 0f;
        targetScrollValue = 0f;
    }


    public void ToggleSatchel() 
    {
        if (animator == null) 
        {
            animator= GetComponent<Animator>();
        }
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0); // capa 0
        bool isPlaying = stateInfo.normalizedTime < 1f; // normalizedTime va de 0 a 1 en un clip sin loop
        //bool isFinished = !isPlaying && stateInfo.IsName("NombreDeTuEstado");
        if (!isPlaying)
        {
            animator.SetBool("Show", !animator.GetBool("Show"));
            isSatchelOpen = animator.GetBool("Show");
        }
    }

    public void BiscuitHeal(float percentage)
    {
        player.playerStats.Heal(player.playerStats.maxHealth*percentage);
    }
    public void BiscuitFear(float percentage)
    {
        player.playerStats.DecreaseFear(player.playerStats.maxFear * percentage);

    }
}
