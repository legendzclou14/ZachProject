using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class GameSceneManager : MonoBehaviour
{
    [SerializeField] private VideoPlayer _player;

    private void Awake() 
    {
        _player.loopPointReached += OnIntroEnd;
        _player.Play();
    }

    private void OnIntroEnd(VideoPlayer vp)
    {
        _player.loopPointReached -= OnIntroEnd;
        _player.gameObject.SetActive(false);
    }
}
