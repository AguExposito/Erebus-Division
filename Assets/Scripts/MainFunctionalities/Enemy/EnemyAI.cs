using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player; // El jugador al que perseguir
    public float detectionRange = 10f; // Rango de detección
    public float attackRange = 2f; // Rango de ataque
    public float visionAngle = 45f; // Ángulo del cono de visión
    public float visionHeight = 1.5f; // Altura a la que el raycast verifica la visión
    public float areaX = 50;
    public float areaZ = 50;
    public LayerMask obstacleLayer; // Capa de obstáculos para el raycast

    private NavMeshAgent agent;
    private Vector3 targetPosition; // El siguiente punto al que se moverá el enemigo
    private enum State { Patrol, Chase, Attack }
    private State currentState = State.Patrol;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // Asignar el jugador por su etiqueta
        agent = GetComponent<NavMeshAgent>();

        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;  // Mejor evasión de obstáculos
        agent.avoidancePriority = Random.Range(0, 99); // Prioridad de evasión aleatoria

        SetRandomDestination(); // Iniciar con un destino aleatorio dentro de la malla de navegación
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
        // Si el enemigo está lo suficientemente cerca del jugador, comienza la persecución
        if (Vector3.Distance(transform.position, player.position) < detectionRange && HasClearLineOfSight())
        {
            currentState = State.Chase;
            return;
        }

        // Si el enemigo ha llegado al destino o ha chocado con un obstáculo, asigna un nuevo punto aleatorio dentro de la malla de navegación
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            SetRandomDestination();
        }
    }

    void Chase()
    {
        agent.SetDestination(player.position); // El enemigo sigue al jugador

        // Si está dentro del rango de ataque, cambia al estado de ataque
        if (Vector3.Distance(transform.position, player.position) < attackRange)
        {
            currentState = State.Attack;
        }

        // Si el enemigo pierde la línea de visión, vuelve a patrullar
        if (Vector3.Distance(transform.position, player.position) > detectionRange || !HasClearLineOfSight())
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
        if (NavMesh.SamplePosition(randomPoint, out hit, 1f, NavMesh.AllAreas))
        {
            targetPosition = hit.position; // Asigna el punto válido dentro de la malla
            agent.SetDestination(targetPosition); // Mueve al enemigo hacia el nuevo destino
        }
    }

    // Método para comprobar si hay una línea de visión clara al jugador utilizando raycasting
    bool HasClearLineOfSight()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        RaycastHit hit;

        // Realizar un raycast hacia el jugador, desde el enemigo hasta la altura de visión
        if (Physics.Raycast(transform.position + Vector3.up * visionHeight, direction, out hit, detectionRange, obstacleLayer))
        {
            // Si el raycast golpea algo que no es el jugador, no hay línea de visión clara
            if (hit.transform != player)
            {
                return false;
            }
        }
        return true;
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