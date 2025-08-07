using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player; // El jugador al que perseguir
    public float detectionRange = 10f; // Rango de detecci�n
    public float attackRange = 2f; // Rango de ataque
    public float visionAngle = 45f; // �ngulo del cono de visi�n
    public float visionHeight = 1.5f; // Altura a la que el raycast verifica la visi�n
    public float areaX = 50;
    public float areaZ = 50;
    public LayerMask obstacleLayer; // Capa de obst�culos para el raycast

    private NavMeshAgent agent;
    private Vector3 targetPosition; // El siguiente punto al que se mover� el enemigo
    private enum State { Patrol, Chase, Attack }
    private State currentState = State.Patrol;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // Asignar el jugador por su etiqueta
        agent = GetComponent<NavMeshAgent>();

        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;  // Mejor evasi�n de obst�culos
        agent.avoidancePriority = Random.Range(0, 99); // Prioridad de evasi�n aleatoria

        SetRandomDestination(); // Iniciar con un destino aleatorio dentro de la malla de navegaci�n
    }

    void Update()
    {
        // Comprobar el estado actual y ejecutar la l�gica correspondiente
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
        // Si el enemigo est� lo suficientemente cerca del jugador, comienza la persecuci�n
        if (Vector3.Distance(transform.position, player.position) < detectionRange && HasClearLineOfSight())
        {
            currentState = State.Chase;
            return;
        }

        // Si el enemigo ha llegado al destino o ha chocado con un obst�culo, asigna un nuevo punto aleatorio dentro de la malla de navegaci�n
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            SetRandomDestination();
        }
    }

    void Chase()
    {
        agent.SetDestination(player.position); // El enemigo sigue al jugador

        // Si est� dentro del rango de ataque, cambia al estado de ataque
        if (Vector3.Distance(transform.position, player.position) < attackRange)
        {
            currentState = State.Attack;
        }

        // Si el enemigo pierde la l�nea de visi�n, vuelve a patrullar
        if (Vector3.Distance(transform.position, player.position) > detectionRange || !HasClearLineOfSight())
        {
            currentState = State.Patrol;
        }
    }

    void Attack()
    {
        // Aqu� va la l�gica para atacar al jugador (puede ser restar vida, por ejemplo)
        Debug.Log("Atacando al jugador!");
    }

    // M�todo para asignar un destino aleatorio dentro de la malla de navegaci�n (mazmorras)
    void SetRandomDestination()
    {
        // Generamos un punto aleatorio dentro de los l�mites de la malla de navegaci�n
        Vector3 randomPoint = new Vector3(
            Random.Range(-areaX, areaX),  // Limita el �rea seg�n el tama�o de la mazmorras, ajusta seg�n tus necesidades
            transform.position.y,     // Mantener la misma altura
            Random.Range(-areaZ, areaZ)   // Limita el �rea seg�n el tama�o de la mazmorras, ajusta seg�n tus necesidades
        );

        // Usamos NavMesh.SamplePosition para asegurarnos de que el punto est� dentro de la malla de navegaci�n
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1f, NavMesh.AllAreas))
        {
            targetPosition = hit.position; // Asigna el punto v�lido dentro de la malla
            agent.SetDestination(targetPosition); // Mueve al enemigo hacia el nuevo destino
        }
    }

    // M�todo para comprobar si hay una l�nea de visi�n clara al jugador utilizando raycasting
    bool HasClearLineOfSight()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        RaycastHit hit;

        // Realizar un raycast hacia el jugador, desde el enemigo hasta la altura de visi�n
        if (Physics.Raycast(transform.position + Vector3.up * visionHeight, direction, out hit, detectionRange, obstacleLayer))
        {
            // Si el raycast golpea algo que no es el jugador, no hay l�nea de visi�n clara
            if (hit.transform != player)
            {
                return false;
            }
        }
        return true;
    }

    // Dibujar Gizmos para visualizar el �rea de detecci�n, el cono de visi�n y el siguiente destino
    private void OnDrawGizmos()
    {
        // Si el enemigo no tiene un agente de navegaci�n, no dibujes los Gizmos
        if (agent == null) return;

        // 1. �rea de Detecci�n: C�rculo que muestra el rango de detecci�n
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position, new Vector3(areaX, 0.1f, areaZ)); // �rea de patrullaje


        // 2. Cono de visi�n: Representado como l�neas que forman un tri�ngulo
        Gizmos.color = Color.yellow;
        Vector3 forward = transform.forward * detectionRange;
        Vector3 leftBound = Quaternion.Euler(0, -visionAngle / 2, 0) * forward;
        Vector3 rightBound = Quaternion.Euler(0, visionAngle / 2, 0) * forward;

        Gizmos.DrawLine(transform.position, transform.position + leftBound);  // L�nea izquierda
        Gizmos.DrawLine(transform.position, transform.position + rightBound); // L�nea derecha
        Gizmos.DrawLine(transform.position + leftBound, transform.position + rightBound); // L�nea de uni�n

        // 3. Siguiente destino (punto de patrullaje o de persecuci�n): Muestra el siguiente punto de movimiento
        Gizmos.color = Color.blue;
        if (targetPosition != Vector3.zero)
        {
            Gizmos.DrawSphere(targetPosition, 0.5f); // Esfera azul que marca el siguiente destino
        }
    }
}