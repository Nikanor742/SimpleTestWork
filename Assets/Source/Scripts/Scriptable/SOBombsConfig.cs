using NaughtyAttributes;
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/BombsConfig")]
sealed public class SOBombsConfig : ScriptableObject
{
    public BombData[] bombsData;
}

[Serializable]
sealed public class BombData
{
    public EBombType bombType;
    public float radius;
    public float delay;
    public float damage;
    public BombProvider prefab;
    public ParticleSystem explosionFX;
}
