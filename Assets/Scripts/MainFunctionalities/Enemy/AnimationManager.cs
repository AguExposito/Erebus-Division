using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class AnimationManager : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    public EnemyAI enemyAI;
    public Animator anim;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
        enemyAI = GetComponent<EnemyAI>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("Speed", navMeshAgent.velocity.magnitude);
    }

    public AnimationClip GetAnimationClip(string clipName)
    {
        foreach (var clip in anim.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
                return clip;
        }
        return null;
    }

    Vector3 pos;
    public void RootMotionState(int state)
    {
        if (state == 0)
        {
            RotatePositionateGently();
            anim.applyRootMotion = false;
            return;
        }
        else
        {
            pos= transform.localPosition;
            anim.applyRootMotion = true;
            return;
        }
    }
    Coroutine rotateEnemyToPlayer;
    Coroutine positionateGently;
    public void RotatePositionateGently() {
        if (rotateEnemyToPlayer != null || positionateGently != null)
        {
            StopCoroutine(rotateEnemyToPlayer);
            StopCoroutine(positionateGently);
        }

        rotateEnemyToPlayer= StartCoroutine(enemyAI.RotateEnemyToPlayer(0.25f));
        positionateGently=StartCoroutine(PositionateGently(0.25f));
    }

    private IEnumerator PositionateGently(float duration)
    {
        Vector3 ini = transform.position;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            transform.localPosition = Vector3.Lerp(ini, pos, t);
            yield return null;
        }

        transform.localPosition = pos; // Asegurarse de terminar exactamente en el destino
    }
}
