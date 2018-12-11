using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class SegmentCUD : StateEntity
{
    [Inject]
    IRegionCtrl _regionCtrl;

    [Inject]
    IMarkersFSM _markersFSM;

    [Inject]
    RRenderUI _rRenderUI;

    [Inject]
    SRenderUI _sRenderUI;

    [Inject]
    IRegionProvider _regionProvider;

    Region prevDraft = null;
    string _prevSegment = "--";
    string _prevRegion = "--";


    public override void Start()
    {
        StartPrevSegment();
        
        _markersFSM.FullFSMReset();
        RegionCtrlConfig();

        //turn selected segment off, others, on
        _regionCtrl.ToggleSegmentsOnBut(_regionCtrl.GetsDDN());
        DraftToMarkers();
    }

    public override void Tick()
    {
    }

    public override void Dispose()
    {
        Debug.Log("segCUD dispose, r: " + _regionCtrl.GetrDDN()+", prev: "+ _prevRegion);
        if (_prevRegion != "--" && prevDraft != null) {
            // render all it's segments on
            //prevDraft
            Dictionary<string, SegmentUI> sDict = _sRenderUI.SelectSegmentDict(_prevRegion);
            _sRenderUI.RenderSegmentsOfSDict(prevDraft, sDict, true);
        }
    }

    public override void MarkerClickHandler(Vector2 mousePos, PointerEventData eventData, bool lCtrlHold, bool lAltHold, bool reset = false)
    {
        if (((lCtrlHold && !lAltHold) || (lCtrlHold && lAltHold)) && _regionCtrl.GetcDDN() != "--")
            mousePos = GetVerTexOfSegment(_regionCtrl.GetcDDN(), lCtrlHold, lAltHold);
        _markersFSM.MarkerClickHandler(mousePos, eventData);
    }

    public override void OnDiscard()
    {
        // original copy from server is in rDict of AppRegion, we get it
        Vector2[] fresh_vertices = _regionProvider.GetRegionSegmentVertices(_regionCtrl.GetrDDN(), _regionCtrl.GetsDDN());

        // with that, overwrite draft
        _regionProvider.OverwriteDraftSegment(fresh_vertices[0], fresh_vertices[1], _regionCtrl.GetsDDN());

        // finally, overwrite sUI and Marker for the selected (in sDDN) segment
        UpdateSegmentUI();
        DraftToMarkers();
        ResetAngles();
        UpdateRegionMarkers();// this will write angles from draft to rUI
    }

    void UpdateRegionMarkers() {
        string rName = _regionProvider.GetDraft()._rName;
        float E1 = _regionProvider.GetDraft()._e1;
        float E2 = _regionProvider.GetDraft()._e2;
        _rRenderUI.UpdateAngles(rName, true, E1, true, E2);
    }

    void ResetAngles() {
        Region rDraft = _regionProvider.GetDraft();
        Region rOriginal = _regionProvider.GetRegionByName(rDraft._rName);
        rDraft._e1 = rOriginal._e1;
        rDraft._e2 = rOriginal._e2;
    }

    public override void OnCommit()
    {
        if (_markersFSM.GetCurMarkerState() != MarkersFSM.MarkerState.MARKER_FINAL) {
            _regionProvider.OverwriteDraftSegment(Vector2.zero, Vector2.zero, _regionCtrl.GetsDDN());
            UpdateForCommit();
            return;
        }

        Vector2 origin = _markersFSM.GetOrigin();
        Vector2 terminus = _markersFSM.GetTerminus();
        if(origin.x==terminus.x && origin.y == terminus.y)
        {
            _regionProvider.OverwriteDraftSegment(Vector2.zero, Vector2.zero, _regionCtrl.GetsDDN());
            UpdateForCommit();
            return;
        }

        // update draft
        Vector2[] vs = _regionProvider.GetDrafSegmentVertices(_regionCtrl.GetsDDN());
        _regionProvider.OverwriteDraftSegment(origin, terminus, _regionCtrl.GetsDDN());
        UpdateForCommit();

        // if _regionCtrl.GetsDDN() belongs-to: {B1, B2, C1, C2}, try calculate angle
    }

    void UpdateForCommit() {
        UpdateSegmentUI();
        UpdateAngles();
        UpdateRegionMarkers();// this will write angles from draft to rUI
    }

    void UpdateAngles() {
        if (_regionCtrl.GetsDDN() == "B1" || _regionCtrl.GetsDDN() == "C1")
            ComputeE1();
        else if (_regionCtrl.GetsDDN() == "B2" || _regionCtrl.GetsDDN() == "C2")
            ComputeE2();

    }

    void ComputeE1() {
        if (_regionProvider.ZeroDraftSegment("B1") || _regionProvider.ZeroDraftSegment("C1"))
            return;
        Region r = _regionProvider.GetDraft();
        float B1C1 = _regionProvider.ComputeAngleOf(r, "B1", "C1");
        _regionProvider.GetDraft()._e1 = B1C1;
    }

    void ComputeE2() {
        if (_regionProvider.ZeroDraftSegment("B2") || _regionProvider.ZeroDraftSegment("C2"))
            return;
        Region r = _regionProvider.GetDraft();
        float B2C2 = _regionProvider.ComputeAngleOf(r, "B2", "C2");
        _regionProvider.GetDraft()._e2 = B2C2;
    }

    void UpdateSegmentUI() {
        Region r = _regionProvider.GetDraft();
        Dictionary<string, SegmentUI>  sDict = _sRenderUI.SelectSegmentDict(r._rName);
        // false: still no need to turn on sUI.s
        _sRenderUI.UpdateSegmentUI(r, sDict, _regionCtrl.GetsDDN(), false);
    }

    void RegionCtrlConfig()
    {
        //_regionCtrl.SDDNEnable(false);
        _regionCtrl.DisableBtns();
        _regionCtrl.EnableCommit();
        _regionCtrl.EnableDiscard();
    }

    void DraftToMarkers() {
        // we rewrite from draft to markers
        Vector2[] vs = _regionProvider.GetDrafSegmentVertices(_regionCtrl.GetsDDN());

        // replacing marker-show with sUI-show
        // show markers in state of the segment that turned off
        PointerEventData ed = new PointerEventData(EventSystem.current);
        ed.button = PointerEventData.InputButton.Left;
        _markersFSM.MarkerClickHandler(vs[0], ed);
        _markersFSM.MarkerClickHandler(vs[1], ed);
    }

    Vector2 GetVerTexOfSegment(string sName, bool LCtrl, bool LAlt) {
        Vector2[] vs = _regionProvider.GetDrafSegmentVertices(sName);
        if (LCtrl && !LAlt)
            return vs[0];// get origin of sName
        else if (LCtrl && LAlt)
            return vs[1];// get terminus of sName
        else
            return Vector2.zero;// TODO throw exception, this is an error
    }

    void StartPrevSegment() {
        _prevRegion = _regionCtrl.GetrDDN();
        _prevSegment = _regionCtrl.GetsDDN();

        if (_prevRegion != "--")
            prevDraft = _regionProvider.GetDraft();
    }

    public class Factory : PlaceholderFactory<SegmentCUD>
    {
    }
}