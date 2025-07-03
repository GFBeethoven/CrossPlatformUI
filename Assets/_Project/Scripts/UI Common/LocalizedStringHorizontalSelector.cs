using Lean.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LocalizedStringHorizontalSelector : Selectable
{
    private event Action<int> OnOptionChanged;

    [SerializeField] private string[] _optionsKey;

    [SerializeField] private LeanLocalizedTextMeshProUGUI _localizedTMP;

    public int Option
    {
        get
        {
            return _currentOption.Value;
        }
    }
    
    private ReactiveProperty<int> _currentOption = new ReactiveProperty<int>(0);

    private bool _withoutNotify;

    protected override void Awake()
    {
        base.Awake();

        _withoutNotify = true;

        _currentOption.Subscribe((v) => UpdateOptionView());
        _currentOption.Subscribe((v) =>
        {
            if (_withoutNotify) return;

            OnOptionChanged?.Invoke(v);
        });

        _withoutNotify = false;
    }

    public void SubscribeOnValueChanged(Action<int> onValueChanged)
    {
        if (onValueChanged == null) return;

        OnOptionChanged += onValueChanged;

        //onValueChanged.Invoke(_currentOption.Value);
    }

    public void SetOption(int option)
    {
        _currentOption.Value = option % _optionsKey.Length;
    }

    public void SetOptionWithoutNotify(int option)
    {
        _withoutNotify = true;

        SetOption(option);

        _withoutNotify = false;
    }

    public void NextOption()
    {
        AddToCurrentOption(1);
    }

    public void PreviousOption()
    {
        AddToCurrentOption(-1);
    }

    public override void OnMove(AxisEventData eventData)
    {
        if (EventSystem.current.currentSelectedGameObject != this.gameObject)
            return;

        if (eventData.moveDir == MoveDirection.Left)
        {
            AddToCurrentOption(-1);
            eventData.Use();
        }
        else if (eventData.moveDir == MoveDirection.Right)
        {
            AddToCurrentOption(1);
            eventData.Use();
        }
        else
        {
            base.OnMove(eventData);
        }
    }

    private void AddToCurrentOption(int add)
    {
        int option = _currentOption.Value + add;

        if (option < 0)
        {
            option = _optionsKey.Length - 1;
        }

        option %= _optionsKey.Length;

        _currentOption.Value = option;
    }

    private void UpdateOptionView()
    {
        if (_currentOption.Value < 0 || _currentOption.Value >= _optionsKey.Length) return;

        _localizedTMP.TranslationName = _optionsKey[_currentOption.Value];
    }
}
