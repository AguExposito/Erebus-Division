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
}
