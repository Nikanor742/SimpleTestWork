using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class BombButtonProvider : MonoBehaviour, IPointerDownHandler
{
    public EBombType bombType;
    public event Action<EBombType> OnButtonDown;

    public void OnPointerDown(PointerEventData eventData)
    {
        OnButtonDown?.Invoke(bombType);
    }
}
