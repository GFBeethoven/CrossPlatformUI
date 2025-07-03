using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CommonUIInputSourceTypeDependence : MonoBehaviour
{
    public abstract void InputTypeChanged(InputSource.SourceType type);
}
