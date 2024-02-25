using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/EnemiesConfig")]
sealed public class SOEnemiesConfig : ScriptableObject
{
    [Range(1, 5)] public float startRandomStayTime;
    [Range(5, 10)] public float endRandomStayTime;
    public EnemyData[] enemiesData;
}

[Serializable]
sealed public class EnemyData
{
    public EEnemyType enemyType;
    public float health;
    public float speed;
    public EnemyProvider prefab;
}
