using DI;
using Lean.Localization;
using MEC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class LoadGameSettingsOperation : ILoadingOperation
{
    public LoadingStatus Status { get; private set; } = LoadingStatus.Running;

    public float Progress { get; private set; } = 0.0f;

    public string DescriptionKey => "LoadGameSettings";

    public string FailureMessageKey { get; private set; } = null;

    private DIContainer _container;

    public LoadGameSettingsOperation(DIContainer projectContainer)
    {
        _container = projectContainer;
    }

    public void Launch()
    {
        SettingsData data = _container.Resolve<ISaveLoadSystem>().LoadGameSettings();
        GameSettings settings = new GameSettings(data, _container.Resolve<LeanLocalization>(),
            _container.Resolve<AudioMixer>(), _container.Resolve<ISaveLoadSystem>());

        _container.RegisterInstance(settings);

        Status = LoadingStatus.Success;

        Progress = 1.0f;
    }
}
