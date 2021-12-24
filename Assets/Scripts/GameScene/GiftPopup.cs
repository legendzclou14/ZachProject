using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GiftPopup : MonoBehaviour
{
    [SerializeField] private Image _gift;
    [SerializeField] private Sprite _OpenSprite;
    [SerializeField] private Sprite _ClosedSprite;
    [SerializeField] private Button _button;
    public event Action OnGiftOpen;

    private void Awake() 
    {
        _button.onClick.AddListener(OnGiftClick);
    }

    private void OnEnable() 
    {
        _gift.sprite = _ClosedSprite;
        StartCoroutine(Zoom(true));
    }

    private void OnGiftClick()
    {
        _gift.sprite = _OpenSprite;
        StartCoroutine(Zoom(false));
    }

    private IEnumerator Zoom(bool zoomIn)
    {
        if (!zoomIn)
        {
            yield return new WaitForSeconds(1.0f);
        }

        float i = zoomIn ? 0 : 1;
        float max = 1;

        while (i <= max && i > 0)
        {
            _gift.transform.localScale = new Vector3(i/max, i/max, 1);
            
            if (zoomIn)
            {
                i += Time.deltaTime;
            }
            else
            {
                i -= Time.deltaTime;
            }
            yield return null;
        }

        if (!zoomIn)
        {
            _gift.transform.localScale = new Vector3(0, 0, 1);
            OnGiftOpen?.Invoke();
            this.gameObject.SetActive(false);
        }
        else
        {
            _gift.transform.localScale = new Vector3(1, 1, 1);
        }
    }

}
