using Leopotam.EcsLite;
using UnityEngine;

public class LevelLoadingSystem : IEcsInitSystem
{
    public void Init(IEcsSystems systems)
    {
        var level = SaveExtension.player.level;
        var levelPrefab = Resources.Load<GameObject>($"Levels/Level_{level}");

        if (levelPrefab == null)
        {
            SaveExtension.player.level = 0;
            SaveExtension.SavePlayerData();
            level = 0;
            levelPrefab = Resources.Load<GameObject>($"Levels/Level_{level}");
        }

        var newLevel = GameObject.Instantiate(levelPrefab);
        var ecsWorld = systems.GetWorld();

        InitEnemySpawnPoints(newLevel, ecsWorld);
        InitEnemyMovePoints(newLevel, ecsWorld);
    }

    private void InitEnemySpawnPoints(GameObject level, EcsWorld ecsWorld)
    {
        var enemySpawnPointsPool = ecsWorld.GetPool<EnemySpawnPointComponent>();
        var enemySpawnPoints = level.GetComponentsInChildren<EnemySpawnPointProvider>();
        foreach (var point in enemySpawnPoints)
        {
            var enemyPointEntity = ecsWorld.NewEntity();

            enemySpawnPointsPool.Add(enemyPointEntity);

            ref var spawnPointComponent = ref enemySpawnPointsPool.Get(enemyPointEntity);

            spawnPointComponent.enemyType = point.enemyType;
            spawnPointComponent.spawnPoint = point.transform;
            spawnPointComponent.spawnCooldown = point.spawnCooldown;

            GameObject.Destroy(point);
        }
    }

    private void InitEnemyMovePoints(GameObject level,EcsWorld ecsWorld)
    {
        var enemyMovePointsPool = ecsWorld.GetPool<EnemyMovePointComponent>();
        var enemyMovePoints = level.GetComponentsInChildren<EnemyMovePointProvider>();
        foreach (var point in enemyMovePoints)
        {
            var enemyMovePointEntity = ecsWorld.NewEntity();

            enemyMovePointsPool.Add(enemyMovePointEntity);

            ref var spawnPointComponent = ref enemyMovePointsPool.Get(enemyMovePointEntity);

            spawnPointComponent.point = point.transform;

            GameObject.Destroy(point);
        }
    }
}
