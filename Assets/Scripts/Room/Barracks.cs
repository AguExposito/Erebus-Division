using UnityEngine;
using System.IO;
using TMPro;

public class Barracks : MonoBehaviour, IClickable
{
    [SerializeField] GameObject roomMenu;
    [SerializeField] GameObject[] BarrackSlot;
    [SerializeField] GameObject hiredAgentPrefab;
    [SerializeField] GameObject agentDisplay;
    bool menuOpen = false;
    public bool hasSpace;
    bool slot1used;
    bool slot2used;
    int currentAgent;

    [SerializeField] int upgradePoints;
    SaveObject saveObject = new SaveObject { };
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        Load();
    }

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
        AgentDisplay(currentAgent);
    }

    public void Recuit(Agent offer)
    {
        //Check for avaliable space
        if (BarrackSlot[0].transform.childCount <= 0 || BarrackSlot[1].transform.childCount <= 0)
        {
            hasSpace = true;
        }
        //checks for empty space to put new agent
        if (BarrackSlot[0].transform.childCount <= 0)
        {
            Instantiate(hiredAgentPrefab, BarrackSlot[0].transform);
            BarrackSlot[0].transform.GetChild(0).GetComponent<Agent>().AcceptStats(offer.AgentName, offer.Health, offer.Dmg, offer.Dodge, offer.StressMax, offer);
            slot1used = true;
            Save();
        }
        else if (BarrackSlot[1].transform.childCount <= 0)
        {
            Instantiate(hiredAgentPrefab, BarrackSlot[1].transform);
            BarrackSlot[1].transform.GetChild(0).GetComponent<Agent>().AcceptStats(offer.AgentName, offer.Health, offer.Dmg, offer.Dodge, offer.StressMax, offer);
            slot2used = true;
            Save();
        }
        else { hasSpace = false; Debug.Log("NO SPACE, DUMB ASS!"); }
    }

    void Save()
    {
        if(slot1used == true)
        {
            saveObject.slotused1 = true;
            saveObject.agentName1 = BarrackSlot[0].transform.GetChild(0).GetComponent<Agent>().AgentName;
            saveObject.health1 = BarrackSlot[0].transform.GetChild(0).GetComponent<Agent>().Health;
            saveObject.dmg1 = BarrackSlot[0].transform.GetChild(0).GetComponent<Agent>().Dmg;
            saveObject.dodge1 = BarrackSlot[0].transform.GetChild(0).GetComponent<Agent>().Dodge;
            saveObject.stressMax1 = BarrackSlot[0].transform.GetChild(0).GetComponent<Agent>().StressMax;
        }
        else { saveObject.slotused1 = false; }
        if (slot2used == true)
        {
            saveObject.slotused2 = true;
            saveObject.agentName2 = BarrackSlot[1].transform.GetChild(0).GetComponent<Agent>().AgentName;
            saveObject.health2 = BarrackSlot[1].transform.GetChild(0).GetComponent<Agent>().Health;
            saveObject.dmg2 = BarrackSlot[1].transform.GetChild(0).GetComponent<Agent>().Dmg;
            saveObject.dodge2 = BarrackSlot[1].transform.GetChild(0).GetComponent<Agent>().Dodge;
            saveObject.stressMax2 = BarrackSlot[1].transform.GetChild(0).GetComponent<Agent>().StressMax;
        }
        else { saveObject.slotused2 = false; }
        string json = JsonUtility.ToJson(saveObject);
        File.WriteAllText(Application.dataPath + "/save.txt", json);
    }

    void Load()
    {
        if (File.Exists(Application.dataPath + "/save.txt"))
        {
            string savedData = File.ReadAllText(Application.dataPath + "/save.txt");

            SaveObject loadObject = JsonUtility.FromJson<SaveObject>(savedData);


            if (BarrackSlot[0].transform.childCount <= 0 && loadObject.slotused1 == true)
            {
                Instantiate(hiredAgentPrefab, BarrackSlot[0].transform);
                BarrackSlot[0].transform.GetChild(0).GetComponent<Agent>().LoadStats(loadObject.agentName1, loadObject.health1, loadObject.dmg1, loadObject.dodge1, loadObject.stressMax1);
                slot1used = true;
                Save();
            }
            if (BarrackSlot[1].transform.childCount <= 0 && loadObject.slotused2 == true)
            {
                Instantiate(hiredAgentPrefab, BarrackSlot[1].transform);
                BarrackSlot[1].transform.GetChild(0).GetComponent<Agent>().LoadStats(loadObject.agentName2, loadObject.health2, loadObject.dmg2, loadObject.dodge2, loadObject.stressMax2);
                slot2used = true;
                Save();
            }

        }
    }

    public void AgentDisplay(int displayedAgent)
    {
        TextMeshProUGUI name = agentDisplay.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI health = agentDisplay.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI dmg = agentDisplay.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI dodge = agentDisplay.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI stress = agentDisplay.transform.GetChild(4).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI upgrades = agentDisplay.transform.GetChild(5).GetComponent<TextMeshProUGUI>();

        if (BarrackSlot.Length >= 1)
        {
            currentAgent = Mathf.Clamp(currentAgent + displayedAgent, 0, BarrackSlot.Length - 1);
            if (BarrackSlot[currentAgent].transform.childCount > 0)
            {
                name.text = BarrackSlot[currentAgent].transform.GetChild(0).GetComponent<Agent>().AgentName;
                health.text = "Health: " + BarrackSlot[currentAgent].transform.GetChild(0).GetComponent<Agent>().Health;
                dmg.text = "DMG: " + BarrackSlot[currentAgent].transform.GetChild(0).GetComponent<Agent>().Dmg;
                dodge.text = "Dodge: " + BarrackSlot[currentAgent].transform.GetChild(0).GetComponent<Agent>().Dodge;
                stress.text = "StressMax: " + BarrackSlot[currentAgent].transform.GetChild(0).GetComponent<Agent>().StressMax;
            }
            else if (BarrackSlot[currentAgent].transform.childCount == 0)
            {
                name.text = "Name";
                health.text = "Health: ";
                dmg.text = "DMG: ";
                dodge.text = "Dodge: ";
                stress.text = "StressMax: ";
            }
            upgrades.text = "Upgrades: " + upgradePoints;
        }
        else { currentAgent = BarrackSlot.Length; }



    }
    //send the string to edit stats if the upgrade points are > than 1
    public void ChangeStat(string statToEdit)
    {
        //prevent errors please :'(
        if (BarrackSlot[currentAgent].transform.childCount > 0)
        {
            Agent agent = BarrackSlot[currentAgent].transform.GetChild(0).GetComponent<Agent>();
            if (statToEdit == "-health" && agent.Health > 1 || statToEdit == "-dmg" && agent.Dmg > 1 || statToEdit == "-dodge" && agent.Dodge > 1 || statToEdit == "-stress" && agent.StressMax > 1)
            {
                upgradePoints += 1;
                BarrackSlot[currentAgent].transform.GetChild(0).GetComponent<Agent>().EditStats(statToEdit);
                AgentDisplay(0);
                Save();
            }
            else if (statToEdit == "health" && upgradePoints > 0 || statToEdit == "dmg" && upgradePoints > 0 || statToEdit == "dodge" && upgradePoints > 0 || statToEdit == "stress" && upgradePoints > 0)
            {
                upgradePoints -= 1;
                BarrackSlot[currentAgent].transform.GetChild(0).GetComponent<Agent>().EditStats(statToEdit);
                AgentDisplay(0);
                Save();
            }
            else { return; }
        }
        
    }

    public void FireAgent()
    {
        if (currentAgent == 0)
        {
            slot1used = false;
        }
        if (currentAgent == 1)
        {
            slot2used = false;
        }
        if(BarrackSlot[currentAgent].transform.childCount > 0) { Destroy(BarrackSlot[currentAgent].transform.GetChild(0).gameObject); AgentDisplay(currentAgent); }
        Save();
    }



    private class SaveObject{
        public bool slotused1;
        public string agentName1;
        public int health1;
        public int dmg1;
        public int dodge1;
        public int stressMax1;

        public bool slotused2;
        public string agentName2;
        public int health2;
        public int dmg2;
        public int dodge2;
        public int stressMax2;

    }
}
