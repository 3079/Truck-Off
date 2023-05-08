using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Guns : MonoBehaviour
{
    bool StartFromRight = false;
    [SerializeField] Gun LeftGun;
    [SerializeField] Gun RightGun;

    //[SerializeField] float ShootDelay = 0.1f;
    [SerializeField] float AccelerationTime = 10f;
    [SerializeField] AnimationCurve GunAccelerationCurve;
    [SerializeField] float DefaultDelay = 1f;
    float ShootingStartTime;
    float LastShootTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        ShootingStartTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire")) 
        {
            ShootingStartTime = Time.time;
        }
        float ShootingDuration = Time.time - ShootingStartTime;
        float ShootDelay = DefaultDelay * GunAccelerationCurve.Evaluate(Mathf.Clamp(ShootingDuration / AccelerationTime, 0f, 1f));
        
        if (Input.GetButton("Fire") && LastShootTime + ShootDelay < Time.time)
        {
            float SpeedValue = 0.1f / ShootDelay;

            if (StartFromRight)
            {
                RightGun.Shoot(SpeedValue);
            }
            else 
            {
                LeftGun.Shoot(SpeedValue);
            }
            StartFromRight = !StartFromRight;
            LastShootTime = Time.time;
        }
    }
}
