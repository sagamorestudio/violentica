using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZO
{
    public static class VExtentionMethods
    {

        public static string Format(this string s, params object[] param)
        {
            return string.Format(s, param);
        }

        public static bool IsNullOrEmpty<T>(this List<T> ls)
        {
            if (ls == null || ls.Count == 0) return true;
            else return false;
        }
        public static bool IsNullOrEmpty<T>(this T[] ls)
        {
            if (ls == null || ls.Length == 0) return true;
            else return false;
        }

        public static string ToStringComplicated(this Vector2 v)
        {
            return "(" + v.x + ", " + v.y + ")";
        }

        public static string ToStringComplicated(this Vector3 v)
        {
            return "(" + v.x + ", " + v.y + ", " + v.z + ")";
        }

        public static List<T> GetTotalChildren<T>(this Transform t) where T : Component
        {
            List<T> c = new List<T>();
            T tmp = null;
            foreach (Transform tt in t)
            {
                tmp = tt.GetComponent<T>();
                if (tmp != null)
                    c.Add(tmp);
            }
            return c;
        }

        public static List<T> VFindAndPopAll_KeepOrder<T>(this List<T> o, Predicate<T> finder)
        {
            var ls = o.VFindAll(finder);
            o.RemoveAll(finder);
            return ls;
        }

        public static List<T> VAdd<T>(this List<T> o, T t)
        {
            o.Add(t);
            return o;
        }

        public static T AddToThisList<T>(this T t, List<T> target)
        {
            target.Add(t);
            return t;
        }

        public static T VFindAndPop_KeepOrder<T>(this List<T> o, Predicate<T> finder)
        {
            Boxer<bool> b = new Boxer<bool>(false);
            var s = o.VFind(finder, b);
            if(b)
                o.Remove(s);
            return s;
        }

        public static T AddAdditionalUpdate<T>(this T o, Util.DelegateVoid1Param<T> update) where T : MonoBehaviour
        {
            additional_updator(o, update).VPlayDangled(o);
            return o;
        }

        private static IEnumerator additional_updator<T>(T o, Util.DelegateVoid1Param<T> update) where T : MonoBehaviour
        {
            while(true)
            {
                if(o.enabled && o.IsGameObjectActiveBoth())
                    update(o);
                yield return null;
            }
        }

        public static T VGetRandomElem_FollowDistributed<T>(this List<T> ls, Util.DelegateParam<float, T> distribution_foreach)
        {
            if (ls == null || ls.Count == 0) return default(T);
            else if (ls.Count == 1) return ls[0];

            float f = 0;
            List<float> distributed = new List<float>();
            foreach (var v in ls)
            {
                distributed.Add(distribution_foreach(v));
                f += distributed.GetLast();
            }
            float current_f = UnityEngine.Random.value * f;

            float accumulative = 0;
            for(int i = 0; i < distributed.Count; ++i)
            {
                accumulative += distributed[i];
                if(accumulative >= current_f)
                {
                    return ls[i];
                }
            }

            //만약 최대까지 왔는데 current_f 를 못넘으면 accumulative랑 같지만 float 오차때문인 에러임
            return ls.GetLast();
        }

        public static List<Transform> GetTotalChildren(this Transform t)
        {
            List<Transform> c = new List<Transform>();
            foreach (var tt in t) c.Add(t);
            return c;
        }

        public static Dictionary<T1, T2> ToDictionary<T1, T2>(this List<Util.Struct<T1, T2>> ps)
        {
            return ToDictionary(ps.ToArray());
        }

        public static Dictionary<T1, T2> ToDictionary<T1, T2>(this Util.Struct<T1, T2>[] ps)
        {
            Dictionary<T1, T2> rtv = new Dictionary<T1, T2>();
            foreach (var v in ps)
            {
                try
                {
                    rtv.Add(v.v1, v.v2);
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning("Warning : ToDictionary Same Key Exception (key = " + v.v1 + ")");                    
                    continue;
                }
            }
            return rtv;
        }

        public static List<T> VSort<T, T2>(this List<T> ls, Util.DelegateParam<T2, T> def) where T2 : IComparable<T2>
        {
            ls.Sort((T t1, T t2) => def(t1).CompareTo(def(t2)));
            return ls;
        }

        public static List<T2> ToListValues<T1, T2>(this Dictionary<T1, T2> dic)
        {
            List<T2> rtv = new List<T2>();
            foreach (var kv in dic)
                rtv.Add(kv.Value);
            return rtv;
        }

        public static List<T1> ToListKeys<T1, T2>(this Dictionary<T1, T2> dic)
        {
            List<T1> rtv = new List<T1>();
            foreach (var kv in dic)
                rtv.Add(kv.Key);
            return rtv;
        }

        public static int VIndexOf<T>(this List<T> body, Predicate<T> det)
        {
            if (body == null || body.Count == 0)
                return -1;
            for(int i = 0; i < body.Count; ++i)
            {
                if (det(body[i]))
                    return i;
            }
            return -1;
        }

        public static T VAggregate<T>(this IEnumerable<T> body, Util.DelegateParam<T, T, T> agf)
        {
            if (body.Count() == 0)
                return default(T);
            else if (body.Count() == 1)
                return body.First();

            var e = body.GetEnumerator();
            e.MoveNext();
            var working = e.Current;
            while(e.MoveNext())
            {
                working = agf(working, e.Current);
            }
            return working;
        }

        public static T ArgMax<T>(this List<T> ls, Util.DelegateParam<float, T> det)
        {
            return Util.ArgMax(det, ls.ToArray());
        }

        public static T ArgMin<T>(this List<T> ls, Util.DelegateParam<float, T> det)
        {
            return Util.ArgMin(det, ls.ToArray());
        }

        public static T ArgMax<T>(this List<T> ls, Util.DelegateParam<int, T> det)
        {
            return Util.ArgMax(det, ls.ToArray());
        }

        public static T ArgMin<T>(this List<T> ls, Util.DelegateParam<int, T> det)
        {
            return Util.ArgMin(det, ls.ToArray());
        }

        public static T ArgMax<T>(this T[] ls, Util.DelegateParam<int, T> det)
        {
            return Util.ArgMax(det, ls);
        }

        public static T ArgMin<T>(this T[] ls, Util.DelegateParam<int, T> det)
        {
            return Util.ArgMin(det, ls);
        }
        
        public static float Clip(this float f, float min, float max)
        {
            if (f < min) return min;
            else if (f > max) return max;
            else return f;
        }

        public static int Clip(this int f, int min, int max)
        {
            if (f < min) return min;
            else if (f > max) return max;
            else return f;
        }

        public static decimal Clip(this decimal f, decimal min, decimal max)
        {
            if (f < min) return min;
            else if (f > max) return max;
            else return f;
        }

        public static double Clip(this double f, double min, double max)
        {
            if (f < min) return min;
            else if (f > max) return max;
            else return f;
        }

        public static uint Clip(this uint f, uint min, uint max)
        {
            if (f < min) return min;
            else if (f > max) return max;
            else return f;
        }

        public static List<T> MakeList<T>(this T t)
        {
            List<T> rtv = new List<T>();
            rtv.Add(t);
            return rtv;
        }

        public static T FindFirstAndRemove<T>(this List<T> ls, Predicate<T> de)
        {
            var d = ls.Find(de);
            ls.Remove(d);
            return d;
        }

        public static bool IsGameObjectActiveSelf(this Component c)
        {
            if (c == null) return false;
            else if (c.gameObject == null) return false;
            else
                return c.gameObject.activeSelf;
        }

        public static bool IsGameObjectActiveInHierarchy(this Component c)
        {
            if (c == null) return false;
            else if (c.gameObject == null) return false;
            else
                return c.gameObject.activeInHierarchy;
        }

        public static bool IsGameObjectActiveBoth(this Component c)
        {
            if (c == null) return false;
            else if (c.gameObject == null) return false;
            else
                return c.gameObject.activeInHierarchy && c.gameObject.activeSelf;
        }

        public static List<T> VRemoveFromList<T>(this List<T> ls, List<T> removeList)
        {
            if (ls != null && removeList != null)
            {
                if (ls == removeList)
                    ls.Clear();                
                else
                    foreach (var v in removeList) ls.Remove(v);
            }
            return ls;
        }

        public static List<T> VPaddingUpTo<T>(this List<T> ls, int amount, Util.DelegateParam<T> val)
        {
            if (ls == null) return ls;
            while (ls.Count < amount) ls.Add(val());
            return ls;
        }

        public static List<T> VPaddingUpTo<T>(this List<T> ls, int amount, T val)
        {
            if (ls == null) return ls;
            while(ls.Count < amount) ls.Add(val);
            return ls;
        }
        public static List<T> VPaddingUpTo<T>(this List<T> ls, int amount)
        {
            if (ls == null) return ls;
            while (ls.Count < amount) ls.Add(default(T));
            return ls;
        }

        public static List<T> VAddValues<T>(this List<T> ls, T val, int amount)
        {
            if (ls == null) return ls;
            for(int i = 0; i < amount; ++i) ls.Add(val);
            return ls;
        }

        public static List<T> VRemoveAt<T>(this List<T> ls, int index)
        {
            if (ls == null) return ls;
            if (ls.Count <= index) return ls;
            ls.RemoveAt(index);
            return ls;
        }

        public static T GetLast<T>(this T[] ls)
        {
            if (ls == null || ls.Length == 0) return default(T);
            else return ls[ls.Length - 1];
        }

        public static T GetLast<T>(this T[] ls, T default_value)
        {
            if (ls == null || ls.Length == 0) return default_value;
            else return ls[ls.Length - 1];
        }

        public static T GetFirst<T>(this T[] ls)
        {
            if (ls == null || ls.Length == 0) return default(T);
            else return ls[0];
        }

        public static T GetFirst<T>(this T[] ls, T default_value)
        {
            if (ls == null || ls.Length == 0) return default_value;
            else return ls[0];
        }



        public static T GetLast<T>(this List<T> ls)
        {
            if (ls == null || ls.Count == 0) return default(T);
            else return ls[ls.Count - 1];
        }

        public static T GetLast<T>(this List<T> ls, T default_value)
        {
            if (ls == null || ls.Count == 0) return default_value;
            else return ls[ls.Count - 1];
        }

        public static T GetFirst<T>(this List<T> ls)
        {
            if (ls == null || ls.Count == 0) return default(T);
            else return ls[0];
        }

        public static T GetFirst<T>(this List<T> ls, T default_value)
        {
            if (ls == null || ls.Count == 0) return default_value;
            else return ls[0];
        }

        public static List<T> VAddRange<T>(this List<T> ls, IEnumerable<T> add)
        {
            ls.AddRange(add);
            return ls;
        }

        public static T GetValue<T>(this List<T> ls, int index)
        {
            if (ls == null || index < 0 || index >= ls.Count) return default(T);
            return ls[index];
        }

        public static T GetValue<T>(this T[] ls, int index)
        {
            if (ls == null || index < 0 || index >= ls.Length) return default(T);
            return ls[index];
        }

        public static T GetValue<T>(this List<T> ls, int index, T basic_value)
        {
            if (ls == null || index < 0 || index >= ls.Count) return basic_value;
            return ls[index];
        }

        public static T GetValue<T>(this T[] ls, int index, T basic_value)
        {
            if (ls == null || index < 0 || index >= ls.Length) return basic_value;
            return ls[index];
        }

        public static T GetValue<T>(this List<T> ls, int index, Util.DelegateParam<T> basic_value)
        {
            if (ls == null || index < 0 || index >= ls.Count) return basic_value();
            return ls[index];
        }

        public static T GetValue<T>(this T[] ls, int index, Util.DelegateParam<T> basic_value)
        {
            if (ls == null || index < 0 || index >= ls.Length) return basic_value();
            return ls[index];
        }

        public static TValue GetValue<Tkey, TValue>(this Dictionary<Tkey, TValue> dic, Tkey key, TValue basic_value)
        {
            TValue tv;
            if (dic.TryGetValue(key, out tv))
                return tv;
            else
                return basic_value;
        }

        public static TValue GetValue<Tkey, TValue>(this Dictionary<Tkey, TValue> dic, Tkey key, Util.DelegateParam<TValue> basic_value)
        {
            TValue tv;
            if (dic.TryGetValue(key, out tv))
                return tv;
            else
                return basic_value();
        }

        public static TValue GetValue<Tkey, TValue>(this Dictionary<Tkey, TValue> dic, Tkey key)
        {
            TValue tv;
            if (dic.TryGetValue(key, out tv))
                return tv;
            else
                return default(TValue);
        }


        public static T GetOrCreate<T>(this List<T> ls, int index)
        {
            if (ls == null || index < 0 || index >= ls.Count)
                ls.ReplaceAtSurpress(index, default(T));
            return ls[index];
        }
        
        public static T GetOrCreate<T>(this List<T> ls, int index, T basic_value)
        {
            if (ls == null || index < 0 || index >= ls.Count)
                ls.ReplaceAtSurpress(index, basic_value);
            return ls[index];
        }

        public static T GetOrCreate<T>(this List<T> ls, int index, T basic_value, Util.DelegateParam<T> padding_value)
        {
            if (ls == null || index < 0 || index >= ls.Count)
                ls.ReplaceAtSurpress(index, basic_value, padding_value());
            return ls[index];
        }

        public static TValue GetOrCreate<Tkey, TValue>(this Dictionary<Tkey, TValue> dic, Tkey key, Util.DelegateParam<TValue> basic_value)
        {
            TValue tv;
            if (dic.TryGetValue(key, out tv))
                return tv;
            else
            {
                dic.Add(key, basic_value());
                return dic[key];
            }
        }

        public static TValue GetOrCreate<Tkey, TValue>(this Dictionary<Tkey, TValue> dic, Tkey key, TValue basic_value)
        {
            TValue tv;
            if (dic.TryGetValue(key, out tv))
                return tv;
            else
            {
                dic.Add(key, basic_value);
                return dic[key];
            }
        }

        public static TValue GetOrCreate<Tkey, TValue>(this Dictionary<Tkey, TValue> dic, Tkey key)
        {
            TValue tv;
            if (dic.TryGetValue(key, out tv))
                return tv;
            else
            {
                dic.Add(key, default(TValue));
                return dic[key];
            }
        }

        public static T GetOrCreateComponent<T>(this Component g) where T : Component
        {
            if (g == null) return null;
            return GetOrCreateComponent<T>(g.gameObject);
        }

        public static T GetOrCreateComponent<T>(this GameObject g) where T : Component
        {
            if (g == null) return null;
            var t = g.GetComponent<T>();
            if (t == null) t = g.AddComponent<T>();
            return t;
        }






        public static List<T> ReplaceAtIfExistFunc<T>(this List<T> ls, int index, Util.DelegateParam<T, T> val)
        {
            if (index >= 0 && index < ls.Count)
                ls[index] = val(ls[index]);
            return ls;
        }
        public static List<T> ReplaceAtSurpressFunc<T>(this List<T> ls, int index, Util.DelegateParam<T, T> val, T padding_val)
        {
            if (ls != null)
            {
                while (ls.Count <= index) ls.Add(padding_val);
                ls[index] = val(ls[index]);
            }
            return ls;
        }

        public static List<T> ReplaceAtSurpressFunc<T>(this List<T> ls, int index, Util.DelegateParam<T, T> val)
        {
            if (ls != null)
            {
                while (ls.Count <= index) ls.Add(default(T));
                ls[index] = val(ls[index]);
            }
            return ls;
        }








        public static List<T> ReplaceAtIfExist<T>(this List<T> ls, int index, T val)
        {
            if (index >= 0 && index < ls.Count)
                ls[index] = val;
            return ls;
        }
        public static List<T> ReplaceAtSurpress<T>(this List<T> ls, int index, T val, T padding_val)
        {
            if (ls != null)
            {
                while (ls.Count <= index) ls.Add(padding_val);
                ls[index] = val;
            }
            return ls;
        }

        public static List<T> ReplaceAtSurpress<T>(this List<T> ls, int index, T val, Util.DelegateParam<T> padding_val)
        {
            if (ls != null)
            {
                while (ls.Count <= index) ls.Add(padding_val());
                ls[index] = val;
            }
            return ls;
        }

        public static List<T> ReplaceAtSurpress<T>(this List<T> ls, int index, T val)
        {
            if (ls != null)
            {
                while (ls.Count <= index) ls.Add(default(T));
                ls[index] = val;
            }
            return ls;
        }

        public static Dictionary<TKEY, T> ReplaceAtSurpress<TKEY, T>(this Dictionary<TKEY, T> dic, TKEY key, T val)
        {
            if (dic != null)
            {
                if (dic.ContainsKey(key)) dic[key] = val;
                else dic.Add(key, val);
            }
            return dic;
        }

        public static Dictionary<TKEY, T> ReplaceAtSurpressFunc<TKEY, T>(this Dictionary<TKEY, T> dic, TKEY key, Util.DelegateParam<T, T> val)
        {
            if (dic != null)
            {
                if (dic.ContainsKey(key)) dic[key] = val(dic[key]);
                else dic.Add(key, val(default(T)));
            }
            return dic;
        }


        public static bool IsIncluded_Inclusive<T>(this T i, T min, T max) where T : IComparable
        {
            if (min.CompareTo(i) > 0) return false;
            if (max.CompareTo(i) < 0) return false;
            return true;
        }

        public static bool IsIncluded_Exclusive<T>(this T i, T min, T max) where T : IComparable
        {
            if (min.CompareTo(i) >= 0) return false;
            if (max.CompareTo(i) <= 0) return false;
            return true;
        }


        public static TRETURN GetSmart<T, TRETURN>(this T c, Util.DelegateParam<TRETURN, T> getter)
        {
            if (c == null) return default(TRETURN);
            else return getter(c);
        }

        public static TRETURN GetSmart<T, TRETURN>(this T c, Util.DelegateParam<TRETURN, T> getter, TRETURN default_value)
        {
            if (c == null) return default_value;
            else return getter(c);
        }

        public static T GetComponentSmart<T>(this Component c) where T : Component
        {
            if (c == null) return null;
            return c.GetComponent<T>();
        }

        public static T GetComponentSmart<T>(this GameObject c) where T : Component
        {   
            if (c == null) return null;
            return c.GetComponent<T>();
        }

        public static List<T> GetComponentsSmart<T>(this Component c) where T : Component
        {
            if (c == null) new List<T>();
            return new List<T>(c.GetComponents<T>());
        }

        public static List<T> GetComponentsSmart<T>(this GameObject c) where T : Component
        {
            if (c == null) new List<T>();
            return new List<T>(c.GetComponents<T>());
        }

        public static CoroutineClass VPlayDangled(this IEnumerator ie, GameObject mb, CoroutineClass.WhenDangledNotAcitve how = CoroutineClass.WhenDangledNotAcitve.Pause)
        {
            return scCoroutine.Instance.Begin_Dangled(mb, ie, how);
        }

        public static CoroutineClass VPlayDangled(this IEnumerator ie, MonoBehaviour mb, CoroutineClass.WhenDangledNotAcitve how = CoroutineClass.WhenDangledNotAcitve.Pause)
        {
            return scCoroutine.Instance.Begin_Dangled(mb, ie, how);
        }

        public static CoroutineClass VPlay(this IEnumerator ie)
        {
            return scCoroutine.Instance.Begin(ie);
        }
        public static void DestroyGameObject(this Component c)
        {
            if (c != null)
                GameObject.Destroy(c.gameObject);
        }

        public static void DestroyGameObject(this GameObject c)
        {
            if (c != null)
                GameObject.Destroy(c);
        }

        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        public static List<string> SmartSplitString(this object o, char delimiter)
        {
            if (o == null)
                return new List<string>();
            var s = o.ToString();
            if (string.IsNullOrEmpty(s))
                return new List<string>();
            else
                return new List<string>(s.Split(delimiter));
        }

        public static Vector3 DistanceTo(this GameObject g, GameObject t, Space s = Space.World)
        {
            if (s == Space.Self)
                return t.transform.localPosition - g.transform.localPosition;
            else
                return t.transform.position - g.transform.position;
        }

        public static Vector3 DistanceTo(this GameObject g, Component t, Space s = Space.World)
        {
            if (s == Space.Self)
                return t.transform.localPosition - g.transform.localPosition;
            else
                return t.transform.position - g.transform.position;
        }

        public static Vector3 DistanceTo(this Component g, Component t, Space s = Space.World)
        {
            if (s == Space.Self)
                return t.transform.localPosition - g.transform.localPosition;
            else
                return t.transform.position - g.transform.position;
        }

        public static Vector3 DistanceTo(this Component g, GameObject t, Space s = Space.World)
        {
            if (s == Space.Self)
                return t.transform.localPosition - g.transform.localPosition;
            else
                return t.transform.position - g.transform.position;
        }

        public static Vector3 DistanceTo(this GameObject g, Vector3 t, Space s = Space.World)
        {
            if (s == Space.Self)
                return t - g.transform.localPosition;
            else
                return t - g.transform.position;
        }

        public static Vector3 DistanceTo(this Component g, Vector3 t, Space s = Space.World)
        {
            if (s == Space.Self)
                return t - g.transform.localPosition;
            else
                return t - g.transform.position;
        }

        public static T1 GetMinimum<T1, T2>(this List<T1> ls, Util.DelegateParam<T2, T1> value_returener) where T2 : IComparable
        {
            T1 rtv = ls[0];
            T2 current_t2 = value_returener(rtv);

            if (ls.Count == 1)
                return rtv;

            for (int i = 0; i < ls.Count; ++i)
            {
                var current = value_returener(ls[i]);
                if (current_t2.CompareTo(current) > 0)
                {
                    rtv = ls[i];
                    current_t2 = current;
                }
            }

            return rtv;
        }

        public static T1 GetMaximum<T1, T2>(this List<T1> ls, Util.DelegateParam<T2, T1> value_returener) where T2 : IComparable
        {
            T1 rtv = ls[0];
            T2 current_t2 = value_returener(rtv);

            if (ls.Count == 1)
                return rtv;

            for (int i = 0; i < ls.Count; ++i)
            {
                var current = value_returener(ls[i]);
                if (current_t2.CompareTo(current) < 0)
                {
                    rtv = ls[i];
                    current_t2 = current;
                }
            }

            return rtv;
        }
        public static string NGUIColored(this object o, string nguicolor)
        {
            return nguicolor + o.ToString() + "[-]";
        }

        public static string str(this object o)
        {
            return "(임시)" + o.ToString();
        }

        public static string str(this object o, params object[] os)
        {
            return "(임시)" + string.Format(o.ToString(), os);
        }

        public static bool HasComponent<T>(this Component c) where T : Component
        {
            return c.GetComponent<T>() != null;
        }

        public static bool HasComponent<T>(this GameObject c) where T : Component
        {
            return c.GetComponent<T>() != null;
        }

        public static void RunIfNotNull(this Util.DelegateVoid0Param f)
        {
            if (f != null) f();
        }

        public static void RunIfNotNull(this Util.DelegateVoidNoParam f)
        {
            if (f != null) f();
        }

        public static List<T> CopyListFrom<T>(this List<T> me, List<T> from)
        {
            if (me == from) return me;
            me.Clear();
            foreach (var v in from) me.Add(v);
            return me;
        }

        public static T nop<T>(this T t)
        {
            return t;
        }

        public static T GetMedian<T>(this List<T> ls, Comparison<T> comparer)
        {
            ls.Sort(comparer);

            //1개 = 0idx
            //2개 = 1idx
            //3개 = 0,1,2 2/2 = 1
            //4개 = 3/2 = 1
            //5개 = 4/2 = 2, 0,1,2,3,4
            //16개 = 15/2 = 7 = 0123456 7 89012345
            //17개 = 16/2 = 8 = 01234567 8 90123456
            //200개 = 199/2 = 99
            return ls[ls.Count / 2];
        }

        public static double ToDouble(this object str, double default_value = 0)
        {
            double f = 0;
            try
            {
                f = System.Convert.ToDouble(str);
                return f;
            }
            catch (System.Exception e)
            {
                return default_value;
            }
        }

        public static float ToFloat(this object str, float default_value = 0)
        {
            float f = 0;
            try
            {
                f = System.Convert.ToSingle(str);
                return f;
            }
            catch (System.Exception e)
            {
                return default_value;
            }
        }

        public static int ToInt(this object str, int default_value = 0)
        {
            int i = 0;
            try
            {
                i = System.Convert.ToInt32(str);
                return i;
            }
            catch (System.Exception e)
            {
                return default_value;
            }
        }

        public static System.Int64 ToInt64(this object str, System.Int64 default_value = 0)
        {
            System.Int64 i = 0;
            try
            {
                i = System.Convert.ToInt64(str);
                return i;
            }
            catch (System.Exception e)
            {
                return default_value;
            }
        }

        public static List<Transform> ToList(this Transform t)
        {
            List<Transform> rtv = new List<Transform>();
            Transform tt = t;
            foreach (Transform ts in tt)
            {
                rtv.Add(ts);
            }
            return rtv;
        }

        public static void ReplayTween(this UITweener t)
        {
            t.ResetToBeginning();
            t.PlayForward();
        }

        public static void AddIfNotContains<T>(this List<T> ls, T o)
        {
            if (!ls.Contains(o)) ls.Add(o);
        }

        public static void SetActive<T>(this T c, bool b) where T : Component
        {
            if (c != null)
                c.gameObject.SetActive(b);
        }

        public static T GetRandomElem<T>(this List<T> from)
        {
            if (from == null || from.Count == 0) return default(T);
            else if (from.Count == 1) return from[0];
            else
                return from[Util.GetRandomNumberIntInclusive(0, from.Count - 1)];
        }

        public static T GetRandomElem<T>(this T[] from)
        {
            if (from == null || from.Length == 0) return default(T);
            else if (from.Length == 1) return from[0];
            else
                return from[Util.GetRandomNumberIntInclusive(0, from.Length - 1)];
        }

        public static T GetRandomElemExceptBy<T>(this T[] from, params T[] exceptions)
        {
            if (from == null || from.Length == 0) return default(T);
            
            var ls = from.ToList();
            ls.RemoveAll(v => exceptions.Contains(v));

            if (ls.Count == 0) return default(T);
            else if (ls.Count == 1) return ls[1];

            var choosed = ls[Util.GetRandomNumberIntInclusive(0, ls.Count - 1)];
            return choosed;
        }

        public static T GetRandomElemExceptBy<T>(this List<T> from, params T[] exceptions)
        {
            if (from == null || from.Count == 0) return default(T);

            var ls = from.ToList();
            ls.RemoveAll(v => exceptions.Contains(v));

            if (ls.Count == 0) return default(T);
            else if (ls.Count == 1) return ls[1];

            var choosed = ls[Util.GetRandomNumberIntInclusive(0, ls.Count - 1)];
            return choosed;
        }


        /// <summary>
        /// 랜덤한 비중복추출. 다만 count보다 사이즈가 작으면 그냥 다 뽑는다 (셔플의 기능)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="from"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<T> GetRandomElems<T>(this List<T> from, int count)
        {
            List<int> fromsize = new List<int>();
            for (int i = 0; i < from.Count; ++i) fromsize.Add(i);
            int removing_sz = fromsize.Count - count;
            for (int i = 0; i < removing_sz; ++i)
            {
                fromsize.RemoveAt(Util.GetRandomNumberIntInclusive(0, fromsize.Count - 1));
            }
            List<T> rtv = new List<T>();
            foreach (int i in fromsize) rtv.Add(from[i]);

            return rtv;
        }


        public static List<T> ToList<T>(this T[] arr)
        {
            List<T> rtv = new List<T>(arr);
            return rtv;
        }

        public static List<T> VFilter<T>(this IEnumerable<T> array, Predicate<T> pr)
        {
            List<T> rtv = new List<T>();
            foreach (var v in array) if (pr(v)) rtv.Add(v);
            return rtv;
        }

        public static List<TNEW> VFilterMap<T, TNEW>(this IEnumerable<T> array, Predicate<T> pr, Util.DelegateParam<TNEW, T> change)
        {
            List<TNEW> rtv = new List<TNEW>();
            foreach (var v in array) if (pr(v)) rtv.Add(change(v));
            return rtv;
        }

        public static List<TNEW> VFilterMultiMap<T, TNEW>(this IEnumerable<T> array, Predicate<T> pr, Util.DelegateParam<IEnumerable<TNEW>, T> change)
        {
            List<TNEW> rtv = new List<TNEW>();
            foreach (var v in array) if (pr(v)) rtv.AddRange(change(v));
            return rtv;
        }

        public static T VFind<T>(this IEnumerable<T> array, Predicate<T> pr, Boxer<bool> IsSucceeded = null)
        {
            foreach (var v in array)
            {
                if (pr(v))
                {
                    if (IsSucceeded != null) IsSucceeded.value = true;
                    return v;
                }
            }
            if (IsSucceeded != null) IsSucceeded.value = false;
            return default(T);
        }

        public static List<T> VFindAndRemoveAll<T>(this List<T> array, Predicate<T> pr)
        {
            var rtv = array.FindAll(pr);
            foreach(var v in rtv)
                array.RemoveAll(pr);
            return rtv;
        }

        public static List<T> VFindAll<T>(this IEnumerable<T> array, Predicate<T> pr)
        {
            List<T> rtv = new List<T>();
            foreach (var v in array)
            {
                if (pr(v)) rtv.Add(v);
            }
            return rtv;
        }
        public static List<TOUT> VCopyTo<T1, TOUT>(this List<T1> me, List<TOUT> to, Util.DelegateParam<TOUT, T1> Matcher)
        {
            to.Clear();
            foreach (var v in me) to.Add(Matcher(v));
            return to;
        }

        public static Dictionary<T1, List<T2>> VMapDictionary<T1, T2>(this List<T2> me, Util.DelegateParam<T1, T2> Matcher)
        {
            Dictionary<T1, List<T2>> rtv = new Dictionary<T1, List<T2>>();
            foreach (var v in me)
            {
                var key = Matcher(v);
                rtv.GetOrCreate(key, () => new List<T2>()).Add(v);
            }
            return rtv;
        }
        
        public static List<List<T1>> VMapList<T1, T2>(this List<T1> me, Util.DelegateParam<T2, T1> Matcher)
        {
            var dic = VMapDictionary(me, Matcher);
            List<List<T1>> rtv = new List<List<T1>>();
            foreach (var kv in dic) rtv.Add(kv.Value);
            return rtv;
        }

        public static List<TOUT> VFlatMap<T1, TOUT>(this IEnumerable<T1> me, Util.DelegateParam<IEnumerable<TOUT>, T1> Matcher)
        {
            List<TOUT> tlist = new List<TOUT>();
            foreach (var v in me) tlist.Add(Matcher(v));
            return tlist;
        }

        public static int VMatchCount<T1, T2>(this IEnumerable<T1> t1, IEnumerable<T2> t2, Util.DelegateParam<bool, T1, T2> Matcher)
        {
            int count = 0;
            foreach (var v1 in t1)
            {
                foreach (var v2 in t2)
                {
                    if (Matcher(v1, v2)) ++count;
                }
            }
            return count;
        }

        public static List<KeyValuePair<TKey, TValue>> ToList<TKey, TValue>(this Dictionary<TKey, TValue> dic)
        {
            List<KeyValuePair<TKey, TValue>> rtv = new List<KeyValuePair<TKey, TValue>>();
            foreach (var v in dic) rtv.Add(v);
            return rtv;
        }

        public static Dictionary<TKey, TValue> FindAll<TKey, TValue>(this Dictionary<TKey, TValue> dic, Predicate<KeyValuePair<TKey, TValue>> pre)
        {
            Dictionary<TKey, TValue> rtv = new Dictionary<TKey, TValue>();
            foreach (KeyValuePair<TKey, TValue> kv in dic)
            {
                if (pre(kv))
                    rtv.Add(kv.Key, kv.Value);
            }
            return rtv;
        }

        public static void RemoveAll<TKey, TValue>(this Dictionary<TKey, TValue> dic, Predicate<KeyValuePair<TKey, TValue>> pre)
        {
            List<KeyValuePair<TKey, TValue>> removelist = new List<KeyValuePair<TKey, TValue>>();
            foreach (KeyValuePair<TKey, TValue> kv in dic)
            {
                if (pre(kv))
                    removelist.Add(kv);
            }
            foreach (var v in removelist)
            {
                dic.Remove(v.Key);
            }
        }

        public static void UpdateTo<Tkey, TValue>(this Dictionary<Tkey, TValue> dic, Tkey key, TValue value)
        {
            if (dic.ContainsKey(key))
                dic[key] = value;
            else
                dic.Add(key, value);
        }
        


        public static GameObject ResetUISpriteAnimation(this GameObject go)
        {
            if (go != null)
            {
                var all_sprite = Util.FindAllInHierarchyRecuvely<UISpriteAnimation>(go);
                foreach (var v in all_sprite)
                {
                    var spr = v;
                    spr.ResetToBeginning();
                    spr.enabled = true;
                    spr.Play();
                }
            }
            return go;
        }
        public static bool IsNull<T>(this T b)
        {
            return b == null;
        }

        public static bool IsNotNull<T>(this T b)
        {
            return b != null;
        }

        public static T IfNull<T>(this T b, Action<T> action)
        {
            if (b == null)
            {
                action(b);
            }
            return b;
        }

        public static T IfNotNull<T>(this T b, Action<T> action)
        {
            if (b != null)
            {
                action(b);                
            }
            return b;
        }

        public static TRETURN GetIfOrElse<T, TRETURN>(this T b, Util.DelegateParam<bool, T> condition, Util.DelegateParam<TRETURN, T> iftrue, Util.DelegateParam<TRETURN, T> iffalse)
        {
            if (condition(b))
                return iftrue(b);
            else
                return iffalse(b);
        }

        public static TRETURN GetIfNotNull<T, TRETURN>(this T b, Util.DelegateParam<TRETURN, T> action)
        {
            if (b != null)
            {
                return action(b);
            }
            else
            {
                return default(TRETURN);
            }
        }

        public static TRETURN GetIfNotNullOrElse<T, TRETURN>(this T b, Util.DelegateParam<TRETURN, T> action, Util.DelegateParam<TRETURN, T> actionelse)
        {
            if (b != null)
            {
                return action(b);
            }
            else
            {
                return actionelse(b);
            }
        }

        /// <summary>
        /// Action만 하고 Return은 무조건 원본그대로
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="b"></param>
        /// <param name="condition"></param>
        /// <param name="iftrue"></param>
        /// <returns></returns>
        public static T DoIf<T>(this T b, Util.DelegateParam<bool, T> condition, Util.DelegateVoid1Param<T> iftrue, Util.DelegateVoid1Param<T> iffalse = null)
        {
            if (condition(b))
                iftrue(b);
            else
                if (iffalse != null) iffalse(b);
            return b;
        }

        /// <summary>
        /// else가 null일때는 원본을 그대로 리턴
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="b"></param>
        /// <param name="condition"></param>
        /// <param name="iftrue"></param>
        /// <returns></returns>
        public static T ChangeIf<T>(this T b, Util.DelegateParam<bool, T> condition, Util.DelegateParam<T, T> iftrue, Util.DelegateParam<T, T> iffalse = null)
        {
            if (condition(b))
                return iftrue(b);
            else
            {
                if (iffalse != null) return iffalse(b);
                else return b;
            }
        }


        public static bool IfTrue(this bool b, Util.DelegateVoid0Param action)
        {
            if (b) action();
            return b;
        }

        public static bool IfFalse(this bool b, Util.DelegateVoid0Param action)
        {
            if (!b) action();
            return !b;
        }

        public static void Else(this bool b, Util.DelegateVoid0Param action)
        {
            if (!b) action();
        }

        public static bool ElseIf(this bool b, Util.DelegateParam<bool> elif_condition, Util.DelegateVoid0Param action)
        {
            var elif = !b && elif_condition();
            if (elif)
                action();
            return elif;
        }

        public static List<TSource> VWhere<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            List<TSource> rtv = new List<TSource>();
            foreach (var v in source)
            {
                predicate(v);
                rtv.Add(v);
            }
            return rtv;
        }

        public static List<TResult> VSelect<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> calculator)
        {
            List<TResult> lst = new List<TResult>();
            foreach (var v in source)
            {
                lst.Add(calculator(v));
            }
            return lst;
        }

        public static List<TResult> VSelect<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, List<TResult>> calculator)
        {
            List<TResult> lst = new List<TResult>();
            foreach (var v in source)
            {
                foreach (var vv in calculator(v))
                    lst.Add(vv);
            }
            return lst;
        }

        public static string ToStringSmart(this object o)
        {
            if (o == null) return string.Empty;
            else return o.ToString();
        }


        public static TSource VRandomElem<TSource>(this List<TSource> source)
        {
            return source[Util.GetRandomNumberIntInclusive(0, source.Count - 1)];
        }

        public static TSource VRandomElem<TSource>(this TSource[] source)
        {
            return source[Util.GetRandomNumberIntInclusive(0, source.Length - 1)];
        }

        public static TSource VRun<TSource>(this TSource source, params Action<TSource>[] action)
        {
            foreach(var v in action)
                v(source);
            return source;
        }

        public static IEnumerable<TSource> VSet<TSource>(this IEnumerable<TSource> source, TSource value)
        {
            source.VRunEach(v => v = value);
            return source;
        }

        public static List<T> VTrySafeRemoveAt<T>(this List<T> ls, int index)
        {
            if (ls == null || ls.Count <= index || index < 0) return ls;
            ls.RemoveAt(index);
            return ls;
        }

        public static List<TSource> VRunEach<TSource, T>(this List<TSource> source, Util.DelegateParam<T, TSource> action)
        {
            foreach (var v in source) action(v);
            return source;
        }

//         public static List<TSource> VRunEachSmart<TSource, T>(this List<TSource> source, Util.DelegateParam<T, TSource> action)
//         {
//             if (!action.Method.IsStatic && action.Method.DeclaringType.GetSmart(v => v.FullName, "") == typeof(TSource).FullName)
//             {
//                 foreach (var v in source)
//                     action.Invoke(v);
//             }
//             return source;
//         }
// 
//         public static List<TSource> VRunEachSmart<TSource>(this List<TSource> source, Util.DelegateVoid1Param<TSource> action)
//         {
//             if (!action.Method.IsStatic && action.Method.DeclaringType.GetSmart(v => v.FullName, "") == typeof(TSource).FullName)
//             {
//                 foreach (var v in source)
//                     action.Invoke(v);
//             }
//             return source;
//         }

        public static TSource[] VRunEachIndexed<TSource>(this TSource[] source, Util.DelegateVoidParam<int, TSource> action)
        {
            for(int i = 0; i < source.Length; ++i)
            {
                action(i, source[i]);
            }
            return source;
        }

        public static List<TSource> VRunEachIndexed<TSource>(this List<TSource> source, Util.DelegateVoidParam<int, TSource> action)
        {
            for (int i = 0; i < source.Count; ++i)
            {
                action(i, source[i]);
            }
            return source;
        }

        public static TSource[] VRunEach<TSource>(this TSource[] source, Action<TSource> action)
        {
            foreach (var v in source)
            {
                action(v);
            }
            return source;
        }

        public static List<TSource> VRunEach<TSource>(this List<TSource> source, Action<TSource> action)
        {
            foreach (var v in source)
            {
                action(v);
            }
            return source;
        }

        public static IEnumerable<TSource> VRunEach<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
        {
            foreach (var v in source)
            {
                action(v);
            }
            return source;
        }

        /// <summary>
        /// First는 다르게 실행하는 VRunEach
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="firstAction"></param>
        /// <param name="normalAction"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> VRunEachWithFirst<TSource>(this IEnumerable<TSource> source, Action<TSource> firstAction, Action<TSource> normalAction)
        {
            if (source.Count() != 0)
            {
                var first = source.First();

                foreach (var v in source)
                {
                    if (v.Equals(first))
                        firstAction(v);
                    else
                        normalAction(v);
                }
            }
            return source;
        }

        /// <summary>
        /// Last는 다르게 실행하는 VRunEach
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="normalAction"></param>
        /// <param name="lastAction"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> VRunEachWithLast<TSource>(this IEnumerable<TSource> source, Action<TSource> normalAction, Action<TSource> lastAction)
        {
            if (source.Count() != 0)
            {
                var last = source.Last();

                foreach (var v in source)
                {
                    if (v.Equals(last))
                        lastAction(v);
                    else
                        normalAction(v);
                }
            }
            return source;
        }

        public static Dictionary<TKey, TValue> VRun<TKey, TValue>(this Dictionary<TKey, TValue> source, Action<KeyValuePair<TKey, TValue>> action)
        {
            foreach (var v in source)
            {
                action(v);
            }
            return source;
        }

        public static TSource VRunIf<TSource>(this TSource source, Predicate<TSource> _if, Action<TSource> _if_do, Action<TSource> _else_do = null)
        {
            if (_if(source))
                _if_do(source);
            else if(_else_do != null)
                _else_do(source);
            return source;
        }

        public static TSource VRunIfNotNull<TSource>(this TSource source, params Action<TSource>[] action)
        {
            if (source != null)
            {
                foreach(var v in action)
                    v(source);
            }
            return source;
        }

        //     public static TReturn VRun<TSource, TReturn>(this TSource source, Util.DelegateParam<TReturn, TSource> action)
        //     {
        //         return action(source);
        //     }

        /// <summary>
        /// return value : source가 null인지
        /// 즉, action이 실행되었는지를 알려줌.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static bool VRunIfNull<TSource>(this TSource source, Util.DelegateVoid0Param action)
        {
            if (source == null)
                action();
            return source == null;
        }

        public static IEnumerable<TSource> VInitForEach<TSource>(this IEnumerable<TSource> source, params Util.DelegateVoidParam<TSource>[] action)
        {
            foreach (var s in source)
            {
                foreach (var a in action) a(s);
            }            
            return source;
        }

        public static TSource VInit<TSource>(this TSource source, params Util.DelegateVoidParam<TSource>[] action)
        {
            foreach (var a in action)
                a(source);
            return source;
        }

        public static TSource VRun<TSource, RTV>(this TSource source, Util.DelegateParam<RTV, TSource> action)
        {
            action(source);
            return source;
        }

        public static TSource VRunIfNotNull<TSource>(this TSource source, Util.DelegateVoid0Param action)
        {
            if (source != null)
                action();
            return source;
        }
        
        public static List<T> Add<T>(this List<T> body, IEnumerable<T> adder)
        {
            foreach (var v in adder) body.Add(v);
            return body;
        }

        public static int VReduceIntDictionary<TKEY, TVALUE>(this Dictionary<TKEY, TVALUE> body, Util.DelegateParam<int, KeyValuePair<TKEY, TVALUE>> converter)
        {
            int sum = 0;
            foreach (var v in body)
            {
                sum += converter(v);
            }
            return sum;
        }

        public static int VReduceInt<TTarget>(this IEnumerable<TTarget> body, Util.DelegateParam<int, TTarget> converter)
        {
            int sum = 0;
            foreach (var v in body)
            {
                sum += converter(v);
            }
            return sum;
        }
        
        public static float VReduceFloat<TTarget>(this IEnumerable<TTarget> body, Util.DelegateParam<float, TTarget> converter)
        {
            float sum = 0;
            foreach (var v in body)
            {
                sum += converter(v);
            }
            return sum;
        }

        public static string VReduceString<TTarget>(this IEnumerable<TTarget> body, Util.DelegateParam<string, TTarget> converter)
        {

            StringBuilder sb = new StringBuilder();
            foreach (var v in body)
            {
                sb.Append(converter(v));
            }
            return sb.ToString();
        }


        /// <summary>
        /// 리스트를 타입변환한다.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="body">바꿀놈</param>
        /// <param name="converter">각 값을 바꿀 함수</param>
        /// <returns></returns>
        public static List<TTarget> VConvert<TSource, TTarget>(this IEnumerable<TSource> body, Util.DelegateParam<TTarget, TSource> converter)
        {
            List<TTarget> rtv = new List<TTarget>();
            foreach (var v in body) rtv.Add(converter(v));
            return rtv;
        }

    }

}