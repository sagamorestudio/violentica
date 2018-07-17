using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZO;

public class scOutCameraController : MonoBehaviour
{

    public scOutCameraLinker MyLinker;
    Util.DelegateVoid1Param<scOutCameraController> event_next_call;
    public GameObject MyOutCameraObject;


    public void Clear()
    {
        if (MyLinker != null)
        {
            Destroy(MyLinker.gameObject);
            MyLinker = null;
        }
        MyLinker = null;
        MyOutCameraObject = null;
    }

    public bool IsLinked()
    {
        return MyOutCameraObject != null;
    }

    public void LinkWith(GameObject created, scOutCameraLinker lnker = null)
    {
        Clear();

        //링커가 null 이면 가장 가까운 링커를 찾는다
        MyLinker = lnker;
        if(MyLinker == null)
            MyLinker = this.GetNearestParent<scOutCameraLinker>();

        //생성
        MyOutCameraObject = MyLinker.Stage.MakeOutcamObject(gameObject, created, 1);
    }
    public void LinkWith<T>(T created, scOutCameraLinker lnker = null) where T : Component
    {
        Clear();

        //링커가 null 이면 가장 가까운 링커를 찾는다
        MyLinker = lnker;
        if (MyLinker == null)
            MyLinker = this.GetNearestParent<scOutCameraLinker>();

        //생성
        MyOutCameraObject = MyLinker.Stage.MakeOutcamObject(gameObject, created, 1).gameObject;
    }
}


public static class scOutCameraController_ExtensionMethods
{
    public static scOutCameraController FindOutCameraControllerInChildren(this Component g)
    {
        return Util.FindInHierarchyRecuvely<scOutCameraController>(g);
    }

    public static scOutCameraController FindOutCameraControllerInChildren(this GameObject g)
    {
        return Util.FindInHierarchyRecuvely<scOutCameraController>(g);
    }
    
}

