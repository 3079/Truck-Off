using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class InputHandler : MonoBehaviour
{
    public static InputHandler Instance;
    public float Steering { get; private set; }
    public float Acceleration { get; private set; }
    public bool Braking { get; private set; }
    public bool Detach { get; private set; }
    public bool Attach { get; private set; }

    public event Action OnRestart;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    private void Update()
    {
        Steering = Input.GetAxisRaw("Horizontal");
        Acceleration = Input.GetAxisRaw("Vertical");
        Braking = Input.GetButton("Brake");
        Detach = Input.GetButtonDown("Detach");
        Attach = Input.GetButtonDown("Attach");
        // if (Input.GetButtonDown("Restart")) OnRestart?.Invoke();
        // Debug.Log(Horizontal + "; " + Vertical);
    }
}
