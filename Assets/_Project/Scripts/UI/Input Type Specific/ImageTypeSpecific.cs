using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageTypeSpecific : CommonUIInputSourceTypeDependence
{
    [SerializeField] private Sprite _keyboardImage;
    [SerializeField] private Sprite _xboxImage;
    [SerializeField] private Sprite _psImage;

    private Image _image;
    private Image Image
    {
        get
        {
            if (_image != null) return _image;

            _image = GetComponent<Image>();

            return _image;
        }
    }

    public override void InputTypeChanged(InputSource.SourceType type)
    {
        switch (type)
        {
            case InputSource.SourceType.KeyboardMouse:
                Image.sprite = _keyboardImage;
                break;
            case InputSource.SourceType.XboxController:
                Image.sprite = _xboxImage;
                break;
            case InputSource.SourceType.PsController:
                Image.sprite = _psImage;
                break;
        }
    }
}
