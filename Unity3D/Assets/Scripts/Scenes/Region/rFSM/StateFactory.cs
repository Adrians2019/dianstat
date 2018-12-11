using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModestTree;

public enum REnvState {
        CreateRegion,
        RegionUD,
        SegmentCUD
}

public class StateFactory
{
    readonly CreateRegion.Factory _createRegionFactory;
    readonly RegionUD.Factory _regionUDFactory;
    readonly SegmentCUD.Factory _segmentCUDFactory;

    public StateFactory(
        CreateRegion.Factory createRegionFactory,
        RegionUD.Factory regionUDFactory,
        SegmentCUD.Factory segmentCUDFactory
        )
    {
        _createRegionFactory = createRegionFactory;
        _regionUDFactory = regionUDFactory;
        _segmentCUDFactory = segmentCUDFactory;
    }

    public StateEntity CreateState(REnvState nextState) {
        switch (nextState) {
            case REnvState.CreateRegion:
                return _createRegionFactory.Create();
            case REnvState.RegionUD:
                return _regionUDFactory.Create();
            case REnvState.SegmentCUD:
                return _segmentCUDFactory.Create();
        }
        throw Assert.CreateException("Code should not be reached here");
    }
}
