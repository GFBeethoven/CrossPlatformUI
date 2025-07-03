using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class GameplayEnterData : StateEnterData
{
    public GameplayStateData State { get; }

    public GameplayEnterData(GameplayStateData state)
    {
        State = state;
    }
}
