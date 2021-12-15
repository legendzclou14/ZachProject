using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionMenuScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject _choiceOutline;

    public void OnPointerEnter(PointerEventData eventData)
    {
        _choiceOutline.transform.position = this.transform.position;
        _choiceOutline.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _choiceOutline.gameObject.SetActive(false);
    }
}
