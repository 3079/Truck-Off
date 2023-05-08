using System.Collections;
using System.Collections.Generic;
using _Scripts;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class Gun : MonoBehaviour
{
    public int damage = 1;
    public float range = 100f;
    [SerializeField] bool AddBulletSpread = true;
    [SerializeField] Vector3 BulletSpreadVariance = new Vector3(0.1f, 0.1f, 0.1f);
    [SerializeField] ParticleSystem Muzzleflash;
    [SerializeField] Transform BulletSpawnPoint;
    [SerializeField] ParticleSystem Impact;
    [SerializeField] TrailRenderer BulletTrail;
    
    [SerializeField] LayerMask Mask;

    [SerializeField] private float Speed = 100;
    [SerializeField] bool BouncingBullets;
    [SerializeField] float BounceDistance = 10f;
    [SerializeField] int Bounces = 1;
    //[SerializeField] bool StartFromRight = false;

    private Animator Animator;

    [SerializeField] Sound[] shotSounds;
    
    // Start is called before the first frame update
    void Start()
    {
        Animator = GetComponent<Animator>();
        foreach(Sound s in shotSounds)
            {
                s.source = gameObject.AddComponent<AudioSource>();

                s.source.clip = s.clip;
                s.source.volume = s.volume;
                s.source.pitch = 1f;
                s.source.loop = false;
                //s.source.outputAudioMixerGroup = sfxMixer;
            }
    }
    public void Shoot(float SpeedValue)
    {

        //Animator.SetBool("isShooting", true);
        Animator.SetTrigger("Shoot");
        Animator.speed = SpeedValue;
        Muzzleflash.Play();
        Sound sound = shotSounds[UnityEngine.Random.Range(0, shotSounds.Length)];    
        sound.source.Play();
        Vector3 direction = transform.up;
        TrailRenderer trail = Instantiate(BulletTrail, BulletSpawnPoint.position, Quaternion.identity);

        if (Physics.Raycast(BulletSpawnPoint.position, direction, out RaycastHit hit, float.MaxValue, Mask))
        {
            //StartCoroutine(SpawnTrail(trail, hit.point, hit.normal, BounceDistance, true));
            StartCoroutine(SpawnTrail(trail, hit.point, hit.normal, Bounces, true));
            var target = hit.transform.GetComponentInParent<IDamageable>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }
        }
        else
        {
            //StartCoroutine(SpawnTrail(trail, BulletSpawnPoint.position + direction * 100, Vector3.zero, BounceDistance, false));
            StartCoroutine(SpawnTrail(trail, BulletSpawnPoint.position + direction * 100, Vector3.zero, Bounces, false));
        }

    }

    //private IEnumerator SpawnTrail(TrailRenderer Trail, Vector3 HitPoint, Vector3 HitNormal, float BounceDistance, bool MadeImpact)
    private IEnumerator SpawnTrail(TrailRenderer Trail, Vector3 HitPoint, Vector3 HitNormal, int Bounces, bool MadeImpact)
    {
        Vector3 startPosition = Trail.transform.position;
        Vector3 direction = (HitPoint - Trail.transform.position).normalized;

        float distance = Vector3.Distance(Trail.transform.position, HitPoint);
        float startingDistance = distance;

        while (distance > 0)
        {
            Trail.transform.position = Vector3.Lerp(startPosition, HitPoint, 1 - (distance / startingDistance));
            distance -= Time.deltaTime * Speed;

            yield return null;
        }

        Trail.transform.position = HitPoint;

        if (MadeImpact)
        {
            Instantiate(Impact, HitPoint, Quaternion.LookRotation(HitNormal));

            //if (BouncingBullets && BounceDistance > 0)
            if (BouncingBullets && Bounces > 0)
            {
                Vector3 bounceDirection = Vector3.Reflect(direction, HitNormal);

                if (Physics.Raycast(HitPoint, bounceDirection, out RaycastHit hit, BounceDistance, Mask))
                {
                    yield return StartCoroutine(SpawnTrail(
                        Trail,
                        hit.point,
                        hit.normal,
                        //BounceDistance - Vector3.Distance(hit.point, HitPoint),
                        Bounces-1,
                        true
                    ));
                }
                else
                {
                    yield return StartCoroutine(SpawnTrail(
                        Trail,
                        HitPoint + bounceDirection * BounceDistance,
                        Vector3.zero,
                        0,
                        false
                    ));
                }
            }
        }

        Destroy(Trail.gameObject, Trail.time);
    }
}
