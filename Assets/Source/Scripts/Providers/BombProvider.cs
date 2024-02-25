using System;
using UnityEngine;

public class BombProvider : MonoBehaviour
{
    public EBombType bombType;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public int poolIndex;

    public event Action<BombProvider, Transform> OnCollisionEnterEvent;

    private bool _detonate;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!_detonate && collision.gameObject.layer != gameObject.layer)
        {
            OnCollisionEnterEvent?.Invoke(this, collision.transform);
            _detonate = true;
        }
    }
}
