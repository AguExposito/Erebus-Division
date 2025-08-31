using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    float lookSpeed;
    FPSController player;
    private void Start()
    {
        gameObject.SetActive(false);
        player = FindAnyObjectByType<FPSController>();
    }
    private void OnEnable()
    {
        //Cursor Lock
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }
    private void OnDisable()
    {
        //Cursor Lock
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void PauseMenu() {
        player.AlternatePauseMenu();
    }
    public void ShopMenu() {
        player.CloseShopMenu();
    }

    public void ShopRedBiscuit() {
        InventoryManager.Instance.resources.TryGetValue("RedBiscuit", out int val);
        if (val <= 0) return;
        player.playerStats.baseHitChance += 1;
        InventoryManager.Instance.RemoveResource("RedBiscuit", 1);
    }
    public void ShopBlueBiscuit() {
        InventoryManager.Instance.resources.TryGetValue("BlueBiscuit", out int val);
        if (val <= 0) return;
        player.playerStats.baseDodgeChance += 1;
        InventoryManager.Instance.RemoveResource("BlueBiscuit", 1);
    }
    public void ShopPurpleBiscuit() {
        InventoryManager.Instance.resources.TryGetValue("PurpleBiscuit", out int val);
        if (val <= 0) return;
        player.playerStats.baseCritChance += 2;
        InventoryManager.Instance.RemoveResource("PurpleBiscuit", 1);
    }
    public void ShopGoldenBiscuit()
    {
        InventoryManager.Instance.resources.TryGetValue("GoldenBiscuit", out int val);
        if (val <= 0) return;
        player.playerStats.baseCritChance += 5;
        player.playerStats.baseHitChance += 5;
        player.playerStats.baseDodgeChance += 5;
        player.playerStats.baseAttackPower += 5;
        InventoryManager.Instance.RemoveResource("GoldenBiscuit", 1);
    }
}
