using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface RRenderUI {
    void RenderRLabels(Region[] regions);
    void RenderRLabelsOf(Region r, bool toSet);
    void UpdateAngles(string rName, bool bE1,float E1, bool bE2, float E2);
}
