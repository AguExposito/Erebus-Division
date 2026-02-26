using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generador de mazmorras procedurales
/// Crea una estructura de habitaciones conectadas en una sola planta
/// </summary>
public class ProceduralDungeonGenerator : MonoBehaviour
{
    [Header("Configuración de la Mazmorra")]
    [Tooltip("Ancho de la cuadrícula de generación")]
    [SerializeField] private int gridWidth = 20;
    
    [Tooltip("Alto de la cuadrícula de generación")]
    [SerializeField] private int gridHeight = 20;
    
    [Tooltip("Tamaño de cada celda en unidades del mundo")]
    [SerializeField] private float cellSize = 10f;
    
    [Header("Generación de Habitaciones")]
    [Tooltip("Número mínimo de habitaciones a generar")]
    [SerializeField] private int minRooms = 8;
    
    [Tooltip("Número máximo de habitaciones a generar")]
    [SerializeField] private int maxRooms = 15;
    
    [Tooltip("Separación mínima entre habitaciones (en celdas)")]
    [SerializeField] private int minRoomSeparation = 2;
    
    [Header("Prefabs")]
    [Tooltip("Prefab base para habitaciones (se usa si no hay prefabs específicos)")]
    [SerializeField] private GameObject roomPrefab;
    
    [Tooltip("Prefab para pasillos")]
    [SerializeField] private GameObject hallwayPrefab;
    
    [Tooltip("ScriptableObject con prefabs específicos por tipo de habitación")]
    [SerializeField] private RoomPrefabsSO roomPrefabsSO;
    
    [Tooltip("Prefabs específicos por tipo de habitación (alternativa al SO)")]
    [SerializeField] private RoomPrefabs roomPrefabs;
    
    [Header("Configuración de Conexiones")]
    [Tooltip("Probabilidad de conectar habitaciones adyacentes (0-100)")]
    [SerializeField] private float connectionChance = 70f;
    
    [Tooltip("Asegurar que todas las habitaciones sean accesibles")]
    [SerializeField] private bool ensureAllRoomsAccessible = true;
    
    [Header("Generación Asíncrona")]
    [Tooltip("Usar corrutinas para generar la mazmorra")]
    [SerializeField] private bool useCoroutines = true;
    
    [Tooltip("Habitaciones generadas por frame (si usa corrutinas)")]
    [SerializeField] private int roomsPerFrame = 2;
    
    // Variables privadas
    private int[,] dungeonGrid; // 0 = vacío, 1 = habitación, 2 = pasillo
    private List<DungeonRoom> generatedRooms = new List<DungeonRoom>();
    private List<GameObject> allRoomInstances = new List<GameObject>();
    private List<GameObject> allHallwayInstances = new List<GameObject>();
    private Transform dungeonParent;
    
    // Habitación inicial
    private DungeonRoom startRoom;
    
    void Start()
    {
        GenerateDungeon();
    }
    
    /// <summary>
    /// Genera la mazmorra completa
    /// </summary>
    public void GenerateDungeon()
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
        // Crear objeto padre para organizar la mazmorra
        if (dungeonParent != null)
        {
            DestroyImmediate(dungeonParent.gameObject);
        }
        
        dungeonParent = new GameObject("ProceduralDungeon").transform;
        dungeonParent.SetParent(transform);
        
        // Inicializar cuadrícula
        dungeonGrid = new int[gridWidth, gridHeight];
        
        // Limpiar listas
        generatedRooms.Clear();
        allRoomInstances.Clear();
        allHallwayInstances.Clear();
        
        // Llenar cuadrícula con espacios vacíos
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                dungeonGrid[x, y] = 0; // 0 = vacío
            }
        }
        
        Debug.Log($"Inicializada mazmorra procedural: {gridWidth}x{gridHeight}, Tamaño de celda: {cellSize}");
    }
    
    private void GenerateDungeonImmediate()
    {
        GenerateRooms();
        ConnectRooms();
        SpawnPlayerAtStart();
        Debug.Log("Mazmorra generada completamente");
    }
    
    private IEnumerator GenerateDungeonCoroutine()
    {
        yield return StartCoroutine(GenerateRoomsCoroutine());
        yield return StartCoroutine(ConnectRoomsCoroutine());
        SpawnPlayerAtStart();
        Debug.Log("Mazmorra generada completamente (corrutinas)");
    }
    
    /// <summary>
    /// Genera las habitaciones en la cuadrícula
    /// </summary>
    private void GenerateRooms()
    {
        int roomsToGenerate = Random.Range(minRooms, maxRooms + 1);
        int roomsCreated = 0;
        int maxAttempts = gridWidth * gridHeight * 3;
        int attempts = 0;
        
        // Crear habitación inicial en el centro
        int centerX = gridWidth / 2;
        int centerY = gridHeight / 2;
        CreateRoomAtPosition(centerX, centerY, DungeonRoomType.Entrance);
        roomsCreated++;
        startRoom = generatedRooms[0];
        
        // Generar habitaciones aleatorias
        while (roomsCreated < roomsToGenerate && attempts < maxAttempts)
        {
            int x = Random.Range(0, gridWidth);
            int y = Random.Range(0, gridHeight);
            attempts++;
            
            if (IsValidRoomPosition(x, y))
            {
                DungeonRoomType roomType = GetRandomRoomType(roomsCreated, roomsToGenerate);
                CreateRoomAtPosition(x, y, roomType);
                roomsCreated++;
            }
        }
        
        Debug.Log($"Generadas {roomsCreated} habitaciones (intentos: {attempts})");
    }
    
    private IEnumerator GenerateRoomsCoroutine()
    {
        int roomsToGenerate = Random.Range(minRooms, maxRooms + 1);
        int roomsCreated = 0;
        int maxAttempts = gridWidth * gridHeight * 3;
        int attempts = 0;
        
        // Crear habitación inicial en el centro
        int centerX = gridWidth / 2;
        int centerY = gridHeight / 2;
        CreateRoomAtPosition(centerX, centerY, DungeonRoomType.Entrance);
        roomsCreated++;
        startRoom = generatedRooms[0];
        
        // Generar habitaciones aleatorias
        while (roomsCreated < roomsToGenerate && attempts < maxAttempts)
        {
            int x = Random.Range(0, gridWidth);
            int y = Random.Range(0, gridHeight);
            attempts++;
            
            if (IsValidRoomPosition(x, y))
            {
                DungeonRoomType roomType = GetRandomRoomType(roomsCreated, roomsToGenerate);
                CreateRoomAtPosition(x, y, roomType);
                roomsCreated++;
                
                if (roomsCreated % roomsPerFrame == 0)
                {
                    yield return null;
                }
            }
        }
        
        Debug.Log($"Generadas {roomsCreated} habitaciones usando corrutinas (intentos: {attempts})");
    }
    
    /// <summary>
    /// Verifica si una posición es válida para colocar una habitación
    /// </summary>
    private bool IsValidRoomPosition(int x, int y)
    {
        // Verificar límites
        if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight)
            return false;
        
        // Verificar que la celda esté vacía
        if (dungeonGrid[x, y] != 0)
            return false;
        
        // Verificar separación mínima con otras habitaciones
        for (int dx = -minRoomSeparation; dx <= minRoomSeparation; dx++)
        {
            for (int dy = -minRoomSeparation; dy <= minRoomSeparation; dy++)
            {
                if (dx == 0 && dy == 0) continue;
                
                int checkX = x + dx;
                int checkY = y + dy;
                
                if (checkX >= 0 && checkX < gridWidth && checkY >= 0 && checkY < gridHeight)
                {
                    if (dungeonGrid[checkX, checkY] == 1) // 1 = habitación
                    {
                        return false;
                    }
                }
            }
        }
        
        return true;
    }
    
    /// <summary>
    /// Crea una habitación en la posición especificada
    /// </summary>
    private void CreateRoomAtPosition(int x, int y, DungeonRoomType roomType)
    {
        // Marcar posición como ocupada
        dungeonGrid[x, y] = 1; // 1 = habitación
        
        // Calcular posición en el mundo
        float worldX = (x - gridWidth / 2f) * cellSize;
        float worldZ = (y - gridHeight / 2f) * cellSize;
        Vector3 worldPos = new Vector3(worldX, 0, worldZ);
        
        // Obtener prefab específico o usar el genérico
        GameObject prefabToUse = GetRoomPrefab(roomType);
        if (prefabToUse == null)
        {
            prefabToUse = roomPrefab;
        }
        
        // Crear instancia de habitación
        GameObject roomObj;
        if (prefabToUse != null)
        {
            roomObj = Instantiate(prefabToUse, worldPos, Quaternion.identity, dungeonParent);
        }
        else
        {
            // Crear cubo básico si no hay prefab
            roomObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            roomObj.transform.SetParent(dungeonParent, false);
            roomObj.transform.position = worldPos;
            roomObj.transform.localScale = Vector3.one * cellSize;
        }
        
        roomObj.name = $"Room_{roomType}_{x}_{y}";
        
        // Agregar componente DungeonRoom
        DungeonRoom room = roomObj.GetComponent<DungeonRoom>();
        if (room == null)
        {
            room = roomObj.AddComponent<DungeonRoom>();
        }
        
        room.Initialize(roomType, x, y, worldPos, cellSize);
        
        generatedRooms.Add(room);
        allRoomInstances.Add(roomObj);
        
        Debug.Log($"Creada habitación {roomType} en ({x}, {y}) -> {worldPos}");
    }
    
    /// <summary>
    /// Obtiene el prefab específico para un tipo de habitación
    /// </summary>
    private GameObject GetRoomPrefab(DungeonRoomType roomType)
    {
        // Prioridad: ScriptableObject > Estructura directa > Prefab genérico
        if (roomPrefabsSO != null)
        {
            GameObject prefab = roomPrefabsSO.GetPrefabForRoomType(roomType);
            if (prefab != null) return prefab;
        }
        
        if (roomPrefabs != null)
        {
            switch (roomType)
            {
                case DungeonRoomType.Entrance:
                    if (roomPrefabs.entrancePrefab != null) return roomPrefabs.entrancePrefab;
                    break;
                case DungeonRoomType.Office:
                    if (roomPrefabs.officePrefab != null) return roomPrefabs.officePrefab;
                    break;
                case DungeonRoomType.Storage:
                    if (roomPrefabs.storagePrefab != null) return roomPrefabs.storagePrefab;
                    break;
                case DungeonRoomType.Hallway:
                    if (roomPrefabs.hallwayPrefab != null) return roomPrefabs.hallwayPrefab;
                    break;
                case DungeonRoomType.LargeRoom:
                    if (roomPrefabs.largeRoomPrefab != null) return roomPrefabs.largeRoomPrefab;
                    break;
                case DungeonRoomType.DeadEnd:
                    if (roomPrefabs.deadEndPrefab != null) return roomPrefabs.deadEndPrefab;
                    break;
            }
        }
        
        // Fallback al prefab genérico
        return roomPrefab;
    }
    
    /// <summary>
    /// Obtiene un tipo de habitación aleatorio
    /// </summary>
    private DungeonRoomType GetRandomRoomType(int currentRoom, int totalRooms)
    {
        // La última habitación puede ser especial
        if (currentRoom == totalRooms - 1)
        {
            int specialRoll = Random.Range(0, 100);
            if (specialRoll < 30) return DungeonRoomType.LargeRoom;
            if (specialRoll < 60) return DungeonRoomType.Storage;
            return DungeonRoomType.Office;
        }
        
        // Distribución de tipos de habitaciones
        int roll = Random.Range(0, 100);
        
        if (roll < 40) return DungeonRoomType.Office;      // 40% oficinas
        if (roll < 70) return DungeonRoomType.Storage;     // 30% almacenes
        if (roll < 85) return DungeonRoomType.LargeRoom;  // 15% habitaciones grandes
        return DungeonRoomType.DeadEnd;                    // 15% callejones sin salida
    }
    
    /// <summary>
    /// Conecta las habitaciones con pasillos
    /// </summary>
    private void ConnectRooms()
    {
        // Conectar habitaciones adyacentes
        ConnectAdjacentRooms();
        
        // Asegurar que todas las habitaciones sean accesibles
        if (ensureAllRoomsAccessible)
        {
            EnsureAllRoomsAccessible();
        }
        
        Debug.Log("Habitaciones conectadas");
    }
    
    private IEnumerator ConnectRoomsCoroutine()
    {
        yield return StartCoroutine(ConnectAdjacentRoomsCoroutine());
        
        if (ensureAllRoomsAccessible)
        {
            yield return StartCoroutine(EnsureAllRoomsAccessibleCoroutine());
        }
        
        Debug.Log("Habitaciones conectadas (corrutinas)");
    }
    
    /// <summary>
    /// Conecta habitaciones que están cerca unas de otras
    /// </summary>
    private void ConnectAdjacentRooms()
    {
        for (int i = 0; i < generatedRooms.Count; i++)
        {
            DungeonRoom currentRoom = generatedRooms[i];
            
            // Buscar habitaciones cercanas en las 4 direcciones
            Vector2Int[] directions = {
                new Vector2Int(0, 1),   // Norte
                new Vector2Int(0, -1),  // Sur
                new Vector2Int(1, 0),   // Este
                new Vector2Int(-1, 0)   // Oeste
            };
            
            foreach (Vector2Int dir in directions)
            {
                int checkX = currentRoom.GridX + dir.x * (minRoomSeparation + 1);
                int checkY = currentRoom.GridY + dir.y * (minRoomSeparation + 1);
                
                if (checkX >= 0 && checkX < gridWidth && checkY >= 0 && checkY < gridHeight)
                {
                    DungeonRoom nearbyRoom = generatedRooms.Find(r => 
                        r.GridX == checkX && r.GridY == checkY);
                    
                    if (nearbyRoom != null && !currentRoom.IsConnectedTo(nearbyRoom))
                    {
                        // Verificar si hay un camino claro
                        if (HasClearPath(currentRoom, nearbyRoom))
                        {
                            // Probabilidad de conexión
                            if (Random.Range(0f, 100f) < connectionChance)
                            {
                                CreateHallway(currentRoom, nearbyRoom);
                            }
                        }
                    }
                }
            }
        }
    }
    
    private IEnumerator ConnectAdjacentRoomsCoroutine()
    {
        int connectionsCreated = 0;
        
        for (int i = 0; i < generatedRooms.Count; i++)
        {
            DungeonRoom currentRoom = generatedRooms[i];
            
            Vector2Int[] directions = {
                new Vector2Int(0, 1),
                new Vector2Int(0, -1),
                new Vector2Int(1, 0),
                new Vector2Int(-1, 0)
            };
            
            foreach (Vector2Int dir in directions)
            {
                int checkX = currentRoom.GridX + dir.x * (minRoomSeparation + 1);
                int checkY = currentRoom.GridY + dir.y * (minRoomSeparation + 1);
                
                if (checkX >= 0 && checkX < gridWidth && checkY >= 0 && checkY < gridHeight)
                {
                    DungeonRoom nearbyRoom = generatedRooms.Find(r => 
                        r.GridX == checkX && r.GridY == checkY);
                    
                    if (nearbyRoom != null && !currentRoom.IsConnectedTo(nearbyRoom))
                    {
                        if (HasClearPath(currentRoom, nearbyRoom))
                        {
                            if (Random.Range(0f, 100f) < connectionChance)
                            {
                                CreateHallway(currentRoom, nearbyRoom);
                                connectionsCreated++;
                                
                                if (connectionsCreated % 3 == 0)
                                {
                                    yield return null;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Verifica si hay un camino claro entre dos habitaciones
    /// </summary>
    private bool HasClearPath(DungeonRoom from, DungeonRoom to)
    {
        int dx = to.GridX - from.GridX;
        int dy = to.GridY - from.GridY;
        
        // Verificar si es una conexión directa (horizontal o vertical)
        if (dx != 0 && dy != 0)
        {
            return false; // No permitir conexiones diagonales directas
        }
        
        // Verificar que el camino esté libre
        int steps = Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy));
        int stepX = dx != 0 ? (dx > 0 ? 1 : -1) : 0;
        int stepY = dy != 0 ? (dy > 0 ? 1 : -1) : 0;
        
        for (int i = 1; i < steps; i++)
        {
            int checkX = from.GridX + stepX * i;
            int checkY = from.GridY + stepY * i;
            
            if (checkX >= 0 && checkX < gridWidth && checkY >= 0 && checkY < gridHeight)
            {
                // Permitir pasillos pero no otras habitaciones bloqueando
                if (dungeonGrid[checkX, checkY] == 1) // 1 = habitación
                {
                    return false;
                }
            }
        }
        
        return true;
    }
    
    /// <summary>
    /// Asegura que todas las habitaciones sean accesibles
    /// </summary>
    private void EnsureAllRoomsAccessible()
    {
        List<List<DungeonRoom>> components = FindConnectedComponents();
        
        if (components.Count > 1)
        {
            Debug.Log($"Encontrados {components.Count} grupos desconectados. Conectándolos...");
            
            for (int i = 0; i < components.Count - 1; i++)
            {
                DungeonRoom room1 = FindClosestRoom(components[i], components[i + 1]);
                DungeonRoom room2 = FindClosestRoom(components[i + 1], components[i]);
                
                if (room1 != null && room2 != null)
                {
                    CreateHallway(room1, room2);
                    Debug.Log($"Conectada {room1.RoomType} a {room2.RoomType} para asegurar accesibilidad");
                }
            }
        }
    }
    
    private IEnumerator EnsureAllRoomsAccessibleCoroutine()
    {
        List<List<DungeonRoom>> components = FindConnectedComponents();
        
        if (components.Count > 1)
        {
            Debug.Log($"Encontrados {components.Count} grupos desconectados. Conectándolos...");
            
            int connectionsCreated = 0;
            
            for (int i = 0; i < components.Count - 1; i++)
            {
                DungeonRoom room1 = FindClosestRoom(components[i], components[i + 1]);
                DungeonRoom room2 = FindClosestRoom(components[i + 1], components[i]);
                
                if (room1 != null && room2 != null)
                {
                    CreateHallway(room1, room2);
                    connectionsCreated++;
                    
                    if (connectionsCreated % 2 == 0)
                    {
                        yield return null;
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Encuentra todos los componentes conectados usando BFS
    /// </summary>
    private List<List<DungeonRoom>> FindConnectedComponents()
    {
        List<List<DungeonRoom>> components = new List<List<DungeonRoom>>();
        HashSet<DungeonRoom> visited = new HashSet<DungeonRoom>();
        
        foreach (DungeonRoom room in generatedRooms)
        {
            if (!visited.Contains(room))
            {
                List<DungeonRoom> component = new List<DungeonRoom>();
                Queue<DungeonRoom> queue = new Queue<DungeonRoom>();
                
                queue.Enqueue(room);
                visited.Add(room);
                
                while (queue.Count > 0)
                {
                    DungeonRoom current = queue.Dequeue();
                    component.Add(current);
                    
                    foreach (DungeonRoom connected in current.ConnectedRooms)
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
    
    /// <summary>
    /// Encuentra la habitación más cercana entre dos grupos
    /// </summary>
    private DungeonRoom FindClosestRoom(List<DungeonRoom> group1, List<DungeonRoom> group2)
    {
        DungeonRoom closest = null;
        float minDistance = float.MaxValue;
        
        foreach (DungeonRoom room1 in group1)
        {
            foreach (DungeonRoom room2 in group2)
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
    
    /// <summary>
    /// Crea un pasillo entre dos habitaciones
    /// </summary>
    private void CreateHallway(DungeonRoom from, DungeonRoom to)
    {
        // Conectar las habitaciones lógicamente
        from.ConnectToRoom(to);
        
        // Marcar celdas del pasillo en la cuadrícula
        MarkHallwayInGrid(from, to);
        
        // Crear el objeto visual del pasillo
        Vector3 direction = (to.WorldPosition - from.WorldPosition).normalized;
        float distance = Vector3.Distance(from.WorldPosition, to.WorldPosition);
        
        Vector3 hallwayPosition = from.WorldPosition + direction * (distance / 2f);
        Quaternion hallwayRotation = Quaternion.LookRotation(direction);
        
        GameObject hallway;
        if (hallwayPrefab != null)
        {
            hallway = Instantiate(hallwayPrefab, hallwayPosition, hallwayRotation, dungeonParent);
        }
        else
        {
            hallway = GameObject.CreatePrimitive(PrimitiveType.Cube);
            hallway.transform.SetParent(dungeonParent, false);
            hallway.transform.position = hallwayPosition;
            hallway.transform.rotation = hallwayRotation;
        }
        
        // Ajustar escala del pasillo
        hallway.transform.localScale = new Vector3(cellSize * 0.6f, cellSize * 0.8f, distance);
        hallway.name = $"Hallway_{from.RoomType}_{to.RoomType}";
        
        allHallwayInstances.Add(hallway);
    }
    
    /// <summary>
    /// Marca las celdas del pasillo en la cuadrícula
    /// </summary>
    private void MarkHallwayInGrid(DungeonRoom from, DungeonRoom to)
    {
        int dx = to.GridX - from.GridX;
        int dy = to.GridY - from.GridY;
        
        int steps = Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy));
        int stepX = dx != 0 ? (dx > 0 ? 1 : -1) : 0;
        int stepY = dy != 0 ? (dy > 0 ? 1 : -1) : 0;
        
        for (int i = 1; i < steps; i++)
        {
            int x = from.GridX + stepX * i;
            int y = from.GridY + stepY * i;
            
            if (x >= 0 && x < gridWidth && y >= 0 && y < gridHeight)
            {
                if (dungeonGrid[x, y] == 0) // Solo marcar si está vacío
                {
                    dungeonGrid[x, y] = 2; // 2 = pasillo
                }
            }
        }
    }
    
    /// <summary>
    /// Spawnea al jugador en la habitación inicial
    /// </summary>
    private void SpawnPlayerAtStart()
    {
        if (startRoom != null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                player.transform.position = startRoom.WorldPosition + Vector3.up * 2f;
                Debug.Log("Jugador spawneado en la entrada");
            }
        }
    }
    
    // Métodos públicos para control externo
    public void RegenerateDungeon()
    {
        ClearDungeon();
        GenerateDungeon();
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
        startRoom = null;
    }
    
    // Getters
    public List<DungeonRoom> GetGeneratedRooms() => generatedRooms;
    public List<GameObject> GetAllRoomInstances() => allRoomInstances;
    public List<GameObject> GetAllHallwayInstances() => allHallwayInstances;
    public int[,] GetDungeonGrid() => dungeonGrid;
    public DungeonRoom GetStartRoom() => startRoom;
    
    // Debug
    private void OnDrawGizmos()
    {
        if (generatedRooms == null) return;
        
        // Dibujar habitaciones
        foreach (DungeonRoom room in generatedRooms)
        {
            Gizmos.color = GetRoomColor(room.RoomType);
            Gizmos.DrawWireCube(room.WorldPosition, Vector3.one * cellSize);
        }
        
        // Dibujar conexiones
        Gizmos.color = Color.yellow;
        foreach (DungeonRoom room in generatedRooms)
        {
            foreach (DungeonRoom connected in room.ConnectedRooms)
            {
                Gizmos.DrawLine(room.WorldPosition, connected.WorldPosition);
            }
        }
    }
    
    private Color GetRoomColor(DungeonRoomType roomType)
    {
        switch (roomType)
        {
            case DungeonRoomType.Entrance: return Color.green;
            case DungeonRoomType.Office: return Color.blue;
            case DungeonRoomType.Storage: return Color.cyan;
            case DungeonRoomType.LargeRoom: return Color.magenta;
            case DungeonRoomType.DeadEnd: return Color.red;
            default: return Color.white;
        }
    }
}

/// <summary>
/// Tipos de habitaciones para mazmorras procedurales
/// </summary>
public enum DungeonRoomType
{
    Entrance,   // Entrada/Inicio
    Office,      // Oficina
    Storage,    // Almacén
    Hallway,    // Pasillo
    LargeRoom,  // Habitación grande
    DeadEnd     // Callejón sin salida
}

/// <summary>
/// Estructura para almacenar prefabs de habitaciones
/// </summary>
[System.Serializable]
public class RoomPrefabs
{
    public GameObject entrancePrefab;
    public GameObject officePrefab;
    public GameObject storagePrefab;
    public GameObject hallwayPrefab;
    public GameObject largeRoomPrefab;
    public GameObject deadEndPrefab;
}

