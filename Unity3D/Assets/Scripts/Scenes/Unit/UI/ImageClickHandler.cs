using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class ImageClickHandler : MonoBehaviour, IPointerClickHandler
{
    [Inject]
    SignalBus _signalBus;

    [Inject]
    IUnitFSM _unitFSM;

    public RectTransform rtMain;

    public void OnPointerClick (PointerEventData eventData) {
        bool reset = false;
        Vector2 mousePos = new Vector2();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rtMain, Input.mousePosition, Camera.main, out mousePos);
        _unitFSM.ClickFSMHandler(mousePos, eventData, reset);
    }
}
