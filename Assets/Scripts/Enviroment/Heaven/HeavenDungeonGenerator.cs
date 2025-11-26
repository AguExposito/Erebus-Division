using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavenDungeonGenerator : MonoBehaviour
{
    [Header("Dungeon Matrix Settings")]
    [SerializeField] public int dungeonWidth = 8;   // Width in blocks
    [SerializeField] public int dungeonHeight = 8;  // Height in blocks
    [SerializeField] public float blockSize = 3f;   // Size of each block in world units
    
    [Header("Room Generation")]
    [SerializeField] public int minRooms = 4;
    [SerializeField] public int maxRooms = 6;
    
    [Header("Room Separation")]
    [Tooltip("Minimum blocks of separation between rooms (1 = adjacent, 2 = 1 block gap, 3 = 2 blocks gap, etc.)")]
    [SerializeField] public int minRoomSeparation = 2;
    
    [Header("Heaven Prefabs")]
    [SerializeField] public GameObject heavenRoomPrefab;
    [SerializeField] public GameObject heavenHallwayPrefab;
    [SerializeField] public GameObject heavenFloorPrefab;
    [SerializeField] public GameObject heavenWallPrefab;
    
    [Header("Generation Settings")]
    [SerializeField] public bool useCoroutines = true;
    [SerializeField] public int maxRoomsPerFrame = 2;
    
    [Header("Heaven Theme")]
    [SerializeField] public Material heavenFloorMaterial;
    [SerializeField] public Material heavenWallMaterial;
    [SerializeField] public Color heavenAmbientColor = new Color(0.8f, 0.9f, 1f, 1f);
    
    // Private variables
    private int[,] dungeonMatrix;
    private List<HeavenRoom> generatedRooms = new List<HeavenRoom>();
    private List<GameObject> allRoomInstances = new List<GameObject>();
    private List<GameObject> allHallwayInstances = new List<GameObject>();
    private Transform dungeonParent;
    
    // Heaven-specific room types
    public enum HeavenRoomType
    {
        Sanctuary,    // Starting room
        Chapel,       // Regular room
        Altar,        // Special room
        Garden,       // Peaceful room
        Library,      // Knowledge room
        Treasury,     // Treasure room
        Boss,         // Final room
        Empty         // Empty space
    }
    
    void Start()
    {
        InitializeDungeon();
        if (useCoroutines)
        {
            StartCoroutine(GenerateDungeonCoroutine());
        }
        else
        {
            GenerateDungeonImmediate();
        }
    }
    
    private void InitializeDungeon()
    {
        // Create parent object for organization
        dungeonParent = new GameObject("HeavenDungeon").transform;
        dungeonParent.SetParent(transform);
        
        // Initialize matrix
        dungeonMatrix = new int[dungeonWidth, dungeonHeight];
        
        // Fill matrix with empty spaces
        for (int x = 0; x < dungeonWidth; x++)
        {
            for (int y = 0; y < dungeonHeight; y++)
            {
                dungeonMatrix[x, y] = 0; // 0 = empty
            }
        }
        
        Debug.Log($"Initialized Heaven Dungeon Matrix: {dungeonWidth}x{dungeonHeight}");
        Debug.Log($"Block Size: {blockSize}");
        Debug.Log($"Heaven Room Prefab: {(heavenRoomPrefab != null ? "Assigned" : "NULL - Will create empty objects")}");
    }
    
    private void GenerateDungeonImmediate()
    {
        GenerateRooms();
        ConnectRooms();
        SpawnPlayerAtStart();
        ApplyHeavenTheme();
    }
    
    private IEnumerator GenerateDungeonCoroutine()
    {
        yield return StartCoroutine(GenerateRoomsCoroutine());
        yield return StartCoroutine(ConnectRoomsCoroutine());
        SpawnPlayerAtStart();
        ApplyHeavenTheme();
    }
    
    private void GenerateRooms()
    {
        int roomsToGenerate = Random.Range(minRooms, maxRooms + 1);
        int roomsCreated = 0;
        int maxAttempts = dungeonWidth * dungeonHeight * 2; // Prevent infinite loops
        int attempts = 0;
        
        // Always create a starting room at center
        int centerX = dungeonWidth / 2;
        int centerY = dungeonHeight / 2;
        CreateRoomAtPosition(centerX, centerY, HeavenRoomType.Sanctuary);
        roomsCreated++;
        
        // Generate random rooms
        while (roomsCreated < roomsToGenerate && attempts < maxAttempts)
        {
            int x = Random.Range(0, dungeonWidth);
            int y = Random.Range(0, dungeonHeight);
            attempts++;
            
            if (dungeonMatrix[x, y] == 0) // Empty space
            {
                HeavenRoomType roomType = GetRandomRoomType(roomsCreated, roomsToGenerate);
                CreateRoomAtPosition(x, y, roomType);
                roomsCreated++;
            }
        }
        
        Debug.Log($"Generated {roomsCreated} rooms in Heaven Dungeon (attempts: {attempts})");
    }
    
    private IEnumerator GenerateRoomsCoroutine()
    {
        int roomsToGenerate = Random.Range(minRooms, maxRooms + 1);
        int roomsCreated = 0;
        int maxAttempts = dungeonWidth * dungeonHeight * 2; // Prevent infinite loops
        int attempts = 0;
        
        // Always create a starting room at center
        int centerX = dungeonWidth / 2;
        int centerY = dungeonHeight / 2;
        CreateRoomAtPosition(centerX, centerY, HeavenRoomType.Sanctuary);
        roomsCreated++;
        
        // Generate random rooms
        while (roomsCreated < roomsToGenerate && attempts < maxAttempts)
        {
            int x = Random.Range(0, dungeonWidth);
            int y = Random.Range(0, dungeonHeight);
            attempts++;
            
            if (dungeonMatrix[x, y] == 0) // Empty space
            {
                HeavenRoomType roomType = GetRandomRoomType(roomsCreated, roomsToGenerate);
                CreateRoomAtPosition(x, y, roomType);
                roomsCreated++;
                
                if (roomsCreated % maxRoomsPerFrame == 0)
                {
                    yield return null;
                }
            }
        }
        
        Debug.Log($"Generated {roomsCreated} rooms in Heaven Dungeon using coroutines (attempts: {attempts})");
    }
    
    private bool IsPositionValid(int x, int y)
    {
        // Check all positions within the minimum separation distance
        for (int dx = -minRoomSeparation; dx <= minRoomSeparation; dx++)
        {
            for (int dy = -minRoomSeparation; dy <= minRoomSeparation; dy++)
            {
                if (dx == 0 && dy == 0) continue; // Skip center position
                
                int checkX = x + dx;
                int checkY = y + dy;
                
                // Check if position is within bounds
                if (checkX >= 0 && checkX < dungeonWidth && checkY >= 0 && checkY < dungeonHeight)
                {
                    // If any cell within separation distance is occupied, position is invalid
                    if (dungeonMatrix[checkX, checkY] != 0)
                    {
                        return false;
                    }
                }
            }
        }
        
        return true;
    }
    
    private void CreateRoomAtPosition(int x, int y, HeavenRoomType roomType)
    {
        // Check if position is already occupied
        if (dungeonMatrix[x, y] != 0)
        {
            Debug.LogWarning($"Position ({x}, {y}) is already occupied! Skipping room creation.");
            return;
        }
        
        // Check if there's a room too close (minimum 1 block separation)
        if (!IsPositionValid(x, y))
        {
            Debug.LogWarning($"Position ({x}, {y}) is too close to an existing room! Skipping room creation.");
            return;
        }
        
        // Mark position as occupied
        dungeonMatrix[x, y] = 1;
        
        // Calculate world position based on matrix coordinates
        // Center the dungeon around world origin (0,0,0)
        float worldX = (x - dungeonWidth / 2f) * blockSize;
        float worldZ = (y - dungeonHeight / 2f) * blockSize;
        
        Vector3 worldPos = new Vector3(worldX, 0, worldZ);
        
        // Create room instance - ALWAYS use basic cube for reliable positioning
        GameObject roomObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        roomObj.name = $"HeavenRoom_{roomType}_{x}_{y}";
        
        // Set parent FIRST, then position to avoid transform issues
        roomObj.transform.SetParent(dungeonParent, false);
        
        // Set position AFTER setting parent
        roomObj.transform.position = worldPos;
        roomObj.transform.localScale = Vector3.one * blockSize;
        
        // Apply heaven theme to the cube
        ApplyHeavenThemeToRoom(roomObj, roomType);
        
        // If prefab is assigned, instantiate it as a child for visual enhancement
        if (heavenRoomPrefab != null)
        {
            GameObject prefabInstance = Instantiate(heavenRoomPrefab, Vector3.zero, Quaternion.identity, roomObj.transform);
            prefabInstance.name = $"Prefab_{roomType}_{x}_{y}";
            // Ensure prefab child is positioned at the center of the room
            prefabInstance.transform.localPosition = Vector3.zero;
            prefabInstance.transform.localRotation = Quaternion.identity;
        }
        
        // Force correct position after all operations
        roomObj.transform.position = worldPos;
        
        // Verify position after all operations
        if (Vector3.Distance(roomObj.transform.position, worldPos) > 0.1f)
        {
            Debug.LogError($"CRITICAL: Position mismatch! Expected: {worldPos}, Actual: {roomObj.transform.position}");
            // Force correct position
            roomObj.transform.position = worldPos;
        }
        
        // Create HeavenRoom component
        HeavenRoom room = roomObj.AddComponent<HeavenRoom>();
        room.Initialize(roomType, x, y, worldPos);
        
        generatedRooms.Add(room);
        allRoomInstances.Add(roomObj);
        
        // Generate room content based on type
        GenerateRoomContent(room, roomObj);
        
        Debug.Log($"Created {roomType} room at matrix position ({x}, {y}) -> world position {worldPos}");
        Debug.Log($"Room GameObject position: {roomObj.transform.position}");
        Debug.Log($"Room GameObject parent: {(roomObj.transform.parent != null ? roomObj.transform.parent.name : "None")}");
    }
    
    private void ApplyHeavenThemeToRoom(GameObject roomObj, HeavenRoomType roomType)
    {
        // Apply different colors based on room type
        Renderer renderer = roomObj.GetComponent<Renderer>();
        if (renderer != null)
        {
            switch (roomType)
            {
                case HeavenRoomType.Sanctuary:
                    renderer.material.color = new Color(1f, 1f, 0.8f, 1f); // Gold
                    break;
                case HeavenRoomType.Chapel:
                    renderer.material.color = new Color(0.9f, 0.9f, 1f, 1f); // Light blue
                    break;
                case HeavenRoomType.Garden:
                    renderer.material.color = new Color(0.8f, 1f, 0.8f, 1f); // Light green
                    break;
                case HeavenRoomType.Library:
                    renderer.material.color = new Color(1f, 0.9f, 0.7f, 1f); // Light brown
                    break;
                case HeavenRoomType.Altar:
                    renderer.material.color = new Color(1f, 0.8f, 0.8f, 1f); // Light pink
                    break;
                case HeavenRoomType.Treasury:
                    renderer.material.color = new Color(1f, 1f, 0.6f, 1f); // Yellow
                    break;
                case HeavenRoomType.Boss:
                    renderer.material.color = new Color(1f, 0.6f, 0.6f, 1f); // Red
                    break;
                default:
                    renderer.material.color = Color.white;
                    break;
            }
        }
    }
    
    private void GenerateRoomContent(HeavenRoom room, GameObject roomObj)
    {
        // Add floor
        if (heavenFloorPrefab != null)
        {
            GameObject floor = Instantiate(heavenFloorPrefab, roomObj.transform);
            floor.transform.localPosition = Vector3.zero;
            floor.name = "Floor";
        }
        
        // Add walls based on room type
        if (heavenWallPrefab != null)
        {
            CreateRoomWalls(roomObj, room.RoomType);
        }
        
        // Add room-specific decorations
        AddRoomDecorations(room, roomObj);
    }
    
    private void CreateRoomWalls(GameObject roomObj, HeavenRoomType roomType)
    {
        float wallHeight = 3f;
        float wallThickness = 0.2f;
        
        // Create 4 walls around the room
        Vector3[] wallPositions = {
            new Vector3(0, wallHeight/2, blockSize/2),      // North
            new Vector3(0, wallHeight/2, -blockSize/2),     // South
            new Vector3(blockSize/2, wallHeight/2, 0),      // East
            new Vector3(-blockSize/2, wallHeight/2, 0)      // West
        };
        
        Vector3[] wallRotations = {
            Vector3.zero,                                   // North
            new Vector3(0, 180, 0),                         // South
            new Vector3(0, 90, 0),                          // East
            new Vector3(0, -90, 0)                          // West
        };
        
        for (int i = 0; i < 4; i++)
        {
            GameObject wall = Instantiate(heavenWallPrefab, roomObj.transform);
            wall.transform.localPosition = wallPositions[i];
            wall.transform.localRotation = Quaternion.Euler(wallRotations[i]);
            wall.transform.localScale = new Vector3(blockSize, wallHeight, wallThickness);
            wall.name = $"Wall_{i}";
        }
    }
    
    private void AddRoomDecorations(HeavenRoom room, GameObject roomObj)
    {
        // Add decorations based on room type
        switch (room.RoomType)
        {
            case HeavenRoomType.Sanctuary:
                // Add altar or special starting decoration
                break;
            case HeavenRoomType.Chapel:
                // Add pews or religious decorations
                break;
            case HeavenRoomType.Altar:
                // Add altar and candles
                break;
            case HeavenRoomType.Garden:
                // Add plants and peaceful elements
                break;
            case HeavenRoomType.Library:
                // Add bookshelves and knowledge elements
                break;
            case HeavenRoomType.Treasury:
                // Add treasure chests
                break;
            case HeavenRoomType.Boss:
                // Add boss-specific decorations
                break;
        }
    }
    
    private HeavenRoomType GetRandomRoomType(int currentRoom, int totalRooms)
    {
        if (currentRoom == totalRooms - 1)
            return HeavenRoomType.Boss;
        
        int roll = Random.Range(0, 100);
        if (roll < 30) return HeavenRoomType.Chapel;
        if (roll < 50) return HeavenRoomType.Garden;
        if (roll < 70) return HeavenRoomType.Library;
        if (roll < 85) return HeavenRoomType.Altar;
        return HeavenRoomType.Treasury;
    }
    
    private void ConnectRooms()
    {
        // First, connect adjacent rooms in matrix
        ConnectAdjacentRooms();
        
        // Then, ensure all rooms are accessible by connecting isolated groups
        EnsureAllRoomsAccessible();
        
        Debug.Log($"Connected rooms based on matrix adjacency and accessibility");
    }
    
    private void ConnectAdjacentRooms()
    {
        // Connect rooms that are within (minRoomSeparation + 1) blocks distance
        // This allows connections through hallways in the empty space between rooms
        int connectionDistance = minRoomSeparation + 1;
        
        for (int i = 0; i < generatedRooms.Count; i++)
        {
            HeavenRoom currentRoom = generatedRooms[i];
            
            // Check for rooms within connection distance (North, South, East, West)
            int[] dx = { -connectionDistance, connectionDistance, 0, 0 }; // North, South, East, West
            int[] dy = { 0, 0, -connectionDistance, connectionDistance };
            
            for (int dir = 0; dir < 4; dir++)
            {
                int newX = currentRoom.MatrixX + dx[dir];
                int newY = currentRoom.MatrixY + dy[dir];
                
                // Check if position is within bounds
                if (newX >= 0 && newX < dungeonWidth && newY >= 0 && newY < dungeonHeight)
                {
                    // Find room at this position
                    HeavenRoom nearbyRoom = generatedRooms.Find(room => 
                        room.MatrixX == newX && room.MatrixY == newY);
                    
                    if (nearbyRoom != null && !currentRoom.IsConnectedTo(nearbyRoom))
                    {
                        // Verify there's a clear path (the middle cells should be empty)
                        bool clearPath = true;
                        for (int step = 1; step < connectionDistance; step++)
                        {
                            int midX = currentRoom.MatrixX + (dx[dir] * step) / connectionDistance;
                            int midY = currentRoom.MatrixY + (dy[dir] * step) / connectionDistance;
                            
                            if (midX >= 0 && midX < dungeonWidth && midY >= 0 && midY < dungeonHeight)
                            {
                                if (dungeonMatrix[midX, midY] != 0)
                                {
                                    clearPath = false;
                                    break;
                                }
                            }
                        }
                        
                        // If path is clear, create hallway
                        if (clearPath)
                        {
                            CreateHallway(currentRoom, nearbyRoom);
                        }
                    }
                }
            }
        }
    }
    
    private void EnsureAllRoomsAccessible()
    {
        // Find all connected components using BFS
        List<List<HeavenRoom>> connectedComponents = FindConnectedComponents();
        
        // If there are multiple components, connect them
        if (connectedComponents.Count > 1)
        {
            Debug.Log($"Found {connectedComponents.Count} disconnected room groups. Connecting them...");
            
            // Connect each component to the next one
            for (int i = 0; i < connectedComponents.Count - 1; i++)
            {
                HeavenRoom room1 = FindClosestRoom(connectedComponents[i], connectedComponents[i + 1]);
                HeavenRoom room2 = FindClosestRoom(connectedComponents[i + 1], connectedComponents[i]);
                
                if (room1 != null && room2 != null)
                {
                    CreateHallway(room1, room2);
                    Debug.Log($"Connected {room1.RoomType} to {room2.RoomType} to ensure accessibility");
                }
            }
        }
    }
    
    private List<List<HeavenRoom>> FindConnectedComponents()
    {
        List<List<HeavenRoom>> components = new List<List<HeavenRoom>>();
        HashSet<HeavenRoom> visited = new HashSet<HeavenRoom>();
        
        foreach (HeavenRoom room in generatedRooms)
        {
            if (!visited.Contains(room))
            {
                List<HeavenRoom> component = new List<HeavenRoom>();
                Queue<HeavenRoom> queue = new Queue<HeavenRoom>();
                
                queue.Enqueue(room);
                visited.Add(room);
                
                while (queue.Count > 0)
                {
                    HeavenRoom current = queue.Dequeue();
                    component.Add(current);
                    
                    // Add all connected rooms to the queue
                    foreach (HeavenRoom connected in current.ConnectedRooms)
                    {
                        if (!visited.Contains(connected))
                        {
                            visited.Add(connected);
                            queue.Enqueue(connected);
                        }
                    }
                }
                
                components.Add(component);
            }
        }
        
        return components;
    }
    
    private HeavenRoom FindClosestRoom(List<HeavenRoom> group1, List<HeavenRoom> group2)
    {
        HeavenRoom closest = null;
        float minDistance = float.MaxValue;
        
        foreach (HeavenRoom room1 in group1)
        {
            foreach (HeavenRoom room2 in group2)
            {
                float distance = Vector3.Distance(room1.WorldPosition, room2.WorldPosition);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closest = room1;
                }
            }
        }
        
        return closest;
    }
    
    private IEnumerator ConnectRoomsCoroutine()
    {
        // First, connect adjacent rooms in matrix
        yield return StartCoroutine(ConnectAdjacentRoomsCoroutine());
        
        // Then, ensure all rooms are accessible by connecting isolated groups
        yield return StartCoroutine(EnsureAllRoomsAccessibleCoroutine());
        
        Debug.Log($"Created hallway connections using coroutines");
    }
    
    private IEnumerator ConnectAdjacentRoomsCoroutine()
    {
        int connectionsCreated = 0;
        
        // Connect rooms that are within (minRoomSeparation + 1) blocks distance
        // This allows connections through hallways in the empty space between rooms
        int connectionDistance = minRoomSeparation + 1;
        
        for (int i = 0; i < generatedRooms.Count; i++)
        {
            HeavenRoom currentRoom = generatedRooms[i];
            
            // Check for rooms within connection distance (North, South, East, West)
            int[] dx = { -connectionDistance, connectionDistance, 0, 0 }; // North, South, East, West
            int[] dy = { 0, 0, -connectionDistance, connectionDistance };
            
            for (int dir = 0; dir < 4; dir++)
            {
                int newX = currentRoom.MatrixX + dx[dir];
                int newY = currentRoom.MatrixY + dy[dir];
                
                // Check if position is within bounds
                if (newX >= 0 && newX < dungeonWidth && newY >= 0 && newY < dungeonHeight)
                {
                    // Find room at this position
                    HeavenRoom nearbyRoom = generatedRooms.Find(room => 
                        room.MatrixX == newX && room.MatrixY == newY);
                    
                    if (nearbyRoom != null && !currentRoom.IsConnectedTo(nearbyRoom))
                    {
                        // Verify there's a clear path (the middle cells should be empty)
                        bool clearPath = true;
                        for (int step = 1; step < connectionDistance; step++)
                        {
                            int midX = currentRoom.MatrixX + (dx[dir] * step) / connectionDistance;
                            int midY = currentRoom.MatrixY + (dy[dir] * step) / connectionDistance;
                            
                            if (midX >= 0 && midX < dungeonWidth && midY >= 0 && midY < dungeonHeight)
                            {
                                if (dungeonMatrix[midX, midY] != 0)
                                {
                                    clearPath = false;
                                    break;
                                }
                            }
                        }
                        
                        // If path is clear, create hallway
                        if (clearPath)
                        {
                            CreateHallway(currentRoom, nearbyRoom);
                            connectionsCreated++;
                            
                            if (connectionsCreated % 3 == 0)
                            {
                                yield return null; // Yield every 3 connections
                            }
                        }
                    }
                }
            }
        }
        
        Debug.Log($"Created {connectionsCreated} adjacent hallway connections");
    }
    
    private IEnumerator EnsureAllRoomsAccessibleCoroutine()
    {
        // Find all connected components using BFS
        List<List<HeavenRoom>> connectedComponents = FindConnectedComponents();
        
        // If there are multiple components, connect them
        if (connectedComponents.Count > 1)
        {
            Debug.Log($"Found {connectedComponents.Count} disconnected room groups. Connecting them...");
            
            int connectionsCreated = 0;
            
            // Connect each component to the next one
            for (int i = 0; i < connectedComponents.Count - 1; i++)
            {
                HeavenRoom room1 = FindClosestRoom(connectedComponents[i], connectedComponents[i + 1]);
                HeavenRoom room2 = FindClosestRoom(connectedComponents[i + 1], connectedComponents[i]);
                
                if (room1 != null && room2 != null)
                {
                    CreateHallway(room1, room2);
                    connectionsCreated++;
                    Debug.Log($"Connected {room1.RoomType} to {room2.RoomType} to ensure accessibility");
                    
                    if (connectionsCreated % 2 == 0)
                    {
                        yield return null; // Yield every 2 connections
                    }
                }
            }
            
            Debug.Log($"Created {connectionsCreated} accessibility connections");
        }
    }
    
    private void CreateHallway(HeavenRoom from, HeavenRoom to)
    {
        Vector3 direction = (to.WorldPosition - from.WorldPosition).normalized;
        float distance = Vector3.Distance(from.WorldPosition, to.WorldPosition);
        
        Vector3 hallwayPosition = from.WorldPosition + direction * (distance / 2f);
        Quaternion hallwayRotation = Quaternion.LookRotation(direction);
        
        GameObject hallway = Instantiate(heavenHallwayPrefab, hallwayPosition, hallwayRotation, dungeonParent);
        hallway.transform.localScale = new Vector3(1f, 2f, distance);
        hallway.name = $"Hallway_{from.RoomType}_{to.RoomType}";
        
        allHallwayInstances.Add(hallway);
    }
    
    private void SpawnPlayerAtStart()
    {
        HeavenRoom startRoom = generatedRooms.Find(r => r.RoomType == HeavenRoomType.Sanctuary);
        if (startRoom != null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                player.transform.position = startRoom.WorldPosition + Vector3.up * 2f;
                Debug.Log("Player spawned at Heaven Sanctuary");
            }
        }
    }
    
    private void ApplyHeavenTheme()
    {
        // Apply heaven-themed lighting and materials
        RenderSettings.ambientLight = heavenAmbientColor;
        
        // Apply materials to all room instances
        foreach (GameObject room in allRoomInstances)
        {
            ApplyHeavenMaterials(room);
        }
    }
    
    private void ApplyHeavenMaterials(GameObject room)
    {
        Renderer[] renderers = room.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            if (renderer.name.Contains("Floor") && heavenFloorMaterial != null)
            {
                renderer.material = heavenFloorMaterial;
            }
            else if (renderer.name.Contains("Wall") && heavenWallMaterial != null)
            {
                renderer.material = heavenWallMaterial;
            }
        }
    }
    
    // Public methods for external control
    public void RegenerateDungeon()
    {
        ClearDungeon();
        InitializeDungeon();
        if (useCoroutines)
        {
            StartCoroutine(GenerateDungeonCoroutine());
        }
        else
        {
            GenerateDungeonImmediate();
        }
    }
    
    public void ClearDungeon()
    {
        if (dungeonParent != null)
        {
            DestroyImmediate(dungeonParent.gameObject);
        }
        
        generatedRooms.Clear();
        allRoomInstances.Clear();
        allHallwayInstances.Clear();
    }
    
    // Getters for external access
    public List<HeavenRoom> GetGeneratedRooms() => generatedRooms;
    public List<GameObject> GetAllRoomInstances() => allRoomInstances;
    public List<GameObject> GetAllHallwayInstances() => allHallwayInstances;
    public int[,] GetDungeonMatrix() => dungeonMatrix;
    
    // Debug methods
    public void PrintMatrix()
    {
        Debug.Log("=== DUNGEON MATRIX ===");
        Debug.Log($"Matrix Size: {dungeonWidth}x{dungeonHeight}");
        Debug.Log($"Block Size: {blockSize}");
        Debug.Log($"Total Rooms Generated: {generatedRooms.Count}");
        
        string matrixString = "";
        for (int y = dungeonHeight - 1; y >= 0; y--) // Print from top to bottom
        {
            matrixString += $"Row {y:00}: ";
            for (int x = 0; x < dungeonWidth; x++)
            {
                matrixString += dungeonMatrix[x, y] == 1 ? "█" : "·";
            }
            matrixString += "\n";
        }
        Debug.Log(matrixString);
    }
    
    public void PrintRoomPositions()
    {
        Debug.Log("=== ROOM POSITIONS ===");
        foreach (HeavenRoom room in generatedRooms)
        {
            Debug.Log($"{room.RoomType} at matrix ({room.MatrixX}, {room.MatrixY}) -> world {room.WorldPosition}");
        }
    }
    
    // Test method to verify generation
    public void TestGeneration()
    {
        Debug.Log("=== TESTING GENERATION ===");
        Debug.Log($"Dungeon Width: {dungeonWidth}");
        Debug.Log($"Dungeon Height: {dungeonHeight}");
        Debug.Log($"Block Size: {blockSize}");
        Debug.Log($"Min Rooms: {minRooms}");
        Debug.Log($"Max Rooms: {maxRooms}");
        
        // Clear existing rooms
        ClearDungeon();
        InitializeDungeon();
        
        // Test room creation at specific positions
        Debug.Log("Testing room creation at (0,0)...");
        CreateRoomAtPosition(0, 0, HeavenRoomType.Chapel);
        
        Debug.Log("Testing room creation at (1,1)...");
        CreateRoomAtPosition(1, 1, HeavenRoomType.Garden);
        
        Debug.Log("Testing room creation at (2,2)...");
        CreateRoomAtPosition(2, 2, HeavenRoomType.Library);
        
        // Verify positions in scene
        Debug.Log("=== VERIFYING POSITIONS IN SCENE ===");
        foreach (GameObject room in allRoomInstances)
        {
            Debug.Log($"Room: {room.name} at position {room.transform.position}");
            Debug.Log($"  - Parent: {room.transform.parent.name}");
            Debug.Log($"  - Local Position: {room.transform.localPosition}");
            Debug.Log($"  - Scale: {room.transform.localScale}");
            
            // Check if room has a prefab child
            Transform prefabChild = room.transform.Find($"Prefab_{room.name.Split('_')[1]}_{room.name.Split('_')[2]}_{room.name.Split('_')[3]}");
            if (prefabChild != null)
            {
                Debug.Log($"  - Prefab Child Position: {prefabChild.position}");
                Debug.Log($"  - Prefab Child Local Position: {prefabChild.localPosition}");
            }
            
            // Check if position matches expected
            HeavenRoom roomComponent = room.GetComponent<HeavenRoom>();
            if (roomComponent != null)
            {
                Vector3 expectedPos = roomComponent.WorldPosition;
                Vector3 actualPos = room.transform.position;
                if (Vector3.Distance(expectedPos, actualPos) > 0.1f)
                {
                    Debug.LogError($"  - POSITION MISMATCH! Expected: {expectedPos}, Actual: {actualPos}");
                }
                else
                {
                    Debug.Log($"  - ✅ Position correct: {actualPos}");
                }
            }
        }
        
        PrintMatrix();
        PrintRoomPositions();
        PrintConnectivity();
    }
    
    // Debug method to print connectivity information
    public void PrintConnectivity()
    {
        Debug.Log("=== CONNECTIVITY ANALYSIS ===");
        
        List<List<HeavenRoom>> components = FindConnectedComponents();
        Debug.Log($"Total connected components: {components.Count}");
        
        for (int i = 0; i < components.Count; i++)
        {
            Debug.Log($"Component {i + 1}: {components[i].Count} rooms");
            foreach (HeavenRoom room in components[i])
            {
                Debug.Log($"  - {room.RoomType} at ({room.MatrixX}, {room.MatrixY}) - {room.ConnectedRooms.Count} connections");
            }
        }
        
        // Check if all rooms are accessible
        if (components.Count == 1)
        {
            Debug.Log("✅ All rooms are connected and accessible!");
        }
        else
        {
            Debug.Log($"⚠️ {components.Count - 1} room groups are isolated!");
        }
    }
    
    // Debug method to diagnose positioning issues
    public void DiagnosePositioning()
    {
        Debug.Log("=== POSITIONING DIAGNOSIS ===");
        Debug.Log($"Dungeon Matrix: {dungeonWidth}x{dungeonHeight}");
        Debug.Log($"Block Size: {blockSize}");
        Debug.Log($"Min Room Separation: {minRoomSeparation} blocks");
        Debug.Log($"Total Rooms: {generatedRooms.Count}");
        Debug.Log($"Total Room Instances: {allRoomInstances.Count}");
        
        // Check for duplicate positions
        Dictionary<Vector3, List<GameObject>> positionGroups = new Dictionary<Vector3, List<GameObject>>();
        
        foreach (GameObject room in allRoomInstances)
        {
            Vector3 pos = room.transform.position;
            if (!positionGroups.ContainsKey(pos))
            {
                positionGroups[pos] = new List<GameObject>();
            }
            positionGroups[pos].Add(room);
        }
        
        // Report duplicates
        foreach (var group in positionGroups)
        {
            if (group.Value.Count > 1)
            {
                Debug.LogError($"DUPLICATE POSITION: {group.Key} has {group.Value.Count} rooms:");
                foreach (GameObject room in group.Value)
                {
                    Debug.LogError($"  - {room.name}");
                }
            }
        }
        
        // Report all positions
        Debug.Log("=== ALL ROOM POSITIONS ===");
        foreach (GameObject room in allRoomInstances)
        {
            Debug.Log($"{room.name}: {room.transform.position} (Parent: {room.transform.parent.name})");
        }
    }
    
    // Visual debugging method
    private void OnDrawGizmos()
    {
        if (generatedRooms == null) return;
        
        // Draw room positions
        foreach (HeavenRoom room in generatedRooms)
        {
            // Different colors for different room types
            switch (room.RoomType)
            {
                case HeavenRoomType.Sanctuary:
                    Gizmos.color = Color.yellow;
                    break;
                case HeavenRoomType.Chapel:
                    Gizmos.color = Color.blue;
                    break;
                case HeavenRoomType.Garden:
                    Gizmos.color = Color.green;
                    break;
                case HeavenRoomType.Library:
                    Gizmos.color = Color.magenta;
                    break;
                case HeavenRoomType.Altar:
                    Gizmos.color = Color.red;
                    break;
                case HeavenRoomType.Treasury:
                    Gizmos.color = Color.cyan;
                    break;
                case HeavenRoomType.Boss:
                    Gizmos.color = Color.black;
                    break;
                default:
                    Gizmos.color = Color.white;
                    break;
            }
            
            // Draw room as a cube
            Gizmos.DrawWireCube(room.WorldPosition, Vector3.one * blockSize);
            
            // Draw room label
            #if UNITY_EDITOR
            UnityEditor.Handles.Label(room.WorldPosition + Vector3.up * 2, $"{room.RoomType}\n({room.MatrixX},{room.MatrixY})");
            #endif
        }
        
        // Draw connections
        Gizmos.color = Color.white;
        foreach (HeavenRoom room in generatedRooms)
        {
            foreach (HeavenRoom connectedRoom in room.ConnectedRooms)
            {
                Gizmos.DrawLine(room.WorldPosition, connectedRoom.WorldPosition);
            }
        }
    }
}
