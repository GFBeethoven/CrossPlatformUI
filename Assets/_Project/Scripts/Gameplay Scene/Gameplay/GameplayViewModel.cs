using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GameplayViewModel
{
    public event Action<int, Vector2> OnNewTarget;
    public event Action<int, int> OnNewScore;
    public event Action<int> OnGameStart;
    public event Action<InputSource> PauseRequested;

    private GameplayModel _model;

    public GameplayViewModel(GameplayModel model)
    {
        _model = model;

        _model.OnGameStart += GameStart;  

        for (int i = 0; i < GameplayModel.MaxPlayerCount; i++)
        {
            int index = i;

            _model.GetPlayerData(i).Score.Subscribe((v) => OnNewScore?.Invoke(index, v));
            _model.GetPlayerData(i).TargetPosition.Subscribe((v) => OnNewTarget?.Invoke(index, v));
        }
    }

    private void GameStart()
    {
        OnGameStart?.Invoke(_model.PlayerCount.Value);
    }

    public void PauseRequest(InputSource invoker)
    {
        PauseRequested?.Invoke(invoker);
    }

    public void HitTarget(int playerIndex)
    {
        _model.HitTarget(_model.GetPlayerData(playerIndex));
    }

    public Vector2 GetTargetRelativePosition(int playerIndex)
    {
        return _model.GetPlayerData(playerIndex)?.TargetPosition?.Value ?? Vector2.zero;
    }

    public int GetScore(int playerIndex)
    {
        return _model.GetPlayerData(playerIndex)?.Score?.Value ?? 0;
    }

    public void NewInputSourceRequired(int playerIndex, InputSource lostSource)
    {

    }

    public void NotEnoughInputSource(int requiredCount)
    {

    }
}
