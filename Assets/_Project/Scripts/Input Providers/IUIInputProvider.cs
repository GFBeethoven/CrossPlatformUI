using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IUIInputProvider : IDisposable
{
    public bool IsAnyActionNow { get; }

    public bool IsAnyButtonDown { get; }

    public bool IsBackButtonDown { get; }

    public bool IsToLeftTabButtonDown { get; }

    public bool IsToRightTabButtonDown { get; }

    public bool IsConfirmButtonDown { get; }

    public bool IsResetToDefaultButtonDown { get; }

    public bool IsDeleteSaveButtonDown { get; }

    public bool SpeedUpCreditsButton { get; }
}
