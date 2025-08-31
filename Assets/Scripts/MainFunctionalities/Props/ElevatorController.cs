using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    public GameObject player;
    public AudioSource audioSource;
    public GameObject playerInstance;
    public Animator animator;
    private Vector3 initialPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        initialPosition = new Vector3(transform.position.x, transform.position.y+0.5f, transform.position.z);
        playerInstance = Instantiate(player, initialPosition, Quaternion.identity);
        if (playerInstance != null) {
            Invoke("OpenCLoseDoor",2);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenCLoseDoor() {
        audioSource.Play();
        animator.SetTrigger("Change");
    }

    public void ExitDD() 
    {
        FPSController playerfpscontroller = playerInstance.GetComponent<FPSController>();
        playerfpscontroller.playerInput.Player.Disable();
        playerfpscontroller.playerInput.Encounter.Disable();
        Invoke("LoadShop", 2);
    }
    public void LoadShop() {
        GetComponent<MainMenu>().LoadScene(3);
    }
    public void NextDD() 
    {
        FPSController playerfpscontroller = playerInstance.GetComponent<FPSController>();
        playerfpscontroller.playerInput.Player.Disable();
        playerfpscontroller.playerInput.Encounter.Disable();
        Invoke("LoadShop", 2);
    }
    public void LoadNewDD() {
        GetComponent<MainMenu>().LoadScene(2);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<FPSController>(out FPSController playerFPSController))
            playerFPSController.isInElevator=true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<FPSController>(out FPSController playerFPSController))
            playerFPSController.isInElevator = false;
    }

    public void SavePlayerContext()
    {
        FPSController playerfpscontroller = playerInstance.GetComponent<FPSController>();
        InventoryManager.Instance.baseAttackPower = playerfpscontroller.playerStats.baseAttackPower;
        InventoryManager.Instance.baseCritChance = playerfpscontroller.playerStats.baseCritChance;
        InventoryManager.Instance.baseDodgeChance = playerfpscontroller.playerStats.baseDodgeChance;
        InventoryManager.Instance.baseHitChance = playerfpscontroller.playerStats.baseHitChance;
    }
}
