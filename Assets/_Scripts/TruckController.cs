using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

public class TruckController : MonoBehaviour
{
    [SerializeField] private Vector2 _centerOfMassOffset;
    [Header("Lead Wheels Settings")]
    [SerializeField] private float _enginePowerPerTrailer;
    [SerializeField] private float _sidewaysFrictionFront;
    [SerializeField] private float _forwardFfrictionFront;
    [SerializeField] private float _rollingFfrictionFront;
    [SerializeField] private float _turnSpeed;
    [SerializeField] private float _maxAngleMainSide;
    [SerializeField] private float _maxAngleOffSide;
    [Header("Off Wheels Settings")]
    [SerializeField] private float _sidewaysFrictionBack;
    [SerializeField] private float _forwardFfrictionBack;
    [SerializeField] private float _rollingFfrictionBack;
    [FormerlySerializedAs("_BrakingFfriction")] [SerializeField] private float _brakingFfriction;
    private TrailerHandler _trailerHandler;
    private List<Wheel> _wheels = new List<Wheel>();
    private List<Wheel> _leadWheels = new List<Wheel>();
    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
        var centerOfMass = _rigidbody.centerOfMass;
        centerOfMass += (Vector2)(transform.right * _centerOfMassOffset.x + transform.up * _centerOfMassOffset.y);
        _rigidbody.centerOfMass = centerOfMass;
        // _rigidbody.centerOfMass = _centerOfMassOffset;
        _trailerHandler = gameObject.GetComponent<TrailerHandler>();
        _trailerHandler.OnAttachedTrailer += OnAttachedTrailer;
        _trailerHandler.OnDetachedTrailer += OnDetachedTrailer;
        foreach (var wheel in GetComponentsInChildren<Wheel>())
        {
            wheel.isConnected = true;
            _wheels.Add(wheel);
            if (wheel.isLeadWheel)
            {
                _leadWheels.Add(wheel);
                wheel.SetConfig(_sidewaysFrictionFront, _forwardFfrictionFront, _rollingFfrictionFront, _turnSpeed, 0, _maxAngleMainSide, _maxAngleOffSide);
            }
            else
            {
                wheel.SetConfig(_sidewaysFrictionBack, _forwardFfrictionBack, _rollingFfrictionBack, 0, _brakingFfriction, 0, 0);
            }
        }
    }

    private void Start()
    {
        
    }

    void OnAttachedTrailer(Rigidbody2D trailer)
    {
        foreach (var wheel in _leadWheels)
        {
            wheel.ChangeEnginePower(_enginePowerPerTrailer);
        }
    }
    
    void OnDetachedTrailer()
    {
        
        foreach (var wheel in _leadWheels)
        {
            wheel.ChangeEnginePower(-_enginePowerPerTrailer);
        }
    }

    private void OnDisable()
    {
        _trailerHandler.OnAttachedTrailer -= OnAttachedTrailer;
        _trailerHandler.OnDetachedTrailer -= OnDetachedTrailer;
    }
}
