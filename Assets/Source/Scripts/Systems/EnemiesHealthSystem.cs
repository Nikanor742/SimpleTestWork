using Leopotam.EcsLite;
using System;
using UniRx;
using UnityEngine;

public class EnemiesHealthSystem : IEcsInitSystem,IEcsRunSystem
{
    private EcsWorld _ecsWorld;

    private EcsPool<ExplosionComponent> _explosionsPool;
    private EcsPool<EnemyComponent> _enemiesPool;

    private EcsFilter _explosionsFilter;

    public void Init(IEcsSystems systems)
    {
        _ecsWorld = systems.GetWorld();
        _explosionsPool = _ecsWorld.GetPool<ExplosionComponent>();
        _explosionsFilter = _ecsWorld.Filter<ExplosionComponent>().End();

        _enemiesPool = _ecsWorld.GetPool<EnemyComponent>();
    }

    private void MakeDamage(EnemyProvider enemy, float damage)
    {
        if(enemy != null)
        {
            ref var enemyComponent = ref _enemiesPool.Get(enemy.poolID);
            if (enemyComponent.state != EEnemyState.death)
            {
                enemyComponent.currentHealth -= damage;
                if (enemyComponent.currentHealth <= 0)
                {
                    enemyComponent.state = EEnemyState.death;
                    enemyComponent.col.enabled = false;
                    enemyComponent.animator.SetTrigger("Death");
                    enemyComponent.navAgent.speed = 0;
                    GameObject.Destroy(enemy.gameObject, 2f);
                    _ecsWorld.DelEntity(enemy.poolID);
                }
                else
                {
                    enemyComponent.animator.SetTrigger("TakeDamage");
                    var navAgent = enemyComponent.navAgent;
                    var startSpeed = navAgent.speed;
                    navAgent.speed = 0;
                    Observable.Timer(TimeSpan.FromSeconds(2f))
                       .Subscribe(_ =>
                       {
                           navAgent.speed = startSpeed;
                       });
                }
            }
        }
    }

    public void Run(IEcsSystems systems)
    {
        foreach (var f in _explosionsFilter)
        {
            ref var explosion = ref _explosionsPool.Get(f);

            Collider[] hitColliders = Physics.OverlapSphere(explosion.pos, explosion.radius);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.TryGetComponent(out EnemyProvider enemy))
                {
                    RaycastHit hit;
                    if (Physics.Raycast(explosion.pos, enemy.transform.position - explosion.pos, out hit, Mathf.Infinity))
                    {
                        MakeDamage(enemy, explosion.damage);
                    }
                }
            }
            _ecsWorld.DelEntity(f);
        }
    }
}
