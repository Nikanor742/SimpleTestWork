using UnityEngine;
using UnityEngine.AI;

public struct EnemyComponent
{
    public EEnemyState state;

    public Transform transform;
    public Animator animator;
    public Collider col;
    public NavMeshAgent navAgent;

    public Vector3 movePos;

    public float maxHealth;
    public float currentHealth;

    public float currentStayTime;
    public float stayTime;
}
