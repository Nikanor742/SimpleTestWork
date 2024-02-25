using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

public class ECS_Startup : MonoBehaviour
{
    [SerializeField] private SOEnemiesConfig _enemiesConfig;
    [SerializeField] private SOGameConfig _gameConfig;
    [SerializeField] private SOBombsConfig _bombsConfig;

    private EcsWorld _world;

    private IEcsSystems _initSystems;
    private IEcsSystems _updateSystems;

    private void Start()
    {
        SaveExtension.Init();

        _world = new EcsWorld();

        _initSystems = new EcsSystems(_world);
        _initSystems
            .Add(new LevelLoadingSystem())
            .Add(new NavMeshSystem())
            .Init();

        _updateSystems = new EcsSystems(_world);
        _updateSystems
            .Add(new EnemiesSpawnSystem())
            .Add(new EnemiesMoveSystem())
            .Add(new NavigationSystem())
            .Add(new DragAndDropBombSystem())
            .Add(new DetonateBombsSystem())
            .Add(new EnemiesHealthSystem())
            .Inject(_enemiesConfig)
            .Inject(_gameConfig)
            .Inject(_bombsConfig)
            .Init();
    }

    private void Update()
    {
        _updateSystems?.Run();
    }

    private void OnDestroy()
    {
        _initSystems?.Destroy();
        _updateSystems?.Destroy();
        _world?.Destroy();
    }
}
