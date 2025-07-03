using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSourceIcons
{
    public static InputSourceIcons Instance { get; private set; }

    private Dictionary<string, (Sprite, Sprite)> _icons;

    public Sprite KeyboardIcon { get; }
    public Sprite XboxControllerIcon { get; }
    public Sprite PlaystationControllerIcon { get; }

    public InputSourceIcons(ControlIcons data)
    {
        Instance = this;

        _icons = new Dictionary<string, (Sprite, Sprite)>();

        for (int i = 0; i < data.Icons.Length; i++)
        {
            _icons[data.Icons[i].ControlPath] = (data.Icons[i].Icon, data.Icons[i].PressedIcon);
        }
    }

    public Sprite GetInputSourceIcon(InputSource.SourceType type)
    {
        switch (type)
        {
            case InputSource.SourceType.KeyboardMouse: return KeyboardIcon;
            case InputSource.SourceType.XboxController: return XboxControllerIcon;
            case InputSource.SourceType.PsController: return PlaystationControllerIcon;
        }

        return null;
    }

    public (Sprite, Sprite) GetIconByControlPath(string controlPath)
    {
        if (_icons.TryGetValue(controlPath, out var icon))
        {
            return icon;
        }

        return (null, null);
    }
}
