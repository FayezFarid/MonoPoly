using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class Multi_InteractionButton : MonoBehaviour
{
    //from all the miserable ways I had to choose this.
    [System.Serializable]
    public class ButtonChangeRefs
    {
        public GameObject RenderHolders;
        public TMPro.TextMeshProUGUI Text;
        public bool Active;
        

        public void ToggleActive(bool toState)
        {
            Active = toState;
            RenderHolders.SetActive(toState);
            Text.gameObject.SetActive(toState);
        }
    }
   [SerializeField] private Button _button;
   [FormerlySerializedAs("BlueMode")] [SerializeField] private ButtonChangeRefs RedMode;
   [SerializeField] private ButtonChangeRefs GreenMode;

   private UnityAction _onClickBlue;
   private UnityAction _onClickGreen;
   
    void Start()
    {
        if (_button == null)
            _button.GetComponent<Button>();
        RedMode.Active = RedMode.RenderHolders.activeInHierarchy;
        GreenMode.Active = GreenMode.RenderHolders.activeInHierarchy;
    }

    public void Add_SingleActionGreen(UnityAction onClick,string buttonText="")
    {
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(onClick);
        RedMode.ToggleActive(false);
        GreenMode.ToggleActive(true);
        if (!string.IsNullOrEmpty(buttonText))
            GreenMode.Text.text = buttonText;

    }
    public void Add_SingleAction_RED_Switchable(UnityAction onClick,string buttonText="")
    {
        _onClickBlue = onClick;
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(ChangeActions);
        RedMode.ToggleActive(true);
        GreenMode.ToggleActive(false);
    
        if (!string.IsNullOrEmpty(buttonText))
            GreenMode.Text.text = buttonText;

    }
    public void Add_SingleAction_Green_Switchable(UnityAction onClick,string buttonText="")
    {
        _onClickBlue = onClick;
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(ChangeActions);
        RedMode.ToggleActive(false);
        GreenMode.ToggleActive(true);
        
        if (!string.IsNullOrEmpty(buttonText))
            GreenMode.Text.text = buttonText;

    }

    public void Add_SingleActionRed(UnityAction onClick)
    {
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(onClick);
        RedMode.ToggleActive(true);
        GreenMode.ToggleActive(false);
        
    }
    // public void Add_SwitchableAction(Action onClickBlue,Action onClickGreen)
    // {
    //     _onClickBlue = onClickBlue;
    //     _onClickGreen = onClickGreen;
    //     _button.onClick.AddListener(ChangeActions);
    // }

    
    private void ChangeActions()
    {
        if (GreenMode.Active)
        {
            GreenMode.ToggleActive(false);
            RedMode.ToggleActive(true);
            
        }
        else
        {
            GreenMode.ToggleActive(true);
            RedMode.ToggleActive(false);
        }
        _onClickBlue?.Invoke();

    }

    private void OnDisable()
    {
        _button.onClick.RemoveAllListeners();
    }
}
