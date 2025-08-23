using UnityEngine;
using UnityEngine.AI;

public class AnimationManager : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    public EnemyAI enemyAI;
    Animator anim;
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
        anim.SetBool("Chasing", enemyAI.currentState == EnemyAI.State.Chase);
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

}
