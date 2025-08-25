using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public Outline outline;
    public AnimationManager animationManager;

    [Space]
    [Header("Target Settings")]
    public Transform player; // El jugador al que perseguir
    public Vector3 targetPosition; // El siguiente punto al que se moverá el enemigo
    public float areaX = 50;
    public float areaZ = 50;

    [Space]
    [Header("Detection Settings")]
    public float detectionRange = 2f; // Rango de detección
    public float attackRange = 2f; // Rango de ataque
    public float visionDistance = 10f; // Rango de detección
    public float visionAngle = 45f; // Ángulo del cono de visión
    public float visionHeight = 1.5f; // Altura a la que el raycast verifica la visión

    [Space]
    [Header("Movement Settings")]
    public float patrolSpeed = 4f; // Velocidad de patrulla
    public float chaseSpeed = 7f; // Velocidad de patrulla
    public float rotationDuration = 0.25f; // Velocidad de patrulla
    public float acceleration = 8f; 
    public float angularVelocity = 300f;
    public float multiplier = 1f; // Multiplicador para la velocidad del enemigo
    public LayerMask obstacleLayer; // Capa de obstáculos para el raycast
    public State currentState = State.Patrol;

    [Space]
    [Header("Status")]
    public bool isDead = false; // Estado de muerte del enemigo
    public bool isAttacking = false; // Estado de muerte del enemigo
    public bool isScreaming = false; // Estado de grito del enemigo
    public bool isChasing = false; // Flag para verificar si se ha iniciado el Chase

    public enum State { Patrol, Chase, Attack }

    private float timeElapsed = 0f; // Tiempo transcurrido para la rotación
    private NavMeshAgent agent;
    private Vector3 tempPos;

    public static float attackers = 0; // Contador de enemigos atacantes
    public static List<EnemyAI> enemies = new List<EnemyAI>(); // Lista de enemigos

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // Asignar el jugador por su etiqueta
        agent = GetComponent<NavMeshAgent>();

        //agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;  // Mejor evasión de obstáculos
        agent.avoidancePriority = Random.Range(0, 99); // Prioridad de evasión aleatoria

        enemies.Add(this); // Agregar este enemigo a la lista de enemigos
        Debug.Log("Enemigo agregado a la lista de enemigos: " + enemies.Count);

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
        agent.speed = patrolSpeed * multiplier; // Velocidad de patrulla
        // Si el enemigo está lo suficientemente cerca del jugador, comienza la persecución
        if (Vector3.Distance(transform.position, player.position) < visionDistance && IsPlayerOnSight())
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
        // Comienza la corutina solo si no ha sido iniciada previamente
        if (!isChasing)
        {
            ChaseState(true); // Marcamos que se ha comenzado el Chase
            isScreaming = true; // Establecemos que está gritando
            agent.speed = 0; // Detenemos el agente
            StartCoroutine(WaitEndOfScream()); // Iniciamos la corutina
        }

        // Después de que el grito haya terminado, el enemigo sigue al jugador
        if (!isScreaming)
        {
            agent.SetDestination(player.position); // El enemigo sigue al jugador
            agent.speed = chaseSpeed * multiplier; // Velocidad de persecución

            // Si está dentro del rango de ataque, cambia al estado de ataque
            if (Vector3.Distance(transform.position, player.position) < attackRange)
            {
                currentState = State.Attack;
            }

            // Si el enemigo pierde la línea de visión, vuelve a patrullar
            if (Vector3.Distance(transform.position, player.position) > visionDistance || !IsPlayerOnSight())
            {
                if (tempPos == Vector3.zero)
                {
                    tempPos = player.position;
                    agent.SetDestination(tempPos);
                    StartCoroutine(LookingForPlayer());
                }
            }
        }
    }

    private void ChaseState(bool state)
    {
        isChasing = state;
        animationManager.anim.SetBool("Chasing", state);
    }

    IEnumerator WaitEndOfScream()
    {
        AnimationClip screamClip = animationManager.GetAnimationClip("Scream");
        if (screamClip != null)
        {
            yield return new WaitForSeconds(screamClip.length/1.5f);
            isScreaming = false;
            Debug.LogWarning("SCREAM ENDED");
        }
        else
        {
            Debug.LogWarning("Scream animation not found!");
            yield return null;
        }
    }

    IEnumerator LookingForPlayer() {
        Debug.Log("Buscando Player");
        yield return new WaitUntil(()=> agent.remainingDistance < agent.stoppingDistance+0.1f);
        Debug.Log("Esperando Delay...");
        yield return new WaitForSeconds(3f);
        tempPos = Vector3.zero;
        currentState = State.Patrol;
    }

    void Attack()
    {
        if (isAttacking) return;

        gameObject.tag = "AttackingEnemy";
        AttackState(true);
        attackers++; // Incrementa el contador de atacantes
        Debug.Log("Atacantes: " + attackers);

        CompletelyStopAgent(true);
        SetAgentSpeed();

        StartCoroutine(player.GetComponent<FPSController>().RotateCameraPlayer(transform)); // Desactiva el controlador del jugador
        StartCoroutine(RotateEnemyToPlayer()); // Rotar hacia jugador
        attackRange = 0;

        TurnManager.instance.AddTurn(GetComponent<EntityInterface>()); // Agrega este enemigo al turno del TurnManager

        Debug.Log("Atacando al jugador!");
    }

    private void AttackState(bool state)
    {
        isAttacking = state; // Cambia el estado a atacando
        animationManager.anim.SetBool("InCombat", state);
    }

    public void OnPlayerFled()
    {
        attackers = 0;
        agent.GetComponent<NavMeshObstacle>().enabled = false;
        RebakeNavmesh.Instance.RebakeNavMesh();
        FinishedEncounter();
        StartCoroutine(ResumeMovementAfterDelay(1f));
    }

    private IEnumerator ResumeMovementAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        foreach (EnemyAI enemy in enemies)
        {
            if (enemy != null && !enemy.isDead)
            {
                enemy.CompletelyStopAgent(false); // Reactiva la navegación
                enemy.AttackState(false);         // Sale del modo combate
                enemy.attackRange = 2f;
                enemy.currentState = State.Patrol;
            }
        }
    }



    public void EndEncounter()
    {
        if (isDead) return; // Si el enemigo ya está muerto, no hacer nada
        isDead = true; // Cambia el estado a muerto
        attackers--; // Decrementa el contador de atacantes

        enemies.Remove(this); // Elimina este enemigo de la lista de enemigos

        if (attackers <= 0) 
        { 
            FinishedEncounter(); // Termina el encuentro si no hay más atacantes
        }

        Destroy(gameObject);
    }

    private void CompletelyStopAgent(bool isStop)
    {
        if (agent == null) return; // Verifica si el agente de navegación está asignado
        if (isStop)
        {
            agent.isStopped = true; // Detiene el agente de navegación
            SetAgentSpeed();
            agent.ResetPath(); // Resetea la ruta del agente
            agent.velocity = Vector3.zero; // Detiene el movimiento del agente
            agent.angularSpeed = 0; // Detiene la rotación del agente
            agent.acceleration = 0; // Detiene la aceleración del agente
            agent.updateRotation = false; // Desactiva la rotación automática del agente
            agent.updatePosition = false; // Desactiva la actualización de la posición del agente
            agent.SetDestination(transform.position); // Establece la posición actual como destino para detener el movimiento
        }
        else 
        {
            agent.isStopped = false; // Reanuda el agente de navegación
            SetAgentSpeed();
            agent.angularSpeed = angularVelocity; // Reanuda la rotación del agente
            agent.acceleration = acceleration; // Reanuda la aceleración del agente
            agent.updateRotation = true; // Reanuda la rotación automática del agente
            agent.updatePosition = true; // Reanuda la actualización de la posición del agente
        }
    }

    public void SetAgentSpeed()
    {
        foreach (var enemy in enemies)
        {
            if (enemy.isAttacking)
            {
                agent.GetComponent<NavMeshObstacle>().enabled = true;
                continue;
            }
            switch (attackers)
            {
                case 0:
                    enemy.multiplier = 1;
                    break;
                case 1:
                    enemy.multiplier = 0.66f;

                    break;
                case 2:
                    enemy.multiplier = 0.33f;
                    break;
                case 3:
                    enemy.multiplier = 0;
                    break;
                default:
                    enemy.multiplier = 1;
                    break;
            }
        }
    }


    void FinishedEncounter() {
        Time.timeScale = 1; // Reanuda el juego
        player.GetComponent<FPSController>().GiveBackControlToPlayer(); // Reactiva el controlador del jugador
        if(!isDead) StartCoroutine(ChaseDelay());
    }

    IEnumerator ChaseDelay() { 
        yield return new WaitForSeconds(2f);
        AttackState(false);
        attackRange = 2f;
        currentState = State.Chase;
    }

    IEnumerator RotateEnemyToPlayer() {

        // Obtener la dirección hacia el objetivo
        Vector3 directionToTarget = player.position - transform.position;
        directionToTarget.y = 0; // Mantener solo la dirección horizontal

        // Calcular el ángulo de rotación (solo en Y)
        float targetPlayerAngle = Mathf.Atan2(directionToTarget.x, directionToTarget.z) * Mathf.Rad2Deg;

        // Obtener la rotación inicial
        float initialPlayerAngle = transform.eulerAngles.y;

        // Normalizar los ángulos para evitar rotaciones largas
        float angleDifference = Mathf.DeltaAngle(initialPlayerAngle, targetPlayerAngle);
        targetPlayerAngle = initialPlayerAngle + angleDifference;

        timeElapsed = 0f;

        // Rotar el player primero
        while (timeElapsed < rotationDuration)
        {
            timeElapsed += Time.unscaledDeltaTime;
            float t = timeElapsed / rotationDuration;

            // Interpolar la rotación del player
            float currentAngle = Mathf.LerpAngle(initialPlayerAngle, targetPlayerAngle, t);
            transform.rotation = Quaternion.Euler(0, currentAngle, 0);

            yield return null;
        }

        // Asegurar que el player esté exactamente en la posición final
        transform.rotation = Quaternion.Euler(0, targetPlayerAngle, 0);
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
        Vector3 forward = transform.forward * visionDistance;
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