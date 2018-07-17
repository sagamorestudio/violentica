using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZO;

public static class ExtensionMethods
{
    public static Util.Struct<List<T>, List<T>> VSplitList<T>(this List<T> o, System.Predicate<T> det)
    {
        List<T> ls1 = new List<T>();
        List<T> ls2 = new List<T>();
        foreach (var v in o)
        {
            if (det(v))
                ls1.Add(v);
            else
                ls2.Add(v);
        }
        return Util.MakeTuple(ls1, ls2);
    }


    public static bool IsNullOrEmptyStr(this object o)
    {
        if (o == null) return true;
        return string.IsNullOrEmpty(o.ToString());
    }

    public static bool IsActiveSmart(this MonoBehaviour c)
    {
        return c != null && c.enabled && c.gameObject.activeInHierarchy && c.gameObject.activeSelf;
    }

    public static bool IsActiveSmart(this GameObject g)
    {
        return g != null && g.activeInHierarchy && g.activeSelf;
    }

    public static T SetActiveSmart<T>(this T g, bool b) where T : Component
    {
        if (g != null && g.gameObject.activeSelf != b) g.gameObject.SetActive(b);
        return g;
    }

    public static GameObject SetActiveSmart(this GameObject g, bool b)
    {
        if (g != null && g.activeSelf != b) g.SetActive(b);
        return g;
    }

    public static bool ActiveSelf(this Component p)
    {
        if (p == null) return false;
        return p.gameObject.activeSelf;
    }


    public static List<T> Clone<T>(this IEnumerable<T> ii)
    {
        List<T> rtv = new List<T>();
        foreach (var v in ii) rtv.Add(v);
        return rtv;
    }

    public static bool IsIt<T>(this Component t) where T : Component
    {
        if (t == null) return false;
        return t.GetComponent<T>() != null;
    }

    public static bool IsIt<T>(this GameObject t) where T : Component
    {
        if (t == null) return false;
        return t.GetComponent<T>() != null;
    }

    public static T[] GetComponentsAndRun<T>(this GameObject g, Util.DelegateVoid1Param<T> func) where T : Component
    {
        var ls = g.GetComponents<T>();
        if (ls != null && ls.Length > 0)
        {
            foreach (var v in ls) func(v);
        }
        return ls;
    }

    public static T[] GetComponentsAndRun<T>(this Component g, Util.DelegateVoid1Param<T> func) where T : Component
    {
        var ls = g.GetComponents<T>();
        if (ls != null && ls.Length > 0)
        {
            foreach (var v in ls) func(v);
        }
        return ls;
    }
    public static List<int> StrToInt(List<string> strList)
    {
        List<int> outputval = new List<int>();
        foreach (var v in strList)
        {
            int id = 0;
            if (int.TryParse(v, out id)) outputval.Add(id);
        }
        return outputval;
    }



    public static bool HaveComponent<T>(this GameObject g) where T : Component
    {
        var t = g.GetComponent<T>();
        return t != null;
    }

    public static bool HaveComponent<T>(this Component g) where T : Component
    {
        var t = g.GetComponent<T>();
        return t != null;
    }

    public static void Shuffle_WriteToNew<T>(this List<T> ls)
    {
        T temp;
        for (int i = 0; i < ls.Count; ++i)
        {
            if (i + 1 <= ls.Count - 1)
            {
                int idx = Util.GetRandomNumberIntInclusive(i + 1, ls.Count - 1);
                temp = ls[i];
                ls[i] = ls[idx];
                ls[idx] = temp;
            }
        }
    }

    public static List<T> Shuffle_WriteToOriginal<T>(this List<T> wo)
    {
        int n = wo.Count;
        while (n > 1)
        {
            --n;
            int k = Util.GetRandomNumberIntInclusive(0, n);
            T value = wo[k];
            wo[k] = wo[n];
            wo[n] = value;
        }
        return wo;
    }


    public static bool IsNullOrEmpty<T>(this List<T> dt)
    {
        if (dt == null || dt.Count == 0) return true;
        else return false;
    }

    public static bool IsNullOrEmpty<T>(this T[] dt)
    {
        if (dt == null || dt.Length == 0) return true;
        else return false;
    }

    public static int SumOfList<T2>(this List<T2> tl, Util.DelegateParam<int, T2> func)
    {
        int sum = 0;
        foreach (var v in tl) sum += func(v);
        return sum;
    }

    public static float SumOfList<T2>(this List<T2> tl, Util.DelegateParam<float, T2> func)
    {
        float sum = 0;
        foreach (var v in tl) sum += func(v);
        return sum;
    }

    public static string SumOfList<T2>(this List<T2> tl, Util.DelegateParam<string, T2> func)
    {
        string sum = string.Empty;
        foreach (var v in tl) sum += func(v);
        return sum;
    }

    public static List<UITweener> ResetAllTweensAndPlay(this Component c)
    {
        return ResetAllTweensAndPlay(c, true);
    }

    public static List<UITweener> ResetAllTweensAndPlay(this GameObject g)
    {
        return ResetAllTweensAndPlay(g, true);
    }

    public static List<UITweener> ResetAllTweensAndPlay(this Component c, bool b)
    {
        var ls = c.FindAllInHierarchyRecuvely<UITweener>();
        foreach (var v in ls)
        {   
            v.ResetToBeginning();            
            v.enabled = b;
        }
        return ls;
    }

    public static List<UITweener> ResetAllTweensAndPlay(this GameObject g, bool b)
    {
        var ls = g.FindAllInHierarchyRecuvely<UITweener>();
        foreach (var v in ls)
        {
            v.ResetToBeginning();
            v.enabled = b;
        }
        return ls;
    }

    public static List<T> CloneTo<T>(this T[] from, List<T> to)
    {
        to.Clear();
        foreach (var v in from) to.Add(v);
        return to;
    }

    public static List<T> CloneTo<T>(this List<T> from, List<T> to)
    {
        to.Clear();
        foreach (var v in from) to.Add(v);
        return to;
    }





    public delegate T Return_T<T>();

    /// <summary>
    /// Return_T 는 만약에 null 이면 할 행동 (없으면 기본적으로 null 을 리턴)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="c"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    public static T GetComponentNullChecked<T>(this Component c, Return_T<T> ifNull = null) where T : Component
    {
        if (c == null) if (ifNull == null) return null; else return ifNull();
        var v = c.GetComponent<T>();
        if (v == null)
        {
            if (ifNull == null) return null;
            else return ifNull();
        }
        else return v;
    }

    public static T GetComponentNullChecked<T>(this GameObject c, Return_T<T> ifNull = null) where T : Component
    {
        if (c == null) if (ifNull == null) return null; else return ifNull();
        var v = c.GetComponent<T>();
        if (v == null)
        {
            if (ifNull == null) return null;
            else return ifNull();
        }
        else return v;
    }

    public static string GetHierarchyName(this Component g)
    {
        if (g == null)
            return "null";
        return GetHierarchyName(g.gameObject);
    }

    public static string GetHierarchyName(this GameObject g)
    {
        if (g == null) return "null";
        LinkedList<string> lsStr = new LinkedList<string>();
        Transform t = g.transform;
        while (t != null && t.GetComponent<Camera>() == null)
        {
            lsStr.AddFirst(t.name + ".");
            t = t.parent;
        }

        if (t != null) lsStr.AddFirst(t.name + ".");

        string str = "";
        foreach (string s in lsStr)
        {
            str += s;
        }
        return str;
    }
}