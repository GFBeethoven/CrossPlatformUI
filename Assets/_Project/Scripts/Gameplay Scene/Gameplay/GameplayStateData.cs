using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class GameplayStateData
{
    public GameDifficult Difficult;

    public PlayerData[] Data;

    public string CreationDateTime;

    public static GameplayStateData GetNewGameState(GameDifficult difficult, int playerCount)
    {
        playerCount = Mathf.Min(playerCount, GameplayModel.MaxPlayerCount);

        GameplayStateData data = new GameplayStateData();
        data.Difficult = difficult;
        data.Data = new PlayerData[playerCount];
        for (int i = 0; i < playerCount; i++)
        {
            data.Data[i] = new PlayerData()
            {
                Score = 0,
                CurrentTargetPosition = new Vector2(0.5f, 0.5f)
            };
        }
        data.CreationDateTime = DateTime.Now.ToString();

        return data;
    }

    public GameplayStateData Copy()
    {
        GameplayStateData newCopy = new GameplayStateData();

        newCopy.Difficult = Difficult;
        newCopy.CreationDateTime = CreationDateTime;

        newCopy.Data = new PlayerData[Data.Length];

        Array.Copy(Data, newCopy.Data, Data.Length);

        return newCopy;
    }

    [Serializable]
    public class PlayerData
    {
        public int Score;

        public Vector2 CurrentTargetPosition;
    }
}
