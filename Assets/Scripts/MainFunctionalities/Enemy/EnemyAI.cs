
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
    public Vector3 targetPosition; // El siguiente punto al que se mover� el enemigo
    public float areaX = 50;
    public float areaZ = 50;

    [Space]
    [Header("Detection Settings")]
    public float detectionRange = 2f; // Rango de detecci�n
    public float attackRange = 2f; // Rango de ataque
    public float visionDistance = 10f; // Rango de detecci�n
    public float visionAngle = 45f; // �ngulo del cono de visi�n
    public float visionHeight = 1.5f; // Altura a la que el raycast verifica la visi�n

    [Space]
    [Header("Movement Settings")]
    public float patrolSpeed = 4f; // Velocidad de patrulla
    public float chaseSpeed = 7f; // Velocidad de patrulla
    public float rotationDuration = 0.25f; // Velocidad de patrulla
    public float acceleration = 8f; 
    public float angularVelocity = 300f;
    public float multiplier = 1f; // Multiplicador para la velocidad del enemigo
    public LayerMask obstacleLayer; // Capa de obst�culos para el raycast
    public State currentState = State.Patrol;

    [Space]
    [Header("Status")]
    public bool isDead = false; // Estado de muerte del enemigo
    public bool isAttacking = false; // Estado de muerte del enemigo
    public bool isScreaming = false; // Estado de grito del enemigo
    public bool isChasing = false; // Flag para verificar si se ha iniciado el Chase

    [Space]
    [Header("Audio")]
    public AudioSource audioSource;
    public List<AudioClip> screamClips = new List<AudioClip>();

    public enum State { Patrol, Chase, Attack }

    private float timeElapsed = 0f; // Tiempo transcurrido para la rotacin
    public NavMeshAgent agent;
    //public NavMeshObstacle obstacle;
    private Vector3 tempPos;

    public static float attackers = 0; // Contador de enemigos atacantes
    public static List<EnemyAI> enemies = new List<EnemyAI>(); // Lista de enemigos

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // Asignar el jugador por su etiqueta
        agent = GetComponent<NavMeshAgent>();

        //agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;  // Mejor evasi�n de obst�culos
        agent.avoidancePriority = Random.Range(0, 99); // Prioridad de evasi�n aleatoria
        //obstacle = GetComponent<NavMeshObstacle>();
        //obstacle.carving = true;
        //obstacle.carveOnlyStationary = true;
        //obstacle.enabled = false; // Initially off

        enemies.Add(this); // Agregar este enemigo a la lista de enemigos
        Debug.Log("Enemigo agregado a la lista de enemigos: " + enemies.Count);

        StartCoroutine(InitializeNavMeshAgent()); // Iniciar la corutina para inicializar el agente de navegaci�n
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
        if (!NVIsRebaked() || !agent.isOnNavMesh) return;
        agent.speed = patrolSpeed * multiplier; // Velocidad de patrulla
        // Si el enemigo est� lo suficientemente cerca del jugador, comienza la persecuci�n
        if (Vector3.Distance(transform.position, player.position) < visionDistance && IsPlayerOnSight())
        {
            currentState = State.Chase;
            return;
        }

        // Si el enemigo ha llegado al destino o ha chocado con un obst�culo, asigna un nuevo punto aleatorio dentro de la malla de navegaci�n
        if (!agent.pathPending && agent.remainingDistance < agent.stoppingDistance+0.1f)
        {
            SetRandomDestination();
        }
    }

    public void PlayScreamAudio() 
    { 
        int randomAudioIndex = Random.Range(0, screamClips.Count);
        float randomPitch = Random.Range(0.8f, 1.2f);
        audioSource.pitch = randomPitch;
        audioSource.PlayOneShot(screamClips[randomAudioIndex]);
    }

    void Chase()
    {
        // Comienza la corutina solo si no ha sido iniciada previamente
        if (!isChasing)
        {
            ChaseState(true); // Marcamos que se ha comenzado el Chase
            agent.obstacleAvoidanceType=ObstacleAvoidanceType.LowQualityObstacleAvoidance; // Cambiamos el tipo de evasi�n de obst�culos para persecuci�n
            agent.avoidancePriority = 99; 
            isScreaming = true; // Establecemos que est� gritando
            PlayScreamAudio();
            if (MusicManager.Instance.currentTrack != 1)
            {
                MusicManager.Instance.PlaySong(1); // Cambiamos la música a la de persecución
            }
            agent.speed = 0; // Detenemos el agente
            StartCoroutine(WaitEndOfScream()); // Iniciamos la corutina
        }

        // Despu�s de que el grito haya terminado, el enemigo sigue al jugador
        if (!isScreaming)
        {
            agent.SetDestination(player.position); // El enemigo sigue al jugador
            agent.speed = chaseSpeed * multiplier; // Velocidad de persecuci�n

            // Si est� dentro del rango de ataque, cambia al estado de ataque
            if (Vector3.Distance(transform.position, player.position) < attackRange && !player.GetComponent<FPSController>().isInElevator)
            {
                currentState = State.Attack;
            }

            // Si el enemigo pierde la l�nea de visi�n, vuelve a patrullar
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
        //obstacle.enabled = false; // Enable obstacle to avoid other agents while screaming
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
        //obstacle.enabled = true; // Disable obstacle after screaming
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
        if (isAttacking || player.GetComponent<FPSController>().isInElevator) return;

        gameObject.tag = "AttackingEnemy";
        AttackState(true);
        attackers++; // Incrementa el contador de atacantes
        Debug.Log("Atacantes: " + attackers);

        CompletelyStopAgent(true);
        SetAgentSpeed();
        ChaseState(false); // Desactiva el estado de Chase

        StartCoroutine(player.GetComponent<FPSController>().RotateCameraPlayer(transform)); // Desactiva el controlador del jugador
        StartCoroutine(RotateEnemyToPlayer(rotationDuration)); // Rotar hacia jugador
        attackRange = 0;

        TurnManager.instance.AddTurn(GetComponent<EntityInterface>()); // Agrega este enemigo al turno del TurnManager

        Debug.Log("Atacando al jugador!");
    }

    private void AttackState(bool state)
    {
        isAttacking = state; // Cambia el estado a atacando
        animationManager.anim.SetBool("InCombat", state);
    }
    public bool IsIdle()
    {
        return agent.enabled && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance+0.05f && !agent.hasPath && Time.timeScale!=0;
    }

    public void OnPlayerFled()
    {
        attackers = 0;
        //StartCoroutine(HandleAgentObstacle());
        FinishedEncounter();
        //StartCoroutine(WaitForNavMeshRebakeAndResume());
    }
    bool canEnableCarve=false;
    //private IEnumerator HandleAgentObstacle() {
    //    obstacle.carving = false;
    //    obstacle.enabled = false;
    //    yield return null;
    //    yield return new WaitUntil(()=> !obstacle.carving);
    //    agent.enabled = true;
    //    agent.Warp(gameObject.transform.position);
    //    yield return null;
    //    yield return new WaitUntil(() => !IsIdle() && agent.isOnNavMesh && canEnableCarve);
    //    obstacle.enabled = true;
    //    obstacle.carving = true;
    //    canEnableCarve = false;
    //}
    public bool TryPlaceBackOnNavMesh(float maxDistance = 0.1f)
    {
        Debug.LogError("REPOSITIONING");
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, maxDistance, NavMesh.AllAreas))
        {
            transform.position = hit.position;
            return true;
        }
        return false;
    }

    void FinishedEncounter()
    {
        Time.timeScale = 1; // Reanuda el juego
        player.GetComponent<FPSController>().GiveBackControlToPlayer(); // Reactiva el controlador del jugador
        if (!isDead) StartCoroutine(PatrolDelay());
        SetAgentSpeed();
        if (MusicManager.Instance.currentTrack != 0)
        {
            MusicManager.Instance.PlaySong(0); // Cambiamos la música a la de persecución
        }
    }

    IEnumerator PatrolDelay()
    {
        yield return new WaitForSeconds(1f);
        if (this != null && !isDead)
        {
            //yield return new WaitUntil(() => !obstacle.carving);
            //canEnableCarve = true;
            agent.Warp(transform.position);
            yield return null;
            if (!agent.isOnNavMesh) if (!TryPlaceBackOnNavMesh()) Debug.LogError("IS NOT ON NAVMESH");
            yield return new WaitUntil(() => agent.isOnNavMesh);
            CompletelyStopAgent(false); // Reactiva la navegacion
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance; // Cambiamos el tipo de evasi�n de obst�culos para patrulla
            agent.avoidancePriority = Random.Range(0, 99); // Prioridad de evasi�n aleatoria
            AttackState(false);
            attackRange = 2f;
            currentState = State.Patrol;
            SetRandomDestination();
        }
        
    }

    public void EndEncounter()
    {
        if (isDead) return; // Si el enemigo ya est� muerto, no hacer nada
        isDead = true; // Cambia el estado a muerto
        attackers--; // Decrementa el contador de atacantes

        enemies.Remove(this); // Elimina este enemigo de la lista de enemigos

        if (attackers <= 0) 
        { 
            FinishedEncounter(); // Termina el encuentro si no hay m�s atacantes
            if (MusicManager.Instance.currentTrack != 0)
            {
                MusicManager.Instance.PlaySong(0); // Cambiamos la música a la de persecución
            }

        }

        Destroy(gameObject);
    }

    private void CompletelyStopAgent(bool isStop)
    {
        if (agent == null) return; // Verifica si el agente de navegacin est asignado
        if (isStop)
        {
            agent.isStopped = true; // Detiene el agente de navegaci�n
            agent.ResetPath(); // Resetea la ruta del agente
            agent.velocity = Vector3.zero; // Detiene el movimiento del agente
            agent.angularSpeed = 0; // Detiene la rotaci�n del agente
            agent.acceleration = 0; // Detiene la aceleraci�n del agente
            agent.updateRotation = false; // Desactiva la rotaci�n autom�tica del agente
            agent.updatePosition = false; // Desactiva la actualizaci�n de la posici�n del agente
            agent.SetDestination(transform.position); // Establece la posici�n actual como destino para detener el movimiento
        }
        else 
        {
            agent.isStopped = false; // Reanuda el agente de navegaci�n
            agent.angularSpeed = angularVelocity; // Reanuda la rotaci�n del agente
            agent.acceleration = acceleration; // Reanuda la aceleraci�n del agente
            agent.updateRotation = true; // Reanuda la rotaci�n autom�tica del agente
            agent.updatePosition = true; // Reanuda la actualizaci�n de la posici�n del agente
        }
    }

    public void SetAgentSpeed()
    {
        foreach (var enemy in enemies)
        {
            if (enemy.isAttacking)
            {
                enemy.agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
                enemy.agent.avoidancePriority = 99;
                continue;
            }
            switch (attackers)
            {
                case 0:
                    enemy.multiplier = 1;
                    enemy.agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance; // Cambiamos el tipo de evasi�n de obst�culos para patrulla
                    enemy.agent.avoidancePriority = Random.Range(0, 99); // Prioridad de evasi�n aleatoria
                    break;
                case 1:
                    enemy.multiplier = 0.66f;
                    enemy.agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance; // Cambiamos el tipo de evasi�n de obst�culos para patrulla
                    break;
                case 2:
                    enemy.multiplier = 0.33f;
                    enemy.agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance; // Cambiamos el tipo de evasi�n de obst�culos para patrulla
                    break;
                case 3:
                    enemy.multiplier = 0;
                    enemy.agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance; // Cambiamos el tipo de evasi�n de obst�culos para patrulla
                    break;
                default:
                    enemy.multiplier = 1;
                    enemy.agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance; // Cambiamos el tipo de evasi�n de obst�culos para patrulla
                    break;
            }
        }
    }


    

    public IEnumerator RotateEnemyToPlayer(float rotationDuration) {

        // Obtener la direcci�n hacia el objetivo
        Vector3 directionToTarget = player.position - transform.position;
        directionToTarget.y = 0; // Mantener solo la direcci�n horizontal

        // Calcular el �ngulo de rotaci�n (solo en Y)
        float targetPlayerAngle = Mathf.Atan2(directionToTarget.x, directionToTarget.z) * Mathf.Rad2Deg;

        // Obtener la rotaci�n inicial
        float initialPlayerAngle = transform.eulerAngles.y;

        // Normalizar los �ngulos para evitar rotaciones largas
        float angleDifference = Mathf.DeltaAngle(initialPlayerAngle, targetPlayerAngle);
        targetPlayerAngle = initialPlayerAngle + angleDifference;

        timeElapsed = 0f;

        // Rotar el player primero
        while (timeElapsed < rotationDuration)
        {
            timeElapsed += Time.unscaledDeltaTime;
            float t = timeElapsed / rotationDuration;

            // Interpolar la rotaci�n del player
            float currentAngle = Mathf.LerpAngle(initialPlayerAngle, targetPlayerAngle, t);
            transform.rotation = Quaternion.Euler(0, currentAngle, 0);

            yield return null;
        }

        // Asegurar que el player est� exactamente en la posici�n final
        transform.rotation = Quaternion.Euler(0, targetPlayerAngle, 0);
    }

    // M�todo para asignar un destino aleatorio dentro de la malla de navegaci�n (mazmorras)
    public void SetRandomDestination()
    {
        // Generamos un punto aleatorio dentro de los l�mites de la malla de navegaci�n
        Vector3 randomPoint = new Vector3(
            Random.Range(-areaX, areaX),  // Limita el �rea seg�n el tama�o de la mazmorras, ajusta seg�n tus necesidades
            transform.position.y,     // Mantener la misma altura
            Random.Range(-areaZ, areaZ)   // Limita el �rea seg�n el tama�o de la mazmorras, ajusta seg�n tus necesidades
        );

        // Usamos NavMesh.SamplePosition para asegurarnos de que el punto est� dentro de la malla de navegaci�n
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 10f, NavMesh.AllAreas))
        {
            if (!IsNearOtherTarget(hit.position))
            {
                targetPosition = hit.position; // Asigna el punto v�lido dentro de la malla
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

    // M�todo para comprobar si hay una l�nea de visi�n clara al jugador utilizando raycasting
    
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
        // Esperar a que RebakeNavmesh.Instance esté disponible
        while (RebakeNavmesh.Instance == null)
        {
            yield return new WaitForSeconds(0.1f);
        }
        
        yield return new WaitUntil(() => RebakeNavmesh.Instance.rebaked);
        agent.enabled = true; // Habilitar el agente de navegacin una vez que la malla de navegacin est lista
        SetRandomDestination(); // Iniciar con un destino aleatorio dentro de la malla de navegacin
    }

    bool NVIsRebaked() {
        if (RebakeNavmesh.Instance != null && RebakeNavmesh.Instance.rebaked)
        {
            return true; // La malla de navegaci�n ha sido rebaked
        }
        else
        {
            return false; // La malla de navegaci�n no ha sido rebaked
        }
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
        Vector3 forward = transform.forward * visionDistance;
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