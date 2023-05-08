using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryPoint : MonoBehaviour
{
    public event Action OnDelivery;
    private bool _isActive;
    private Collider2D _trigger;
    private MeshRenderer _renderer;

    private void Awake()
    {
        _trigger = GetComponent<Collider2D>();
        _renderer = GetComponentInChildren<MeshRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_isActive) return;
        var trailer = other.GetComponent<TrailerController>();
        if(!trailer) return;

        trailer.OnDeath += OnDeliveryComplete;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!_isActive) return;
        var trailer = other.GetComponent<TrailerController>();
        if(!trailer) return;

        trailer.OnDeath -= OnDeliveryComplete;
    }

    public void Activate()
    {
        _isActive = true;
        _renderer.enabled = true;
    }

    public void Deactivate()
    {
        _isActive = false;
        _renderer.enabled = false;
    }

    void OnDeliveryComplete(TrailerController trailer, float deliveryTime)
    {
        GameLogic.Instance.OnDelivery(this, deliveryTime);
        trailer.OnDeath -= OnDeliveryComplete;
        OnDelivery?.Invoke();
        Deactivate();
    }

    private void OnDrawGizmos()
    {
        if (!_isActive) return;
        Gizmos.color = Color.green;
        Gizmos.DrawCube(_trigger.bounds.center, _trigger.bounds.size + new Vector3(0,0,1));
    }
}
