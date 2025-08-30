using UnityEngine;

[CreateAssetMenu(fileName = "CraftingRecipe", menuName = "Scriptable Objects/CraftingRecipe")]
public class CraftingRecipe : ScriptableObject
{
    public int costR1;
    public int costR2;
    public int costR3;
    public Sprite objSprite;
    public ScriptableObject result;
}
