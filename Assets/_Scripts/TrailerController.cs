using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts;
using UnityEngine;

public class TrailerController : Target
{
    public TrailerHandler _handler;
    public event Action<TrailerController, float> OnDeath;
    private Rigidbody2D _rigidbody;
    private float _spawnTime;

    public override void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        GameLogic.Instance.InitializeTrailer(this);
        base.Awake();
    }

    public void ResetTime()
    {
        _spawnTime = Time.time;
    }
    
    private void Detach()
    {
        if (_handler == null) return;
        
        _handler.DetachTrailer(_rigidbody);
    }
    public override void Die()
    {
        var deliveryTime = Time.time - _spawnTime;
        Detach();
        OnDeath?.Invoke(this, deliveryTime);
        base.Die();
    }
}
