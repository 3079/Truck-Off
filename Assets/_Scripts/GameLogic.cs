using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

public class GameLogic : MonoBehaviour
{
    public static GameLogic Instance;
    
    [Serializable]
    public class TimeTier
    {
        public float timeThreshold;
        public float timeBonus;
        public int scoreBonus;
    }
    
    [Header("Timer")] 
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private float _startTimer;

    [Header("Score")]
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private List<TimeTier> _timeTiers;

    public List<DeliveryPoint> _deliveryPoints;
    public List<DeliveryPoint> currentOrder = new List<DeliveryPoint>();

    [SerializeField] private float _trailerCount;
    [SerializeField] private float _deliveriesLeft;
    public bool orderPending => _deliveriesLeft > 0;
    public int activePoints => currentOrder.Count;
    
    private float _timer;
    private int _score = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        _deliveryPoints = FindObjectsOfType<DeliveryPoint>().ToList();
        _timer = _startTimer;
        
        Debug.Log(_deliveryPoints.Count);
    }

    private void Start()
    {
        DeactivateAllDeliveryPoints();
    }

    public void OnDelivery(DeliveryPoint deliveryPoint, float time)
    {
        if(!currentOrder.Contains(deliveryPoint)) return;

        currentOrder.Remove(deliveryPoint);
        _deliveriesLeft--;
        if (_deliveriesLeft < _trailerCount)
        {
            _deliveriesLeft++;
        }
        deliveryPoint.Deactivate();
        Debug.Log("Deliveries left: " + currentOrder.Count());
        Debug.Log(orderPending);
        
        for (int i = 0; i < _timeTiers.Count - 1; i++)
        {
            if (time < _timeTiers[i].timeThreshold)
            {
                AddBonus(_timeTiers[i].timeBonus, _timeTiers[i].scoreBonus);
                return;
            }
        }
        AddBonus(_timeTiers[^1].timeBonus, _timeTiers[^1].scoreBonus);
    }

    public void NewOrder(int number)
    {
        while (currentOrder.Count < number)
        {
            var randomPoint = _deliveryPoints[Random.Range(0, _deliveryPoints.Count)];
            if(currentOrder.Contains(randomPoint)) continue;
            currentOrder.Add(randomPoint);
            randomPoint.Activate();
        }

        _deliveriesLeft = currentOrder.Count;
    }

    private void DeactivateAllDeliveryPoints()
    {
        foreach (var deliveryPoint in _deliveryPoints)
        {
            deliveryPoint.Deactivate();
        }

        _deliveriesLeft = 0;
    }

    private void AddBonus(float timeBonus, int scoreBonus)
    {
        
        _timer += timeBonus;
        _score += scoreBonus;
        UpdateUI();
    }

    private void Update()
    {
        _timer -= Time.deltaTime;
        UpdateUI();
        if (_timer <= 0) GameOver();
    }

    private void UpdateUI()
    {
        _timerText.text = _timer.ToString("0.00");
        _scoreText.text = _score.ToString();
    }

    public void InitializeTrailer(TrailerController trailer)
    {
        _trailerCount++;
        trailer.OnDeath += OnTrailerDestroyed;
    }

    private void OnTrailerDestroyed(TrailerController trailer, float time)
    {
        _trailerCount--;
        if (_trailerCount < _deliveriesLeft)
            _deliveriesLeft--;
        trailer.OnDeath -= OnTrailerDestroyed;
    }

    void GameOver()
    {
        SceneManager.instance.EndScreen(_score);
    }
}
