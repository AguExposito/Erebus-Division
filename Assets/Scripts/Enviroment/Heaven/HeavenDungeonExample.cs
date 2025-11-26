using UnityEngine;

/// <summary>
/// Example script demonstrating how to use the Heaven Dungeon Generator
/// Attach this to a GameObject in your scene to test the dungeon generation
/// </summary>
public class HeavenDungeonExample : MonoBehaviour
{
    [Header("Heaven Dungeon Generator")]
    [SerializeField] private HeavenDungeonGenerator dungeonGenerator;
    
    [Header("Controls")]
    [SerializeField] private KeyCode regenerateKey = KeyCode.R;
    [SerializeField] private KeyCode clearKey = KeyCode.C;
    [SerializeField] private KeyCode toggleDebugKey = KeyCode.D;
    [SerializeField] private KeyCode printMatrixKey = KeyCode.M;
    [SerializeField] private KeyCode printRoomsKey = KeyCode.P;
    [SerializeField] private KeyCode testGenerationKey = KeyCode.T;
    
    private bool debugMode = false;
    
    void Start()
    {
        // If no generator is assigned, try to find one
        if (dungeonGenerator == null)
        {
            dungeonGenerator = FindObjectOfType<HeavenDungeonGenerator>();
        }
        
        // If no generator found, create one
        if (dungeonGenerator == null)
        {
            CreateDungeonGenerator();
        }
        
        // Configuration is now done directly in the inspector
        
        Debug.Log("Heaven Dungeon Example initialized. Controls:");
        Debug.Log("R - Regenerate dungeon");
        Debug.Log("C - Clear dungeon");
        Debug.Log("D - Toggle debug mode");
        Debug.Log("M - Print matrix");
        Debug.Log("P - Print room positions");
        Debug.Log("T - Test generation (3 rooms at fixed positions)");
        Debug.Log("C - Print connectivity analysis");
        Debug.Log("X - Diagnose positioning issues");
    }
    
    void Update()
    {
        HandleInput();
    }
    
    private void HandleInput()
    {
        if (Input.GetKeyDown(regenerateKey))
        {
            RegenerateDungeon();
        }
        
        if (Input.GetKeyDown(clearKey))
        {
            ClearDungeon();
        }
        
        if (Input.GetKeyDown(toggleDebugKey))
        {
            ToggleDebugMode();
        }
        
        if (Input.GetKeyDown(printMatrixKey))
        {
            PrintMatrix();
        }
        
        if (Input.GetKeyDown(printRoomsKey))
        {
            PrintRoomPositions();
        }
        
        if (Input.GetKeyDown(testGenerationKey))
        {
            TestGeneration();
        }
        
        if (Input.GetKeyDown(KeyCode.C))
        {
            PrintConnectivity();
        }
        
        if (Input.GetKeyDown(KeyCode.X))
        {
            DiagnosePositioning();
        }
    }
    
    private void CreateDungeonGenerator()
    {
        // Create a new GameObject for the dungeon generator
        GameObject generatorObj = new GameObject("HeavenDungeonGenerator");
        generatorObj.transform.SetParent(transform);
        
        // Add the generator component
        dungeonGenerator = generatorObj.AddComponent<HeavenDungeonGenerator>();
        
        Debug.Log("Created Heaven Dungeon Generator");
    }
    
    
    public void RegenerateDungeon()
    {
        if (dungeonGenerator != null)
        {
            Debug.Log("Regenerating Heaven Dungeon...");
            dungeonGenerator.RegenerateDungeon();
            
            // Log dungeon information
            LogDungeonInfo();
        }
        else
        {
            Debug.LogWarning("No Heaven Dungeon Generator found!");
        }
    }
    
    public void ClearDungeon()
    {
        if (dungeonGenerator != null)
        {
            Debug.Log("Clearing Heaven Dungeon...");
            dungeonGenerator.ClearDungeon();
        }
        else
        {
            Debug.LogWarning("No Heaven Dungeon Generator found!");
        }
    }
    
    private void ToggleDebugMode()
    {
        debugMode = !debugMode;
        Debug.Log("Debug mode: " + (debugMode ? "ON" : "OFF"));
        
        // Enable/disable debug visualization
        if (dungeonGenerator != null)
        {
            // This would enable debug visualization in the generator
            // For now, we'll just log the state
        }
    }
    
    private void LogDungeonInfo()
    {
        if (dungeonGenerator != null)
        {
            var rooms = dungeonGenerator.GetGeneratedRooms();
            var roomInstances = dungeonGenerator.GetAllRoomInstances();
            var hallwayInstances = dungeonGenerator.GetAllHallwayInstances();
            var matrix = dungeonGenerator.GetDungeonMatrix();
            
            Debug.Log($"Dungeon Info:");
            Debug.Log($"- Rooms: {rooms.Count}");
            Debug.Log($"- Room Instances: {roomInstances.Count}");
            Debug.Log($"- Hallway Instances: {hallwayInstances.Count}");
            Debug.Log($"- Matrix Size: {matrix.GetLength(0)}x{matrix.GetLength(1)}");
            
            // Log room types
            foreach (var room in rooms)
            {
                Debug.Log($"- Room: {room.RoomType} at ({room.MatrixX}, {room.MatrixY})");
            }
        }
    }
    
    // Public methods for UI buttons
    public void OnRegenerateButtonClicked()
    {
        RegenerateDungeon();
    }
    
    public void OnClearButtonClicked()
    {
        ClearDungeon();
    }
    
    public void OnDebugToggleButtonClicked()
    {
        ToggleDebugMode();
    }
    
    public void PrintMatrix()
    {
        if (dungeonGenerator != null)
        {
            dungeonGenerator.PrintMatrix();
        }
        else
        {
            Debug.LogWarning("No Heaven Dungeon Generator found!");
        }
    }
    
    public void PrintRoomPositions()
    {
        if (dungeonGenerator != null)
        {
            dungeonGenerator.PrintRoomPositions();
        }
        else
        {
            Debug.LogWarning("No Heaven Dungeon Generator found!");
        }
    }
    
    public void TestGeneration()
    {
        if (dungeonGenerator != null)
        {
            dungeonGenerator.TestGeneration();
        }
        else
        {
            Debug.LogWarning("No Heaven Dungeon Generator found!");
        }
    }
    
    public void PrintConnectivity()
    {
        if (dungeonGenerator != null)
        {
            dungeonGenerator.PrintConnectivity();
        }
        else
        {
            Debug.LogWarning("No Heaven Dungeon Generator found!");
        }
    }
    
    public void DiagnosePositioning()
    {
        if (dungeonGenerator != null)
        {
            dungeonGenerator.DiagnosePositioning();
        }
        else
        {
            Debug.LogWarning("No Heaven Dungeon Generator found!");
        }
    }
    
    // Example of how to customize the dungeon generation
    public void SetDungeonSize(int width, int height)
    {
        if (dungeonGenerator != null)
        {
            // This would set the dungeon size
            // You would need to add a public method to HeavenDungeonGenerator
            Debug.Log($"Setting dungeon size to {width}x{height}");
        }
    }
    
    public void SetRoomCount(int minRooms, int maxRooms)
    {
        if (dungeonGenerator != null)
        {
            // This would set the room count
            // You would need to add a public method to HeavenDungeonGenerator
            Debug.Log($"Setting room count to {minRooms}-{maxRooms}");
        }
    }
    
    // Example of how to get specific room information
    public HeavenRoom GetRoomAt(int x, int y)
    {
        if (dungeonGenerator != null)
        {
            var rooms = dungeonGenerator.GetGeneratedRooms();
            return rooms.Find(room => room.MatrixX == x && room.MatrixY == y);
        }
        return null;
    }
    
    public HeavenRoom GetRoomByType(HeavenDungeonGenerator.HeavenRoomType roomType)
    {
        if (dungeonGenerator != null)
        {
            var rooms = dungeonGenerator.GetGeneratedRooms();
            return rooms.Find(room => room.RoomType == roomType);
        }
        return null;
    }
    
    // Example of how to get all rooms of a specific type
    public HeavenRoom[] GetRoomsByType(HeavenDungeonGenerator.HeavenRoomType roomType)
    {
        if (dungeonGenerator != null)
        {
            var rooms = dungeonGenerator.GetGeneratedRooms();
            return rooms.FindAll(room => room.RoomType == roomType).ToArray();
        }
        return new HeavenRoom[0];
    }
}
