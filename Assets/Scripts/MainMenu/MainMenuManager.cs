using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Button _playButton;
    [SerializeField] private RectTransform _playButtonRT;
    [SerializeField] private RectTransform _titleTextRT;
    [SerializeField] private Vector2 _finalTitlePos;
    [SerializeField] private Vector2 _finalPlayPos;
    [SerializeField] private float _animTime;
    [SerializeField] private AnimationCurve _animCurve;
    [SerializeField] private VideoPlayer _player;
    [SerializeField] private VideoClip _menuLoopClip;
    private Vector2 _startTitlePos;
    private Vector2 _startPlayPos;

    void Awake()
    {
        _playButton.onClick.AddListener(OnPlayClicked);
        _startPlayPos = _playButtonRT.anchoredPosition;
        _startTitlePos = _titleTextRT.anchoredPosition;
        _player.loopPointReached += OnIntroEnd;
        _player.Play();
    }

    private void OnIntroEnd(VideoPlayer vp)
    {
        _player.loopPointReached -= OnIntroEnd;
        _player.isLooping = true;
        _player.clip = _menuLoopClip;
        _player.Play();
        StartCoroutine(UIAnimStartApp());
    }

    private void OnPlayClicked()
    {
        SceneManager.LoadScene("GameScene");
    }

    private IEnumerator UIAnimStartApp()
    {
        float i = 0;
        float lerpRatio = 0;
        Vector2 playAnimStartingPos = _playButtonRT.anchoredPosition;
        Vector2 titleAnimStartingPos = _titleTextRT.anchoredPosition;

        while (i < _animTime)
        {
            lerpRatio = i / _animTime;
            _playButtonRT.anchoredPosition = playAnimStartingPos + (_animCurve.Evaluate(lerpRatio) * (_finalPlayPos - playAnimStartingPos));
            _titleTextRT.anchoredPosition = titleAnimStartingPos + (_animCurve.Evaluate(lerpRatio) * (_finalTitlePos - titleAnimStartingPos));
            i += Time.deltaTime;
            yield return null;
        }

        _playButtonRT.anchoredPosition = _finalPlayPos;
        _titleTextRT.anchoredPosition = _finalTitlePos;
    }
    
    //UNUSED FOR NOW
    private IEnumerator UIAnimPlayGame()
    {
        float i = 0;
        float lerpRatio = 0;
        Vector2 playAnimStartingPos = _playButtonRT.anchoredPosition;
        Vector2 titleAnimStartingPos = _titleTextRT.anchoredPosition;

        while (i < _animTime)
        {
            lerpRatio = i / _animTime;
            _playButtonRT.anchoredPosition = playAnimStartingPos + (_animCurve.Evaluate(lerpRatio) * (_startPlayPos - playAnimStartingPos));
            _titleTextRT.anchoredPosition = titleAnimStartingPos + (_animCurve.Evaluate(lerpRatio) * (_startTitlePos - titleAnimStartingPos));
            i += Time.deltaTime;
            yield return null;
        }

        _playButtonRT.anchoredPosition = _startPlayPos;
        _titleTextRT.anchoredPosition = _startTitlePos;

        SceneManager.LoadScene("GameScene");
    }
}
