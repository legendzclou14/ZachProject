using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class GuessPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _wordToGuessText;
    [SerializeField] private TextMeshProUGUI _badGuyDialogueText;
    [SerializeField] private TMP_InputField _guessField;
    [SerializeField] private Button _attackButton;
    [SerializeField] private string[] _badGuyDialogueBank;
    public event Action<string> OnAttackConfirmed;

    private void Awake() 
    {
        _attackButton.onClick.AddListener(OnAttackClick);
        _guessField.onValueChanged.AddListener(MakeAttackButtonInteractible);
    }

    private void MakeAttackButtonInteractible(string text)
    {
        _attackButton.interactable = text.Length > 0;
    }

    public void Setup(string word)
    {
        _guessField.text = string.Empty;
        _wordToGuessText.text = word;
        StartCoroutine(BadGuyDialogue(_badGuyDialogueBank[UnityEngine.Random.Range(0, _badGuyDialogueBank.Length)]));
    }

    private IEnumerator BadGuyDialogue(string text)
    {
        _badGuyDialogueText.text = string.Empty;

        foreach (char i in text)
        {
            _badGuyDialogueText.text += i;
            yield return new WaitForSeconds(0.05f);
        }
    }

    private void OnAttackClick()
    {
        OnAttackConfirmed?.Invoke(_guessField.text);
        this.gameObject.SetActive(false);
    }
}
