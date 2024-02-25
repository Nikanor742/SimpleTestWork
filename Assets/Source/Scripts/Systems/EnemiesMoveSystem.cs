using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

public class EnemiesMoveSystem : IEcsInitSystem,IEcsRunSystem
{
    private EcsWorld _ecsWorld;

    private EcsPool<EnemyMovePointComponent> _movePointsPool;
    private EcsPool<EnemyComponent> _enemiesPool;

    private EcsFilter _enemiesFilter;
    private EcsFilter _movePointsFilter;

    private int _moveHash = Animator.StringToHash("Move");

    private readonly EcsCustomInject<SOGameConfig> _gameConfig;
    private readonly EcsCustomInject<SOEnemiesConfig> _enemiesConfig;

    public void Init(IEcsSystems systems)
    {
        _ecsWorld = systems.GetWorld();
        _enemiesPool = _ecsWorld.GetPool<EnemyComponent>();
        _enemiesFilter = _ecsWorld.Filter<EnemyComponent>().End();

        _movePointsPool = _ecsWorld.GetPool<EnemyMovePointComponent>();
        _movePointsFilter = _ecsWorld.Filter<EnemyMovePointComponent>().End();
    }

    private void SetNewPoint(ref EnemyComponent enemy)
    {
        enemy.currentStayTime = 0;
        enemy.stayTime = Random.Range(_enemiesConfig.Value.startRandomStayTime,
            _enemiesConfig.Value.endRandomStayTime);
        enemy.state = EEnemyState.move;
        enemy.animator.SetBool(_moveHash, true);

        var newPos = Vector3.zero;

        foreach (var f in _movePointsFilter)
        {
            newPos = _movePointsPool.Get(f).point.position;
            if(newPos!= enemy.movePos && Random.Range(0,2) == 1) break;
        }

        enemy.movePos = newPos;
        enemy.navAgent.SetDestination(enemy.movePos);
    }

    public void Run(IEcsSystems systems)
    {
        foreach (var e in _enemiesFilter)
        {
            ref var enemy = ref _enemiesPool.Get(e);
            if (enemy.state == EEnemyState.stay)
            {
                enemy.animator.SetBool(_moveHash, false);
                enemy.currentStayTime += Time.deltaTime;
                if (enemy.currentStayTime >= enemy.stayTime)
                {
                    SetNewPoint(ref enemy);
                }
            }
            else if (enemy.state == EEnemyState.move)
            {
                if (Vector3.Distance(enemy.transform.position, enemy.movePos) <= 
                    _gameConfig.Value.enemiesDestinationDistance)
                {
                    enemy.state = EEnemyState.stay;
                }
            }
        }
    }
}
