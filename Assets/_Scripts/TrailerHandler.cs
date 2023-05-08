using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrailerHandler : MonoBehaviour
{
    public event Action<Rigidbody2D> OnAttachedTrailer;
    public event Action OnDetachedTrailer;

    [SerializeField] private Collider2D _pickupTrigger;
    [SerializeField] private Collider2D _backCollider;
    [SerializeField] private ContactFilter2D _layerMask;
    [SerializeField] private float _maxLaunchForce = 50f;

    private TrailerController _attachedTrailer;
    
    public Stack<Rigidbody2D> _trailers = new Stack<Rigidbody2D>();
    private int _maxTrailers = 10; //= Settings.MaxTrailers;

    private void Start()
    {
        var connectedObject = gameObject.GetComponent<Joint2D>().connectedBody;
        while (connectedObject != null)
        {
            AttachTrailer(connectedObject);
            connectedObject = connectedObject.gameObject.GetComponent<Joint2D>().connectedBody;
        }

        foreach (var point in GameLogic.Instance._deliveryPoints)
        {
            point.OnDelivery += OnDelivery;
        }
    }

    private void OnDelivery()
    {
        foreach (var trailer in _trailers)
        {
            trailer.gameObject.GetComponent<TrailerController>().ResetTime();
        }
    }

    private void Update()
    {
        if (InputHandler.Instance.Detach)
        {   
            DetachTrailer();
        }
        
        if (InputHandler.Instance.Attach)
        {
            var trailer = LookForTrailers();
            if(trailer == null) return;
            AttachTrailer(trailer);
        }
    }

    Rigidbody2D LookForTrailers()
    {
        List<Collider2D> colliders = new List<Collider2D>();
        // Physics.OverlapSphere(transform.position + (Vector3) _pickupTrigger, _pickupTrigger.w, _layerMask, QueryTriggerInteraction.Collide).ToList();
        _pickupTrigger.OverlapCollider(_layerMask, colliders);
        return colliders.Select(x => x.gameObject.GetComponent<Rigidbody2D>()).FirstOrDefault(x => !_trailers.Contains(x) && x.gameObject != gameObject);
    }

    public void AttachTrailer(Rigidbody2D trailer)
    {
        if (_trailers.Count == _maxTrailers) return;
        
        _backCollider.enabled = false;
        var controller = trailer.gameObject.GetComponent<TrailerController>();
        controller._handler = this;
        var jointHolder = _trailers.Count == 0 ? gameObject : _trailers.Peek().gameObject;
        var joint = jointHolder.GetComponent<Joint2D>();
        joint.connectedBody = trailer;
        joint.enabled = true;
        _trailers.Push(trailer);
        foreach (var wheel in trailer.gameObject.GetComponentsInChildren<Wheel>())
        {
            wheel.isConnected = true;
        }

        OnAttachedTrailer?.Invoke(trailer);
        
        // if (_trailers.Count == _maxTrailers) return;
        //
        // _backCollider.enabled = false;
        // if (_attachedTrailer == null)
        // {
        //     _joint.connectedBody = trailer;
        // }
        // else
        // {
        //     _attachedTrailer.AttachTrailer(trailer);
        // }
        //
        // _trailers.Push(trailer);
        // foreach (var wheel in trailer.gameObject.GetComponentsInChildren<Wheel>())
        // {
        //     wheel.isConnected = true;
        // }
        //
        // OnAttachedTrailer?.Invoke(trailer);
    }

    public void DetachTrailer()
    {
        if (_trailers.Count == 0) return;

        var trailer = _trailers.Pop();
        var controller = trailer.gameObject.GetComponent<TrailerController>();
        controller._handler = null;
        var jointHolder = _trailers.Count == 0 ? gameObject : _trailers.Peek().gameObject;
        var joint = jointHolder.GetComponent<Joint2D>();
        joint.connectedBody = null;
        joint.enabled = false;
        trailer.AddForce( (_maxLaunchForce * trailer.velocity.magnitude / 50f) * trailer.velocity.normalized, ForceMode2D.Impulse);
        foreach (var wheel in trailer.gameObject.GetComponentsInChildren<Wheel>())
        {
            wheel.isConnected = false;
        }

        if (_trailers.Count == 0) _backCollider.enabled = true;

        OnDetachedTrailer?.Invoke();
    }

    public void DetachTrailer(Rigidbody2D trailer)
    {
        if (_trailers.Count == 0) return;
        if (!_trailers.Contains(trailer)) return;

        var index = _trailers.ToList().IndexOf(trailer);

        for (int i = 0; i <= index; i++)
        {
            DetachTrailer();
        }
    }

    private void OnDisable()
    {
        foreach (var point in GameLogic.Instance._deliveryPoints)
        {
            point.OnDelivery -= OnDelivery;
        }
    }
}
