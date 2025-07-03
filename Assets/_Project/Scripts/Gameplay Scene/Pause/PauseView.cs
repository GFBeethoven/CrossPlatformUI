using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseView : HFSMStateView
{
    public event Action MainMenuRequested;
    public event Action SaveStateRequested;
    public event Action UnpauseRequested;

    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _saveButton;
    [SerializeField] private Button _mainMenuButton;

    [SerializeField] private Selectable _firstSelectable;

    [SerializeField] private EventSystem _eventSystem;

    [SerializeField] private ControlGuide _controlGuide;

    private InputSource _calledInputSource;

    public void Initialize()
    {
        _continueButton.onClick.AddListener(() => UnpauseRequested?.Invoke());
        _saveButton.onClick.AddListener(() => 
        {
            _saveButton.interactable = false;
            SaveStateRequested?.Invoke();
        });
        _mainMenuButton.onClick.AddListener(() => MainMenuRequested?.Invoke());
    }

    public void Show(InputSource calledInputSource)
    {
        _calledInputSource = calledInputSource;

        _controlGuide?.Show();

        _saveButton.interactable = true;

        _eventSystem.SetSelectedGameObject(_firstSelectable.gameObject);

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        _calledInputSource = null;

        _controlGuide?.Hide();

        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_calledInputSource != null)
        {
            if (_calledInputSource.GameplayInputProvider != null)
            {
                if (_calledInputSource.GameplayInputProvider.IsPauseRequested)
                {
                    AudioPlayer.Instance?.PlayNavigationBack();

                    UnpauseRequested?.Invoke();
                }
            }

            if (_calledInputSource.UIInputProvider != null)
            {
                if (_calledInputSource.UIInputProvider.IsBackButtonDown)
                {
                    AudioPlayer.Instance?.PlayNavigationBack();

                    UnpauseRequested?.Invoke();
                }
            }
        }
    }
}
