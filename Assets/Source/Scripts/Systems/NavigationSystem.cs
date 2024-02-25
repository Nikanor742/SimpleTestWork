using Cinemachine;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

public class NavigationSystem : IEcsInitSystem,IEcsRunSystem
{
    private Transform _cameraTarget;
    private Vector3 _startPos;
    private Camera _mainCam;

    private EcsWorld _ecsWorld;
    private EcsPool<GameStateComponent> _statePool;
    private EcsFilter _stateFilter;

    private readonly EcsCustomInject<SOGameConfig> _gameConfig;

    public void Init(IEcsSystems systems)
    {
        _ecsWorld = systems.GetWorld();

        var gameStateEntity = _ecsWorld.NewEntity();
        _statePool = _ecsWorld.GetPool<GameStateComponent>();
        _statePool.Add(gameStateEntity);
        _stateFilter = _ecsWorld.Filter<GameStateComponent>().End();

        ref var stateComponent = ref _statePool.Get(gameStateEntity);
        stateComponent.navigation = true;

        _mainCam = Camera.main;
        _cameraTarget = new GameObject().transform;
        _cameraTarget.name = "CameraTarget";

        CinemachineVirtualCamera vCam = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
        vCam.m_Follow = _cameraTarget;
        vCam.m_LookAt = _cameraTarget;
    }

    public void Run(IEcsSystems systems)
    {
        var navigation = true;
        foreach (var item in _stateFilter) navigation = _statePool.Get(item).navigation;
        
        if (navigation)
        {
            if (Input.GetMouseButtonDown(0))
            {
                _startPos = _mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
                    Input.mousePosition.y,
                    _mainCam.transform.position.y));
            }
            if (Input.GetMouseButton(0))
            {
                var direction = _startPos - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
                    Input.mousePosition.y,
                    _mainCam.transform.position.y));

                _cameraTarget.transform.position = new Vector3(_cameraTarget.transform.position.x + direction.x,
                    _cameraTarget.transform.position.y,
                    _cameraTarget.transform.position.z + direction.z);

                _cameraTarget.transform.position = new Vector3(
                    Mathf.Clamp(_cameraTarget.transform.position.x, -_gameConfig.Value.levelBounds.x,
                    _gameConfig.Value.levelBounds.x),
                    _cameraTarget.transform.position.y,
                    Mathf.Clamp(_cameraTarget.transform.position.z, -_gameConfig.Value.levelBounds.y,
                    _gameConfig.Value.levelBounds.y));
            }
        }
    }
}
