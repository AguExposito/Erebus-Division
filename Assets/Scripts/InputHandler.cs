using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private Camera _mainCamera;
    private bool menuOpen = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    public void OnMenuClose()
    {
        menuOpen = !menuOpen;
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        
                var rayhit = Physics2D.GetRayIntersection(_mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue()));

                if (!rayhit.collider) return;
        if (menuOpen == false)
        {
            rayhit.collider.gameObject.GetComponent<IClickable>().Clicked();
            menuOpen = true;
        }
               
    }
}
