using UnityEngine;

public class Storage : MonoBehaviour, IClickable
{
    [SerializeField] GameObject roomMenu;
    bool menuOpen = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        roomMenu.SetActive(menuOpen);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Clicked()
    {
        menuOpen = !menuOpen;
        roomMenu.SetActive(menuOpen);
        gameObject.GetComponent<BoxCollider2D>().enabled = !menuOpen;
    }
}
