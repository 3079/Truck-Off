using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private ScriptableCurve _fovCurve;
    [SerializeField] private float _fovMin;
    [SerializeField] private float _fovMax;
    private Rigidbody2D _rigidbody;
    private TrailerHandler _trailerHandler;
    private Camera _mainCamera;

    private void Awake()
    {
        _rigidbody = _target.gameObject.GetComponent<Rigidbody2D>();
        _trailerHandler = _target.gameObject.GetComponent<TrailerHandler>();
        _trailerHandler.OnAttachedTrailer += OnAttachedTrailer;
        _trailerHandler.OnDetachedTrailer += OnDetachedTrailer;
        _mainCamera = Camera.main;
        _fovMin = _mainCamera.fieldOfView;
    }

    void Update()
    {
        transform.position = new Vector3(_target.position.x, _target.position.y, transform.position.z);
        var samplePoint = _fovCurve.curve.Evaluate(_rigidbody.velocity.magnitude / 50f);
        _mainCamera.fieldOfView = Mathf.Lerp(_fovMin, _fovMax, samplePoint);
    }

    void OnAttachedTrailer(Rigidbody2D trailer)
    {
        ChangeFov(5);
    }
    
    void OnDetachedTrailer()
    {
        ChangeFov(-5);
    }

    void ChangeFov(float value)
    {
        _fovMin += value;
        _fovMax += value;
    }

    private void OnDisable()
    {
        _trailerHandler.OnAttachedTrailer -= OnAttachedTrailer;
        _trailerHandler.OnDetachedTrailer -= OnDetachedTrailer;
    }
}
