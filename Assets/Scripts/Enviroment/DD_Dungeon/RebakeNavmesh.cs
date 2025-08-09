using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;

public class RebakeNavmesh : MonoBehaviour
{
    public bool rebaked=false;
    void Start()
    {
        StartCoroutine(RebakeNavMesh());
    }

    private IEnumerator RebakeNavMesh()
    {
        // Esperar un fotograma para asegurarse de que todo est� cargado antes de realizar el rebake
        yield return new WaitForSeconds(1);

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
        }
    }
}
