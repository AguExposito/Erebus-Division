using UnityEngine;

/// <summary>
/// Script simple para demostrar el uso básico del Heaven Dungeon Generator
/// Solo configura valores básicos y genera la mazmorra
/// </summary>
public class HeavenDungeonSimple : MonoBehaviour
{
    [Header("Configuración Básica")]
    [SerializeField] private int dungeonSize = 8;        // 8x8 matriz
    [SerializeField] private float blockSize = 3f;       // 3 unidades por bloque
    [SerializeField] private int minRooms = 4;           // Mínimo 4 habitaciones
    [SerializeField] private int maxRooms = 6;           // Máximo 6 habitaciones
    
    [Header("Generador")]
    [SerializeField] private HeavenDungeonGenerator generator;
    
    void Start()
    {
        SetupGenerator();
        GenerateDungeon();
    }
    
    private void SetupGenerator()
    {
        // Buscar o crear el generador
        if (generator == null)
        {
            generator = FindObjectOfType<HeavenDungeonGenerator>();
        }
        
        if (generator == null)
        {
            GameObject generatorObj = new GameObject("HeavenDungeonGenerator");
            generator = generatorObj.AddComponent<HeavenDungeonGenerator>();
        }
        
        // Aplicar configuración básica
        generator.dungeonWidth = dungeonSize;
        generator.dungeonHeight = dungeonSize;
        generator.blockSize = blockSize;
        generator.minRooms = minRooms;
        generator.maxRooms = maxRooms;
        generator.useCoroutines = true;
        generator.maxRoomsPerFrame = 2;
        
        Debug.Log($"Heaven Dungeon configurado: {dungeonSize}x{dungeonSize}, {minRooms}-{maxRooms} habitaciones");
    }
    
    private void GenerateDungeon()
    {
        if (generator != null)
        {
            generator.RegenerateDungeon();
            
            // Mostrar información de debug
            generator.PrintMatrix();
            generator.PrintRoomPositions();
        }
    }
    
    // Métodos públicos para cambiar configuración
    public void SetDungeonSize(int size)
    {
        dungeonSize = size;
        if (generator != null)
        {
            generator.dungeonWidth = size;
            generator.dungeonHeight = size;
        }
    }
    
    public void SetBlockSize(float size)
    {
        blockSize = size;
        if (generator != null)
        {
            generator.blockSize = size;
        }
    }
    
    public void SetRoomCount(int min, int max)
    {
        minRooms = min;
        maxRooms = max;
        if (generator != null)
        {
            generator.minRooms = min;
            generator.maxRooms = max;
        }
    }
    
    public void Regenerate()
    {
        SetupGenerator();
        GenerateDungeon();
    }
    
    // Configuraciones predefinidas
    public void SetSmallDungeon()
    {
        SetDungeonSize(5);
        SetBlockSize(2f);
        SetRoomCount(3, 4);
        Regenerate();
    }
    
    public void SetMediumDungeon()
    {
        SetDungeonSize(8);
        SetBlockSize(3f);
        SetRoomCount(4, 6);
        Regenerate();
    }
    
    public void SetLargeDungeon()
    {
        SetDungeonSize(12);
        SetBlockSize(4f);
        SetRoomCount(6, 10);
        Regenerate();
    }
}
