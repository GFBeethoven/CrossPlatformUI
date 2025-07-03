using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MainMenuToGameplayExitData : StateExitData
{
    public GameplayStateData GameplayStateData { get; }

    public MainMenuToGameplayExitData(GameplayStateData gameplayStateData)
    {
        GameplayStateData = gameplayStateData;
    }
}
