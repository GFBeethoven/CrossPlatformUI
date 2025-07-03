using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface IGameplayInputProvider : IDisposable
{
    public Vector2 ControlAxis { get; }

    public bool IsShootingUp { get; }

    public bool IsShootingDown { get; }

    public bool IsPauseRequested { get; }
}
