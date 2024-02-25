using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System;
using System.Linq;
using UniRx;
using UnityEngine;

public class DetonateBombsSystem : IEcsInitSystem,IEcsRunSystem
{
    private EcsWorld _ecsWorld;

    private EcsPool<BombComponent> _bombsPool;
    private EcsPool<ExplosionComponent> _explosionPool;

    private EcsFilter _bombsFilter;

    private readonly EcsCustomInject<SOBombsConfig> _bombsConfig;

    public void Init(IEcsSystems systems)
    {
        _ecsWorld = systems.GetWorld();
        _bombsPool = _ecsWorld.GetPool<BombComponent>();
        _bombsFilter = _ecsWorld.Filter<BombComponent>().End();
        _explosionPool = _ecsWorld.GetPool<ExplosionComponent>();
    }

    private void OnBombCollisionEnter(BombProvider bomb,Transform other)
    {
        var bombData = _bombsConfig.Value.bombsData.Where(b => b.bombType == bomb.bombType).First();
        bomb.OnCollisionEnterEvent -= OnBombCollisionEnter;
        Observable.Timer(TimeSpan.FromSeconds(bombData.delay))
               .Subscribe(_ =>
               {
                   var explosionFX = GameObject.Instantiate(bombData.explosionFX, bomb.transform.position, Quaternion.identity);
                   explosionFX.transform.localScale = new Vector3(bombData.radius, bombData.radius, bombData.radius);
                   explosionFX.Play();

                   var explosionEntity = _ecsWorld.NewEntity();
                   _explosionPool.Add(explosionEntity);

                   ref var explosionComponent = ref _explosionPool.Get(explosionEntity);
                   explosionComponent.pos = bomb.transform.position;
                   explosionComponent.damage = bombData.damage;
                   explosionComponent.radius = bombData.radius;

                   _ecsWorld.DelEntity(bomb.poolIndex);
                   GameObject.Destroy(bomb.gameObject);
               });
    }

    private void CheckSubscribers()
    {
        foreach (var b in _bombsFilter)
        {
            ref var bomb = ref _bombsPool.Get(b);
            if (!bomb.hasSubscriber)
            {
                bomb.bombProvider.OnCollisionEnterEvent += OnBombCollisionEnter;
                bomb.hasSubscriber = true;
            }
        }
    }

    public void Run(IEcsSystems systems)
    {
        CheckSubscribers();
    }
}
