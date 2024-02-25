using UnityEngine;

public struct EnemySpawnPointComponent
{
    public Transform spawnPoint;
    public EEnemyType enemyType;
    public float spawnCooldown;
    public float currentTime;
}
