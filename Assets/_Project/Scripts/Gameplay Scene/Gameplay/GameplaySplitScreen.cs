using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GameplaySplitScreen : MonoBehaviour
{
    private static readonly Vector4[,] ScreenRects = new Vector4[GameplayModel.MaxPlayerCount, GameplayModel.MaxPlayerCount]
    {
        {
            new Vector4(0.0f, 0.0f, 1.0f, 1.0f), Vector4.zero, Vector4.zero, Vector4.zero
        },
        {
            new Vector4(0.0f, 0.0f, 0.5f, 1.0f), new Vector4(0.5f, 0.0f, 1.0f, 1.0f), Vector4.zero, Vector4.zero
        },
        {
            new Vector4(0.0f, 0.0f, 1.0f / 3.0f, 1.0f), new Vector4(1.0f / 3.0f, 0.0f, 2.0f / 3.0f, 1.0f), new Vector4(2.0f / 3.0f, 0.0f, 1.0f, 1.0f), Vector4.zero
        },
        {
            new Vector4(0.0f, 0.5f, 0.5f, 1.0f), new Vector4(0.5f, 0.5f, 1.0f, 1.0f), new Vector4(0.0f, 0.0f, 0.5f, 0.5f), new Vector4(0.5f, 0.0f, 1.0f, 0.5f)
        }
    };

    [SerializeField] private GameplayScreen[] _screens;

    [SerializeField] private GameplaySplitScreenCanvas _canvas;

    private Dictionary<InputSource, GameplayScreen> _inputs;

    private GameplayViewModel _viewModel;

    private InputDispatcher _inputDispatcher;

    private int _requiredScreenCount = 0;

    private bool _isSetuped = false;

    public void Setup(GameplayViewModel viewModel, InputDispatcher inputDispatcher, GameSettings gameSettings)
    {
        if (_isSetuped) return;

        _inputs = new Dictionary<InputSource, GameplayScreen>();

        _inputDispatcher = inputDispatcher;

        _viewModel = viewModel;

        _viewModel.OnGameStart += OnGameStart;
        _viewModel.OnNewTarget += OnNewTarget;
        _viewModel.OnNewScore += OnNewScore;

        for (int i = 0; i < _screens.Length; i++)
        {
            _screens[i].Setup(gameSettings);

            _screens[i].OnPauseRequested += OnPauseRequested;
            _screens[i].OnTargetHit += OnTargetHit;
        }

        for (int i = 0; i < _inputDispatcher.PlayerInputSources.Count; i++)
        {
            if (_inputDispatcher.PlayerInputSources[i] == null) continue;

            _inputs.Add(_inputDispatcher.PlayerInputSources[i], null);
        }

        _inputDispatcher.PlayerInputSources.OnItemAdd += OnNewInputSourceAdd;
        _inputDispatcher.PlayerInputSources.OnItemRemove += OnInputSourceRemove;
        _inputDispatcher.PlayerInputSources.OnBeforeClear += OnAllInputSourceClear;

        _isSetuped = true;
    }

    public void Tick()
    {
        for (int i = 0; i < _requiredScreenCount; i++)
        {
            _screens[i].Tick();
        }
    }

    private void OnTargetHit(GameplayScreen screen)
    {
        _viewModel.HitTarget(GetScreenIndex(screen));
    }

    private void OnPauseRequested(GameplayScreen screen)
    {
        foreach (var pair in _inputs)
        {
            if (pair.Value == screen)
            {
                _viewModel.PauseRequest(pair.Key);
            }
        }
    }

    private void OnNewScore(int index, int score)
    {
        _screens[index].SetupScore(score);
    }

    private void OnNewTarget(int index, Vector2 target)
    {
        _screens[index].SetupNewTarget(target);
    }

    private void OnGameStart(int requiredCount)
    {
        _requiredScreenCount = requiredCount;

        _canvas.SetScreenCount(requiredCount);

        for (int i = 0; i < requiredCount; i++)
        {
            _screens[i].TurnOn();

            _screens[i].SetViewport(new Vector2(ScreenRects[requiredCount - 1, i].x, ScreenRects[requiredCount - 1, i].y),
                new Vector2(ScreenRects[requiredCount - 1, i].z, ScreenRects[requiredCount - 1, i].w));

            _screens[i].SetupScore(_viewModel.GetScore(i));

            _screens[i].SetupNewTarget(_viewModel.GetTargetRelativePosition(i));
        }

        for (int i = requiredCount; i < _screens.Length; i++)
        {
            _screens[i].TurnOff();  
        }

        if (!TryDispatchInputSourcesBetweenRequiredScreens())
        {
            _viewModel.NotEnoughInputSource(_requiredScreenCount);
        }
    }

    private bool TryDispatchInputSourcesBetweenRequiredScreens()
    {
        for (int i = 0; i < _requiredScreenCount; i++)
        {
            if (_screens[i].InputProvider == null)
            {
                bool foundNew = false;

                for (int j = _requiredScreenCount; j < _screens.Length; j++)
                {
                    if (_screens[j].InputProvider != null)
                    {
                        InputSource newInputSource = null;
                        foreach (var key in _inputs.Keys)
                        {
                            if (_inputs[key] == _screens[j])
                            {
                                newInputSource = key;
                                break;
                            }
                        }

                        if (newInputSource != null)
                        {
                            foundNew = true;
                            _inputs[newInputSource] = _screens[i];
                            _screens[i].InputProvider = newInputSource;
                            _screens[j].InputProvider = null;
                            break;
                        }
                    }
                }

                if (!foundNew)
                {
                    foreach (var key in _inputs.Keys)
                    {
                        if (_inputs[key] == null)
                        {
                            _inputs[key] = _screens[i];
                            _screens[i].InputProvider = key;
                            foundNew = true;
                            break;
                        }
                    }

                    if (!foundNew)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    private void OnNewInputSourceAdd(int index, InputSource inputSource)
    {
        if (inputSource == null) return;

        _inputs.Add(inputSource, null);
    }

    private void OnInputSourceRemove(int index, InputSource inputSource)
    {
        if (inputSource == null) return;

        if (_inputs[inputSource] != null)
        {
            _inputs[inputSource].InputProvider = null;

            _viewModel.NewInputSourceRequired(GetScreenIndex(_inputs[inputSource]), inputSource);
        }

        _inputs.Remove(inputSource);
    }

    private void OnAllInputSourceClear()
    {
        _inputs.Clear();
    }

    private int GetScreenIndex(GameplayScreen screen)
    {
        for (int i = 0; i < _screens.Length; i++)
        {
            if (_screens[i] == screen) return i;
        }

        return -1;
    }
}
