using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuView : HFSMStateView
{
    [SerializeField] private ControlGuide _associatedControlGuide;

    [SerializeField] private Button _startGameButton;
    [SerializeField] private Button _loadGameButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _exitGameButton;

    [SerializeField] private Selectable _firstSelectedObject;

    [SerializeField] private EventSystem _eventSystem;

    private MainMenuViewModel _viewModel;

    private bool _isSetuped;

    public void Setup(MainMenuViewModel viewModel)
    {
        if (_isSetuped) return;

        _viewModel = viewModel;

        _viewModel.SavedStatesCount.SubscribeWithEnableBinding((c) => { _loadGameButton.interactable = c > 0; }, this);

        _startGameButton.onClick.AddListener(() => _viewModel.RequestStartGame());
        _loadGameButton.onClick.AddListener(() => _viewModel.RequestLoadMenu());
        _settingsButton.onClick.AddListener(() => _viewModel.RequestSettings());
        _exitGameButton.onClick.AddListener(() => _viewModel.RequestGameExit());

        _isSetuped = true;
    }

    public void Show()
    {
        gameObject.SetActive(true);

        _eventSystem.SetSelectedGameObject(_firstSelectedObject.gameObject);

        _associatedControlGuide.Show();
    }

    public void Hide()
    {
        gameObject.SetActive(false);

        _associatedControlGuide.Hide();
    }
}
