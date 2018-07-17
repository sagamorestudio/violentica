using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scUIRoot : ZO.scSingleToneForMB<scUIRoot> {

    public Transform leftUp;

    public Transform rightBottom;

    void Start()
    {
        if (leftUp != null)
        {
            LeftUpGlobalPos = leftUp.transform.position;
            LeftUpLocalPos = leftUp.transform.localPosition;
        }
        if (rightBottom != null)
        {
            RightBottomGlobalPos = rightBottom.transform.position;
            RightBottomLocalPos = rightBottom.transform.localPosition;
        }
    }

    public Vector3 LeftUpGlobalPos;
    public Vector3 RightBottomGlobalPos;
    public Vector3 LeftUpLocalPos;
    public Vector3 RightBottomLocalPos;

    public float WidthInGolbal
    {
        get
        {
            return RightBottomGlobalPos.x - LeftUpGlobalPos.x;
        }
    }

    public float HeightInGlobal
    {
        get
        {
            return LeftUpGlobalPos.y - RightBottomGlobalPos.y;
        }
    }

    public float WidthInLocal
    {
        get
        {
            return RightBottomLocalPos.x - LeftUpLocalPos.x;
        }
    }

    public float HeightInLocal
    {
        get
        {
            return LeftUpLocalPos.y - RightBottomLocalPos.y;
        }
    }

}

