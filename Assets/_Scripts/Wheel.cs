using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

public class Wheel : MonoBehaviour
{
    
    [Header("Acceleration Parameters")]
    [SerializeField] private bool _isLeadWheel;
    [SerializeField] private bool _canBrake;
    [SerializeField] private ScriptableCurve _engineForceCurve;
    [SerializeField] private float _enginePower;
    [SerializeField] private float _maxForwardSpeed;
    [SerializeField] private float _maxBackwardSpeed;

    [Header("Steering Parameters")]
    [SerializeField] private bool _canTurn;
    [SerializeField] private bool _isLeft;
    [SerializeField] private float _turnSpeed;
    [SerializeField] private float _maxTurnAngleMainSide = 50f;
    [SerializeField] private float _maxTurnAngleOffSide = 40f;
    [SerializeField] private float _rollingFriction;
    [SerializeField] private float _brakingFriction;
    [SerializeField] private ScriptableCurve _slidingFrictionCurve;
    [SerializeField] private float _sidewaysFrictionModifier;
    [SerializeField] private float _forwardFfrictionModifier;

    [Header("Debug")] 
    [SerializeField] private bool _debug;
    [SerializeField] private TMP_Text _debugFrontSlip;
    [SerializeField] private TMP_Text _debugSideSlip;
    [SerializeField] private TMP_Text _debugFrontFriction;
    [SerializeField] private TMP_Text _debugSideFriction;

    public bool isConnected;
    public bool isLeadWheel => _isLeadWheel;
    private float _turnAngle;
    private Vector3 _maxTurnedMain;
    private Vector3 _maxTurnedOff;
    private float _wheelCount;
    private Rigidbody2D _carRb;
    private Vector3 _targetDirection;
    private TrailRenderer _trailRenderer;

    // private Vector3 slidingFriction;
    // private Vector3 rollingFriction;

    void Awake()
    {
        _carRb = GetComponentInParent<Rigidbody2D>();
        _wheelCount = transform.parent.GetComponentsInChildren<Wheel>().Length;
        Debug.Log("Wheel Count: " + _wheelCount);
        _targetDirection = _carRb.transform.up;
        _trailRenderer = GetComponent<TrailRenderer>();
        
        
        var side = _isLeft ? 1 : -1;
        _maxTurnedMain = Quaternion.Euler(0,0, side * _maxTurnAngleMainSide) * _carRb.transform.up;
        _maxTurnedOff = Quaternion.Euler(0,0, -side * _maxTurnAngleOffSide) * _carRb.transform.up;
    }

    void Update()
    {
        ApplyRotation();
    }

    void FixedUpdate()
    {
        ApplyAcceleration();
        ApplyFriction();
    }

    void ApplyRotation()
    {
        if (!_canTurn) return;
        
        // var rotation = transform.rotation;
        // var horizontal = InputHandler.Instance.Steering;
        // var maxAngle =
        //     horizontal < 0 && _isLeft || horizontal > 0 && !_isLeft
        //         ? _maxTurnAngleMainSide
        //         : _maxTurnAngleOffSide;
        //
        // _targetDirection = Mathf.Abs(horizontal) < 0.01f ? _carRb.transform.up :
        //     Quaternion.Euler(0,0, -Mathf.Sign(horizontal) * maxAngle) * _carRb.transform.up;
        //
        // // Debug.Log(rotation);
        // rotation = Quaternion.Slerp(rotation, Quaternion.LookRotation(Vector3.back, _targetDirection), _turnSpeed * Time.deltaTime);
        // transform.rotation = rotation;

        var rotation = transform.localRotation;
        var horizontal = InputHandler.Instance.Steering;
        var maxAngle =
            horizontal < 0 && _isLeft || horizontal > 0 && !_isLeft
                ? _maxTurnAngleMainSide
                : -_maxTurnAngleOffSide;

        var side = _isLeft ? 1 : -1;
        var targetAngle = Mathf.Abs(horizontal) < 0.01f ? 0f : maxAngle;
        _turnAngle = Mathf.MoveTowards(_turnAngle, targetAngle, _turnSpeed * Time.deltaTime);
        var progress = (_turnAngle + _maxTurnAngleOffSide) / (_maxTurnAngleMainSide + _maxTurnAngleOffSide);
        _targetDirection = Quaternion.Euler(0,0, -Mathf.Sign(horizontal) * targetAngle) * _carRb.transform.up;
        var target = Vector3.Slerp(_maxTurnedOff, _maxTurnedMain, progress);
        rotation = Quaternion.LookRotation(Vector3.forward, target);
        transform.localRotation = rotation;
    }

    private Vector3 _acceleration;
    void ApplyAcceleration()
    {
        if (!_isLeadWheel) return;

        var velocity = transform.up * Vector3.Dot(_carRb.GetPointVelocity(transform.position), transform.up);
        var samplePoint = velocity.magnitude / _maxForwardSpeed;
        var enginePitch = _engineForceCurve.curve.Evaluate(samplePoint);
        AudioManager.instance.SetEnginePitch(enginePitch);
        var engineForce = samplePoint < 1 ? enginePitch * _enginePower : 0f;
        _acceleration = InputHandler.Instance.Acceleration * engineForce * transform.up;
        _carRb.AddForceAtPosition(_acceleration, transform.position);
        // _carRb.AddForce(_acceleration);
    }

    private Vector3 alignedVelocity;
    private Vector3 lateralVelocity;
    private Vector3 forwardFriction;
    private Vector3 sidewaysFriction;

    void ApplyFriction()
    {
        var carVelocity = _carRb.velocity;
        var carLongVelocity = _carRb.transform.up * Vector2.Dot(carVelocity, _carRb.transform.up);
        var carLatVelocity = _carRb.transform.right * Vector2.Dot(carVelocity, _carRb.transform.right);
        var mass = _carRb.mass / _wheelCount;
        alignedVelocity = transform.up * Vector2.Dot(transform.up, _carRb.GetPointVelocity(transform.position));
        lateralVelocity = transform.right * Vector2.Dot(transform.right, _carRb.GetPointVelocity(transform.position));



        // SIDEWAYS FRICTION

        var samplePointDrift = 
            // carVelocity.magnitude > 0 ?
            alignedVelocity.magnitude > 0 ?
            // (lateralVelocity.magnitude - alignedVelocity.magnitude *
            // Mathf.Cos(-Mathf.Atan(lateralVelocity.magnitude / alignedVelocity.magnitude))) / alignedVelocity.magnitude : 0f;

            // lateralVelocity.magnitude / carVelocity.magnitude : 0f;
            // lateralVelocity.magnitude / _carRb.GetPointVelocity(transform.position).magnitude : 0f;


            Mathf.Atan(lateralVelocity.magnitude / alignedVelocity.magnitude) * Mathf.Rad2Deg / 90f : 0f;
        var lateralFriction = InputHandler.Instance.Braking && _canBrake ? _brakingFriction : _slidingFrictionCurve.curve.Evaluate(samplePointDrift);
        sidewaysFriction = -lateralFriction * _sidewaysFrictionModifier * mass * lateralVelocity / Time.fixedDeltaTime;



        // FORWARD FRICTION

        var inputModifier = isConnected ? InputHandler.Instance.Acceleration : 0;
        // var samplePointSlide = carLongVelocity.magnitude > 0 ? 
        // (alignedVelocity.magnitude * inputModifier - carLongVelocity.magnitude) / carLongVelocity.magnitude : 0f;
        // samplePointSlide = (samplePointDrift + 1) / 2;
        // _rollingFriction = _rollingFrictionCurve.curve.Evaluate(samplePointSlide);


        // rollingFriction = inputModifier != 0 ? 
        // Vector3.zero : -_rollingFriction * _frictionModifier * mass * alignedVelocity / Time.fixedDeltaTime;
        forwardFriction = -_forwardFfrictionModifier * mass * alignedVelocity / Time.deltaTime;
        var grip = _canBrake && InputHandler.Instance.Braking ? _brakingFriction :
            inputModifier == 0 ? _rollingFriction : 0f;
        forwardFriction *= grip;



        _carRb.AddForceAtPosition(forwardFriction, transform.position);
        _carRb.AddForceAtPosition(sidewaysFriction, transform.position);

        if (_trailRenderer != null)
        {
            _trailRenderer.material.SetFloat("_Friction", (forwardFriction + sidewaysFriction).magnitude);
        }


        // DEBUG
        if (!_debug) return;
        _debugFrontSlip.text = grip.ToString("0.0");
        _debugSideSlip.text = samplePointDrift.ToString("0.0");
        _debugFrontFriction.text = forwardFriction.magnitude.ToString("0.0");
        // _debugSideFriction.text = slidingFriction.magnitude.ToString("0.0");
        _debugSideFriction.text = sidewaysFriction.magnitude.ToString("0.0");
    }

    public void ChangeEnginePower(float change)
    {
        _enginePower += change;
    }

    public void SetConfig(float sideFrictionCoeff, float forwardFrictionCoeff, float rollingFriction, float turnSpeed,
        float brakeCoeff, float maxAngleMain, float maxAngleOff)
    {
        _sidewaysFrictionModifier = sideFrictionCoeff;
        _forwardFfrictionModifier = forwardFrictionCoeff;
        _rollingFriction = rollingFriction;
        _turnSpeed = turnSpeed;
        _brakingFriction = brakeCoeff;
        _maxTurnAngleMainSide = maxAngleMain;
        _maxTurnAngleOffSide = maxAngleOff;
    }

    private void OnDrawGizmos()
    {
        if (_canTurn)
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawLine(transform.position, transform.position + _targetDirection * 2);
        }
        
        // Gizmos.color = Color.yellow;
        // Gizmos.DrawLine(transform.position, transform.position + (Vector3)_carRb.GetPointVelocity(transform.position));
        Gizmos.color = Color.green;
        // Gizmos.DrawLine(transform.position, transform.position + alignedVelocity);
        Gizmos.DrawLine(transform.position, transform.position + _acceleration);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + lateralVelocity);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + sidewaysFriction);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + forwardFriction);
    }
}
