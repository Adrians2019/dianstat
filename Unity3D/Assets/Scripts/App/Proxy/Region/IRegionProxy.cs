using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRegionProxy {

    void ReqSaveNewRegionDS(string rName, string json);
    void ReqRegionsByUrl(string url);
    void ReqDeleteRegionByName(string rName);
    void ExportRegionsToCSV();
}
