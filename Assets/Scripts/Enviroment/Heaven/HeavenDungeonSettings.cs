using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HeavenDungeonSettings", menuName = "Heaven/Dungeon Settings")]
public class HeavenDungeonSettings : ScriptableObject
{
    [Header("Matrix Configuration")]
    [Tooltip("Width of the dungeon in blocks")]
    public int dungeonWidth = 8;
    
    [Tooltip("Height of the dungeon in blocks")]
    public int dungeonHeight = 8;
    
    [Tooltip("Size of each block in world units")]
    public float blockSize = 3f;
    
    [Header("Room Generation")]
    [Tooltip("Minimum number of rooms to generate")]
    public int minRooms = 4;
    
    [Tooltip("Maximum number of rooms to generate")]
    public int maxRooms = 6;
    
    [Header("Room Separation")]
    [Tooltip("Minimum blocks of separation between rooms (1 = adjacent, 2 = 1 block gap, 3 = 2 blocks gap, etc.)")]
    public int minRoomSeparation = 2;
    
    [Header("Room Type Probabilities")]
    [Range(0f, 100f)]
    [Tooltip("Probability of generating a Chapel room")]
    public float chapelProbability = 30f;
    
    [Range(0f, 100f)]
    [Tooltip("Probability of generating a Garden room")]
    public float gardenProbability = 20f;
    
    [Range(0f, 100f)]
    [Tooltip("Probability of generating a Library room")]
    public float libraryProbability = 20f;
    
    [Range(0f, 100f)]
    [Tooltip("Probability of generating an Altar room")]
    public float altarProbability = 15f;
    
    [Range(0f, 100f)]
    [Tooltip("Probability of generating a Treasury room")]
    public float treasuryProbability = 15f;
    
    [Header("Heaven Prefabs")]
    [Tooltip("Prefab for Heaven rooms")]
    public GameObject heavenRoomPrefab;
    
    [Tooltip("Prefab for Heaven hallways")]
    public GameObject heavenHallwayPrefab;
    
    [Tooltip("Prefab for Heaven floors")]
    public GameObject heavenFloorPrefab;
    
    [Tooltip("Prefab for Heaven walls")]
    public GameObject heavenWallPrefab;
    
    [Header("Heaven Materials")]
    [Tooltip("Material for Heaven floors")]
    public Material heavenFloorMaterial;
    
    [Tooltip("Material for Heaven walls")]
    public Material heavenWallMaterial;
    
    [Tooltip("Material for Heaven ceilings")]
    public Material heavenCeilingMaterial;
    
    [Header("Heaven Theme")]
    [Tooltip("Ambient color for the Heaven dungeon")]
    public Color heavenAmbientColor = new Color(0.8f, 0.9f, 1f, 1f);
    
    [Tooltip("Light intensity multiplier for Heaven rooms")]
    public float lightIntensityMultiplier = 1f;
    
    [Tooltip("Enable special Heaven effects")]
    public bool enableSpecialEffects = true;
    
    [Header("Generation Settings")]
    [Tooltip("Use coroutines for generation to prevent frame drops")]
    public bool useCoroutines = true;
    
    [Tooltip("Maximum rooms to generate per frame when using coroutines")]
    public int maxRoomsPerFrame = 3;
    
    [Tooltip("Maximum hallways to create per frame when using coroutines")]
    public int maxHallwaysPerFrame = 5;
    
    [Header("Room Content")]
    [Tooltip("Minimum enemies per room")]
    public int minEnemiesPerRoom = 0;
    
    [Tooltip("Maximum enemies per room")]
    public int maxEnemiesPerRoom = 3;
    
    [Tooltip("Enemy spawn probability")]
    [Range(0f, 100f)]
    public float enemySpawnProbability = 60f;
    
    [Tooltip("Treasure spawn probability")]
    [Range(0f, 100f)]
    public float treasureSpawnProbability = 40f;
    
    [Header("Heaven Decorations")]
    [Tooltip("List of Heaven-themed decorations")]
    public List<HeavenDecoration> heavenDecorations = new List<HeavenDecoration>();
    
    [Tooltip("List of Heaven-themed enemies")]
    public List<HeavenEnemy> heavenEnemies = new List<HeavenEnemy>();
    
    [Tooltip("List of Heaven-themed treasures")]
    public List<HeavenTreasure> heavenTreasures = new List<HeavenTreasure>();
    
    [Header("Advanced Settings")]
    [Tooltip("Enable debug visualization")]
    public bool enableDebugVisualization = false;
    
    [Tooltip("Enable room connection validation")]
    public bool enableConnectionValidation = true;
    
    [Tooltip("Maximum connection distance between rooms")]
    public float maxConnectionDistance = 5f;
    
    [Tooltip("Enable automatic NavMesh rebaking")]
    public bool enableNavMeshRebaking = true;
    
    // Validation methods
    private void OnValidate()
    {
        // Ensure valid ranges
        dungeonWidth = Mathf.Max(5, dungeonWidth);
        dungeonHeight = Mathf.Max(5, dungeonHeight);
        blockSize = Mathf.Max(0.1f, blockSize);
        
        minRooms = Mathf.Max(1, minRooms);
        maxRooms = Mathf.Max(minRooms, maxRooms);
        
        // Ensure probabilities don't exceed 100%
        float totalProbability = chapelProbability + gardenProbability + libraryProbability + 
                               altarProbability + treasuryProbability;
        
        if (totalProbability > 100f)
        {
            Debug.LogWarning("HeavenDungeonSettings: Total room type probabilities exceed 100%. Adjusting...");
            
            float scaleFactor = 100f / totalProbability;
            chapelProbability *= scaleFactor;
            gardenProbability *= scaleFactor;
            libraryProbability *= scaleFactor;
            altarProbability *= scaleFactor;
            treasuryProbability *= scaleFactor;
        }
    }
    
    // Helper methods
    public HeavenDungeonGenerator.HeavenRoomType GetRandomRoomType()
    {
        float randomValue = Random.Range(0f, 100f);
        float currentProbability = 0f;
        
        currentProbability += chapelProbability;
        if (randomValue <= currentProbability)
            return HeavenDungeonGenerator.HeavenRoomType.Chapel;
        
        currentProbability += gardenProbability;
        if (randomValue <= currentProbability)
            return HeavenDungeonGenerator.HeavenRoomType.Garden;
        
        currentProbability += libraryProbability;
        if (randomValue <= currentProbability)
            return HeavenDungeonGenerator.HeavenRoomType.Library;
        
        currentProbability += altarProbability;
        if (randomValue <= currentProbability)
            return HeavenDungeonGenerator.HeavenRoomType.Altar;
        
        currentProbability += treasuryProbability;
        if (randomValue <= currentProbability)
            return HeavenDungeonGenerator.HeavenRoomType.Treasury;
        
        // Default fallback
        return HeavenDungeonGenerator.HeavenRoomType.Chapel;
    }
    
    public bool ShouldSpawnEnemies(HeavenDungeonGenerator.HeavenRoomType roomType)
    {
        if (roomType == HeavenDungeonGenerator.HeavenRoomType.Sanctuary ||
            roomType == HeavenDungeonGenerator.HeavenRoomType.Garden)
        {
            return false; // Peaceful rooms
        }
        
        return Random.Range(0f, 100f) <= enemySpawnProbability;
    }
    
    public bool ShouldSpawnTreasures(HeavenDungeonGenerator.HeavenRoomType roomType)
    {
        if (roomType == HeavenDungeonGenerator.HeavenRoomType.Treasury)
        {
            return true; // Always spawn treasures in treasury
        }
        
        return Random.Range(0f, 100f) <= treasureSpawnProbability;
    }
    
    public int GetEnemyCount(HeavenDungeonGenerator.HeavenRoomType roomType, int roomLevel)
    {
        if (!ShouldSpawnEnemies(roomType))
            return 0;
        
        int baseCount = Random.Range(minEnemiesPerRoom, maxEnemiesPerRoom + 1);
        
        // Adjust based on room type
        switch (roomType)
        {
            case HeavenDungeonGenerator.HeavenRoomType.Chapel:
                baseCount = Mathf.Max(1, baseCount - 1);
                break;
            case HeavenDungeonGenerator.HeavenRoomType.Library:
                baseCount = Mathf.Max(1, baseCount);
                break;
            case HeavenDungeonGenerator.HeavenRoomType.Boss:
                baseCount = 1; // Boss only
                break;
        }
        
        return baseCount;
    }
}

[System.Serializable]
public class HeavenDecoration
{
    [Tooltip("Name of the decoration")]
    public string name;
    
    [Tooltip("Prefab for the decoration")]
    public GameObject prefab;
    
    [Tooltip("Probability of spawning this decoration")]
    [Range(0f, 100f)]
    public float spawnProbability = 50f;
    
    [Tooltip("Room types where this decoration can spawn")]
    public List<HeavenDungeonGenerator.HeavenRoomType> allowedRoomTypes = new List<HeavenDungeonGenerator.HeavenRoomType>();
    
    [Tooltip("Maximum number of this decoration per room")]
    public int maxPerRoom = 3;
}

[System.Serializable]
public class HeavenEnemy
{
    [Tooltip("Name of the enemy")]
    public string name;
    
    [Tooltip("Prefab for the enemy")]
    public GameObject prefab;
    
    [Tooltip("Minimum room level required to spawn")]
    public int minRoomLevel = 0;
    
    [Tooltip("Maximum room level where this enemy can spawn")]
    public int maxRoomLevel = 10;
    
    [Tooltip("Spawn probability")]
    [Range(0f, 100f)]
    public float spawnProbability = 50f;
    
    [Tooltip("Room types where this enemy can spawn")]
    public List<HeavenDungeonGenerator.HeavenRoomType> allowedRoomTypes = new List<HeavenDungeonGenerator.HeavenRoomType>();
}

[System.Serializable]
public class HeavenTreasure
{
    [Tooltip("Name of the treasure")]
    public string name;
    
    [Tooltip("Prefab for the treasure")]
    public GameObject prefab;
    
    [Tooltip("Spawn probability")]
    [Range(0f, 100f)]
    public float spawnProbability = 30f;
    
    [Tooltip("Room types where this treasure can spawn")]
    public List<HeavenDungeonGenerator.HeavenRoomType> allowedRoomTypes = new List<HeavenDungeonGenerator.HeavenRoomType>();
    
    [Tooltip("Minimum room level required to spawn")]
    public int minRoomLevel = 0;
}
