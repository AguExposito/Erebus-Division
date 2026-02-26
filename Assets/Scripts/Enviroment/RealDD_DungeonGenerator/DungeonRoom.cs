using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Representa una habitación individual en la mazmorra procedural
/// </summary>
public class DungeonRoom : MonoBehaviour
{
    [Header("Información de la Habitación")]
    public DungeonRoomType RoomType { get; private set; }
    public int GridX { get; private set; }
    public int GridY { get; private set; }
    public Vector3 WorldPosition { get; private set; }
    public float RoomSize { get; private set; }
    
    [Header("Estado de la Habitación")]
    public bool IsVisited { get; private set; }
    public bool IsCleared { get; private set; }
    public bool HasEnemies { get; private set; }
    public bool HasLoot { get; private set; }
    
    [Header("Conexiones")]
    public List<DungeonRoom> ConnectedRooms { get; private set; }
    
    [Header("Contenido de la Habitación")]
    public List<GameObject> Enemies { get; private set; }
    public List<GameObject> LootItems { get; private set; }
    public List<GameObject> Props { get; private set; }
    
    [Header("Sockets de Conexión")]
    public List<RoomSocket> RoomSockets { get; private set; }
    
    private void Awake()
    {
        ConnectedRooms = new List<DungeonRoom>();
        Enemies = new List<GameObject>();
        LootItems = new List<GameObject>();
        Props = new List<GameObject>();
        RoomSockets = new List<RoomSocket>();
    }
    
    /// <summary>
    /// Inicializa la habitación con sus propiedades básicas
    /// </summary>
    public void Initialize(DungeonRoomType roomType, int gridX, int gridY, Vector3 worldPosition, float roomSize)
    {
        RoomType = roomType;
        GridX = gridX;
        GridY = gridY;
        WorldPosition = worldPosition;
        RoomSize = roomSize;
        
        IsVisited = false;
        IsCleared = false;
        
        // Configurar propiedades según el tipo de habitación
        SetupRoomProperties();
        
        // Generar contenido inicial
        GenerateRoomContent();
        
        Debug.Log($"Habitación {RoomType} inicializada en ({GridX}, {GridY})");
    }
    
    /// <summary>
    /// Configura las propiedades específicas según el tipo de habitación
    /// </summary>
    private void SetupRoomProperties()
    {
        switch (RoomType)
        {
            case DungeonRoomType.Entrance:
                HasEnemies = false;
                HasLoot = false;
                break;
                
            case DungeonRoomType.Office:
                HasEnemies = Random.Range(0, 100) < 40; // 40% de probabilidad
                HasLoot = Random.Range(0, 100) < 30; // 30% de probabilidad
                break;
                
            case DungeonRoomType.Storage:
                HasEnemies = Random.Range(0, 100) < 50; // 50% de probabilidad
                HasLoot = Random.Range(0, 100) < 70; // 70% de probabilidad (almacenes tienen más loot)
                break;
                
            case DungeonRoomType.LargeRoom:
                HasEnemies = Random.Range(0, 100) < 60; // 60% de probabilidad
                HasLoot = Random.Range(0, 100) < 50; // 50% de probabilidad
                break;
                
            case DungeonRoomType.DeadEnd:
                HasEnemies = Random.Range(0, 100) < 30; // 30% de probabilidad
                HasLoot = Random.Range(0, 100) < 80; // 80% de probabilidad (callejones suelen tener loot)
                break;
                
            default:
                HasEnemies = false;
                HasLoot = false;
                break;
        }
    }
    
    /// <summary>
    /// Genera el contenido inicial de la habitación
    /// </summary>
    private void GenerateRoomContent()
    {
        // Esto se puede expandir para spawnear enemigos y loot reales
        // Por ahora solo marca las propiedades
        
        if (HasEnemies)
        {
            // Aquí se spawnean enemigos cuando el jugador entre
            Debug.Log($"Habitación {RoomType} tendrá enemigos");
        }
        
        if (HasLoot)
        {
            // Aquí se spawnean items de loot
            Debug.Log($"Habitación {RoomType} tendrá loot");
        }
    }
    
    /// <summary>
    /// Conecta esta habitación con otra
    /// </summary>
    public void ConnectToRoom(DungeonRoom otherRoom)
    {
        if (!ConnectedRooms.Contains(otherRoom))
        {
            ConnectedRooms.Add(otherRoom);
            if (!otherRoom.ConnectedRooms.Contains(this))
            {
                otherRoom.ConnectedRooms.Add(this);
            }
            Debug.Log($"Conectada {RoomType} a {otherRoom.RoomType}");
        }
    }
    
    /// <summary>
    /// Desconecta esta habitación de otra
    /// </summary>
    public void DisconnectFromRoom(DungeonRoom otherRoom)
    {
        if (ConnectedRooms.Contains(otherRoom))
        {
            ConnectedRooms.Remove(otherRoom);
            if (otherRoom.ConnectedRooms.Contains(this))
            {
                otherRoom.ConnectedRooms.Remove(this);
            }
            Debug.Log($"Desconectada {RoomType} de {otherRoom.RoomType}");
        }
    }
    
    /// <summary>
    /// Verifica si está conectada a otra habitación
    /// </summary>
    public bool IsConnectedTo(DungeonRoom otherRoom)
    {
        return ConnectedRooms.Contains(otherRoom);
    }
    
    /// <summary>
    /// Marca la habitación como visitada cuando el jugador entra
    /// </summary>
    public void VisitRoom()
    {
        if (!IsVisited)
        {
            IsVisited = true;
            OnRoomEntered();
            Debug.Log($"Jugador entró a habitación {RoomType}");
        }
    }
    
    /// <summary>
    /// Marca la habitación como limpiada
    /// </summary>
    public void ClearRoom()
    {
        if (!IsCleared)
        {
            IsCleared = true;
            OnRoomCleared();
            Debug.Log($"Habitación {RoomType} ha sido limpiada");
        }
    }
    
    /// <summary>
    /// Se llama cuando el jugador entra a la habitación
    /// </summary>
    private void OnRoomEntered()
    {
        // Spawnear enemigos si la habitación los tiene
        if (HasEnemies && Enemies.Count == 0)
        {
            SpawnEnemies();
        }
        
        // Activar efectos de la habitación
        ActivateRoomEffects();
    }
    
    /// <summary>
    /// Se llama cuando la habitación es limpiada
    /// </summary>
    private void OnRoomCleared()
    {
        // Spawnear loot si la habitación lo tiene
        if (HasLoot && LootItems.Count == 0)
        {
            SpawnLoot();
        }
        
        // Desactivar enemigos restantes
        foreach (GameObject enemy in Enemies)
        {
            if (enemy != null)
            {
                // Aquí se puede hacer que los enemigos huyan o desaparezcan
            }
        }
    }
    
    /// <summary>
    /// Spawnea enemigos en la habitación
    /// </summary>
    private void SpawnEnemies()
    {
        // TODO: Implementar spawn de enemigos reales
        // Por ahora solo es un placeholder
        int enemyCount = CalculateEnemyCount();
        
        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 spawnPos = GetRandomSpawnPosition();
            // GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity, transform);
            // Enemies.Add(enemy);
            Debug.Log($"Spawneando enemigo {i + 1} en {RoomType} en posición {spawnPos}");
        }
    }
    
    /// <summary>
    /// Spawnea loot en la habitación
    /// </summary>
    private void SpawnLoot()
    {
        // TODO: Implementar spawn de loot real
        int lootCount = CalculateLootCount();
        
        for (int i = 0; i < lootCount; i++)
        {
            Vector3 spawnPos = GetRandomSpawnPosition();
            // GameObject loot = Instantiate(lootPrefab, spawnPos, Quaternion.identity, transform);
            // LootItems.Add(loot);
            Debug.Log($"Spawneando loot {i + 1} en {RoomType} en posición {spawnPos}");
        }
    }
    
    /// <summary>
    /// Calcula cuántos enemigos debe tener la habitación
    /// </summary>
    private int CalculateEnemyCount()
    {
        switch (RoomType)
        {
            case DungeonRoomType.Office:
                return Random.Range(0, 2); // 0-1 enemigos
            case DungeonRoomType.Storage:
                return Random.Range(1, 3); // 1-2 enemigos
            case DungeonRoomType.LargeRoom:
                return Random.Range(2, 5); // 2-4 enemigos
            case DungeonRoomType.DeadEnd:
                return Random.Range(0, 2); // 0-1 enemigos
            default:
                return 0;
        }
    }
    
    /// <summary>
    /// Calcula cuántos items de loot debe tener la habitación
    /// </summary>
    private int CalculateLootCount()
    {
        switch (RoomType)
        {
            case DungeonRoomType.Office:
                return Random.Range(0, 2); // 0-1 items
            case DungeonRoomType.Storage:
                return Random.Range(2, 5); // 2-4 items
            case DungeonRoomType.LargeRoom:
                return Random.Range(1, 3); // 1-2 items
            case DungeonRoomType.DeadEnd:
                return Random.Range(1, 4); // 1-3 items
            default:
                return 0;
        }
    }
    
    /// <summary>
    /// Obtiene una posición aleatoria dentro de la habitación para spawnear objetos
    /// </summary>
    private Vector3 GetRandomSpawnPosition()
    {
        float halfSize = RoomSize / 2f;
        float offsetX = Random.Range(-halfSize * 0.7f, halfSize * 0.7f);
        float offsetZ = Random.Range(-halfSize * 0.7f, halfSize * 0.7f);
        
        return WorldPosition + new Vector3(offsetX, 0.5f, offsetZ);
    }
    
    /// <summary>
    /// Activa efectos específicos de la habitación
    /// </summary>
    private void ActivateRoomEffects()
    {
        // Aquí se pueden activar efectos de iluminación, sonido, etc.
        switch (RoomType)
        {
            case DungeonRoomType.Storage:
                // Efectos de almacén (luces parpadeantes, etc.)
                break;
            case DungeonRoomType.LargeRoom:
                // Efectos de habitación grande (eco, etc.)
                break;
        }
    }
    
    /// <summary>
    /// Obtiene el número de conexiones de la habitación
    /// </summary>
    public int GetConnectionCount()
    {
        return ConnectedRooms.Count;
    }
    
    /// <summary>
    /// Calcula la distancia a otra habitación
    /// </summary>
    public float GetDistanceTo(DungeonRoom otherRoom)
    {
        return Vector3.Distance(WorldPosition, otherRoom.WorldPosition);
    }
    
    /// <summary>
    /// Verifica si la habitación es un callejón sin salida
    /// </summary>
    public bool IsDeadEnd()
    {
        return ConnectedRooms.Count == 1;
    }
    
    // Debug visualization
    private void OnDrawGizmos()
    {
        // Dibujar bounds de la habitación
        Gizmos.color = GetRoomColor();
        Gizmos.DrawWireCube(WorldPosition, Vector3.one * RoomSize);
        
        // Dibujar conexiones
        Gizmos.color = Color.yellow;
        foreach (DungeonRoom connected in ConnectedRooms)
        {
            if (connected != null)
            {
                Gizmos.DrawLine(WorldPosition, connected.WorldPosition);
            }
        }
        
        // Marcar si está visitada
        if (IsVisited)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(WorldPosition + Vector3.up * (RoomSize / 2f + 1f), 0.5f);
        }
    }
    
    private Color GetRoomColor()
    {
        switch (RoomType)
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

