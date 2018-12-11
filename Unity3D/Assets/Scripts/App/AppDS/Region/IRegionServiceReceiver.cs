using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRegionServiceReceiver {
    void RegionsByUrl_CB(string jsonRegionsDS);
    void UpdatedRegion_CB(string jsonRegionDS);    
}
