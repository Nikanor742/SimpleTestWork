using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/GameConfig")]
sealed public class SOGameConfig : ScriptableObject
{
    public DrawLineProvider lineDrawer;
    public float enemiesDestinationDistance;
    public Vector2 levelBounds;
}
