using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface SRenderUI
{
    void ToggleSUI(string rName, string sName, bool toSet);
    void RenderSegments(Region[] regions);
    void RenderSegmentsOfSDict(Region region, Dictionary<string, SegmentUI> sDict, bool toSet);
    void UpdateSegmentUI(Region region, Dictionary<string, SegmentUI> sDict, string sName, bool toSet);
    SRenderDS SegmentDS();
    Dictionary<string, SegmentUI> SelectSegmentDict(string rName);
}
