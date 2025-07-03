using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameSettingsView : HFSMStateView
{
    public event Action BackToMainMenuRequested;
    public event Action CreditsRequested;
    public event Action DeleteAllSavedRequested;

    [SerializeField] private GameSettingsViewTab[] _tabs;

    [SerializeField] private EventSystem _eventSystem;

    [SerializeField] private GameSettingsDescriptionPanel _descriptionPanel;

    [SerializeField] private Color _activeTabButton;
    [SerializeField] private Color _notActiveTabButton;

    private ReactiveProperty<GameSettingsViewItem> _selectedItem = new ReactiveProperty<GameSettingsViewItem>(null);
    private GameSettingsViewItem _prevSelectedItem;

    private int _currentTabIndex;
    private GameSettingsViewTab _currentTab;

    private GameSettings _model;

    private InputDispatcher _inputDispatcher;

    private bool _isInitialized;

    public void Initialize(GameSettings model, InputDispatcher inputDispatcher, ReadOnlyReactiveProperty<int> savesCount)
    {
        if (_isInitialized) return;

        _model = model;

        _inputDispatcher = inputDispatcher;

        _currentTabIndex = 0;
        _currentTab = null;

        for (int i = 0; i < _tabs.Length; i++)
        {
            _tabs[i].AssociatedButton.SetupColors(_activeTabButton, _notActiveTabButton);

            int index = i;
            _tabs[i].AssociatedButton.onClick.AddListener(() => SetTab(index));

            _tabs[i].Initialize(model, this, _eventSystem, inputDispatcher.UIInputType);
        }

        _selectedItem.Subscribe((i) =>
        {
            _descriptionPanel.SetDescription(i);
            
            if (_prevSelectedItem != null && _prevSelectedItem.ControlGuide != null)
            {
                _prevSelectedItem.ControlGuide.Hide();
            }

            if (i != null)
            {
                i.ControlGuide.Show();
            }
        });

        for (int i = 0; i < _tabs.Length; i++)
        {
            if (_tabs[i] is GameSettingsViewControlTab controlTab)
            {
                controlTab.VibrationIsOn += () =>
                {
                    _inputDispatcher.VibrateAll();
                };
            }

            if (_tabs[i] is GameSettingsViewGeneralTab generalTab)
            {
                generalTab.CreditsRequested += () => CreditsRequested?.Invoke();
                generalTab.DeleteSavesRequested += () => DeleteAllSavedRequested?.Invoke();

                generalTab.SetupSavesCountReact(savesCount);
            }
        }

        _isInitialized = true;
    }

    public void Show()
    {
        _prevSelectedItem = null;
        _selectedItem.Value = null;
        _currentTab = null;

        for (int i = 0; i < _tabs.Length; i++)
        {
            _tabs[i].AssociatedButton.SetTabActive(false);
        }

        SetTab(0);

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        _selectedItem.Value?.ControlGuide?.Hide();
        _prevSelectedItem?.ControlGuide?.Hide();

        _currentTab?.Hide();

        _selectedItem.Value = null;
        _prevSelectedItem = null;
        _currentTab = null;

        gameObject.SetActive(false);
    }

    public void HandleItemSelect(GameSettingsViewItem selectedItem)
    {
        if (selectedItem != _selectedItem.Value)
        {
            _prevSelectedItem = _selectedItem.Value;
            _selectedItem.Value = selectedItem;
        }
    }

    public void HandleItemDeselect(GameSettingsViewItem deselectedItem)
    {
        if (deselectedItem == _selectedItem.Value)
        {
            _prevSelectedItem = deselectedItem;
            _selectedItem.Value = null;
        }
    }

    private void Update()
    {
        if (_inputDispatcher.UIInput.UIInputProvider.IsToLeftTabButtonDown)
        {
            AudioPlayer.Instance?.PlayPrevTab();

            SetTab(_currentTabIndex - 1);
        }
        else if (_inputDispatcher.UIInput.UIInputProvider.IsToRightTabButtonDown)
        {
            AudioPlayer.Instance?.PlayNextTab();

            SetTab(_currentTabIndex + 1);
        }
        else if (_inputDispatcher.UIInput.UIInputProvider.IsResetToDefaultButtonDown)
        {
            _model.ResetToDefault();
        }
        else if (_inputDispatcher.UIInput.UIInputProvider.IsBackButtonDown)
        {
            AudioPlayer.Instance?.PlayNavigationBack();

            BackToMainMenuRequested?.Invoke();
        }
    }

    private void SetTab(int index)
    {
        if (_currentTab != null)
        {
            _currentTab.Hide();
        }

        if (index < 0)
        {
            index = _tabs.Length - 1;
        }

        index %= _tabs.Length;

        _currentTabIndex = index % _tabs.Length;
        _currentTab = _tabs[index];

        _currentTab.Show();

        for (int i = 0; i < _tabs.Length; i++)
        {
            var navigation = _tabs[i].AssociatedButton.navigation;

            navigation.selectOnDown = _currentTab.FirstObjectToSelect;

            _tabs[i].AssociatedButton.navigation = navigation;
        }

        Navigation nav = _currentTab.FirstObjectToSelect.navigation;
        nav.mode = Navigation.Mode.Explicit;
        nav.selectOnUp = _currentTab.AssociatedButton;

        nav.selectOnDown = _currentTab.FirstObjectToSelect.FindSelectableOnDown();
        nav.selectOnLeft = null;
        nav.selectOnRight = null;

        _currentTab.FirstObjectToSelect.navigation = nav;
    }
}
