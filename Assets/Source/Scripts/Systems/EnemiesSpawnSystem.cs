using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemiesSpawnSystem : IEcsInitSystem,IEcsRunSystem
{
    private EcsWorld _ecsWorld;
    private EcsFilter _filter;
    private EcsPool<EnemySpawnPointComponent> _spawnPointsPool;

    private readonly EcsCustomInject<SOEnemiesConfig> _enemiesConfig = default;

    private Transform _enemyRoot;

    public void Init(IEcsSystems systems)
    {
        _ecsWorld = systems.GetWorld();

        _spawnPointsPool = _ecsWorld.GetPool<EnemySpawnPointComponent>();
        _filter = _ecsWorld.Filter<EnemySpawnPointComponent>().End();
        foreach (var p in _filter)
        {
            ref var point = ref _spawnPointsPool.Get(p);
            point.currentTime = point.spawnCooldown;
        }

        _enemyRoot = new GameObject().transform;
        _enemyRoot.gameObject.name = "Enemies";
    }

    private void Spawn(ref EnemySpawnPointComponent spawnPoint)
    {
        var enemyType = spawnPoint.enemyType;
        var enemyData = _enemiesConfig.Value.enemiesData.Where(d => d.enemyType == enemyType).First();

        var newEnemy = GameObject.Instantiate(enemyData.prefab, spawnPoint.spawnPoint.position,Quaternion.identity);
        newEnemy.transform.parent = _enemyRoot;

        var enemyEntity = _ecsWorld.NewEntity();
        newEnemy.poolID = enemyEntity;

        var enemiesPool = _ecsWorld.GetPool<EnemyComponent>();
        enemiesPool.Add(enemyEntity);

        ref var enemyComponent = ref enemiesPool.Get(enemyEntity);
        enemyComponent.transform = newEnemy.transform;
        enemyComponent.animator = newEnemy.GetComponentInChildren<Animator>();
        enemyComponent.col = newEnemy.GetComponent<Collider>();
        enemyComponent.navAgent = newEnemy.GetComponent<NavMeshAgent>();

        enemyComponent.state = EEnemyState.stay;

        enemyComponent.navAgent.speed = enemyData.speed;
        enemyComponent.maxHealth = enemyData.health;
        enemyComponent.currentHealth = enemyData.health;
    }

    public void Run(IEcsSystems systems)
    {
        foreach (var p in _filter)
        {
            ref var point = ref _spawnPointsPool.Get(p);
            point.currentTime += Time.deltaTime;
            if (point.currentTime >= point.spawnCooldown)
            {
                point.currentTime = 0;
                Spawn(ref point);
            }
        }
    }
}
