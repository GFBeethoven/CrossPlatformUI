using Lean.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GameplayScreen : MonoBehaviour
{
    public event Action<GameplayScreen> OnTargetHit;
    public event Action<GameplayScreen> OnPauseRequested;

    [SerializeField] private Camera _camera;

    [SerializeField] private LeanLocalToken _scoreToken;

    [SerializeField] private Transform _crossTranform;
    [SerializeField] private Transform _gunTransform;

    [SerializeField] private SpriteRenderer _background;

    [SerializeField] private Transform _target;

    private Gun _gun;

    private InputSource _inputProvider;
    public InputSource InputProvider
    {
        get
        {
            return _inputProvider;
        }

        set
        {
            _inputProvider = value;

            _gun.GameplayInputProvider = _inputProvider.GameplayInputProvider;
        }
    }

    private bool _isSetuped = false;

    private Vector2 _currentViewportMin = Vector2.zero;
    private Vector2 _currentViewportMax = Vector2.one;

    public void Setup(GameSettings gameSettings)
    {
        if (_isSetuped) return;

        _gun = new Gun(_crossTranform, _gunTransform, gameSettings);

        _gun.OnShot += OnGunShot;

        _isSetuped = true;
    }

    private void OnGunShot(Vector2 crossPosition)
    {
        InputProvider?.TryVibrate();

        AudioPlayer.Instance?.PlayGunShot();

        if (Physics2D.OverlapPoint(crossPosition) != null)
        {
            AudioPlayer.Instance?.PlayTargetHit();

            OnTargetHit?.Invoke(this);
        }
    }

    public void TurnOn()
    {
        gameObject.SetActive(true);
    }

    public void TurnOff()
    {
        gameObject.SetActive(false);
    }

    public void SetupNewTarget(Vector2 relativePosition)
    {
        CalculateCurrentViewport(out Vector2 leftBottom, out Vector2 rightTop);

        Vector2 newPosition = leftBottom + (rightTop - leftBottom) * relativePosition;

        _target.position = newPosition;
    } 

    public void SetupScore(int score)
    {
        _scoreToken.SetValue(score);
    }

    public void SetViewport(Vector2 min, Vector2 max)
    {
        _currentViewportMin = min;
        _currentViewportMax = max;

        _camera.rect = new Rect(min, max - min);

        float aspectRatio = _camera.rect.width / _camera.rect.height;

        float screenAspectRatio = 1.0f * Screen.width / Screen.height;

        _background.size = new Vector2(screenAspectRatio * aspectRatio * _camera.orthographicSize * 2.0f, _camera.orthographicSize * 2.0f);
    }

    private void CalculateCurrentViewport(out Vector2 leftBottom, out Vector2 rightTop)
    {
        Vector2 fullLeftBottom = GameViewport.Viewport.Size.Value / -2.0f;
        Vector2 fullRightTop = GameViewport.Viewport.Size.Value / 2.0f;

        leftBottom = fullLeftBottom + (fullRightTop - fullLeftBottom) * _currentViewportMin;
        rightTop = fullLeftBottom + (fullRightTop - fullLeftBottom) * _currentViewportMax;
    }

    public void Tick()
    {
        _gun.Tick();

        if (InputProvider != null)
        {
            if (InputProvider.GameplayInputProvider.IsPauseRequested)
            {
                OnPauseRequested?.Invoke(this);
            }
        }
    }
}
