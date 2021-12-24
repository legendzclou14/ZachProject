using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public const int HP_GOODGUY = 10;
    public const int HP_BADGUY = 5;
    [SerializeField] private Slider _goodGuyHealth;
    [SerializeField] private Slider _badGuyHealth;
    [SerializeField] private Image[] _giftImages;
    [SerializeField] private Sprite _openGift;
    public event Action OnReviveEnd;
    public event Action OnGameReady;
    public event Action OnGoodGuyKilled;
    public event Action<int> OnBadGuyHit;
    private float _goodHealthValue = 1;
    private float _badHealthValue = 1;
    public int HitsOnBadGuy = 0;

    public void GoodGuyRefill()
    {
        StartCoroutine(GoodRefillCoroutine());
    }

    private IEnumerator GoodRefillCoroutine()
    {
        float t = 0;
        float max = 1.0f;

        while (t < max)
        {
            _goodGuyHealth.value = 0.5f * t/max;
            t += Time.deltaTime;
            yield return null;
        }

        _goodGuyHealth.value = 0.5f;
        _goodHealthValue = 0.5f;;
        
        OnReviveEnd?.Invoke();
    }

    public void FillBars()
    {
        StartCoroutine(FillBarsCoroutine());
    }

    private IEnumerator FillBarsCoroutine()
    {
        float t = 0;
        float max = 2.0f;

        while (t < max)
        {
            _goodGuyHealth.value = t/max;
            _badGuyHealth.value = t/max;
            t += Time.deltaTime;
            yield return null;
        }

        _goodGuyHealth.value = 1.0f;
        _badGuyHealth.value = 1.0f;
        OnGameReady?.Invoke();
    }

    public void AttackGoodGuy()
    {
        StartCoroutine(Attack(_goodGuyHealth, 1.0f / HP_GOODGUY));
    }
    
    public void AttackBadGuy()
    {
        StartCoroutine(Attack(_badGuyHealth, 1.0f / HP_BADGUY));
    }


    private IEnumerator Attack(Slider slider, float percentToTake)
    {
        float startPercent = slider == _goodGuyHealth ? _goodHealthValue : _badHealthValue;

        float i = 0f;
        float max = slider == _goodGuyHealth ? 1.0f : 2.0f;
        while (i < max)
        {
            slider.value = startPercent - (percentToTake * (i/max));
            i += Time.deltaTime;
            yield return null;
        }

        slider.value = startPercent - percentToTake;
        
        if (slider == _goodGuyHealth)
        {
            _goodHealthValue -= percentToTake;
            if (_goodHealthValue <= 0.05f)
            {
                _goodHealthValue = 0;
                OnGoodGuyKilled?.Invoke();
            }
        }
        else
        {
            _badHealthValue -= percentToTake;
            _giftImages[HitsOnBadGuy].sprite = _openGift;
            OnBadGuyHit?.Invoke(HitsOnBadGuy++);
        }
    }
}
