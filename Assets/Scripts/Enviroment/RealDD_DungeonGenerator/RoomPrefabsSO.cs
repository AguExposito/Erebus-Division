using UnityEngine;

/// <summary>
/// ScriptableObject para almacenar y gestionar los prefabs de habitaciones
/// Útil para crear diferentes sets de prefabs (estilo industrial, moderno, etc.)
/// </summary>
[CreateAssetMenu(fileName = "RoomPrefabs", menuName = "Dungeon/Room Prefabs")]
public class RoomPrefabsSO : ScriptableObject
{
    [Header("Prefabs de Habitaciones")]
    [Tooltip("Prefab para la habitación de entrada")]
    public GameObject entrancePrefab;
    
    [Tooltip("Prefab para oficinas")]
    public GameObject officePrefab;
    
    [Tooltip("Prefab para almacenes")]
    public GameObject storagePrefab;
    
    [Tooltip("Prefab para pasillos")]
    public GameObject hallwayPrefab;
    
    [Tooltip("Prefab para habitaciones grandes")]
    public GameObject largeRoomPrefab;
    
    [Tooltip("Prefab para callejones sin salida")]
    public GameObject deadEndPrefab;
    
    [Header("Prefab Genérico")]
    [Tooltip("Prefab genérico que se usará si no hay uno específico")]
    public GameObject genericRoomPrefab;
    
    /// <summary>
    /// Obtiene el prefab para un tipo de habitación específico
    /// </summary>
    public GameObject GetPrefabForRoomType(DungeonRoomType roomType)
    {
        switch (roomType)
        {
            case DungeonRoomType.Entrance:
                return entrancePrefab != null ? entrancePrefab : genericRoomPrefab;
            case DungeonRoomType.Office:
                return officePrefab != null ? officePrefab : genericRoomPrefab;
            case DungeonRoomType.Storage:
                return storagePrefab != null ? storagePrefab : genericRoomPrefab;
            case DungeonRoomType.Hallway:
                return hallwayPrefab != null ? hallwayPrefab : genericRoomPrefab;
            case DungeonRoomType.LargeRoom:
                return largeRoomPrefab != null ? largeRoomPrefab : genericRoomPrefab;
            case DungeonRoomType.DeadEnd:
                return deadEndPrefab != null ? deadEndPrefab : genericRoomPrefab;
            default:
                return genericRoomPrefab;
        }
    }
    
    /// <summary>
    /// Convierte este ScriptableObject a la estructura RoomPrefabs
    /// </summary>
    public RoomPrefabs ToRoomPrefabs()
    {
        return new RoomPrefabs
        {
            entrancePrefab = entrancePrefab,
            officePrefab = officePrefab,
            storagePrefab = storagePrefab,
            hallwayPrefab = hallwayPrefab,
            largeRoomPrefab = largeRoomPrefab,
            deadEndPrefab = deadEndPrefab
        };
    }
}

