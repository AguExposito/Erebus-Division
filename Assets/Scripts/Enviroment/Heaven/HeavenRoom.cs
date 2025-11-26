using System.Collections.Generic;
using UnityEngine;

public class HeavenRoom : MonoBehaviour
{
    [Header("Room Information")]
    public HeavenDungeonGenerator.HeavenRoomType RoomType { get; private set; }
    public int MatrixX { get; private set; }
    public int MatrixY { get; private set; }
    public Vector3 WorldPosition { get; private set; }
    
    [Header("Room Properties")]
    public bool IsVisited { get; private set; }
    public bool IsCleared { get; private set; }
    public int RoomLevel { get; private set; }
    
    [Header("Connections")]
    public List<HeavenRoom> ConnectedRooms { get; private set; }
    public List<HeavenRoomSocket> RoomSockets { get; private set; }
    
    [Header("Room Content")]
    public List<GameObject> Enemies { get; private set; }
    public List<GameObject> Treasures { get; private set; }
    public List<GameObject> Decorations { get; private set; }
    
    [Header("Heaven Theme")]
    public Color RoomAmbientColor = Color.white;
    public float RoomLightIntensity = 1f;
    public bool HasSpecialEffects = false;
    
    private void Awake()
    {
        ConnectedRooms = new List<HeavenRoom>();
        RoomSockets = new List<HeavenRoomSocket>();
        Enemies = new List<GameObject>();
        Treasures = new List<GameObject>();
        Decorations = new List<GameObject>();
    }
    
    public void Initialize(HeavenDungeonGenerator.HeavenRoomType roomType, int matrixX, int matrixY, Vector3 worldPosition)
    {
        RoomType = roomType;
        MatrixX = matrixX;
        MatrixY = matrixY;
        WorldPosition = worldPosition;
        IsVisited = false;
        IsCleared = false;
        RoomLevel = CalculateRoomLevel();
        
        // Set room-specific properties
        SetRoomProperties();
        
        // Add room socket component (temporarily disabled to fix positioning)
        // HeavenRoomSocket socket = gameObject.AddComponent<HeavenRoomSocket>();
        // RoomSockets.Add(socket);
        
        Debug.Log($"Initialized Heaven Room: {RoomType} at ({MatrixX}, {MatrixY})");
    }
    
    private int CalculateRoomLevel()
    {
        // Calculate room level based on distance from starting position (0,0)
        return Mathf.RoundToInt(Vector2.Distance(Vector2.zero, new Vector2(MatrixX, MatrixY)));
    }
    
    private void SetRoomProperties()
    {
        // Set properties based on room type
        switch (RoomType)
        {
            case HeavenDungeonGenerator.HeavenRoomType.Sanctuary:
                RoomAmbientColor = new Color(1f, 1f, 0.9f, 1f); // Warm white
                RoomLightIntensity = 1.2f;
                HasSpecialEffects = true;
                break;
                
            case HeavenDungeonGenerator.HeavenRoomType.Chapel:
                RoomAmbientColor = new Color(0.9f, 0.95f, 1f, 1f); // Cool white
                RoomLightIntensity = 1f;
                HasSpecialEffects = false;
                break;
                
            case HeavenDungeonGenerator.HeavenRoomType.Altar:
                RoomAmbientColor = new Color(1f, 0.95f, 0.8f, 1f); // Golden white
                RoomLightIntensity = 1.1f;
                HasSpecialEffects = true;
                break;
                
            case HeavenDungeonGenerator.HeavenRoomType.Garden:
                RoomAmbientColor = new Color(0.8f, 1f, 0.9f, 1f); // Green tint
                RoomLightIntensity = 0.9f;
                HasSpecialEffects = true;
                break;
                
            case HeavenDungeonGenerator.HeavenRoomType.Library:
                RoomAmbientColor = new Color(0.95f, 0.9f, 0.8f, 1f); // Warm brown
                RoomLightIntensity = 0.8f;
                HasSpecialEffects = false;
                break;
                
            case HeavenDungeonGenerator.HeavenRoomType.Treasury:
                RoomAmbientColor = new Color(1f, 1f, 0.7f, 1f); // Golden
                RoomLightIntensity = 1.3f;
                HasSpecialEffects = true;
                break;
                
            case HeavenDungeonGenerator.HeavenRoomType.Boss:
                RoomAmbientColor = new Color(1f, 0.8f, 0.8f, 1f); // Red tint
                RoomLightIntensity = 1.5f;
                HasSpecialEffects = true;
                break;
                
            default:
                RoomAmbientColor = Color.white;
                RoomLightIntensity = 1f;
                HasSpecialEffects = false;
                break;
        }
    }
    
    public void ConnectToRoom(HeavenRoom otherRoom)
    {
        if (!ConnectedRooms.Contains(otherRoom))
        {
            ConnectedRooms.Add(otherRoom);
            otherRoom.ConnectedRooms.Add(this);
            Debug.Log($"Connected {RoomType} to {otherRoom.RoomType}");
        }
    }
    
    public void DisconnectFromRoom(HeavenRoom otherRoom)
    {
        if (ConnectedRooms.Contains(otherRoom))
        {
            ConnectedRooms.Remove(otherRoom);
            otherRoom.ConnectedRooms.Remove(this);
            Debug.Log($"Disconnected {RoomType} from {otherRoom.RoomType}");
        }
    }
    
    public void VisitRoom()
    {
        if (!IsVisited)
        {
            IsVisited = true;
            OnRoomEntered();
            Debug.Log($"Player entered {RoomType} room");
        }
    }
    
    public void ClearRoom()
    {
        if (!IsCleared)
        {
            IsCleared = true;
            OnRoomCleared();
            Debug.Log($"Room {RoomType} has been cleared");
        }
    }
    
    private void OnRoomEntered()
    {
        // Apply room-specific effects when entered
        ApplyRoomLighting();
        
        if (HasSpecialEffects)
        {
            StartRoomEffects();
        }
        
        // Spawn enemies if this is a combat room
        if (ShouldSpawnEnemies())
        {
            SpawnEnemies();
        }
    }
    
    private void OnRoomCleared()
    {
        // Apply effects when room is cleared
        if (RoomType == HeavenDungeonGenerator.HeavenRoomType.Treasury)
        {
            SpawnTreasures();
        }
        
        // Unlock connected rooms
        UnlockConnectedRooms();
    }
    
    private void ApplyRoomLighting()
    {
        // Apply room-specific lighting
        Light[] lights = GetComponentsInChildren<Light>();
        foreach (Light light in lights)
        {
            light.color = RoomAmbientColor;
            light.intensity = RoomLightIntensity;
        }
    }
    
    private void StartRoomEffects()
    {
        // Start special effects based on room type
        switch (RoomType)
        {
            case HeavenDungeonGenerator.HeavenRoomType.Sanctuary:
                // Add healing effects
                break;
            case HeavenDungeonGenerator.HeavenRoomType.Altar:
                // Add blessing effects
                break;
            case HeavenDungeonGenerator.HeavenRoomType.Garden:
                // Add peaceful effects
                break;
            case HeavenDungeonGenerator.HeavenRoomType.Treasury:
                // Add treasure glow effects
                break;
            case HeavenDungeonGenerator.HeavenRoomType.Boss:
                // Add dramatic effects
                break;
        }
    }
    
    private bool ShouldSpawnEnemies()
    {
        // Determine if enemies should spawn based on room type
        switch (RoomType)
        {
            case HeavenDungeonGenerator.HeavenRoomType.Chapel:
            case HeavenDungeonGenerator.HeavenRoomType.Library:
            case HeavenDungeonGenerator.HeavenRoomType.Boss:
                return true;
            default:
                return false;
        }
    }
    
    private void SpawnEnemies()
    {
        // Spawn enemies based on room type and level
        int enemyCount = CalculateEnemyCount();
        
        for (int i = 0; i < enemyCount; i++)
        {
            // This would spawn actual enemy prefabs
            // For now, just log the intention
            Debug.Log($"Would spawn enemy {i + 1} in {RoomType} room");
        }
    }
    
    private void SpawnTreasures()
    {
        // Spawn treasures based on room type
        if (RoomType == HeavenDungeonGenerator.HeavenRoomType.Treasury)
        {
            Debug.Log($"Spawning treasures in {RoomType} room");
        }
    }
    
    private int CalculateEnemyCount()
    {
        // Calculate enemy count based on room level and type
        int baseCount = 1;
        
        switch (RoomType)
        {
            case HeavenDungeonGenerator.HeavenRoomType.Chapel:
                baseCount = 2;
                break;
            case HeavenDungeonGenerator.HeavenRoomType.Library:
                baseCount = 1;
                break;
            case HeavenDungeonGenerator.HeavenRoomType.Boss:
                baseCount = 1; // Boss only
                break;
        }
        
        return baseCount + (RoomLevel / 2);
    }
    
    private void UnlockConnectedRooms()
    {
        // Unlock all connected rooms when this room is cleared
        foreach (HeavenRoom connectedRoom in ConnectedRooms)
        {
            // This would unlock doors or remove barriers
            Debug.Log($"Unlocked connection to {connectedRoom.RoomType}");
        }
    }
    
    // Public getters
    public bool IsConnectedTo(HeavenRoom otherRoom)
    {
        return ConnectedRooms.Contains(otherRoom);
    }
    
    public int GetConnectionCount()
    {
        return ConnectedRooms.Count;
    }
    
    public float GetDistanceTo(HeavenRoom otherRoom)
    {
        return Vector3.Distance(WorldPosition, otherRoom.WorldPosition);
    }
    
    // Debug visualization
    private void OnDrawGizmos()
    {
        // Draw room bounds
        Gizmos.color = GetRoomColor();
        Gizmos.DrawWireCube(transform.position, Vector3.one * 2f);
        
        // Draw connections
        Gizmos.color = Color.yellow;
        foreach (HeavenRoom connectedRoom in ConnectedRooms)
        {
            if (connectedRoom != null)
            {
                Gizmos.DrawLine(transform.position, connectedRoom.transform.position);
            }
        }
    }
    
    private Color GetRoomColor()
    {
        switch (RoomType)
        {
            case HeavenDungeonGenerator.HeavenRoomType.Sanctuary: return Color.green;
            case HeavenDungeonGenerator.HeavenRoomType.Chapel: return Color.blue;
            case HeavenDungeonGenerator.HeavenRoomType.Altar: return Color.yellow;
            case HeavenDungeonGenerator.HeavenRoomType.Garden: return Color.cyan;
            case HeavenDungeonGenerator.HeavenRoomType.Library: return Color.magenta;
            case HeavenDungeonGenerator.HeavenRoomType.Treasury: return Color.red;
            case HeavenDungeonGenerator.HeavenRoomType.Boss: return Color.black;
            default: return Color.white;
        }
    }
}
