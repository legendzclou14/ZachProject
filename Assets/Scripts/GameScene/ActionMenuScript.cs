using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionMenuScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject _choiceOutline;
    [SerializeField] APAudioManager _audioManager;

    public void OnPointerEnter(PointerEventData eventData)
    {
        _audioManager.PlaySFX(APAudioManager.SFXSound.HOVER);
        _choiceOutline.transform.position = this.transform.position;
        _choiceOutline.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _choiceOutline.gameObject.SetActive(false);
    }
}
