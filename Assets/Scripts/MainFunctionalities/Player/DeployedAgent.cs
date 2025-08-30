using UnityEngine;

public class DeployedAgent : MonoBehaviour
{
    public AgentOnField agentOnField = new AgentOnField { };
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(agentOnField.health);
    }
}

public class AgentOnField
{
    //The Agent
    public int health;
    public int dmg;
    public int dodge;
    public int stressMax;
    //Carried Resources
    public int biscuitRed;
    public int biscuitBlue;
    public int biscuitPurple;
    public int biscuitGold;
}
