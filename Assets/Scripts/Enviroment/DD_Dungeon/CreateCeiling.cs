using UnityEngine;

public class CreateCeiling : MonoBehaviour
{
    [Header("Ceiling Settings")]
    [SerializeField] GameObject ceilingTile;
    [SerializeField] GenerationSettings generationSettings;
    
    [Header("Optimization Settings")]
    [SerializeField] Vector3 tileSize = new Vector3(2f, 0.2f, 2f); // Larger tiles for better performance
    [SerializeField] float ceilingHeight = 6.8f;
    
    [Header("Safety Settings")]
    [SerializeField] int maxTilesPerFrame = 100; // Prevent frame drops
    [SerializeField] bool useCoroutines = true;
    
    private void Start()
    {
        if (!ValidateComponents())
            return;
            
        if (useCoroutines)
        {
            StartCoroutine(CreateCeilingCoroutine());
        }
        else
        {
            CreateCeilingImmediate();
        }
    }
    
    private bool ValidateComponents()
    {
        if (ceilingTile == null)
        {
            Debug.LogError("CreateCeiling: ceilingTile is not assigned!");
            return false;
        }
        
        if (generationSettings == null)
        {
            Debug.LogError("CreateCeiling: generationSettings is not assigned!");
            return false;
        }
        
        if (generationSettings.generation == null)
        {
            Debug.LogError("CreateCeiling: generationSettings.generation is null!");
            return false;
        }
        
        // Validate area dimensions
        if (generationSettings.generation.areaWidth <= 0 || generationSettings.generation.areaHeight <= 0)
        {
            Debug.LogError($"CreateCeiling: Invalid area dimensions: {generationSettings.generation.areaWidth}x{generationSettings.generation.areaHeight}");
            return false;
        }
        
        // Safety check for extremely large areas
        int totalTiles = generationSettings.generation.areaWidth * generationSettings.generation.areaHeight;
        if (totalTiles > 10000) // 100x100 = 10,000 tiles max
        {
            Debug.LogWarning($"CreateCeiling: Large area detected ({totalTiles} tiles). Consider using larger tiles or reducing area size.");
        }
        
        return true;
    }
    
    private void CreateCeilingImmediate()
    {
        int areaWidth = generationSettings.generation.areaWidth;
        int areaHeight = generationSettings.generation.areaHeight;
        
        // Calculate how many tiles we need based on tile size
        int tilesX = Mathf.CeilToInt(areaWidth / tileSize.x);
        int tilesZ = Mathf.CeilToInt(areaHeight / tileSize.z);
        
        Debug.Log($"Creating ceiling with {tilesX}x{tilesZ} tiles for area {areaWidth}x{areaHeight}");
        
        for (int x = 0; x < tilesX; x++)
        {
            for (int z = 0; z < tilesZ; z++)
            {
                Vector3 position = new Vector3(
                    x * tileSize.x + tileSize.x * 0.5f, // Center the tile
                    ceilingHeight,
                    z * tileSize.z + tileSize.z * 0.5f
                );
                
                GameObject newTile = Instantiate(ceilingTile, position, Quaternion.identity, transform);
                newTile.name = $"CeilingTile_{x}_{z}";
            }
        }
    }
    
    private System.Collections.IEnumerator CreateCeilingCoroutine()
    {
        int areaWidth = generationSettings.generation.areaWidth;
        int areaHeight = generationSettings.generation.areaHeight;
        
        // Calculate how many tiles we need based on tile size
        int tilesX = Mathf.CeilToInt(areaWidth / tileSize.x);
        int tilesZ = Mathf.CeilToInt(areaHeight / tileSize.z);
        
        Debug.Log($"Creating ceiling with {tilesX}x{tilesZ} tiles for area {areaWidth}x{areaHeight} using coroutines");
        
        int tilesCreated = 0;
        
        for (int x = 0; x < tilesX; x++)
        {
            for (int z = 0; z < tilesZ; z++)
            {
                Vector3 position = new Vector3(
                    x * tileSize.x + tileSize.x * 0.5f, // Center the tile
                    ceilingHeight,
                    z * tileSize.z + tileSize.z * 0.5f
                );
                
                GameObject newTile = Instantiate(ceilingTile, position, Quaternion.identity, transform);
                newTile.name = $"CeilingTile_{x}_{z}";
                
                tilesCreated++;
                
                // Yield every few tiles to prevent frame drops
                if (tilesCreated % maxTilesPerFrame == 0)
                {
                    yield return null;
                }
            }
        }
        
        Debug.Log($"Ceiling creation completed. Created {tilesCreated} tiles.");
    }
    
    // Optional: Method to clear all ceiling tiles
    public void ClearCeiling()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            if (transform.GetChild(i).name.StartsWith("CeilingTile_"))
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }
    }
    
    // Optional: Method to recreate ceiling
    public void RecreateCeiling()
    {
        ClearCeiling();
        if (useCoroutines)
        {
            StartCoroutine(CreateCeilingCoroutine());
        }
        else
        {
            CreateCeilingImmediate();
        }
    }
}
