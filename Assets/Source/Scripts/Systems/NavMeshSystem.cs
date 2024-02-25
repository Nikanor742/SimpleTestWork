using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshSystem : IEcsInitSystem
{
    public void Init(IEcsSystems systems)
    {
        var navMeshObject = new GameObject();
        navMeshObject.name = "NavMeshSurface";
        var navMeshSurface = navMeshObject.AddComponent<NavMeshSurface>();
        navMeshSurface.agentTypeID = GetNavMeshAgentID("Spider");
        navMeshSurface.BuildNavMesh();
    }
    private int GetNavMeshAgentID(string name)
    {
        for (int i = 0; i < NavMesh.GetSettingsCount(); i++)
        {
            var settings = NavMesh.GetSettingsByIndex(index: i);
            if (name == NavMesh.GetSettingsNameFromID(agentTypeID: settings.agentTypeID))
            {
                return settings.agentTypeID;
            }
        }
        return 0;
    }
}
