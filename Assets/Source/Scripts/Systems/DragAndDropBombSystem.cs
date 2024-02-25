using Cinemachine;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Linq;
using UnityEngine;

public class DragAndDropBombSystem : IEcsInitSystem,IEcsRunSystem,IEcsDestroySystem
{
    private EcsWorld _ecsWorld;
    private EcsPool<GameStateComponent> _statePool;
    private EcsFilter _stateFilter;

    private BombButtonProvider[] _bombButtons;

    private readonly EcsCustomInject<SOBombsConfig> _bombsConfig;
    private readonly EcsCustomInject<SOGameConfig> _gameConfig;

    private BombProvider _currentBomb;
    private Camera _mainCamera;
    private DrawLineProvider _lineDrawer;

    private CinemachineVirtualCamera _vcam;
    private CinemachineTransposer _vcamTransposer;



    public void Init(IEcsSystems systems)
    {
        _ecsWorld = systems.GetWorld();
        _statePool = _ecsWorld.GetPool<GameStateComponent>();
        _stateFilter = _ecsWorld.Filter<GameStateComponent>().End();

        _bombButtons = GameObject.FindObjectsOfType<BombButtonProvider>();

        foreach (var b in _bombButtons)
        {
            b.OnButtonDown += OnButtonDown;
        }
        _vcam = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
        _vcamTransposer = _vcam.GetCinemachineComponent<CinemachineTransposer>();
        _mainCamera = Camera.main;
        _lineDrawer = GameObject.Instantiate(_gameConfig.Value.lineDrawer);
    }

    private void OnButtonDown(EBombType bombType)
    {
        foreach (var item in _stateFilter) 
        {
            ref var stateComponent = ref _statePool.Get(item);
            stateComponent.navigation = false;
        }

        foreach (var b in _bombButtons)
        {
            b.gameObject.SetActive(false);
        }
        var bombData = _bombsConfig.Value.bombsData.Where(d => d.bombType == bombType).First();

        var newBomb = GameObject.Instantiate(bombData.prefab);
        _currentBomb = newBomb;

        var bombEntity = _ecsWorld.NewEntity();
        var bombPool = _ecsWorld.GetPool<BombComponent>();
        bombPool.Add(bombEntity);
        ref var bombComponent = ref bombPool.Get(bombEntity);
        bombComponent.bombProvider = newBomb;
        bombComponent.bombProvider.poolIndex = bombEntity;
    }
    private void DrawLine()
    {
        _lineDrawer.line.enabled = true;
        _lineDrawer.line.SetPosition(0, _currentBomb.transform.position);

        var startPos = _currentBomb.transform.position;
        var endPos = startPos;

        endPos.y -= 100;
        RaycastHit hit;
        if (Physics.Raycast(startPos, endPos,out hit))
        {
            var newPos = hit.point;
            _lineDrawer.line.SetPosition(1, newPos);
            _lineDrawer.point.gameObject.SetActive(true);
            newPos.y += 0.5f;
            _lineDrawer.point.transform.position = newPos;
        }
        else
        {
            _lineDrawer.line.enabled = false;
            _lineDrawer.point.gameObject.SetActive(false);
        }
    }

    private void LaunchBomb()
    {
        _currentBomb.rb.isKinematic = false;
        _currentBomb.rb.useGravity = true;
        _currentBomb = null;

        _lineDrawer.line.enabled = false;
        _lineDrawer.point.gameObject.SetActive(false);

        foreach (var b in _bombButtons)
        {
            b.gameObject.SetActive(true);
        }
        foreach (var item in _stateFilter)
        {
            ref var stateComponent = ref _statePool.Get(item);
            stateComponent.navigation = true;
        }
    }

    public void Run(IEcsSystems systems)
    {
        if (_currentBomb != null)
        {
            DrawLine();
            var mousePosition = Input.mousePosition;
            var distanceFromCameraToGround = _vcamTransposer.m_FollowOffset.y;
            var mouseWorldPosition = _mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, distanceFromCameraToGround));
            var bombPosition = new Vector3(mouseWorldPosition.x, 10, mouseWorldPosition.z);
            _currentBomb.transform.position = Vector3.Lerp(_currentBomb.transform.position, bombPosition, Time.deltaTime * 15);

            if (Input.GetMouseButtonUp(0))
            {
                LaunchBomb();
            }
        }
    }

    public void Destroy(IEcsSystems systems)
    {
        foreach (var b in _bombButtons)
        {
            b.OnButtonDown -= OnButtonDown;
        }
    }
}
