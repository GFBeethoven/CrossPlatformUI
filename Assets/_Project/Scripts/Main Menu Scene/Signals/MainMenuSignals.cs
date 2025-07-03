using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToMainMenuSignal : Signal { public readonly static ToMainMenuSignal Empty = new ToMainMenuSignal(); }

public class ToSaveMenuSignal : Signal { public readonly static ToSaveMenuSignal Empty = new ToSaveMenuSignal(); }

public class ToLoadMenuSignal : Signal { public readonly static ToLoadMenuSignal Empty = new ToLoadMenuSignal(); }

public class ToCreditsSignal : Signal { public readonly static ToCreditsSignal Empty = new ToCreditsSignal(); }

public class ToSettingsSignal : Signal { public readonly static ToSettingsSignal Empty = new ToSettingsSignal(); }

public class ToGameplaySignal : Signal
{
    public GameplayStateData GameplayState { get; }

    public ToGameplaySignal(GameplayStateData gameplayState)
    {
        GameplayState = gameplayState;
    }
}