using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameSettingsViewTabButton : Button
{ 
    private Selectable _selectable;
    private Selectable Selectable
    {
        get
        {
            if (_selectable == null)
            {
                _selectable = gameObject.GetComponent<Selectable>();
            }

            return _selectable;
        }
    }

    private Color _tabActiveDeselectedColor;
    private Color _tabNotActiveDeselectedColor;

    public void SetupColors(Color tabActiveDeselectedColor, Color tabNotActiveDeselectedColor)
    {
        _tabActiveDeselectedColor = tabActiveDeselectedColor;

        _tabNotActiveDeselectedColor = tabNotActiveDeselectedColor;
    }

    public void SetTabActive(bool active)
    {
        ColorBlock block = Selectable.colors;

        block.normalColor = active ? _tabActiveDeselectedColor : _tabNotActiveDeselectedColor;

        Selectable.colors = block;
    }
}
