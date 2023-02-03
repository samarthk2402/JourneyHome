using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Magazine",menuName = "Scriptable objects/Magazine")]
public class Magazine : ScriptableObject
{
    public int maxAmmo;
    public float damage;
    public ParticleSystem shootParticleSystem;
    public LineRenderer lineRenderer;
    public float psTime;
    public float reloadSpeed;
    public bool damageOverTime;
    public float hit_num;
    public bool slowEffect;
}
