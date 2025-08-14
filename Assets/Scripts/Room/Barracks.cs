using UnityEngine;
using System.IO;

public class Barracks : MonoBehaviour, IClickable
{
    [SerializeField] GameObject roomMenu;
    [SerializeField] GameObject BarrackSlot1;
    [SerializeField] GameObject BarrackSlot2;
    [SerializeField] GameObject hiredAgentPrefab;    
    bool menuOpen = false;
    public bool hasSpace;
    bool slot1used;
    bool slot2used;
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
    }

    public void Recuit(Agent offer)
    {
        //Check for avaliable space
        if (BarrackSlot1.transform.childCount <= 0 || BarrackSlot2.transform.childCount <= 0)
        {
            hasSpace = true;
        }
        //checks for empty space to put new agent
        if (BarrackSlot1.transform.childCount <= 0)
        {
            Instantiate(hiredAgentPrefab, BarrackSlot1.transform);
            BarrackSlot1.transform.GetChild(0).GetComponent<Agent>().AcceptStats(offer.AgentName, offer.Health, offer.Dmg, offer.Dodge, offer.StressMax, offer);
            slot1used = true;
            Save();
        }
        else if (BarrackSlot2.transform.childCount <= 0)
        {
            Instantiate(hiredAgentPrefab, BarrackSlot2.transform);
            BarrackSlot2.transform.GetChild(0).GetComponent<Agent>().AcceptStats(offer.AgentName, offer.Health, offer.Dmg, offer.Dodge, offer.StressMax, offer);
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
            saveObject.agentName1 = BarrackSlot1.transform.GetChild(0).GetComponent<Agent>().AgentName;
            saveObject.health1 = BarrackSlot1.transform.GetChild(0).GetComponent<Agent>().Health;
            saveObject.dmg1 = BarrackSlot1.transform.GetChild(0).GetComponent<Agent>().Dmg;
            saveObject.dodge1 = BarrackSlot1.transform.GetChild(0).GetComponent<Agent>().Dodge;
            saveObject.stressMax1 = BarrackSlot1.transform.GetChild(0).GetComponent<Agent>().StressMax;
        }
        if (slot2used == true)
        {
            saveObject.slotused2 = true;
            saveObject.agentName2 = BarrackSlot2.transform.GetChild(0).GetComponent<Agent>().AgentName;
            saveObject.health2 = BarrackSlot2.transform.GetChild(0).GetComponent<Agent>().Health;
            saveObject.dmg2 = BarrackSlot2.transform.GetChild(0).GetComponent<Agent>().Dmg;
            saveObject.dodge2 = BarrackSlot2.transform.GetChild(0).GetComponent<Agent>().Dodge;
            saveObject.stressMax2 = BarrackSlot2.transform.GetChild(0).GetComponent<Agent>().StressMax;
        }
        string json = JsonUtility.ToJson(saveObject);
        File.WriteAllText(Application.dataPath + "/save.txt", json);
    }

    void Load()
    {
        if (File.Exists(Application.dataPath + "/save.txt"))
        {
            string savedData = File.ReadAllText(Application.dataPath + "/save.txt");

            SaveObject loadObject = JsonUtility.FromJson<SaveObject>(savedData);


            if (BarrackSlot1.transform.childCount <= 0 && loadObject.slotused1 == true)
            {
                Instantiate(hiredAgentPrefab, BarrackSlot1.transform);
                BarrackSlot1.transform.GetChild(0).GetComponent<Agent>().LoadStats(loadObject.agentName1, loadObject.health1, loadObject.dmg1, loadObject.dodge1, loadObject.stressMax1);
                slot1used = true;
                Save();
            }
            if (BarrackSlot2.transform.childCount <= 0 && loadObject.slotused2 == true)
            {
                Instantiate(hiredAgentPrefab, BarrackSlot2.transform);
                BarrackSlot2.transform.GetChild(0).GetComponent<Agent>().LoadStats(loadObject.agentName2, loadObject.health2, loadObject.dmg2, loadObject.dodge2, loadObject.stressMax2);
                slot2used = true;
                Save();
            }

        }
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
