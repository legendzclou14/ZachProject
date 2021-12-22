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
    public event Action OnGoodGuyKilled;
    public event Action<int> OnBadGuyHit;
    private float _goodHealthValue = 1;
    private float _badHealthValue = 1;
    private int _hitsOnBadGuy = 0;

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
        float max = 1.0f;
        while (i < max)
        {
            slider.value = startPercent - (percentToTake * (i/max));
            i += Time.deltaTime;
            yield return null;
        }

        slider.value = startPercent - percentToTake;

        if (slider.value <= 0)
        {
            slider.gameObject.SetActive(false);
        }
        
        if (slider == _goodGuyHealth)
        {
            _goodHealthValue -= percentToTake;
            if (_goodHealthValue <= 0)
            {
                OnGoodGuyKilled?.Invoke();
            }
        }
        else
        {
            _badHealthValue -= percentToTake;
            _giftImages[_hitsOnBadGuy].sprite = _openGift;
            OnBadGuyHit?.Invoke(_hitsOnBadGuy++);
        }
    }
}
