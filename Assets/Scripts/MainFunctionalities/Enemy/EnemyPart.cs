using UnityEngine;

public class EnemyPart : MonoBehaviour
{
    public enum bodyPartType
    {
        Head,
        Body,
        Arm,
        Leg
    }
    public enum bodyPartStatus
    {
        Strong,
        Normal,
        Weak,
    }
    public bodyPartType partType;
    public bodyPartStatus partStatus;
    public Vector2 hitChanceMultMinMax;
    public float hitChanceMultiplier = 1.0f;
    public Vector2 critChanceMultMinMax;
    public float critChanceMultiplier = 1.0f;
    public float damageMultiplier = 1.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hitChanceMultiplier = Random.Range(hitChanceMultMinMax.x, hitChanceMultMinMax.y);
        critChanceMultiplier = Random.Range(critChanceMultMinMax.x, critChanceMultMinMax.y);
        switch (partStatus)
        {
            case bodyPartStatus.Strong:
                damageMultiplier = 0.5f;
                break;
            case bodyPartStatus.Normal:
                damageMultiplier = 1.0f;
                break;
            case bodyPartStatus.Weak:
                damageMultiplier = 2.0f;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
