using UnityEngine;

public class UnitInitEvt { }

public class UnitLatestDSUpdateEvt {
    public UnitLatest unitLatest = new UnitLatest();
}

public class ReqSaveUnitDSEvt
{
    public UnitLatest unitLatest = new UnitLatest();
}

public class UnitOriginPointEvt
{
    public Vector2 vorigin = Vector2.zero;
}

public class UnitTerminusPointEvt
{
    public Vector2 vterminus = Vector2.zero;
}

public class UnitLineImgUpdateEvt
{
    public Vector2 anchoredPosition = Vector2.zero;
    public Vector2 sizeDelta = Vector2.zero;
    public float angle = 0f;
    public float magnitude = 0f;
}



