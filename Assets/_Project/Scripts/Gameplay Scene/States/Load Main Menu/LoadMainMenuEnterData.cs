using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class LoadMainMenuEnterData : StateEnterData
{
    public string ThumbnailPath { get; }

    public GameplayStateData GameplayState { get; }

    public LoadMainMenuEnterData(GameplayStateData gameplayState, string thumbnailPath)
    {
        GameplayState = gameplayState;
        ThumbnailPath = thumbnailPath;
    }
}
