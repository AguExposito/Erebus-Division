using UnityEngine;

/// <summary>
/// Ejemplo de cómo usar el generador de mazmorras procedurales
/// Este script muestra diferentes formas de interactuar con el generador
/// </summary>
public class EjemploUso : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Referencia al generador de mazmorras")]
    [SerializeField] private ProceduralDungeonGenerator dungeonGenerator;
    
    [Header("Configuración de Prueba")]
    [Tooltip("Regenerar mazmorra al presionar tecla")]
    [SerializeField] private KeyCode regenerateKey = KeyCode.R;
    
    [Tooltip("Mostrar información de debug")]
    [SerializeField] private bool showDebugInfo = true;
    
    private void Start()
    {
        // Si no hay referencia asignada, intentar encontrarla
        if (dungeonGenerator == null)
        {
            dungeonGenerator = FindObjectOfType<ProceduralDungeonGenerator>();
        }
        
        if (dungeonGenerator == null)
        {
            Debug.LogWarning("No se encontró ProceduralDungeonGenerator. Asegúrate de tener uno en la escena.");
        }
    }
    
    private void Update()
    {
        // Regenerar mazmorra con tecla
        if (Input.GetKeyDown(regenerateKey))
        {
            RegenerateDungeon();
        }
        
        // Mostrar información de debug
        if (showDebugInfo && Input.GetKeyDown(KeyCode.I))
        {
            ShowDungeonInfo();
        }
    }
    
    /// <summary>
    /// Regenera la mazmorra
    /// </summary>
    public void RegenerateDungeon()
    {
        if (dungeonGenerator != null)
        {
            Debug.Log("Regenerando mazmorra...");
            dungeonGenerator.RegenerateDungeon();
            Debug.Log("Mazmorra regenerada!");
        }
    }
    
    /// <summary>
    /// Muestra información sobre la mazmorra generada
    /// </summary>
    public void ShowDungeonInfo()
    {
        if (dungeonGenerator == null) return;
        
        var rooms = dungeonGenerator.GetGeneratedRooms();
        var startRoom = dungeonGenerator.GetStartRoom();
        
        Debug.Log("=== INFORMACIÓN DE LA MAZMORRA ===");
        Debug.Log($"Total de habitaciones: {rooms.Count}");
        Debug.Log($"Habitación inicial: {startRoom?.RoomType}");
        
        // Contar habitaciones por tipo
        var roomTypes = new System.Collections.Generic.Dictionary<DungeonRoomType, int>();
        foreach (var room in rooms)
        {
            if (!roomTypes.ContainsKey(room.RoomType))
            {
                roomTypes[room.RoomType] = 0;
            }
            roomTypes[room.RoomType]++;
        }
        
        Debug.Log("Distribución de habitaciones:");
        foreach (var kvp in roomTypes)
        {
            Debug.Log($"  {kvp.Key}: {kvp.Value}");
        }
        
        // Contar conexiones
        int totalConnections = 0;
        int deadEnds = 0;
        foreach (var room in rooms)
        {
            totalConnections += room.GetConnectionCount();
            if (room.IsDeadEnd())
            {
                deadEnds++;
            }
        }
        
        Debug.Log($"Total de conexiones: {totalConnections}");
        Debug.Log($"Callejones sin salida: {deadEnds}");
    }
    
    /// <summary>
    /// Encuentra la habitación más cercana a una posición
    /// </summary>
    public DungeonRoom FindClosestRoom(Vector3 position)
    {
        if (dungeonGenerator == null) return null;
        
        var rooms = dungeonGenerator.GetGeneratedRooms();
        if (rooms.Count == 0) return null;
        
        DungeonRoom closest = null;
        float minDistance = float.MaxValue;
        
        foreach (var room in rooms)
        {
            float distance = Vector3.Distance(position, room.WorldPosition);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = room;
            }
        }
        
        return closest;
    }
    
    /// <summary>
    /// Obtiene todas las habitaciones de un tipo específico
    /// </summary>
    public System.Collections.Generic.List<DungeonRoom> GetRoomsOfType(DungeonRoomType roomType)
    {
        if (dungeonGenerator == null) return new System.Collections.Generic.List<DungeonRoom>();
        
        var allRooms = dungeonGenerator.GetGeneratedRooms();
        var filteredRooms = new System.Collections.Generic.List<DungeonRoom>();
        
        foreach (var room in allRooms)
        {
            if (room.RoomType == roomType)
            {
                filteredRooms.Add(room);
            }
        }
        
        return filteredRooms;
    }
    
    /// <summary>
    /// Visita todas las habitaciones conectadas desde una habitación inicial
    /// Útil para debugging o para sistemas de descubrimiento
    /// </summary>
    public void VisitAllConnectedRooms(DungeonRoom startRoom)
    {
        if (startRoom == null) return;
        
        var visited = new System.Collections.Generic.HashSet<DungeonRoom>();
        var queue = new System.Collections.Generic.Queue<DungeonRoom>();
        
        queue.Enqueue(startRoom);
        visited.Add(startRoom);
        
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            current.VisitRoom();
            
            foreach (var connected in current.ConnectedRooms)
            {
                if (!visited.Contains(connected))
                {
                    visited.Add(connected);
                    queue.Enqueue(connected);
                }
            }
        }
        
        Debug.Log($"Visitadas {visited.Count} habitaciones desde {startRoom.RoomType}");
    }
    
    // Método para ser llamado desde UI
    public void OnRegenerateButtonClicked()
    {
        RegenerateDungeon();
    }
    
    // Método para ser llamado desde UI
    public void OnShowInfoButtonClicked()
    {
        ShowDungeonInfo();
    }
}

