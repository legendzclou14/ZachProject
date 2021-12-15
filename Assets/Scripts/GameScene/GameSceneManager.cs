using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using TMPro;

public class GameSceneManager : MonoBehaviour
{
    [SerializeField] private VideoPlayer _player;
    [SerializeField] private Button _attackButton;
    [SerializeField] private Button _thinkButton;
    [SerializeField] private TextMeshProUGUI _gameText;
    [SerializeField] private int _thinkUncoveredLetters;
    [SerializeField] private string[] _wordsToGuess;
    [SerializeField] private int[] _lettersAtStart;
    private int _currentWordIndex = 0;
    private List<int> _uncoveredLetters = new List<int>();

    private void Awake() 
    {
        _player.gameObject.SetActive(true);
        _player.loopPointReached += OnIntroEnd;
        _player.Play();
    }

    private void OnIntroEnd(VideoPlayer vp)
    {
        _player.loopPointReached -= OnIntroEnd;
        _player.gameObject.SetActive(false);
        StartGame();
    }

    private void StartGame()
    {
        PrepareWord(_currentWordIndex);
        _thinkButton.onClick.AddListener(OnThinkClick);
        BackToActionMenu();
    }

    private void OnThinkClick()
    {
        UncoverLetters(_thinkUncoveredLetters);
        //vv THIS GOES AFTER ANIM vv
        BackToActionMenu();
    }
    
    private void OnAttackClick()
    {
        UncoverLetters(_thinkUncoveredLetters);
        //vv THIS GOES AFTER ANIM vv
        BackToActionMenu();
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
