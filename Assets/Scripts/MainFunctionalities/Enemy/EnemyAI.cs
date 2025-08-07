using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform player; // El jugador al que perseguir
    public Vector3 targetPosition; // El siguiente punto al que se moverá el enemigo
    public float areaX = 50;
    public float areaZ = 50;

    [Header("Detection Settings")]
    public float detectionRange = 2f; // Rango de detección
    public float attackRange = 2f; // Rango de ataque
    public float visionDistance = 10f; // Rango de detección
    public float visionAngle = 45f; // Ángulo del cono de visión
    public float visionHeight = 1.5f; // Altura a la que el raycast verifica la visión

    [Header("Movement Settings")]
    public float patrolSpeed = 4f; // Velocidad de patrulla
    public float chaseSpeed = 7f; // Velocidad de patrulla
    public LayerMask obstacleLayer; // Capa de obstáculos para el raycast

    private NavMeshAgent agent;
    private enum State { Patrol, Chase, Attack }
    private State currentState = State.Patrol;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // Asignar el jugador por su etiqueta
        agent = GetComponent<NavMeshAgent>();

        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;  // Mejor evasión de obstáculos
        agent.avoidancePriority = Random.Range(0, 99); // Prioridad de evasión aleatoria


        StartCoroutine(InitializeNavMeshAgent()); // Iniciar la corutina para inicializar el agente de navegación
    }

    void Update()
    {
        // Comprobar el estado actual y ejecutar la lógica correspondiente
        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                break;
            case State.Chase:
                Chase();
                break;
            case State.Attack:
                Attack();
                break;
        }
    }

    void Patrol()
    {
        if (!NVIsRebaked() || !agent.isOnNavMesh) return;
        agent.speed = patrolSpeed; // Velocidad de patrulla
        // Si el enemigo está lo suficientemente cerca del jugador, comienza la persecución
        if (Vector3.Distance(transform.position, player.position) < detectionRange && IsPlayerOnSight())
        {
            currentState = State.Chase;
            return;
        }

        // Si el enemigo ha llegado al destino o ha chocado con un obstáculo, asigna un nuevo punto aleatorio dentro de la malla de navegación
        if (!agent.pathPending && agent.remainingDistance < agent.stoppingDistance+0.1f)
        {
            SetRandomDestination();
        }
    }

    void Chase()
    {
        agent.SetDestination(player.position); // El enemigo sigue al jugador
        agent.speed = chaseSpeed; // Velocidad de persecución
        // Si está dentro del rango de ataque, cambia al estado de ataque
        if (Vector3.Distance(transform.position, player.position) < attackRange)
        {
            currentState = State.Attack;
        }

        // Si el enemigo pierde la línea de visión, vuelve a patrullar
        if (Vector3.Distance(transform.position, player.position) > detectionRange || !IsPlayerOnSight())
        {
            currentState = State.Patrol;
        }
    }

    void Attack()
    {
        // Aquí va la lógica para atacar al jugador (puede ser restar vida, por ejemplo)
        Debug.Log("Atacando al jugador!");
    }

    // Método para asignar un destino aleatorio dentro de la malla de navegación (mazmorras)
    void SetRandomDestination()
    {
        // Generamos un punto aleatorio dentro de los límites de la malla de navegación
        Vector3 randomPoint = new Vector3(
            Random.Range(-areaX, areaX),  // Limita el área según el tamaño de la mazmorras, ajusta según tus necesidades
            transform.position.y,     // Mantener la misma altura
            Random.Range(-areaZ, areaZ)   // Limita el área según el tamaño de la mazmorras, ajusta según tus necesidades
        );

        // Usamos NavMesh.SamplePosition para asegurarnos de que el punto está dentro de la malla de navegación
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 10f, NavMesh.AllAreas))
        {
            if (!IsNearOtherTarget(hit.position))
            {
                targetPosition = hit.position; // Asigna el punto válido dentro de la malla
                agent.SetDestination(targetPosition); // Mueve al enemigo hacia el nuevo destino
            }
            else { 
                SetRandomDestination(); // Si hay otro enemigo cerca del destino, intenta encontrar otro destino
            }
        }
    }

    private bool IsNearOtherTarget(Vector3 newPos)
    {
        Debug.Log("Comprobando si hay otro enemigo cerca del destino: " + targetPosition);
        foreach (EnemyAI enemy in FindObjectsByType<EnemyAI>(FindObjectsSortMode.None))
        {
            if (enemy != this && Vector3.Distance(enemy.targetPosition, newPos) < 10f)
            {
                return true; // Hay otro enemigo cerca del destino
            }
        }
        return false; // No hay otros enemigos cerca del destino
    }

    // Método para comprobar si hay una línea de visión clara al jugador utilizando raycasting
    
    bool IsPlayerOnSight()
    {
        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        // Verifies if Player is inside vision distance
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        // Verifies if Player is inside vision angle
        if (angleToPlayer < visionAngle / 2)
        {
            if (distanceToPlayer < visionDistance)
            {
                // Perform a raycast to check if there is a clear line of sight
                if (!Physics.Raycast(transform.position + Vector3.up * visionHeight, directionToPlayer, distanceToPlayer, obstacleLayer))
                {
                    return true;
                }
            }
        }
        if (distanceToPlayer < detectionRange)
        {
            if (!Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleLayer))
            {
                return true;
            }
        }

        return false;
    }
    IEnumerator InitializeNavMeshAgent() { 
        yield return new WaitUntil(()=> GameObject.FindAnyObjectByType<RebakeNavmesh>().rebaked);
        agent.enabled = true; // Habilitar el agente de navegación una vez que la malla de navegación esté lista
        SetRandomDestination(); // Iniciar con un destino aleatorio dentro de la malla de navegación
    }

    bool NVIsRebaked() {
        if (FindAnyObjectByType<RebakeNavmesh>().rebaked)
        {
            return true; // La malla de navegación ha sido rebaked
        }
        else
        {
            return false; // La malla de navegación no ha sido rebaked
        }
    }

    // Dibujar Gizmos para visualizar el área de detección, el cono de visión y el siguiente destino
    private void OnDrawGizmos()
    {
        // Si el enemigo no tiene un agente de navegación, no dibujes los Gizmos
        if (agent == null) return;

        // 1. Área de Detección: Círculo que muestra el rango de detección
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position, new Vector3(areaX, 0.1f, areaZ)); // Área de patrullaje


        // 2. Cono de visión: Representado como líneas que forman un triángulo
        Gizmos.color = Color.yellow;
        Vector3 forward = transform.forward * detectionRange;
        Vector3 leftBound = Quaternion.Euler(0, -visionAngle / 2, 0) * forward;
        Vector3 rightBound = Quaternion.Euler(0, visionAngle / 2, 0) * forward;

        Gizmos.DrawLine(transform.position, transform.position + leftBound);  // Línea izquierda
        Gizmos.DrawLine(transform.position, transform.position + rightBound); // Línea derecha
        Gizmos.DrawLine(transform.position + leftBound, transform.position + rightBound); // Línea de unión

        // 3. Siguiente destino (punto de patrullaje o de persecución): Muestra el siguiente punto de movimiento
        Gizmos.color = Color.blue;
        if (targetPosition != Vector3.zero)
        {
            Gizmos.DrawSphere(targetPosition, 0.5f); // Esfera azul que marca el siguiente destino
        }
    }
}