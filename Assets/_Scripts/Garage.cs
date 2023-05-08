using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Garage : MonoBehaviour
{
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private GameObject _trailerPrefab;
    [SerializeField] private int _maxTrailers;
    [SerializeField] private float _spawnOffset;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.isTrigger) return;
        var truck = other.GetComponent<TrailerHandler>();
        if (truck == null) return;
        Debug.Log("Pending order check");
        if (GameLogic.Instance.orderPending) return;
        
        Debug.Log("Pending order check passed");

        var trailerCount = truck._trailers.Count;
        
        Debug.Log("Trailer count: " + trailerCount);
        
        if(trailerCount == _maxTrailers) return;

        // StartCoroutine(Spawn(truck, trailerCount));
        var truckRb = truck.gameObject.GetComponent<Rigidbody2D>();
        truckRb.velocity = Vector2.zero;
        truckRb.angularVelocity =0f;
        truckRb.freezeRotation = true;
        truck.gameObject.transform.SetPositionAndRotation(_spawnPoint.position, Quaternion.identity);
        // yield return new WaitForEndOfFrame();
        var trailerArray = truck._trailers.ToArray();
        for (int i = 0; i < _maxTrailers; i++)
        {
            Vector3 spawnPos;
            // var offset = i == trailerCount - 1 ? new Vector3(0, -_spawnOffset + 1, 0) : new Vector3(0, -_spawnOffset, 0);
            var offset = new Vector3(0, -_spawnOffset, 0);
            if (i < trailerCount)
            {
                trailerArray[i].velocity = Vector2.zero;
                trailerArray[i].angularVelocity =0f;
                trailerArray[i].freezeRotation = true;
                spawnPos = _spawnPoint.position + (trailerCount - i) * offset + 0.75f * Vector3.back;
                trailerArray[i].transform.SetPositionAndRotation(spawnPos, Quaternion.identity);
            }
            else
            {
                spawnPos = _spawnPoint.position + (i + 1) * offset + 0.75f * Vector3.back;
                var trailer = Instantiate(_trailerPrefab, spawnPos, Quaternion.identity);
                var t = trailer.GetComponent<Rigidbody2D>();
                truck.AttachTrailer(t);
                t.velocity = Vector2.zero;
                t.angularVelocity = 0f;
                t.freezeRotation = true;
                // trailer.transform.SetPositionAndRotation(trailerArray[trailerCount - i + 1].position + new Vector2(0, -3.15f), _spawnPoint.rotation);
            }
        }

        foreach (var trailer in truck._trailers)
        {
            trailer.freezeRotation = false;
        }
        truckRb.freezeRotation = false;
        
        GameLogic.Instance.NewOrder(_maxTrailers);
    }
}
