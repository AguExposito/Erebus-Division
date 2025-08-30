using UnityEngine;
using TMPro;

public class GameManagerMS : MonoBehaviour
{
    public int Resource1;
    public int Resource2;
    public int Resource3;
    [SerializeField] TextMeshProUGUI r1Display;
    [SerializeField] TextMeshProUGUI r2Display;
    [SerializeField] TextMeshProUGUI r3Display;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        r1Display.text = "" + Resource1;
        r2Display.text = "" + Resource2;
        r3Display.text = "" + Resource3;
    }

    public void updateDisplays()
    {
        r1Display.text = "" + Resource1;
        r2Display.text = "" + Resource2;
        r3Display.text = "" + Resource3;
    }

}
