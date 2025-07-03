using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Control Icons", menuName = "_Project/Control Icons")]
public class ControlIcons : ScriptableObject
{
    [field: SerializeField] public IconEntry[] Icons { get; private set; }

    [field: SerializeField] public Sprite KeyboardIcon { get; private set; }
    [field: SerializeField] public Sprite XboxControllerIcon { get; private set; }
    [field: SerializeField] public Sprite PlaystationControllerIcon { get; private set; }

    [System.Serializable]
    public class IconEntry
    {
        public string ControlPath;
        public Sprite Icon;
        public Sprite PressedIcon;
    }
}
