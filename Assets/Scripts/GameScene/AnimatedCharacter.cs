using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimatedCharacter : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private Sprite[] _idleSprites;
    [SerializeField] private Sprite[] _hurtSprites;
    [SerializeField] private Sprite[] _attackSprites;
    private Coroutine IdleCoroutine;

    private void Awake() 
    {
        IdleCoroutine = StartCoroutine(IdleAnim());
    }

    private IEnumerator IdleAnim()
    {
        int i = 0;
        while (true)
        {
            _image.sprite = _idleSprites[i++];
            yield return new WaitForSeconds(0.1f);
            i = i >= _idleSprites.Length ? 0 : i;
        }
    }

    public void Attack()
    {
        StartCoroutine(AttackAnim());
    }

    private IEnumerator AttackAnim()
    {
        StopCoroutine(IdleCoroutine);

        for (int i = 0; i < _attackSprites.Length; i++)
        {
            _image.sprite = _attackSprites[i];
            yield return new WaitForSeconds(0.1f);
        }
    
        yield return new WaitForSeconds(0.5f);

        IdleCoroutine = StartCoroutine(IdleAnim());
    }
    
    public void Hurt()
    {
        StartCoroutine(HurtAnim());
    }

    private IEnumerator HurtAnim()
    {
        StopCoroutine(IdleCoroutine);

        for (int i = 0; i < _attackSprites.Length; i++)
        {
            _image.sprite = _hurtSprites[i];
            yield return new WaitForSeconds(0.1f);
        }
    
        yield return new WaitForSeconds(0.5f);

        IdleCoroutine = StartCoroutine(IdleAnim());
    }
}
