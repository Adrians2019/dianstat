using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRegionProvider {
    Dictionary<string, Region> GetRegions();
    void ReqRegionsByUrl(string url);
    void ReNewRegionDS();
    Region GetDraft();
    Region GetRegionByName(string rName);
    Vector2[] GetDrafSegmentVertices(string sName);
    Vector2[] GetRegionSegmentVertices(string rName, string sName);
    bool ZeroDraftSegment(string sName);
    float ComputeAngleOf(Region r, string sName1, string sName2);
    void ReqDeleteRegionByName(string rName);
    string GetCurrentUrl();

    void OverwriteSDraft(Vector2 p1, Vector2 p2, string sName);
    void NewDraft(string rName);// Overwrite draft by rDict's original
    void OverwriteDraftName(string rName);
    void OverwriteDraftDiagonal(Vector2 p1, Vector2 p2);
    void OverwriteDraftUrl(string url);
    void OverwriteDraftCenter(Vector2 vcenter);
    void OverwriteDraftEffDist(Vector2 veffDist);
    void OverwriteDraftSegment(Vector2 toP1, Vector2 toP2, string sName);

    void ServerSaveDraft();
    void ExportRegionsToCSV();
}
