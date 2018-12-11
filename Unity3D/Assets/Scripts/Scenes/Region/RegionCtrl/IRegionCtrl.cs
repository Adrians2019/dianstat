using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRegionCtrl
{
    void initRegions();

    void RDDNEnable(bool toSet);
    void SDDNEnable(bool toSet);

    string GetsDDN();
    string GetrDDN();
    string GetcDDN();

    void EnableBtns();
    void EnableSave();
    void EnableDiscard();
    void EnableCommit();
    void EnableDeleteRegion();

    void DisableBtns();
    void DisableSave();
    void DisableDiscard();
    void DisableCommit();
    void DisableDeleteRegion();

    void NewRegionSavingDiag(bool ifShown);
    void BlockScreen(bool ifBlock, string msg = "");
    void ToggleSegmentsOnBut(string sName);
}
