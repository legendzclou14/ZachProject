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
    [SerializeField] private Button _quitButton;
    [SerializeField] private RectTransform _playButtonRT;
    [SerializeField] private Vector2 _finalPlayPos;
    [SerializeField] private float _animTime;
    [SerializeField] private AnimationCurve _animCurve;
    [SerializeField] private VideoPlayer _player;
    [SerializeField] private VideoClip _menuLoopClip;
    [SerializeField] private APAudioManager _audioManager;
    [SerializeField] private Image _fadeOutImage;
    private Vector2 _startPlayPos;

    void Awake()
    {
        _playButton.onClick.AddListener(OnPlayClicked);
        _quitButton.onClick.AddListener(QuitApp);
        _startPlayPos = _playButtonRT.anchoredPosition;
        _player.loopPointReached += OnIntroEnd;
        _player.Play();
    }

    private void QuitApp()
    {
        Application.Quit();
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
        _audioManager.PlaySFX(APAudioManager.SFXSound.CLICK);
        StartCoroutine(UIAnimPlayGame());
    }

    private IEnumerator UIAnimStartApp()
    {
        _audioManager.PlayMusic();
        
        float i = 0;
        float lerpRatio = 0;
        Vector2 playAnimStartingPos = _playButtonRT.anchoredPosition;

        while (i < _animTime)
        {
            lerpRatio = i / _animTime;
            _playButtonRT.anchoredPosition = playAnimStartingPos + (_animCurve.Evaluate(lerpRatio) * (_finalPlayPos - playAnimStartingPos));
            i += Time.deltaTime;
            yield return null;
        }

        _playButtonRT.anchoredPosition = _finalPlayPos;
    }
    
    private IEnumerator UIAnimPlayGame()
    {
        _fadeOutImage.gameObject.SetActive(true);
        float i = 0;

        while (i < _animTime)
        {
            _fadeOutImage.color = new Color(0, 0, 0, i/_animTime);
            i += Time.deltaTime;
            yield return null;
        }

        _fadeOutImage.color = Color.black;

        SceneManager.LoadScene("GameScene");
    }
}
