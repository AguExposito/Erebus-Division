using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;

public class RebakeNavmesh : MonoBehaviour
{
    public static RebakeNavmesh Instance;
    public bool rebaked=false;
    void Start()
    {
        StartCoroutine(RebakeNavMesh());
    }

    public IEnumerator RebakeNavMesh()
    {
        // Esperar un fotograma para asegurarse de que todo esté cargado antes de realizar el rebake
        yield return new WaitForSeconds(1);

        // Obtener el componente NavMeshSurface
        NavMeshSurface navMeshSurface = GetComponent<NavMeshSurface>();

        // Verificar si el componente existe
        if (navMeshSurface != null)
        {
            // Realizar el rebake de la malla de navegación
            navMeshSurface.BuildNavMesh();
            Debug.Log("NavMesh rebaked successfully.");
            rebaked = true;
        }
        else
        {
            Debug.LogError("No NavMeshSurface found on this GameObject.");
        }
    }
}
