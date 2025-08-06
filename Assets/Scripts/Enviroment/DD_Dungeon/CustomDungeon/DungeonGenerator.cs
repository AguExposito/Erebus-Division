using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator3D : MonoBehaviour
{
    [Header("Dungeon Settings")]
    public int levels = 4;
    public int minRoomsPerLevel = 2;
    public int maxRoomsPerLevel = 4;
    public float horizontalSpacing = 20f;
    public float verticalSpacing = 25f;

    [Header("Prefabs")]
    public GameObject roomPrefab;
    public GameObject hallwayPrefab;

    public Transform dungeonParent;
    public List<List<DungeonNode>> dungeonMap = new();

    [Header("Dungeon")]
    public List<GameObject> allRoomInstances = new();
    public List<GameObject> allHallwayInstances = new();


    void Start()
    {
        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        dungeonMap.Clear();

        for (int level = 0; level < levels; level++)
        {
            int roomsThisLevel = Random.Range(minRoomsPerLevel, maxRoomsPerLevel + 1);
            List<DungeonNode> levelNodes = new();

            for (int i = 0; i < roomsThisLevel; i++)
            {
                NodeType type = GetTypeForLevel(level);
                var node = new DungeonNode(type, level, i);
                levelNodes.Add(node);
            }

            dungeonMap.Add(levelNodes);
        }

        // Conectar nodos
        for (int level = 0; level < dungeonMap.Count - 1; level++)
        {
            var current = dungeonMap[level];
            var next = dungeonMap[level + 1];

            foreach (var node in current)
            {
                int connections = Random.Range(1, 3);
                for (int i = 0; i < connections; i++)
                {
                    var target = next[Random.Range(0, next.Count)];
                    if (!node.connections.Contains(target))
                        node.connections.Add(target);
                }
            }
        }

        // Instanciar habitaciones
        for (int level = 0; level < dungeonMap.Count; level++)
        {
            var nodes = dungeonMap[level];
            float levelWidth = (nodes.Count - 1) * horizontalSpacing;

            for (int i = 0; i < nodes.Count; i++)
            {
                Vector3 worldPos = new Vector3(i * horizontalSpacing - levelWidth / 2f, 0, -level * verticalSpacing);
                GameObject room = Instantiate(roomPrefab, worldPos, Quaternion.identity, dungeonParent);
                room.name = $"Room_L{level}_I{i}";

                nodes[i].roomInstance = room;
                nodes[i].worldPosition = worldPos;
            }
        }

        // Instanciar pasillos
        foreach (var level in dungeonMap)
        {
            foreach (var node in level)
            {
                foreach (var target in node.connections)
                {
                    CreateHallway(node.worldPosition, target.worldPosition);
                }
            }
        }

        // Spawnear jugador en la habitación inicial
        SpawnPlayerAtStart();
    }

    void CreateHallway(Vector3 from, Vector3 to)
    {
        Vector3 direction = (to - from).normalized;
        float totalDistance = Vector3.Distance(from, to);

        float entranceDepth = 8.0f; // cuánto se "mete" el pasillo en cada habitación
        float effectiveLength = totalDistance - (entranceDepth * 2); // acorta para no atravesar habitaciones

        if (effectiveLength <= 0) return; // demasiado cerca, no instancias pasillo

        Vector3 hallwayPosition = from + direction * (entranceDepth + effectiveLength / 2f);
        Quaternion hallwayRotation = Quaternion.LookRotation(direction);

        GameObject hallway = Instantiate(hallwayPrefab, hallwayPosition, hallwayRotation, dungeonParent);
        hallway.transform.localScale = new Vector3(1f, 2f, effectiveLength);

        allHallwayInstances.Add(hallway);
    }



    void SpawnPlayerAtStart()
    {
        var startNode = dungeonMap[0][0];
        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            player.transform.position = startNode.worldPosition + Vector3.up * 2f;
        }
    }

    NodeType GetTypeForLevel(int level)
    {
        if (level == 0) return NodeType.Start;
        if (level == levels - 1) return NodeType.Boss;

        int roll = Random.Range(0, 100);
        if (roll < 60) return NodeType.Enemy;
        if (roll < 85) return NodeType.Treasure;
        return NodeType.Shop;
    }
}



public enum NodeType { Enemy, Treasure, Shop, Start, Boss }

public class DungeonNode
{
    public NodeType type;
    public int level; // qué fila es
    public int indexInLevel; // posición en la fila
    public List<DungeonNode> connections = new();
    public GameObject roomInstance; // <- habitación 3D
    public Vector3 worldPosition;   // <- posición en mundo

    // Constructor
    public DungeonNode(NodeType type, int level, int index)
    {
        this.type = type;
        this.level = level;
        this.indexInLevel = index;
    }
}

