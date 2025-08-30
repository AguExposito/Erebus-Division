using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;

public class RebakeNavmesh : MonoBehaviour
{
    public static RebakeNavmesh Instance;
    public bool rebaked = false;
    
    void Awake()
    {
        // Configurar el patrón Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        StartCoroutine(RebakeNavMesh());
    }

    public IEnumerator RebakeNavMesh()
    {
        // Resetear la variable rebaked
        rebaked = false;
        
        // Esperar un fotograma para asegurarse de que todo est cargado antes de realizar el rebake
        yield return new WaitForSeconds(0.5f);

        // Obtener el componente NavMeshSurface
        NavMeshSurface navMeshSurface = GetComponent<NavMeshSurface>();

        // Verificar si el componente existe
        if (navMeshSurface != null)
        {
            // Realizar el rebake de la malla de navegaci�n
            navMeshSurface.BuildNavMesh();
            Debug.Log("NavMesh rebaked successfully.");
            rebaked = true;
        }
        else
        {
            Debug.LogError("No NavMeshSurface found on this GameObject.");
            rebaked = true; // Marcar como completado para evitar que se quede colgado
        }
    }
}
