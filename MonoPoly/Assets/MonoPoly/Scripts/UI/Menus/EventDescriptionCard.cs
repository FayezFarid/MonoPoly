using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EventDescriptionCard : MenuBase
{
    [SerializeField] private TextMeshProUGUI textMesh;

    public struct EventDescriptionCardInitHandle
    {
        public string Description;
    }

    public override void Init<T>(InitalizationHandle<T> initalizationHandle)
    {
        gameObject.SetActive(true);
        EventDescriptionCardInitHandle handle = (EventDescriptionCardInitHandle)(initalizationHandle.Data as object);
        textMesh.text = handle.Description;
    }

    public override void CloseWindow()
    {
        gameObject.SetActive(false);
    }
}