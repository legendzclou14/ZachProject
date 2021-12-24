using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Video;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    [SerializeField] private VideoPlayer _player;
    [SerializeField] private GuessPopup _guessPopup;
    [SerializeField] private Button _attackButton;
    [SerializeField] private Button _thinkButton;
    [SerializeField] private TextMeshProUGUI _gameText;
    [SerializeField] private HealthManager _healthManager;
    [SerializeField] private int _thinkUncoveredLetters;
    [SerializeField] private string[] _wordsToGuess;
    [SerializeField] private int[] _lettersAtStart;
    [SerializeField] private VideoClip _goodEnding;
    [SerializeField] private VideoClip _badEnding;
    [SerializeField] private AnimatedCharacter _goodGuy;
    [SerializeField] private AnimatedCharacter _badGuy;
    [SerializeField] private RectTransform _actionUI;
    [SerializeField] private RectTransform _healthUI;
    [SerializeField] private AnimationCurve _animCurve;
    [SerializeField] private APAudioManager _audioManager;
    [SerializeField] private Image _fadeInImage;
    [SerializeField] private AudioSource _voiceSound;
    [SerializeField] private GiftPopup _giftPopup;
    
    private bool _canInteract = false;
    private float _actionUIorig;
    private float _healthUIorig;
    private int _currentWordIndex = 0;
    private string _currentWordWithCoveredChars;
    private List<int> _uncoveredLetters = new List<int>();

    private void Awake() 
    {
        _player.gameObject.SetActive(true);
        _player.loopPointReached += OnIntroEnd;
        _guessPopup.OnAttackConfirmed += CheckAnswer;

        _healthManager.OnGoodGuyKilled += OnGoodGuyKilled;
        _healthManager.OnBadGuyHit += OnBadGuyHit;
        _healthManager.OnGameReady += StartGame;
        _healthManager.OnReviveEnd += BackToActionMenu;
        _healthManager.OnReviveEnd += _goodGuy.StartIdle;

        _goodGuy.BackToFight += BackToActionMenu;
        _badGuy.BackToFight += BackToActionMenu;

        _giftPopup.OnGiftOpen += FromGiftOpened;

        _actionUIorig = _actionUI.anchoredPosition.y;
        _healthUIorig = _healthUI.anchoredPosition.y;
        _actionUI.anchoredPosition = new Vector2(_actionUI.anchoredPosition.x, _actionUIorig - 230);
        _healthUI.anchoredPosition = new Vector2(_healthUI.anchoredPosition.x, _healthUIorig + 230);

        _player.Play();
        StartCoroutine(FadeinPlayVideo());
    }

    private void FromGiftOpened()
    {
        _healthManager.AttackBadGuy();
        _goodGuy.Attack(_healthManager.HitsOnBadGuy);
        _badGuy.Hurt();
    }

    private void CheckAnswer(string answer)
    {
        if (answer.ToLower() == _wordsToGuess[_currentWordIndex].ToLower())
        {
            _giftPopup.gameObject.SetActive(true);
            Debug.Log("good answer!");
        }
        else
        {
            _healthManager.AttackGoodGuy();
            _badGuy.Attack(0);
            _goodGuy.Hurt();
            Debug.Log("Bad answer dumbfuck");
        }
    }

    private void OnGoodGuyKilled()
    {
        Debug.Log("Good guy is dead Sadge :(");
        _goodGuy.StayHurt();
        _audioManager.PlaySFX(APAudioManager.SFXSound.PLAYERDIE);
        _player.clip = _badEnding;
        _player.loopPointReached += ShowUIEndRevive;
        StartCoroutine(HideUI());
    }

    private void OnBadGuyHit(int hit)
    {
        switch (hit)
        {
            case 0:
                Debug.Log("First hit!");
                NextWord();
                break;
            case 1:
                Debug.Log("Second hit!");
                NextWord();
                break;
            case 2:
                Debug.Log("Third hit!");
                NextWord();
                break;
            case 3:
                Debug.Log("Fourth hit!");
                NextWord();
                break;
            case 4:
                Debug.Log("Fifth and final hit!");
                _audioManager.StopMusic();
                _badGuy.StayHurt();
                _player.clip = _goodEnding;
                _player.gameObject.SetActive(true);
                _player.loopPointReached += (x) => SceneManager.LoadScene("MainMenuScene");
                StartCoroutine(ClearAndPlay());
                _player.Play();
                break;
            default:
                break;
        }
    }
    
    private IEnumerator FadeinPlayVideo()
    {
        float i = 0;
        float _animTime = 3.0f;
        yield return new WaitForSeconds(2.5f);

        while (i < _animTime)
        {
            _fadeInImage.color = new Color(0, 0, 0, 1f - (i/_animTime));
            i += Time.deltaTime;
            yield return null;
        }

        _fadeInImage.gameObject.SetActive(false);
    }

    private void OnIntroEnd(VideoPlayer vp)
    {
        _player.loopPointReached -= OnIntroEnd;
        _player.gameObject.SetActive(false);
        StartCoroutine(ShowUI(true));
    }

    private void ShowUIEndRevive(VideoPlayer vp)
    {
        _player.gameObject.SetActive(false);
        _goodGuy.StartIdle();
        StartCoroutine(ShowUI(false));
    }

    private IEnumerator ShowUI(bool fromMenu)
    {
        _audioManager.PlayMusic();

        float t = 0;
        float max = 2.0f;
        float offset = 230;

        while (t < max)
        {
            _actionUI.anchoredPosition = new Vector2(_actionUI.anchoredPosition.x, _actionUIorig - offset * _animCurve.Evaluate(1 - (t/max)));
            _healthUI.anchoredPosition = new Vector2(_healthUI.anchoredPosition.x, _healthUIorig + offset * _animCurve.Evaluate(1 - (t/max)));
            t += Time.deltaTime;
            yield return null;
        }

        _actionUI.anchoredPosition = new Vector2(_actionUI.anchoredPosition.x, _actionUIorig);
        _healthUI.anchoredPosition = new Vector2(_healthUI.anchoredPosition.x, _healthUIorig);

        if (fromMenu)
        {
            _healthManager.FillBars();
        }
        else
        {
            _player.loopPointReached -= ShowUIEndRevive;
            _healthManager.GoodGuyRefill();
        }
    }
    
    private IEnumerator HideUI()
    {
        _audioManager.StopMusic();
        
        float t = 0;
        float max = 2.0f;
        float offset = 230;

        while (t < max)
        {
            _actionUI.anchoredPosition = new Vector2(_actionUI.anchoredPosition.x, _actionUIorig - offset * _animCurve.Evaluate((t/max)));
            _healthUI.anchoredPosition = new Vector2(_healthUI.anchoredPosition.x, _healthUIorig + offset * _animCurve.Evaluate((t/max)));
            t += Time.deltaTime;
            yield return null;
        }
        
        StartCoroutine(ClearAndPlay());
    }

    private IEnumerator ClearAndPlay()
    {
        _player.targetTexture.Release();
        yield return new WaitForEndOfFrame();
        _player.gameObject.SetActive(true);
        _player.Play();
    }

    private void StartGame()
    {
        PrepareWord(_currentWordIndex);
        _thinkButton.onClick.AddListener(OnThinkClick);
        _attackButton.onClick.AddListener(OnAttackClick);
        BackToActionMenu();
    }

    private void OnThinkClick()
    {
        _audioManager.PlaySFX(APAudioManager.SFXSound.CLICK);

        if (_canInteract)
        {
            _canInteract = false;
            UncoverLetters(_thinkUncoveredLetters);
            _healthManager.AttackGoodGuy();
            _badGuy.Attack(0);
            _goodGuy.Hurt();
        }
    }
    
    private void OnAttackClick()
    {
        _audioManager.PlaySFX(APAudioManager.SFXSound.CLICK);

        if (_canInteract)
        {
            _canInteract = false;
            _guessPopup.gameObject.SetActive(true);
            _guessPopup.Setup(_currentWordWithCoveredChars);
        }
    }

    private void BackToActionMenu()
    {
        StartCoroutine(PopulateGameText());
    }

    private void PrepareWord(int wordIndex)
    {
        int i = 0;
        while (i < _wordsToGuess[_currentWordIndex].Length)
        {
            if (_wordsToGuess[_currentWordIndex][i] == ' ')
            {
                _uncoveredLetters.Add(i);
            }
            i++;
        }

        UncoverLetters(_lettersAtStart[_currentWordIndex]);
    }

    private IEnumerator PopulateGameText()
    {
        _gameText.text = string.Empty;

        _voiceSound.Play();

        int i = 0;
        while (i < _wordsToGuess[_currentWordIndex].Length)
        {
            if (_uncoveredLetters.Contains(i))
            {
                _gameText.text += _wordsToGuess[_currentWordIndex][i];
            }
            else
            {
                _gameText.text += '_';
            }

            yield return new WaitForSeconds(0.05f);
            i++;
        }

        _currentWordWithCoveredChars = _gameText.text;
        _voiceSound.Stop();
        _canInteract = true;
    }

    //Play after good guy attack anim
    private void NextWord()
    {
        _currentWordIndex++;
        if (_currentWordIndex < _wordsToGuess.Length)
        {
            _uncoveredLetters.Clear();
            PrepareWord(_currentWordIndex);
        }
        else
        {
            //Game over, you won
        }
    }

    private void UncoverLetters(int nbToUncover)
    {
        int i = 0;
        while (i < nbToUncover && _uncoveredLetters.Count < _wordsToGuess[_currentWordIndex].Length)
        {
            int temp = Random.Range(0, _wordsToGuess[_currentWordIndex].Length);
            if (!_uncoveredLetters.Contains(temp))
            {
                _uncoveredLetters.Add(temp);
                i++;
            }
        }
    }
}
