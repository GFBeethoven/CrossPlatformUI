using MEC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameViewport : MonoBehaviour
{
    public static GameViewport Viewport { get; private set; }

    public ReadOnlyReactiveProperty<Vector2> Size => _size;
    private ReactiveProperty<Vector2> _size;

    public ReadOnlyReactiveProperty<float> Aspect => _aspect;
    private ReactiveProperty<float> _aspect;

    private Camera _cachedCamera;
    private Camera Camera
    {
        get
        {
            if (_cachedCamera == null)
            {
                _cachedCamera = Camera.main;
            }

            return _cachedCamera;
        }
    }

    private bool _isInitialized = false;

    public void Initialize()
    {
        if (_isInitialized) return;

        Viewport = this;

        if (Camera == null)
        {
            _size = new ReactiveProperty<Vector2>(Vector2.zero);

            _aspect = new ReactiveProperty<float>(0.0f);
        }
        else
        {
            _size = new ReactiveProperty<Vector2>(new Vector2(Camera.orthographicSize * 2.0f * Camera.aspect,
                Camera.orthographicSize * 2.0f));

            _aspect = new ReactiveProperty<float>(Camera.aspect);
        }

        SceneManager.activeSceneChanged += ActiveSceneChanged;

        Timing.RunCoroutine(_CheckAspectChange());

        _isInitialized = true;
    }

    private void ActiveSceneChanged(Scene current, Scene next)
    {
        _cachedCamera = Camera.main;
    }

    private IEnumerator<float> _CheckAspectChange()
    {
#if UNITY_EDITOR
        const float Period = 0.02f;
#else
        const float Period = 0.25f;
#endif

        while (true)
        {
            yield return Timing.WaitForSeconds(Period);

            if (Camera == null) continue;

            if (_aspect.Value != Camera.aspect)
            {
                _size.Value = new Vector2(Camera.orthographicSize * 2.0f * Camera.aspect, Camera.orthographicSize * 2.0f);

                _aspect.Value = Camera.aspect;
            }
        }
    }
}
