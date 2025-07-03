using Lean.Localization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettingsDescriptionPanel : MonoBehaviour
{
    [SerializeField] private LeanLocalizedTextMeshProUGUI _title;
    [SerializeField] private LeanLocalizedTextMeshProUGUI _description;

    public void SetDescription(GameSettingsViewItem item)
    {
        if (item == null)
        {
            _title.gameObject.SetActive(false);
            _description.gameObject.SetActive(false);
        }
        else
        {
            _title.gameObject.SetActive(true);
            _description.gameObject.SetActive(true);

            _title.TranslationName = item.DescriptionTitleKey;
            _description.TranslationName = item.DescriptionKey;    
        }
    }
}
