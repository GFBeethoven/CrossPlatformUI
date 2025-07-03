using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JsonSaveLoadSystem : ISaveLoadSystem
{
    private const string GameplayStatesFolderName = "Saves";
    private const string GameSettingsFileName = "SettingsData.json";

    public ReadOnlyReactiveProperty<int> SavedStateCount => _savedStateCount;

    private List<(GameplayStateData, Texture2D)> _savedStatesWithThumbnails = new List<(GameplayStateData, Texture2D)>();

    private Dictionary<GameplayStateData, string> _savedStatesPathes = new Dictionary<GameplayStateData, string>();

    private ReactiveProperty<int> _savedStateCount = new ReactiveProperty<int>(0);

    public void SaveGameplayState(GameplayStateData gameplayStateData, string thumbnailPath = null)
    {
        string existedPath = null;

        if (_savedStatesPathes.TryGetValue(gameplayStateData, out string val))
        {
            existedPath = val;
        }

        gameplayStateData.CreationDateTime = DateTime.Now.ToString();

        string pathToFolder = Path.Combine(Application.persistentDataPath, GameplayStatesFolderName);

        if (Directory.Exists(pathToFolder) == false)
        {
            Directory.CreateDirectory(pathToFolder);
        }

        if (existedPath == null)
        {
            string[] fileNames = Directory.GetFiles(pathToFolder);

            int i = 0;
            while (i < 1000000)
            {
                string newPath = Path.Combine(pathToFolder, $"{i}.json");

                bool contains = false;
                foreach (string file in fileNames)
                {
                    if (newPath == file)
                    {
                        contains = true;
                        break;
                    }
                }

                if (!contains)
                {
                    existedPath = newPath;
                    break;
                }

                i++;
            }

            if (existedPath == null)
            {
                throw new ApplicationException("Too many saves");
            }

            _savedStatesPathes[gameplayStateData] = existedPath;

            Texture2D thumbnail = null;

            if (thumbnailPath != null && File.Exists(thumbnailPath))
            {
                Texture2D texture = new Texture2D(2, 2);

                if (texture.LoadImage(File.ReadAllBytes(thumbnailPath)))
                {
                    thumbnail = texture;
                }
            }

            _savedStatesWithThumbnails.Add((gameplayStateData, thumbnail));
        }
        else
        {
            Texture2D thumbnail = null;

            if (thumbnailPath != null && File.Exists(thumbnailPath))
            {
                Texture2D texture = new Texture2D(2, 2);

                if (texture.LoadImage(File.ReadAllBytes(thumbnailPath)))
                {
                    thumbnail = texture;
                }
            }

            if (thumbnail != null)
            {
                for (int i = 0; i < _savedStatesWithThumbnails.Count; i++)
                {
                    if (_savedStatesWithThumbnails[i].Item1 == gameplayStateData)
                    {
                        _savedStatesWithThumbnails[i] = (gameplayStateData, thumbnail);
                        break;
                    }
                }
            }
        }

        File.WriteAllText(existedPath, JsonUtility.ToJson(gameplayStateData));

        if (thumbnailPath != null && File.Exists(thumbnailPath))
        {
            File.Move(thumbnailPath, existedPath + ".png");
        }

        _savedStateCount.Value = _savedStatesWithThumbnails.Count;
    }

    public void DeleteGameplayState(GameplayStateData gameplayStateData)
    {
        string fileName = null;

        if (_savedStatesPathes.TryGetValue(gameplayStateData, out string fileVal))
        {
            fileName = fileVal;
        }

        if (fileName != null)
        {
            File.Delete(fileName);

            if (File.Exists(fileName + ".png"))
            {
                File.Delete(fileName + ".png");
            }
            
            _savedStatesPathes.Remove(gameplayStateData);
        }

        int index = -1;

        for (int i = 0; i < _savedStatesWithThumbnails.Count; i++)
        {
            if (_savedStatesWithThumbnails[i].Item1 == gameplayStateData)
            {
                index = i;

                break;
            }
        }

        if (index != -1)
        {
            _savedStatesWithThumbnails.RemoveAt(index);
        }

        _savedStateCount.Value = _savedStatesWithThumbnails.Count;
    }

    public void DeleteAllGameplayStates()
    {
        while (_savedStatesWithThumbnails.Count > 0)
        {
            DeleteGameplayState(_savedStatesWithThumbnails[0].Item1);
        }
    }

    public (GameplayStateData, Texture2D)[] GetAllSavedGameplayStates()
    {
        if (_savedStatesWithThumbnails != null && _savedStatesWithThumbnails.Count > 0)
        {
            return _savedStatesWithThumbnails.ToArray();
        }

        string path = Path.Combine(Application.persistentDataPath, GameplayStatesFolderName);

        if (Directory.Exists(path))
        {
            string[] files = Directory.GetFiles(path);

            var resultList = new List<(GameplayStateData, Texture2D)>();

            for (int i = 0; i < files.Length; i++)
            {
                if (Path.GetExtension(files[i]) != ".json") continue;             

                string json = File.ReadAllText(files[i]);

                (GameplayStateData, Texture2D) current = (JsonUtility.FromJson<GameplayStateData>(json), null);

                _savedStatesPathes[current.Item1] = files[i];

                if (File.Exists(files[i] + ".png"))
                {
                    Texture2D thumb = new Texture2D(2, 2);
                    if (thumb.LoadImage(File.ReadAllBytes(files[i] + ".png")))
                    {
                        current.Item2 = thumb;
                    }
                }

                resultList.Add(current);
                _savedStatesWithThumbnails.Add(current);
            }

            _savedStateCount.Value = _savedStatesWithThumbnails.Count;

            return resultList.ToArray();
        }
        else
        {
            _savedStateCount.Value = 0;

            return new (GameplayStateData, Texture2D)[0];
        }
    }

    public void SaveGameSettings(SettingsData settingsData)
    {
        string path = Path.Combine(Application.persistentDataPath, GameSettingsFileName);

        File.WriteAllText(path, JsonUtility.ToJson(settingsData));
    }

    public SettingsData LoadGameSettings()
    {
        string path = Path.Combine(Application.persistentDataPath, GameSettingsFileName);

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);

            return JsonUtility.FromJson<SettingsData>(json);
        }
        else
        {
            SaveGameSettings(SettingsData.Default());

            return SettingsData.Default();
        }
    }
}
