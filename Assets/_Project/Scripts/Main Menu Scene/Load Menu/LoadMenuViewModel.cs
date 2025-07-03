using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LoadMenuViewModel
{
    public event Action<GameplayStateData> OnLoadStateRequsted;
    public event Action OnMainMenuButtonClicked;

    public ReadOnlyReactiveList<ViewData> AllViewData { get; }
    private ReactiveList<ViewData> _viewData = new();

    private Dictionary<GameplayStateData, ViewData> _gameplayStateViewDataMap = new();

    private LoadMenuModel _model;

    public LoadMenuViewModel(LoadMenuModel model)
    {
        _model = model;

        AllViewData = new ReadOnlyReactiveList<ViewData>(_viewData);

        _model.SavedStates.OnItemAdd += OnNewStateAdd;
        _model.SavedStates.OnItemRemove += OnStateRemove;
        _model.SavedStates.OnBeforeClear += OnBeforeStatesClear;
    }

    public void DeleteRequested(ViewData selectedView)
    {
        GameplayStateData stateData = null;
        foreach (var pair in _gameplayStateViewDataMap)
        {
            if (pair.Value == selectedView)
            {
                stateData = pair.Key;

                break;
            }
        }

        if (stateData != null)
        {
            _model.DeleteState(stateData);
        }
    }

    public void MainMenuRequested()
    {
        OnMainMenuButtonClicked?.Invoke();
    }

    public void LoadRequested(ViewData selectedView)
    {
        foreach (var pair in _gameplayStateViewDataMap)
        {
            if (pair.Value == selectedView)
            {
                OnLoadStateRequsted?.Invoke(pair.Key);
            }
        }
    }

    private void OnNewStateAdd(int index, (GameplayStateData, Texture2D) state)
    {
        if (state.Item1 == null) return;

        var viewData = new ViewData(state.Item2, DateTime.Parse(state.Item1.CreationDateTime), GetBestScore(state.Item1.Data));

        _viewData.Add(viewData);

        _gameplayStateViewDataMap[state.Item1] = viewData;
    }

    private void OnStateRemove(int index, (GameplayStateData, Texture2D) state)
    {
        if (state.Item1 == null) return;

        if (_gameplayStateViewDataMap.TryGetValue(state.Item1, out ViewData viewData))
        {
            _viewData.Remove(viewData);
            
            _gameplayStateViewDataMap.Remove(state.Item1);
        }
    }

    private void OnBeforeStatesClear()
    {
        _gameplayStateViewDataMap.Clear();

        _viewData.Clear();
    }

    private static int GetBestScore(GameplayStateData.PlayerData[] data)
    {
        if (data == null || data.Length == 0) return 0;

        int score = data[0].Score;

        for (int i = 1; i < data.Length; i++)
        {
            if (score < data[i].Score)
            {
                score = data[i].Score;
            }
        }

        return score;
    }

    public class ViewData
    {
        public Texture2D Thumbnail { get; }
        public int BestScore { get; }
        public DateTime CreationTime { get; }

        public ViewData(Texture2D thumbnail, DateTime creationTime, int bestScore)
        {
            Thumbnail = thumbnail;
            CreationTime = creationTime;
            BestScore = bestScore;
        }   
    }
}
