using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WorkshopRecipeTab : MonoBehaviour
{
    Image itemSprite;
    TextMeshProUGUI resource1;
    TextMeshProUGUI resource2;
    TextMeshProUGUI resource3;
    Button craftButton;
    GameManagerMS gameManager;
    [SerializeField] CraftingRecipe recipe;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        itemSprite = gameObject.transform.GetChild(0).GetChild(0).GetComponent<Image>();
        if (recipe != null)
        {
            gameManager = GameObject.FindAnyObjectByType<GameManagerMS>();
        }
        

        resource1 = gameObject.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
        resource2 = gameObject.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>();
        resource3 = gameObject.transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>();
        craftButton = gameObject.transform.GetChild(4).GetComponent<Button>();

        if (recipe != null)
        {
            craftButton.onClick.AddListener(Craft);
            resource1.text = "" + recipe.costR1;
            resource2.text = "" + recipe.costR2;
            resource3.text = "" + recipe.costR3;
            itemSprite.sprite = recipe.objSprite;
        }
    }

    public void Craft()
    {
        if (gameManager.Resource1 >= recipe.costR1 && gameManager.Resource2 >= recipe.costR2 && gameManager.Resource3 >= recipe.costR3) 
        {
            gameManager.Resource1 -= recipe.costR1;
            gameManager.Resource2 -= recipe.costR2;
            gameManager.Resource3 -= recipe.costR3;
            gameManager.updateDisplays();
        }
        else { Debug.Log("Lack Resources"); }
    }
}
