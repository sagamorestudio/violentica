using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZO;

public class scNetworkLoading : MonoBehaviour
{

    private static scNetworkLoading _instance;
    public static scNetworkLoading Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = scPrefabs.Create(scPrefabs.Instance.PrefabNetworkLoading);
                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

    public static bool HasInstance
    {
        get
        {
            return _instance != null;
        }
    }


    public GameObject Content;

    List<object> ccs;

    [SerializeField]
    UILabel lbLoading;

    void Awake()
    {
        ccs = new List<object>();
        mainloop().VPlay();
        animation().VPlay();
    }

    [SerializeField]
    UISprite spr_background;

    public bool LoadingLogo
    {
        get
        {
            return lbLoading.ActiveSelf();
        }
        set
        {
            lbLoading.SetActiveSmart(value);
        }
    }

    public bool Background
    {
        get
        {
            return spr_background.ActiveSelf();
        }
        set
        {
            spr_background.SetActiveSmart(value);
        }
    }

    IEnumerator animation()
    {
        while (true)
        {
            while (!Content.activeSelf) yield return null;
            lbLoading.text = "Loading...";
            yield return CWait.ForSeconds(0.1f);
            while (!Content.activeSelf) yield return null;
            lbLoading.text = "LOading...";
            yield return CWait.ForSeconds(0.1f);
            while (!Content.activeSelf) yield return null;
            lbLoading.text = "LOAding...";
            yield return CWait.ForSeconds(0.1f);
            while (!Content.activeSelf) yield return null;
            lbLoading.text = "LOADing...";
            yield return CWait.ForSeconds(0.1f);
            while (!Content.activeSelf) yield return null;
            lbLoading.text = "LOADIng...";
            yield return CWait.ForSeconds(0.1f);
            while (!Content.activeSelf) yield return null;
            lbLoading.text = "LOADINg...";
            yield return CWait.ForSeconds(0.1f);
            while (!Content.activeSelf) yield return null;
            lbLoading.text = "LOADING...";
            yield return CWait.ForSeconds(0.1f);
            while (!Content.activeSelf) yield return null;
            lbLoading.text = "lOADING...";
            yield return CWait.ForSeconds(0.1f);
            while (!Content.activeSelf) yield return null;
            lbLoading.text = "loADING...";
            yield return CWait.ForSeconds(0.1f);
            while (!Content.activeSelf) yield return null;
            lbLoading.text = "loaDING...";
            yield return CWait.ForSeconds(0.1f);
            while (!Content.activeSelf) yield return null;
            lbLoading.text = "loadING...";
            yield return CWait.ForSeconds(0.1f);
            while (!Content.activeSelf) yield return null;
            lbLoading.text = "loadiNG...";
            yield return CWait.ForSeconds(0.1f);
            while (!Content.activeSelf) yield return null;
            lbLoading.text = "loadinG...";
            yield return CWait.ForSeconds(0.1f);
            while (!Content.activeSelf) yield return null;
            lbLoading.text = "loading...";
            yield return CWait.ForSeconds(0.1f);
        }
    }

    IEnumerator mainloop()
    {
        while (true)
        {
            if (ccs.Count > 0)
            {
                Content.SetActiveSmart(true);
                var ls = ccs.ToArray();
                foreach (var o in ls)
                {
                    var c = o as CoroutineClass;
                    var d = o as Util.DelegateBoolNoParam;
                    if (c != null)
                    {
                        if (!c.br) ccs.Remove(o);
                    }
                    else if (d != null)
                    {
                        if (!d()) ccs.Remove(o);
                    }
                    else
                        ccs.Remove(o);
                }
            }
            else
            {
                Content.SetActiveSmart(false);
            }
            yield return null;
        }
        yield break;
    }

    public bool IsLoading
    {
        get
        {
            return Content.activeSelf;
        }
    }

    public scNetworkLoading LoadingForWhile(CoroutineClass cc)
    {
        Content.SetActiveSmart(true);
        ccs.Add(cc);
        return this;
    }

    public scNetworkLoading LoadingForWhile(Util.DelegateBoolNoParam cc)
    {
        Content.SetActiveSmart(true);
        ccs.Add(cc);
        return this;
    }

}
