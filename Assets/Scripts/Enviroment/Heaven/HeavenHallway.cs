using UnityEngine;

public class HeavenHallway : MonoBehaviour
{
    [Header("Hallway Information")]
    public HeavenRoomSocket startSocket;
    public HeavenRoomSocket endSocket;
    public float hallwayLength;
    public Vector3 hallwayDirection;
    
    [Header("Heaven Theme")]
    public Material hallwayFloorMaterial;
    public Material hallwayWallMaterial;
    public Material hallwayCeilingMaterial;
    
    [Header("Visual Effects")]
    public ParticleSystem dustParticles;
    public Light[] hallwayLights;
    public Color lightColor = Color.white;
    public float lightIntensity = 1f;
    
    [Header("Hallway Content")]
    public GameObject[] decorations;
    public GameObject[] obstacles;
    public bool hasObstacles = false;
    
    private void Awake()
    {
        // Initialization is handled by the Initialize() method called from HeavenRoomSocket
    }
    
    public void Initialize(HeavenRoomSocket start, HeavenRoomSocket end)
    {
        startSocket = start;
        endSocket = end;
        
        CalculateHallwayProperties();
        SetupHallwayGeometry();
        ApplyHeavenTheme();
        GenerateHallwayContent();
    }
    
    private void CalculateHallwayProperties()
    {
        if (startSocket != null && endSocket != null)
        {
            Vector3 startPos = startSocket.transform.position;
            Vector3 endPos = endSocket.transform.position;
            
            hallwayLength = Vector3.Distance(startPos, endPos);
            hallwayDirection = (endPos - startPos).normalized;
            
            // Position hallway at midpoint
            transform.position = (startPos + endPos) / 2f;
            transform.rotation = Quaternion.LookRotation(hallwayDirection);
        }
    }
    
    private void SetupHallwayGeometry()
    {
        // Create hallway floor
        CreateHallwayFloor();
        
        // Create hallway walls
        CreateHallwayWalls();
        
        // Create hallway ceiling
        CreateHallwayCeiling();
    }
    
    private void CreateHallwayFloor()
    {
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "HallwayFloor";
        floor.transform.SetParent(transform);
        floor.transform.localPosition = Vector3.zero;
        floor.transform.localScale = new Vector3(2f, 0.1f, hallwayLength);
        
        // Apply material
        Renderer floorRenderer = floor.GetComponent<Renderer>();
        if (hallwayFloorMaterial != null)
        {
            floorRenderer.material = hallwayFloorMaterial;
        }
        
        // Remove collider if not needed
        Collider floorCollider = floor.GetComponent<Collider>();
        if (floorCollider != null)
        {
            floorCollider.isTrigger = false; // Floor should have collision
        }
    }
    
    private void CreateHallwayWalls()
    {
        // Left wall
        GameObject leftWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        leftWall.name = "HallwayWall_Left";
        leftWall.transform.SetParent(transform);
        leftWall.transform.localPosition = new Vector3(-1f, 1.5f, 0f);
        leftWall.transform.localScale = new Vector3(0.2f, 3f, hallwayLength);
        
        // Right wall
        GameObject rightWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rightWall.name = "HallwayWall_Right";
        rightWall.transform.SetParent(transform);
        rightWall.transform.localPosition = new Vector3(1f, 1.5f, 0f);
        rightWall.transform.localScale = new Vector3(0.2f, 3f, hallwayLength);
        
        // Apply materials
        Renderer leftRenderer = leftWall.GetComponent<Renderer>();
        Renderer rightRenderer = rightWall.GetComponent<Renderer>();
        
        if (hallwayWallMaterial != null)
        {
            leftRenderer.material = hallwayWallMaterial;
            rightRenderer.material = hallwayWallMaterial;
        }
        
        // Remove colliders if not needed for gameplay
        Collider leftCollider = leftWall.GetComponent<Collider>();
        Collider rightCollider = rightWall.GetComponent<Collider>();
        
        if (leftCollider != null) leftCollider.isTrigger = false;
        if (rightCollider != null) rightCollider.isTrigger = false;
    }
    
    private void CreateHallwayCeiling()
    {
        GameObject ceiling = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ceiling.name = "HallwayCeiling";
        ceiling.transform.SetParent(transform);
        ceiling.transform.localPosition = new Vector3(0f, 3f, 0f);
        ceiling.transform.localScale = new Vector3(2.2f, 0.1f, hallwayLength);
        
        // Apply material
        Renderer ceilingRenderer = ceiling.GetComponent<Renderer>();
        if (hallwayCeilingMaterial != null)
        {
            ceilingRenderer.material = hallwayCeilingMaterial;
        }
        
        // Remove collider for ceiling
        Collider ceilingCollider = ceiling.GetComponent<Collider>();
        if (ceilingCollider != null)
        {
            ceilingCollider.isTrigger = true;
        }
    }
    
    private void ApplyHeavenTheme()
    {
        // Apply Heaven-themed lighting
        SetupHallwayLighting();
        
        // Apply Heaven-themed effects
        SetupHallwayEffects();
    }
    
    private void SetupHallwayLighting()
    {
        // Create lights along the hallway
        int lightCount = Mathf.Max(2, Mathf.RoundToInt(hallwayLength / 4f));
        hallwayLights = new Light[lightCount];
        
        for (int i = 0; i < lightCount; i++)
        {
            GameObject lightObj = new GameObject($"HallwayLight_{i}");
            lightObj.transform.SetParent(transform);
            
            float t = (float)i / (lightCount - 1);
            Vector3 lightPos = new Vector3(0f, 2.5f, Mathf.Lerp(-hallwayLength/2f, hallwayLength/2f, t));
            lightObj.transform.localPosition = lightPos;
            
            Light light = lightObj.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = lightColor;
            light.intensity = lightIntensity;
            light.range = 5f;
            
            hallwayLights[i] = light;
        }
    }
    
    private void SetupHallwayEffects()
    {
        // Create dust particles for atmosphere
        if (dustParticles == null)
        {
            GameObject particleObj = new GameObject("DustParticles");
            particleObj.transform.SetParent(transform);
            particleObj.transform.localPosition = Vector3.zero;
            
            dustParticles = particleObj.AddComponent<ParticleSystem>();
            var main = dustParticles.main;
            main.startLifetime = 5f;
            main.startSpeed = 0.1f;
            main.startSize = 0.05f;
            main.startColor = new Color(1f, 1f, 0.9f, 0.3f);
            main.maxParticles = 20;
            
            var emission = dustParticles.emission;
            emission.rateOverTime = 2f;
            
            var shape = dustParticles.shape;
            shape.shapeType = ParticleSystemShapeType.Box;
            shape.scale = new Vector3(2f, 1f, hallwayLength);
        }
    }
    
    private void GenerateHallwayContent()
    {
        // Generate decorations
        GenerateDecorations();
        
        // Generate obstacles if needed
        if (hasObstacles)
        {
            GenerateObstacles();
        }
    }
    
    private void GenerateDecorations()
    {
        if (decorations != null && decorations.Length > 0)
        {
            int decorationCount = Random.Range(0, 3); // 0-2 decorations per hallway
            
            for (int i = 0; i < decorationCount; i++)
            {
                GameObject decorationPrefab = decorations[Random.Range(0, decorations.Length)];
                if (decorationPrefab != null)
                {
                    GameObject decoration = Instantiate(decorationPrefab, transform);
                    
                    // Position decoration randomly along hallway
                    float randomZ = Random.Range(-hallwayLength/2f + 1f, hallwayLength/2f - 1f);
                    decoration.transform.localPosition = new Vector3(0f, 0f, randomZ);
                    
                    decoration.name = $"HallwayDecoration_{i}";
                }
            }
        }
    }
    
    private void GenerateObstacles()
    {
        if (obstacles != null && obstacles.Length > 0)
        {
            int obstacleCount = Random.Range(1, 3); // 1-2 obstacles per hallway
            
            for (int i = 0; i < obstacleCount; i++)
            {
                GameObject obstaclePrefab = obstacles[Random.Range(0, obstacles.Length)];
                if (obstaclePrefab != null)
                {
                    GameObject obstacle = Instantiate(obstaclePrefab, transform);
                    
                    // Position obstacle randomly along hallway
                    float randomZ = Random.Range(-hallwayLength/2f + 1f, hallwayLength/2f - 1f);
                    obstacle.transform.localPosition = new Vector3(0f, 0f, randomZ);
                    
                    obstacle.name = $"HallwayObstacle_{i}";
                }
            }
        }
    }
    
    public void SetHallwayActive(bool active)
    {
        gameObject.SetActive(active);
        
        // Enable/disable lights
        if (hallwayLights != null)
        {
            foreach (Light light in hallwayLights)
            {
                if (light != null)
                {
                    light.enabled = active;
                }
            }
        }
        
        // Enable/disable particles
        if (dustParticles != null)
        {
            if (active)
            {
                dustParticles.Play();
            }
            else
            {
                dustParticles.Stop();
            }
        }
    }
    
    public void UpdateHallwayMaterials(Material floorMat, Material wallMat, Material ceilingMat)
    {
        hallwayFloorMaterial = floorMat;
        hallwayWallMaterial = wallMat;
        hallwayCeilingMaterial = ceilingMat;
        
        // Apply new materials
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            if (renderer.name.Contains("Floor") && floorMat != null)
            {
                renderer.material = floorMat;
            }
            else if (renderer.name.Contains("Wall") && wallMat != null)
            {
                renderer.material = wallMat;
            }
            else if (renderer.name.Contains("Ceiling") && ceilingMat != null)
            {
                renderer.material = ceilingMat;
            }
        }
    }
    
    public void UpdateHallwayLighting(Color color, float intensity)
    {
        lightColor = color;
        lightIntensity = intensity;
        
        if (hallwayLights != null)
        {
            foreach (Light light in hallwayLights)
            {
                if (light != null)
                {
                    light.color = color;
                    light.intensity = intensity;
                }
            }
        }
    }
    
    // Getters
    public bool IsConnected => startSocket != null && endSocket != null;
    public float Length => hallwayLength;
    public Vector3 Direction => hallwayDirection;
    
    // Debug visualization
    private void OnDrawGizmos()
    {
        if (startSocket != null && endSocket != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(startSocket.transform.position, endSocket.transform.position);
            
            // Draw hallway bounds
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, new Vector3(2f, 3f, hallwayLength));
        }
    }
}
