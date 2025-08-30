using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Elevator : MonoBehaviour, IClickable
{
    [SerializeField] GameObject roomMenu;
    [SerializeField] GameObject[] BarrackSlot;
    [SerializeField] GameObject agentStats;
    [SerializeField] GameObject DeployedAgent;
    [SerializeField] Button sendButton;
    TextMeshProUGUI nameAg;
    TextMeshProUGUI health;
    TextMeshProUGUI dmg;
    TextMeshProUGUI dodge;
    TextMeshProUGUI stressMax;
    int currentAgent = 0;


    bool menuOpen = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nameAg = agentStats.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        health = agentStats.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        dmg = agentStats.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        dodge = agentStats.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        stressMax = agentStats.transform.GetChild(4).GetComponent<TextMeshProUGUI>();
        sendButton.onClick.AddListener(Deploy);
        roomMenu.SetActive(menuOpen);

        if (BarrackSlot[0].transform.childCount != 0)
        {
            ChosenAgent(currentAgent);
        }
        else if (BarrackSlot[1].transform.childCount != 0)
        {
            ChosenAgent(currentAgent);
        }
    }

    public void ChosenAgent(int chosenAgent)
    {
        currentAgent = Mathf.Clamp(currentAgent + chosenAgent, 0, BarrackSlot.Length - 1);
        Debug.Log(currentAgent);
        if (BarrackSlot[currentAgent].transform.childCount != 0)
        {
            nameAg.text = " " + BarrackSlot[currentAgent].transform.GetChild(0).GetComponent<Agent>().name;
            health.text = "Health: " + BarrackSlot[currentAgent].transform.GetChild(0).GetComponent<Agent>().Health;
            dmg.text = "DMG: " + BarrackSlot[currentAgent].transform.GetChild(0).GetComponent<Agent>().Dmg;
            dodge.text = "Dodge: " + BarrackSlot[currentAgent].transform.GetChild(0).GetComponent<Agent>().Dodge;
            stressMax.text = "StressMax: " + BarrackSlot[currentAgent].transform.GetChild(0).GetComponent<Agent>().StressMax;
        }
    }

    void Deploy()
    {
        AgentOnField agentOnField = DeployedAgent.GetComponent<DeployedAgent>().agentOnField;
        agentOnField.health = BarrackSlot[currentAgent].transform.GetChild(0).GetComponent<Agent>().Health;
        agentOnField.dmg = BarrackSlot[currentAgent].transform.GetChild(0).GetComponent<Agent>().Dmg;
        agentOnField.dodge = BarrackSlot[currentAgent].transform.GetChild(0).GetComponent<Agent>().Dodge;
        agentOnField.stressMax = BarrackSlot[currentAgent].transform.GetChild(0).GetComponent<Agent>().StressMax;
    }

    public void Clicked()
    {
        menuOpen = !menuOpen;
        roomMenu.SetActive(menuOpen);
        gameObject.GetComponent<BoxCollider2D>().enabled = !menuOpen;
    }

    private void OnDestroy()
    {
        sendButton.onClick.RemoveListener(Deploy);
    }
}
