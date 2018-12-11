using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using System.Runtime.InteropServices;
public class RegionProxy : MonoBehaviour, IRegionProxy
{

    [DllImport("__Internal")]
    private static extern void JS_ReqRegionsByUrl(string url);

    [DllImport("__Internal")]
    private static extern void JS_ReqSaveNewRegionDS(string rName, string json);

    [DllImport("__Internal")]
    private static extern void JS_ReqDeleteRegionByName(string rName);

    [DllImport("__Internal")]
    private static extern void JS_ExportRegionsToCSV();


    [Inject]
    IRegionServiceReceiver _srvReceiver;

    public void ReqSaveNewRegionDS(string rName, string json) {
        JS_ReqSaveNewRegionDS(rName, json);
    }

    public virtual void ReqRegionsByUrl(string url)
    {
        JS_ReqRegionsByUrl(url);
    }

    public void RegionsByUrl_CB(string jsonRegionsDS)
    {
        _srvReceiver.RegionsByUrl_CB(jsonRegionsDS);
    }

    public void UpdatedRegion_CB(string jsonRegionsDS)
    {
        _srvReceiver.UpdatedRegion_CB(jsonRegionsDS);
    }

    public void ReqDeleteRegionByName(string rName)
    {
        JS_ReqDeleteRegionByName(rName);
    }

    public void ExportRegionsToCSV() {
        JS_ExportRegionsToCSV();
    }
}
