using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class RegionUD : StateEntity
{
    [Inject]
    IRegionCtrl _regionCtrl;

    [Inject]
    IMarkersFSM _markersFSM;

    [Inject]
    IRegionProvider _regionProvider;

    [Inject]
    RRenderUI _rRenderUI;

    [Inject]
    SRenderUI _sRenderUI;

    bool regionCDS = false;

    public override void Start()
    {
        RegionCtrlConfig();

        // Markers ready for Region Update-Delete in RegionUD
        _markersFSM.FullFSMReset();

        // refresh draft from rDict to current rDDN selection
        _regionProvider.NewDraft(_regionCtrl.GetrDDN());

        // Obtain original region of rDict (fresh copy)
        Region r = _regionProvider.GetRegions()[_regionCtrl.GetrDDN()];

        // render region UI, using it
        _rRenderUI.RenderRLabelsOf(r, true);

        // render segment UI, using it
        Dictionary<string, SegmentUI> sDict = _sRenderUI.SegmentDS().sUIDict[r._rName];
        _sRenderUI.RenderSegmentsOfSDict(r, sDict, true);
    }

    void RegionCtrlConfig()
    {
        // allow segment selection in RegionUD state
        _regionCtrl.SDDNEnable(true);

        _regionCtrl.DisableBtns();
        _regionCtrl.EnableSave();
        _regionCtrl.EnableDeleteRegion();
    }

    public override void OnSave() {
        // TODO: "are you sure to save region x?", then...
        _regionCtrl.BlockScreen(true, "Saving region '"+_regionCtrl.GetrDDN()+"' to server, please wait...");
        _regionProvider.ServerSaveDraft();
    }

    public override void OnSaveConfirmed() { }

    public class Factory : PlaceholderFactory<RegionUD>
    {
    }
}