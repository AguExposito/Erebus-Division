using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class HR : MonoBehaviour, IClickable
{
    [SerializeField] GameObject roomMenu;
    [SerializeField] GameObject agentPrefab;

    [SerializeField] GameObject agentSlot1;
    [SerializeField] GameObject agentOfferDisplay1;
    [SerializeField] GameObject emptyOffer1;
    [SerializeField] GameObject agentSlot2;
    [SerializeField] GameObject agentOfferDisplay2;
    [SerializeField] GameObject emptyOffer2;

    [SerializeField] Barracks barracks;
    bool menuOpen = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Deactivates its own menu just in case
        roomMenu.SetActive(menuOpen);
    }

    //open menu when clicked
    public void Clicked()
    {
        menuOpen = !menuOpen;
        roomMenu.SetActive(menuOpen);
        gameObject.GetComponent<BoxCollider2D>().enabled = !menuOpen;
    }

    //instantiate agents for hire
    public void Interview()
    {
        //Everything related to agent 1
        Agent offer1;
        TextMeshProUGUI NameText1 = agentOfferDisplay1.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI DMGText1 = agentOfferDisplay1.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI HealthText1 = agentOfferDisplay1.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI DodgeText1 = agentOfferDisplay1.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI StressText1 = agentOfferDisplay1.transform.GetChild(4).GetComponent<TextMeshProUGUI>();

        //Everything related to agent 2
        Agent offer2;
        TextMeshProUGUI NameText2 = agentOfferDisplay2.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI DMGText2 = agentOfferDisplay2.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI HealthText2 = agentOfferDisplay2.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI DodgeText2 = agentOfferDisplay2.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI StressText2 = agentOfferDisplay2.transform.GetChild(4).GetComponent<TextMeshProUGUI>();

        if (agentSlot1.transform.childCount <= 0)
        {
            //Creats agents in their slot
            Instantiate(agentPrefab, agentSlot1.transform);
            //Finds the Agent
            offer1 = agentSlot1.transform.GetChild(0).gameObject.GetComponent<AgentCharacter>();


            //Sets menu offer for Agent 1
            NameText1.text = offer1.AgentName;
            DMGText1.text = "DMG: " + offer1.Dmg;
            HealthText1.text = "Health: " + offer1.Health;
            DodgeText1.text = "Dodge: " + offer1.Dodge;
            StressText1.text = "StressM: " + offer1.StressMax;

            agentOfferDisplay1.SetActive(true);
            emptyOffer1.SetActive(false);
        }
        else if (agentSlot2.transform.childCount <= 0)
        {
            //Creats agents in their slot
            Instantiate(agentPrefab, agentSlot2.transform);
            //Finds the Agent
            offer2 = agentSlot2.transform.GetChild(0).gameObject.GetComponent<AgentCharacter>();

            //Sets menu offer for Agent 2
            NameText2.text = offer2.AgentName;
            DMGText2.text = "DMG: " + offer2.Dmg;
            HealthText2.text = "Health: " + offer2.Health;
            DodgeText2.text = "Dodge: " + offer2.Dodge;
            StressText2.text = "StressM: " + offer2.StressMax;
            agentOfferDisplay2.SetActive(true);
            emptyOffer2.SetActive(false);
        }
        else { Debug.Log("No space Nigga"); }

        //Creats agents in their slot
        


        //Finds the Agent

    }

    public void Hire1()
    {
        Agent offer = agentSlot1.transform.GetChild(0).gameObject.GetComponent<AgentCharacter>();
        barracks.Recuit(offer);
        if (barracks.hasSpace == true)
        {
            Destroy(offer.gameObject);
            agentOfferDisplay1.SetActive(false);
            emptyOffer1.SetActive(true);
        }
    }

    public void Hire2()
    {
        Agent offer = agentSlot2.transform.GetChild(0).gameObject.GetComponent<AgentCharacter>();
        barracks.Recuit(offer);
        if (barracks.hasSpace == true)
        {
            Destroy(offer.gameObject);
            agentOfferDisplay2.SetActive(false);
            emptyOffer2.SetActive(true);
        }
    }
}
