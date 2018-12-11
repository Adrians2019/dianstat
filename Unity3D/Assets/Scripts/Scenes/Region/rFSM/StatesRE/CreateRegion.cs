using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class CreateRegion : StateEntity
{
    [Inject]
    IRegionCtrl _regionCtrl;

    [Inject]
    IMarkersFSM _markersFSM;

    [Inject]
    IRegionProvider _regionProvider;

    [Inject]
    IImgProvider _imgProvider;

    public override void Start()
    {
        _regionProvider.NewDraft("");// to new Region()
        _markersFSM.FullFSMReset();
        RegionCtrlConfig();
    }

    public override void MarkerClickHandler(Vector2 mousePos, PointerEventData eventData, bool lCtrlHold, bool lAltHold, bool reset = false)
    {
        // if Origin point is chosen by user and creationRegion, second point could
        // be lower vertex of the left-top_right-btm diagonal of the square
            if (_markersFSM.GetCurMarkerState() == MarkersFSM.MarkerState.MARKER_START
            && eventData.button.Equals(PointerEventData.InputButton.Left))
            if (_markersFSM.GetOrigin().x >= mousePos.x || _markersFSM.GetOrigin().y < mousePos.y) return;

        _markersFSM.MarkerClickHandler(mousePos, eventData);
    }

    public override void OnSave()
    {
        if (_markersFSM.GetCurMarkerState() != MarkersFSM.MarkerState.MARKER_FINAL)
            return;
        _regionCtrl.NewRegionSavingDiag(true);
    }

    public override void OnSaveConfirmed() {
        _regionProvider.OverwriteDraftDiagonal(_markersFSM.GetOrigin(), _markersFSM.GetTerminus());
        _regionProvider.OverwriteDraftUrl(_imgProvider.GetCurrentUrl());
        _regionProvider.OverwriteDraftCenter(_markersFSM.GetRegionMarkerCenter());
        _regionProvider.OverwriteDraftEffDist(_markersFSM.GetRegionMarkerEffDist());

        _regionProvider.ServerSaveDraft();
    }

    public override void OnDiscard()
    {
        _markersFSM.FullFSMReset();
    }

    void RegionCtrlConfig() {
        _regionCtrl.SDDNEnable(false);

        _regionCtrl.DisableBtns();
        _regionCtrl.EnableSave();
        _regionCtrl.EnableDiscard();
    }

    public class Factory : PlaceholderFactory<CreateRegion>
    {
    }
}
