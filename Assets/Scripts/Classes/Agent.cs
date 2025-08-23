using UnityEngine;

[System.Serializable]
public class Agent : MonoBehaviour
{
    string[] agentNames = { "Matt", "Alex", "Rose", "Penguin", "Cheese", "Soap", "Muffin" };
   [SerializeField] string _agentName;
    public string AgentName { get; private set; }
   [SerializeField] int _health;
    public int Health { get; private set; }
    [SerializeField] int _dmg;
    public int Dmg { get; private set; }
    [SerializeField] int _dodge;
    public int Dodge { get; private set; }
    [SerializeField] int _stressMax;
    public int StressMax { get; private set; }

    void Awake()
    {
        RandomStats();

    }

    protected virtual void RandomStats()
    {
        this._agentName = agentNames[Random.Range(0, agentNames.Length)];
        this.AgentName = _agentName;
        this._health = Random.Range(1, 5);
        this.Health = _health;
        this._dmg = Random.Range(1, 5);
        this.Dmg = _dmg;
        this._dodge = Random.Range(1, 5);
        this.Dodge = _dodge;
        this._stressMax = Random.Range(1, 5);
        this.StressMax = _stressMax;
        // ///uwu///
        gameObject.name = "Agent: " + _agentName;
    }

    public void AcceptStats(string agentName, int health, int dmg, int dodge, int stressMax, Agent agent)
    {
        if (agent.AgentName == agentName && agent.Health == health && agent.Dmg == dmg && agent.Dodge == dodge && agent.StressMax == stressMax)
        {
            this._agentName = agentName;
            this.AgentName = _agentName;
            this._health = health;
            this.Health = _health;
            this._dmg = dmg;
            this.Dmg = _dmg;
            this._dodge = dodge;
            this.Dodge = _dodge;
            this._stressMax = stressMax;
            this.StressMax = _stressMax;
            gameObject.name = "Agent: " + _agentName;
        }
        else { Debug.Log("YOU CHEATER SCUM!"); }
    }
    //function to load and set the stats of the hired agents
    public void LoadStats(string agentName, int health, int dmg, int dodge, int stressMax)
    {
        this._agentName = agentName;
        this.AgentName = _agentName;
        this._health = health;
        this.Health = _health;
        this._dmg = dmg;
        this.Dmg = _dmg;
        this._dodge = dodge;
        this.Dodge = _dodge;
        this._stressMax = stressMax;
        this.StressMax = _stressMax;
        gameObject.name = "Agent: " + _agentName;
    }

    public void EditStats(string statToEdit)
    {
        int minStats = 1;
        switch (statToEdit)
        {
            case "health":
                this._health += 1;
                this.Health = _health;
                break;
            case "-health":
                if (_health <= minStats) { return; }
                this._health -= 1;
                this.Health = _health;
                break;
            case "dmg":
                this._dmg += 1;
                this.Dmg = _dmg;
                break;
            case "-dmg":
                if (_dmg <= minStats) { return; }
                this._dmg -= 1;
                this.Dmg = _dmg;
                break;
            case "dodge":
                this._dodge += 1;
                this.Dodge = _dodge;
                break;
            case "-dodge":
                if (_dodge <= minStats) { return; }
                this._dodge -= 1;
                this.Dodge = _dodge;
                break;
            case "stress":
                this._stressMax += 1;
                this.StressMax = _stressMax;
                break;
            case "-stress":
                if (_stressMax <= minStats) { return; }
                this._stressMax -= 1;
                this.StressMax = _stressMax;
                break;
            default:

                break;
        }
    }
}
