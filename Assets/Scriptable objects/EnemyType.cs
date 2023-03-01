using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyType",menuName = "Scriptable objects/EnemyType")]
public class EnemyType : ScriptableObject
{
    public float walkPointRange;
    public float speed;
    public float timeBetweenAttacks;
    public float damage = 10;
    public float sightRange, attackRange;
    public ParticleSystem ps;
    public float psTime;
    public LineRenderer lineRenderer;
    public bool damageOverTime;
    public int hit_num;
}
