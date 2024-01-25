using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SmallLandCard : CardBase
{
    [SerializeField] private Button button;
    [SerializeField] private Image colorImage;
    [SerializeField] private Image Arrow;
    [SerializeField] private TextMeshProUGUI landName;
    [SerializeField] private TextMeshProUGUI initialValue;

    private Action<SmallLandCard,LandTitleInstance> _onClickedEvent;
    private LandTitleInstance _landTitleInstance;
    private bool _directionToLeft;

    private bool _hasActivated = false;

    public struct SmallLandCardInitHandle
    {

        public LandTitleInstance Land;
        public Action<SmallLandCard,LandTitleInstance>  OnClickedEvent;
        //True to left , false to right
        public bool DirectionToLeft;
    }
    public override void Init<T>(InitalizationHandle<T> initalizationHandle)
    {
        SmallLandCardInitHandle handle =(SmallLandCardInitHandle) (initalizationHandle.Data as object );
        Init(handle);
    }
    private void Init(SmallLandCardInitHandle handle)
    {
   
       
        // colorImage.color = LandStaticInformation.GetColorFromLandType(handle.Land.LandDef.LandType);
        colorImage.color =  MatchSettings.SingletonInstance.GetColorFromLandType(handle.Land.LandDef.LandType);
        this.landName.text = handle.Land.LandDef.LandName;
        this.initialValue.text = handle.Land.LandDef.LandValue.ToString();
        _landTitleInstance = handle.Land;
        _onClickedEvent = handle.OnClickedEvent;
        _directionToLeft = handle.DirectionToLeft;
        

        button.onClick.AddListener(OnClicked);
    }

    private void OnClicked()
    {
        _hasActivated = !_hasActivated;
        Arrow.enabled = _hasActivated;
        if (_hasActivated)
        {
            FlipArrow();
        }

        _onClickedEvent?.Invoke(this,_landTitleInstance);
    }

    private void FlipArrow()
    {
        Arrow.transform.rotation=Quaternion.identity;
        if (_directionToLeft)
        {
            Arrow.transform.Rotate(new Vector3(0, 0, 1), 90);
        }
        else
        {
            Arrow.transform.Rotate(new Vector3(0, 0, 1), -90);
        }
    }

 
}