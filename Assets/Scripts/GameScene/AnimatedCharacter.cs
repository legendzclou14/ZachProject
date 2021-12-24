using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class AnimatedCharacter : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private bool _isGoodGuy;
    [SerializeField] private Sprite[] _idleSprites;
    [SerializeField] private Sprite[] _hurtSprites;
    [SerializeField] private Sprite[] _attack1Sprites;
    [SerializeField] private Sprite[] _attack2Sprites;
    [SerializeField] private Sprite[] _attack3Sprites;
    [SerializeField] private Sprite[] _attack4Sprites;
    [SerializeField] private Sprite[] _attack5Sprites;
    [SerializeField] private float _timeBetweenIdleFrames;
    [SerializeField] private APAudioManager _audioManager;
    private Coroutine IdleCoroutine;

    public event Action BackToFight;

    private void Awake() 
    {
        StartIdle();
    }

    public void StartIdle()
    {
        StopAllCoroutines();
        IdleCoroutine = StartCoroutine(IdleAnim());
    }

    private IEnumerator IdleAnim()
    {
        int i = 0;
        while (true)
        {
            _image.sprite = _idleSprites[i++];
            yield return new WaitForSeconds(_timeBetweenIdleFrames);
            i = i >= _idleSprites.Length ? 0 : i;
        }
    }

    public void Attack(int attackNumber)
    {
        StartCoroutine(AttackAnim(attackNumber));
    }

    private IEnumerator AttackAnim(int attackNumber)
    {
        StopCoroutine(IdleCoroutine);

        List<Sprite> sprites = new List<Sprite>();;

        switch (attackNumber)
        {
            case 0:
                sprites = _attack1Sprites.ToList();
                break;
            case 1:
                sprites = _attack2Sprites.ToList();
                break;
            case 2:
                sprites = _attack3Sprites.ToList();
                break;
            case 3:
                sprites = _attack4Sprites.ToList();
                break;
            case 4:
                sprites = _attack5Sprites.ToList();
                break;
            default:
                sprites = _attack1Sprites.ToList();
                break;
        }

        if (!_isGoodGuy)
        {
            for (int i = 0; i < sprites.Count; i++)
            {
                _image.sprite = sprites[i];
                yield return new WaitForSeconds(0.1f);
            }
        }
        else
        {
            for (int i = 0; i < sprites.Count; i++)
            {
                _image.sprite = sprites[i];
                yield return new WaitForSeconds(0.5f);
            }
        }

        StartIdle();
    }
    
    public void Hurt()
    {
        StartCoroutine(HurtAnim());
    }

    public void StayHurt()
    {
        StopAllCoroutines();
        _image.sprite = _hurtSprites[0];
    }

    private IEnumerator HurtAnim()
    {
        float time = _isGoodGuy ? 0.25f : 0.5f;
        yield return new WaitForSeconds(time);


        StopCoroutine(IdleCoroutine);

        _audioManager.PlaySFX(APAudioManager.SFXSound.HURT);
        _image.sprite = _hurtSprites[0];

        float t = 0;
        //5 frames between every drawing. 0.2sec becomes 1 sec.
        float max = 0.2f;
        int i = 0;
        float modifier = 8;
        Vector3 ogPos = _image.transform.position;

        while (t < max)
        {
            if (i % 5 == 0)
            {
                modifier *= -0.8f;
            }
            
            _image.transform.position = new Vector3(_image.transform.position.x + modifier, _image.transform.position.y, _image.transform.position.z);
            t += Time.deltaTime;
            i++;
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            yield return null;
        }
        
        _image.transform.position = ogPos;
        yield return new WaitForSeconds(0.5f);

        BackToFight?.Invoke();
        
        StartIdle();
    }
}
