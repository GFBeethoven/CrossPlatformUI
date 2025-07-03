using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class GameplayModel
{
    public const int MaxPlayerCount = 4;

    public event Action OnGameStart;
    public event Action<PlayerData> OnScoreChanged;
    public event Action<PlayerData> OnTargetPositionChanged;

    private readonly PlayerData[] _playerData = new PlayerData[MaxPlayerCount];
    private readonly IPlayerDataSetuper[] _playerDataSetupers = new IPlayerDataSetuper[MaxPlayerCount];

    public GameDifficult GameDifficult
    {
        get
        {
            return _data.Difficult;
        }
    }

    public ReadOnlyReactiveProperty<int> PlayerCount => _playerCount;
    private ReactiveProperty<int> _playerCount;

    private GameplayStateData _data;

    public GameplayModel()
    {
        for (int i = 0; i < _playerData.Length; i++)
        {
            _playerData[i] = new PlayerData(this, i);
            _playerDataSetupers[i] = _playerData[i];
        }

        _playerCount = new ReactiveProperty<int>(0);
    }

    public GameplayStateData GetGameplayStateCopy()
    {
        return _data.Copy();
    }

    public PlayerData GetPlayerData(int index)
    {
        if (index < 0 || index >= _playerData.Length)
        {
            return null;
        }

        return _playerData[index];
    }

    public void Setup(GameplayStateData data)
    {
        if (data == null) return;

        _data = data;

        for (int i = 0; i < _playerData.Length; i++)
        {
            _playerData[i].TryUpdateValue();
        }

        _playerCount.Value = _data.Data.Length;

        for (int i = 0; i < _playerData.Length; i++)
        {
            SetupTimeToHit(_playerData[i]);
        }

        OnGameStart?.Invoke();
    }

    public void HitTarget(PlayerData data)
    {
        AddScore(data, UnityEngine.Random.Range(1, 3));

        SetupNewTargetPosition(data);

        SetupTimeToHit(data);
    }

    public void Tick()
    {
        for (int i = 0; i < _playerCount.Value; i++)
        {
            _playerDataSetupers[i].SetTimeToHit(_playerData[i].TimeToHit - Time.deltaTime);

            if (_playerData[i].TimeToHit <= 0.0f)
            {
                SetupNewTargetPosition(_playerData[i]);
                SetupTimeToHit(_playerData[i]);
            }
        }
    }

    private void AddScore(PlayerData data, int score)
    {
        if (data == null) return;

        IPlayerDataSetuper writeData = _playerDataSetupers[data.Index];

        if (score < 0) score = 0;

        writeData.SetScore(data.Score.Value + score);
    }
    
    private void HandleScoreChange(PlayerData data)
    {
        OnScoreChanged?.Invoke(data);
    }

    private void HandleTargetPositionChange(PlayerData data)
    {
        OnTargetPositionChanged?.Invoke(data);
    }

    private void SetupNewTargetPosition(PlayerData data)
    {
        if (data == null) return;

        IPlayerDataSetuper writeData = _playerDataSetupers[data.Index];

        float x = UnityEngine.Random.Range(0.0f, 1.0f);
        float y = UnityEngine.Random.Range(0.0f, 1.0f);

        writeData.SetTargetPosition(new Vector2(x, y));
    }

    private void SetupTimeToHit(PlayerData data)
    {
        if (data == null) return;

        IPlayerDataSetuper writeData = _playerDataSetupers[data.Index];

        switch (GameDifficult)
        {
            case GameDifficult.Easy:
                writeData.SetTimeToHit(10.0f);
                break;
            case GameDifficult.Medium:
                writeData.SetTimeToHit(7.0f);
                break;
            case GameDifficult.Hard:
                writeData.SetTimeToHit(4.0f);
                break;
        }
    }

    public class PlayerData : IPlayerDataSetuper
    {
        private GameplayModel _model;
        private int _index;

        public int Index => _index;

        public ReadOnlyReactiveProperty<int> Score => _score;
        private ReactiveProperty<int> _score;

        private ReactiveProperty<Vector2> _targetPosition;
        public ReadOnlyReactiveProperty<Vector2> TargetPosition => _targetPosition;

        private float _timeToHit;
        public float TimeToHit => _timeToHit;

        public PlayerData(GameplayModel model, int index)
        {
            _model = model;
            _index = index;

            GameplayStateData.PlayerData data = GetModelData();

            if (data == null)
            {
                _score = new ReactiveProperty<int>(0);
                _targetPosition = new ReactiveProperty<Vector2>(Vector2.zero);
            }
            else
            {
                _score = new ReactiveProperty<int>(data.Score);
                _targetPosition = new ReactiveProperty<Vector2>(data.CurrentTargetPosition);
            }

            _score.Subscribe(OnScoreChanged);
            _targetPosition.Subscribe(OnTargetPositionChanged);
        }

        public void TryUpdateValue()
        {
            var data = GetModelData();

            if (data != null)
            {
                _score.Value = data.Score;
                _targetPosition.Value = data.CurrentTargetPosition;
            }
        }

        private void OnScoreChanged(int score)
        {
            var data = GetModelData();

            if (data != null)
            {
                data.Score = score;
            }

            _model.HandleScoreChange(this);
        }

        private void OnTargetPositionChanged(Vector2 targetPosition)
        {
            var data = GetModelData();

            if (data != null)
            {
                data.CurrentTargetPosition = targetPosition;
            }

            _model.HandleTargetPositionChange(this);
        }

        private GameplayStateData.PlayerData GetModelData()
        {
            if (_model._data != null &&
                _model._data.Data != null &&
                _index < _model._data.Data.Length)
            {
                return _model._data.Data[_index];
            }

            return null;
        }

        void IPlayerDataSetuper.SetScore(int score)
        {
            _score.Value = score;
        }

        void IPlayerDataSetuper.SetTargetPosition(Vector2 targetPosition)
        {
            _targetPosition.Value = targetPosition;
        }

        void IPlayerDataSetuper.SetTimeToHit(float timeToHit)
        {
            _timeToHit = timeToHit;
        }
    }

    private interface IPlayerDataSetuper
    {
        public void SetScore(int score);

        public void SetTargetPosition(Vector2 targetPosition);  

        public void SetTimeToHit(float timeToHit);
    }
}
