using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GameplaySplitScreenCanvas : MonoBehaviour
{
    [SerializeField] private GameObject[] _splitterGroups;

    public void SetScreenCount(int count)
    {
        for (int i = 0; i < _splitterGroups.Length; i++)
        {
            _splitterGroups[i].SetActive(false);
        }

        if (count < 0 || count >= _splitterGroups.Length)
        {
            count = 1;
        }

        _splitterGroups[count - 1].SetActive(true);
    }
}
