using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ZO
{
    //boxer 들
    public class Boxer<T>
    {
        public static implicit operator T(Boxer<T> b)
        {
            return b.value;
        }

        public override string ToString()
        {
            return value.ToString();
        }
        public T value;
        public Boxer(T t)
        {
            Set(t);
        }
        public void Set(T t)
        {
            value = t;
        }
        public Boxer<T> Clone()
        {
            return new Boxer<T>(value);
        }
    }

    public class intbox : Boxer<int>
    {
        public intbox(int i) : base(i)
        {

        }

        public intbox()
            : base(0)
        {

        }
    }

    public class strbox : Boxer<string>
    {
        public strbox(string i) : base(i)
        {

        }

        public strbox()
            : base(string.Empty)
        {

        }
    }

    public class box : Boxer<object>
    {
        public box(object o) : base(o)
        {

        }

        public box()
            : base(null)
        {

        }
    }


    public static partial class Util
    {

        public class FileName
        {
            public string FullPath;
            public string NameOnly;
            public string NameOnlyWithExt;
            public string PathOnly;
            public string ExtensionOnly;

            public FileName(string full_path)
            {
                FullPath = full_path.Replace('\\', '/').Replace("//", "/");
                var ls = FullPath.Split('/').ToList();
                NameOnlyWithExt = ls.GetLast();
                var lsnameonly = NameOnlyWithExt.Split('.').ToList();
                ExtensionOnly = lsnameonly.GetLast();
                lsnameonly.RemoveAt(lsnameonly.Count - 1);
                NameOnly = lsnameonly.MergeListToString(".");
                ls.RemoveAt(ls.Count - 1);
                PathOnly = ls.MergeListToString("/");
            }

            public override string ToString()
            {
                return FullPath;
            }
        }



        public static void VRunWithCount(int count, Util.DelegateVoid0Param func)
        {
            for (int i = 0; i < count; ++i)
                func();
        }

        public static List<T> FindAllInCurrentScene<T>() where T : Component
        {
            List<T> rtv = new List<T>();
            var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            var root = scene.GetRootGameObjects();
            foreach (var v in root)
            {
                rtv.Add(v.FindAllInHierarchyRecuvely<T>());
            }
            return rtv;
        }

        public static Color GetSpriteAverageColor(UISprite spr)
        {
            Texture2D t2d = spr.atlas.texture as Texture2D;
            int width = spr.GetAtlasSprite().width;
            int height = spr.GetAtlasSprite().height;


            Vector3 agv = Vector3.zero;
            int count = 0;

            int x_jump_amount = (int)(width/10.0f);
            int y_jump_amount = (int)(height / 10.0f);
            if (x_jump_amount == 0) x_jump_amount = 1;
            if (y_jump_amount == 0) y_jump_amount = 1;

            for (int x = spr.GetAtlasSprite().x; x < width; x += x_jump_amount)
            {
                for (int y = spr.GetAtlasSprite().y; y < height; y += y_jump_amount)
                {
                    ++count;
                    Color color = t2d.GetPixel(x, y);
                    agv.x *= (color.r * color.a);
                    agv.y *= (color.g * color.a);
                    agv.z *= (color.b * color.a);
                }
            }

            agv /= count;
            var c = new Color(agv.x, agv.y, agv.z);
            return c;
        }

        public static T SelectIfOrElse<T>(DelegateParam<bool, T, T> det, T t1, T t2)
        {
            if (det(t1, t2))
            {
                return t1;
            }
            else
                return t2;
        }



        public static string ToReadableString(this System.TimeSpan s)
        {
            if (s.Days >= 36500)
            {
                int century = (int)(s.Days / 36500.0f);
                int year = (int)((s.Days - 36500 * century) / 365.0f);
                return string.Format(LocaleTable.Instance.Get("CenturyYear"), century, year);
            }
            else if (s.Days >= 365)
            {
                int year = (int)(s.Days / 365.0f);
                int month = (int)((s.Days - 365 * year) / 30.0f);
                return string.Format(LocaleTable.Instance.Get("YearMonth"), year, month);
            }
            else if (s.Days >= 30)
            {
                int month = (int)(s.Days / 30.0f);
                int week = (int)((s.Days - 30 * month) / 7.0f);
                return string.Format(LocaleTable.Instance.Get("MonthWeek"), month, week);
            }
            else if (s.Days >= 7)
            {
                int week = (int)(s.Days / 7);
                int day = (int)((s.Days - 7 * week));
                return string.Format(LocaleTable.Instance.Get("WeekDay"), week, day);
            }
            else if (s.Days >= 1)
            {
                return string.Format(LocaleTable.Instance.Get("DayHour"), s.Days, s.Hours);
            }
            else if (s.Hours >= 1)
            {
                return string.Format(LocaleTable.Instance.Get("HourMin"), s.Hours, s.Minutes);
            }
            else if (s.Minutes >= 5)
            {
                return string.Format(LocaleTable.Instance.Get("Min"), s.Minutes);
            }
            else if (s.Minutes >= 1)
            {
                return string.Format(LocaleTable.Instance.Get("MinSec"), s.Minutes, s.Seconds);
            }
            //else if(s.Seconds >= 1)
            else
            {
                return string.Format(LocaleTable.Instance.Get("Sec"), s.Seconds);
            }
        }

        public static T ArgMax<T>(DelegateParam<float, T> det, params T[] tt)
        {
            if (tt == null || tt.Length == 0) return default(T);

            var val = tt[0];
            float max = det(val);
            for (int i = 1; i < tt.Length; ++i)
            {
                var v = tt[i];
                float current = det(v);
                if (current > max)
                {
                    max = current;
                    val = v;
                }
            }
            return val;
        }

        public static T ArgMin<T>(DelegateParam<float, T> det, params T[] tt)
        {
            if (tt == null || tt.Length == 0) return default(T);

            var val = tt[0];
            float min = det(val);
            for (int i = 1; i < tt.Length; ++i)
            {
                var v = tt[i];
                float current = det(v);
                if (current < min)
                {
                    min = current;
                    val = v;
                }
            }
            return val;
        }

        public static T ArgMax<T>(DelegateParam<int, T> det, params T[] tt)
        {
            if (tt == null || tt.Length == 0) return default(T);

            var val = tt[0];
            int max = det(val);            
            for(int i = 1; i < tt.Length; ++i)
            {
                var v = tt[i];
                int current = det(v);
                if(current > max)
                {
                    max = current;
                    val = v;
                }
            }
            return val;
        }

        public static T ArgMin<T>(DelegateParam<int, T> det, params T[] tt)
        {
            if (tt == null || tt.Length == 0) return default(T);

            var val = tt[0];
            int min = det(val);
            for (int i = 1; i < tt.Length; ++i)
            {
                var v = tt[i];
                int current = det(v);
                if (current < min)
                {
                    min = current;
                    val = v;
                }
            }
            return val;
        }

        public static T ArgMax<T>(DelegateParam<System.Int64, T> det, params T[] tt)
        {
            if (tt == null || tt.Length == 0) return default(T);

            var val = tt[0];
            System.Int64 max = det(val);
            for (int i = 1; i < tt.Length; ++i)
            {
                var v = tt[i];
                System.Int64 current = det(v);
                if (current >= max)
                {
                    max = current;
                    val = v;
                }
            }
            return val;
        }

        public static T ArgMin<T>(DelegateParam<System.Int64, T> det, params T[] tt)
        {
            if (tt == null || tt.Length == 0) return default(T);

            var val = tt[0];
            System.Int64 min = det(val);
            for (int i = 1; i < tt.Length; ++i)
            {
                var v = tt[i];
                System.Int64 current = det(v);
                if (current < min)
                {
                    min = current;
                    val = v;
                }
            }
            return val;
        }

        public static float Random_n1Top1_Inclusive()
        {
            return Random.value - Random.value;
        }

        public static RTV Closure<RTV>(DelegateParam<RTV> d)
        {
            return d();
        }

        public static Struct<T1, T2> MakeTuple<T1, T2>(T1 t1, T2 t2)
        {
            return new Struct<T1, T2>(t1, t2);
        }

        public static Struct<T1, T2, T3> MakeTuple<T1, T2, T3>(T1 t1, T2 t2, T3 t3)
        {
            return new Struct<T1, T2, T3>(t1, t2, t3);
        }
        public static Struct<T1, T2, T3, T4> MakeTuple<T1, T2, T3, T4>(T1 t1, T2 t2, T3 t3, T4 t4)
        {
            return new Struct<T1, T2, T3, T4>(t1, t2, t3, t4);
        }
        public static Struct<T1, T2, T3, T4, T5> MakeTuple<T1, T2, T3, T4, T5>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
        {
            return new Struct<T1, T2, T3, T4, T5>(t1, t2, t3, t4, t5);
        }

        public static string ClipBoard
        {
            get
            {
                return GUIUtility.systemCopyBuffer;
            }
            set
            {
                GUIUtility.systemCopyBuffer = value;
            }
        }

        public static string MergeListToString<T>(this List<T> iiii, string delimeter = ",", System.Text.StringBuilder usethis = null)
        {
            string ltv = "";
            if (iiii.Count == 0)
                return ltv;

            for (int i = 0; i < iiii.Count; ++i)
            {
                ltv += iiii[i].ToString();
                if (i < iiii.Count - 1)
                    ltv += delimeter;
            }
            return ltv;
        }

        public class Struct<T1, T2>
        {
            public T1 v1;
            public T2 v2;
            public Struct() { }
            public Struct(T1 _t1, T2 _t2) { v1 = _t1; v2 = _t2; }
        }

        public class Struct<T1, T2, T3>
        {
            public T1 v1;
            public T2 v2;
            public T3 v3;
            public Struct() { }
            public Struct(T1 _t1, T2 _t2, T3 _t3) { v1 = _t1; v2 = _t2; v3 = _t3; }
        }

        public class Struct<T1, T2, T3, T4>
        {
            public T1 v1;
            public T2 v2;
            public T3 v3;
            public T4 v4;
            public Struct() { }
            public Struct(T1 _t1, T2 _t2, T3 _t3, T4 _t4) { v1 = _t1; v2 = _t2; v3 = _t3; v4 = _t4; }
        }

        public class Struct<T1, T2, T3, T4, T5>
        {
            public T1 v1;
            public T2 v2;
            public T3 v3;
            public T4 v4;
            public T5 v5;
            public Struct() { }
            public Struct(T1 _t1, T2 _t2, T3 _t3, T4 _t4, T5 _t5) { v1 = _t1; v2 = _t2; v3 = _t3; v4 = _t4; v5 = _t5; }
        }

        public static int Min(int i1, int i2)
        {
            if (i1 > i2)
            {
                return i2;
            }
            else return i1;
        }
        
        public static T Max<T>(T t1, T t2)
        {
            return (Comparer<T>.Default.Compare(t1, t2) > 0) ? t1 : t2;
        }

        public static bool IsDaySame(System.DateTime now, System.DateTime last)
        {
            if (now.Day != last.Day) return false;
            if (now.Month != last.Month) return false;
            if (now.Year != last.Year) return false;
            return true;
        }

        public static T Min<T>(T t1, T t2)
        {
            return (Comparer<T>.Default.Compare(t1, t2) > 0) ? t2 : t1;
        }

        static List<char> _alphabets = new List<char>();

        public static T RandomGet<T>(params T[] p)
        {
            if (p == null || p.Length == 0) return default(T);
            if (p.Length == 1) return p[0];
            return p[Util.GetRandomNumberIntInclusive(0, p.Length - 1)];
        }

        public static List<string> FindFilesRecursively(string dir, List<string> usethis = null)
        {
            if (usethis == null)
                usethis = new List<string>();

            {
                if (System.IO.Directory.Exists(dir))
                {
                    var ls = System.IO.Directory.GetFiles(dir);
                    foreach (var v in ls) usethis.Add(v);
                }
            }
            {
                if (System.IO.Directory.Exists(dir))
                {
                    var ls = System.IO.Directory.GetDirectories(dir);
                    foreach (var v in ls)
                        FindFilesRecursively(v, usethis);
                }
            }
            return usethis;
        }

        public static List<string> FindSubDirectioriesRecursively(string dir, List<string> usethis = null)
        {
            if (usethis == null)
                usethis = new List<string>();

            {
                if (System.IO.Directory.Exists(dir))
                {
                    var ls = System.IO.Directory.GetDirectories(dir);
                    foreach (var v in ls) usethis.Add(v);
                    foreach (var v in ls) FindSubDirectioriesRecursively(v, usethis);
                }
            }
            return usethis;
        }


        public static bool DisposeAndNull(ref System.IDisposable d_o)
        {
            if (d_o != null)
            {
                d_o.Dispose();
                d_o = null;
                return true;
            }
            return false;
        }

        static System.Random intRandomizer = null;
        public static int GetRandomNumberIntInclusive(int min, int max)
        {
            if (min >= max) return min;

            int addr = 0;
            if (min < 0)
                addr = -min;

            min += addr;
            max += addr;

            if (intRandomizer == null)
                intRandomizer = new System.Random();

            return intRandomizer.Next() % ((max + 1) - min) + min - addr;
        }


        /// <summary>
        /// 풀 타입 이름을 얻어낸다. 단 `1[[ 이나 + 등 필요없는건 전부 없앤다
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetFriendlyFullTypeName(this System.Type type)
        {
            if (type.IsGenericParameter)
            {
                return GetFriendlyFullTypeName(type);
            }

            if (!type.IsGenericType)
            {
                return type.FullName.Replace("+", ".");
            }

            var builder = new System.Text.StringBuilder();
            var name = type.Name;
            var index = name.IndexOf("`");
            builder.AppendFormat("{0}.{1}", type.Namespace, name.Substring(0, index));
            builder.Append('<');
            var first = true;
            foreach (var arg in type.GetGenericArguments())
            {
                if (!first)
                {
                    builder.Append(',');
                }
                builder.Append(GetFriendlyFullTypeName(arg));
                first = false;
            }
            builder.Append('>');
            return builder.ToString();
        }

        /// <summary>
        /// 해당 type의 모든 subclass 타입을 찾는다
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<System.Type> FindSubClassesOf<T>()
        {
            List<System.Type> rtv = new List<System.Type>();

            var assems = System.AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assems)
            {
                var typelist = assembly.GetTypes();
                foreach (var t in typelist)
                {
                    if (t.IsSubclassOf(typeof(T)))
                        rtv.Add(t);
                }
            }

            return rtv;
        }
        

        
        public static Transform Put2DOnlyUsingTransform(this Transform p1, Vector3 pos, Space space = Space.World)
        {
            if (p1 != null)
            {
                if (space == Space.World) p1.position = new Vector3(pos.x, pos.y, p1.position.z);
                else p1.localPosition = new Vector3(pos.x, pos.y, p1.position.z);
            }
            return p1;
        }

        public static T Put2DOnly<T>(this T p1, Vector3 pos, Space space = Space.World) where T : Component
        {
            if (p1 != null)
                Put2DOnlyUsingTransform(p1.transform, pos, space);
            return p1;
        }
        

        /// <summary>
        /// Put2DOnly 상대방 위치에 직접 놓는 방법
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="target"></param>
        /// <param name="space"></param>
        /// <returns></returns>
        public static T Put2DOnly<T>(this T p1, GameObject target, Space space = Space.World) where T : Component
        {
            if (p1 != null && target != null)
            {
                if (space == Space.World)
                    Put2DOnlyUsingTransform(p1.transform, target.transform.position, space);
                else
                    Put2DOnlyUsingTransform(p1.transform, target.transform.localPosition, space);
            }
            return p1;
        }

        public static T Put2DOnly<T>(this T p1, Component target, Space space = Space.World) where T : Component
        {
            Put2DOnly(p1, target.gameObject, space);
            return p1;
        }
        

        public static List<T> MergeList<T>(params List<T>[] ls)
        {
            List<T> rtv = new List<T>();
            foreach (var v in ls)
            {
                foreach (var vv in v)
                {
                    rtv.Add(vv);
                }
            }
            return rtv;
        }

        public static Dictionary<T1, T2> Clone<T1, T2>(Dictionary<T1, T2> dic)
        {
            var d2 = new Dictionary<T1, T2>();
            foreach (var v in dic) d2.Add(v.Key, v.Value);
            return d2;
        }

        public static List<T> Clone<T>(List<T> ls)
        {
            var ls2 = new List<T>();
            foreach (var v in ls) ls2.Add(v);
            return ls2;
        }

        public static bool Equals<T>(List<T> ls1, List<T> ls2)
        {
            if (ls1.Count != ls2.Count) return false;
            for (int i = 0; i < ls1.Count; ++i)
            {
                if ((ls1[i] == null && ls2[i] != null)
                    || (ls1[i] != null && ls2[i] == null))
                {
                    return false;
                }

                if (!ls1[i].Equals(ls2[i])) return false;
            }
            return true;
        }


        public static string GetHierarchyName(this Component g)
        {
            if (g == null)
                return "null";
            return GetHierarchyName(g.gameObject);
        }

        public static string GetHierarchyName(this GameObject g)
        {
            LinkedList<string> lsStr = new LinkedList<string>();
            Transform t = g.transform;
            while (t != null)
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



        public static string GetHierarchyNameUntil<T>(this Component g) where T : Component
        {
            if (g == null)
                return "null";
            return GetHierarchyNameUntil<T>(g.gameObject);
        }

        public static string GetHierarchyNameUntil<T>(this GameObject g) where T : Component
        {
            LinkedList<string> lsStr = new LinkedList<string>();
            Transform t = g.transform;
            while (t != null && t.GetComponent<T>() == null)
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
        
        /// <summary>
        /// 중복테스트는 하지않는다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        public static List<T> AddList_NoChangeOriginal<T>(List<T> t1, List<T> t2)
        {
            List<T> rtv = new List<T>();
            foreach (T t in t1)
            {
                rtv.Add(t);
            }
            foreach (T t in t2)
            {
                rtv.Add(t);
            }
            return rtv;
        }

        public static T[] AddList_NoChangeOriginal<T>(T[] t1, T[] t2)
        {
            T[] rtv = new T[t1.Length + t2.Length];
            int i = 0;
            for (; i < t1.Length; ++i)
            {
                rtv[i] = t1[i];
            }
            for (int j = 0; j < t2.Length; ++j, ++i)
            {
                rtv[i] = t2[j];
            }
            return rtv;
        }

        public static List<T> AddList_NoChangeOriginal<T>(List<T> t1, T[] t2)
        {
            List<T> rtv = new List<T>();
            foreach (T t in t1)
            {
                rtv.Add(t);
            }
            foreach (T t in t2)
            {
                rtv.Add(t);
            }
            return rtv;
        }

        public static T[] AddList_NoChangeOriginal<T>(T[] t1, List<T> t2)
        {
            T[] rtv = new T[t1.Length + t2.Count];
            int i = 0;
            for (; i < t1.Length; ++i)
            {
                rtv[i] = t1[i];
            }
            for (int j = 0; j < t2.Count; ++j, ++i)
            {
                rtv[i] = t2[j];
            }
            return rtv;
        }

        public static List<T> AddList_WriteResultToMe<T>(List<T> me, List<T> t2)
        {
            foreach (T t in t2)
                me.Add(t);
            return me;
        }

        public static List<T> AddList_WriteResultToMe<T>(List<T> me, T[] t2)
        {
            foreach (var v in t2)
                me.Add(v);
            return me;
        }

        public static List<T> SubtractList_WriteResultToMe<T>(List<T> from, T subaddr)
        {
            from.Remove(subaddr);
            return from;
        }


        public static List<T> SubtractList_NoChangeOriginal<T>(List<T> from, T subaddr)
        {
            var rtv = new List<T>();
            foreach (T t in from)
            {
                if (t == null && subaddr == null)
                    continue;

                if (!t.Equals(subaddr))
                    rtv.Add(t);
            }
            return rtv;
        }

        public static List<T> SubtractList_NoChangeOriginal<T>(List<T> from, List<T> addr)
        {
            List<T> left = new List<T>();
            foreach (T ft in from)
            {
                if (!addr.Contains(ft))
                    left.Add(ft);
            }
            return left;
        }

        public static string TimeSpanToReadableText(System.TimeSpan s)
        {
            if (s.Days >= 36500)
            {
                int century = (int)(s.Days / 36500.0f);
                int year = (int)((s.Days - 36500 * century) / 365.0f);
                return string.Format("{0}세기 {1}년", century, year);
            }
            else if (s.Days >= 365)
            {
                int year = (int)(s.Days / 365.0f);
                int month = (int)((s.Days - 365 * year) / 30.0f);
                return string.Format("{0}년 {1}개월", year, month);
            }
            else if (s.Days >= 30)
            {
                int month = (int)(s.Days / 30.0f);
                int week = (int)((s.Days - 30 * month) / 7.0f);
                return string.Format("{0}개월 {1}주", month, week);
            }
            else if (s.Days >= 7)
            {
                int week = (int)(s.Days / 7);
                int day = (int)((s.Days - 7 * week));
                return string.Format("{0}주 {1}일", week, day);
            }
            else if (s.Days >= 1)
            {
                return string.Format("{0}일 {1}시간", s.Days, s.Hours);
            }
            else if (s.Hours >= 1)
            {
                return string.Format("{0}시간 {1}분", s.Hours, s.Minutes);
            }
            else if (s.Minutes >= 5)
            {
                return string.Format("{0}분", s.Minutes);
            }
            else if (s.Minutes >= 1)
            {
                return string.Format("{0}분 {1}초", s.Minutes, s.Seconds);
            }
            //else if(s.Seconds >= 1)
            else
            {
                return string.Format("{0}초", s.Seconds);
            }
        }

        public class WidgetBounds : System.IDisposable
        {
            public float digonatic_width(Space s)
            {
                if(s == Space.Self)
                    return (left_top.localPosition - right_bottom.localPosition).magnitude;
                else
                    return (left_top.position - right_bottom.position).magnitude;
            }

            public Vector3 CenterPosGlobal
            {
                get
                {
                    return (left_top.position + right_bottom.position)/2;
                }
            }

            public bool IsAABBCross(WidgetBounds w)
            {
                if (
                    (right_top.position.x < w.left_top.position.x || left_top.position.x > w.right_top.position.x)
                    || (right_top.position.y < w.right_bottom.position.y || right_bottom.position.y > w.right_top.position.y)
                    )
                    return false;

                return true;
            }

            public Transform left_top;
            public Transform left_bottom;
            public Transform right_top;
            public Transform right_bottom;

            public float widthLocal
            {
                get
                {
                    return right_top.localPosition.x - left_top.localPosition.x;
                }
            }

            public float heightLocal
            {
                get
                {
                    return left_top.localPosition.y - left_bottom.localPosition.y;
                }
            }

            public override string ToString()
            {
                return string.Format("l:{0}, t:{1}, r:{2}, b:{3}",
                    left_top.transform.localPosition.x.ToString("F2"),
                    left_top.transform.localPosition.y.ToString("F2"),
                    right_bottom.transform.localPosition.x.ToString("F2"),
                    right_bottom.transform.localPosition.y.ToString("F2")
                    );
            }

            public WidgetBounds()
            {
                left_top = new GameObject().transform;
                left_bottom = new GameObject().transform;
                right_top = new GameObject().transform;
                right_bottom = new GameObject().transform;

                left_top.name = "(temp) BoundLeftTop";
                left_bottom.name = "(temp) BoundLeftBottom";
                right_top.name = "(temp) BoundRightTop";
                right_bottom.name = "(temp) BoundRightBottom";

                Clear();
            }

            public void Clear()
            {
                left_top.parent = null;
                right_top.parent = null;
                left_bottom.parent = null;
                right_bottom.parent = null;

                left_top.position = new Vector3(float.MaxValue, float.MinValue, 0);
                right_top.position = new Vector3(float.MinValue, float.MinValue, 0);
                left_bottom.position = new Vector3(float.MaxValue, float.MaxValue, 0);
                right_bottom.position = new Vector3(float.MinValue, float.MaxValue, 0);
            }

            public void SetParent(Component c)
            {
                var t = c.transform;
                left_top.parent = t;
                left_bottom.parent = t;
                right_top.parent = t;
                right_bottom.parent = t;
            }

            public void Calculate(Component c)
            {
                Calculate(c.gameObject);
            }

            public void Calculate(GameObject g, bool EnabledOnly = true)
            {
                Clear();
                left_top.name = "(" + g.name + ") BoundLeftTop";
                left_bottom.name = "(" + g.name + ") BoundLeftBottom";
                right_top.name = "(" + g.name + ") BoundRightTop";
                right_bottom.name = "(" + g.name + ") BoundRightBottom";
                var widgets = Util.GetComponentsInChildrenRecusively<UIWidget>(g, !EnabledOnly);
                foreach (var w in widgets)
                {
                    var wt = w.transform;
                    SetParent(w.transform);
                    Vector3 lt = left_top.localPosition;
                    Vector3 lb = left_bottom.localPosition;
                    Vector3 rt = right_top.localPosition;
                    Vector3 rb = right_bottom.localPosition;

                    float left = 0;
                    float right = 0;
                    float top = 0;
                    float bottom = 0;

                    switch (w.pivot)
                    {
                        case UIWidget.Pivot.Bottom:
                            {
                                left = -w.width / 2 * wt.localScale.x;
                                right = +w.width / 2 * wt.localScale.x;
                                top = +w.height * wt.localScale.y;
                                bottom = 0;
                            }
                            break;
                        case UIWidget.Pivot.BottomLeft:
                            {
                                left = 0;
                                right = +w.width * wt.localScale.x;
                                top = +w.height * wt.localScale.y;
                                bottom = 0;
                            }
                            break;
                        case UIWidget.Pivot.BottomRight:
                            {
                                left = 0;
                                right = +w.width * wt.localScale.x;
                                top = +w.height * wt.localScale.y;
                                bottom = 0;
                            }
                            break;
                        case UIWidget.Pivot.Center:
                            {
                                left = -w.width / 2 * wt.localScale.x;
                                right = +w.width / 2 * wt.localScale.x;
                                top = +w.height / 2 * wt.localScale.y;
                                bottom = -w.height / 2 * wt.localScale.y;
                            }
                            break;
                        case UIWidget.Pivot.Left:
                            {
                                left = 0;
                                right = +w.width * wt.localScale.x;
                                top = +w.height / 2 * wt.localScale.y;
                                bottom = -w.height / 2 * wt.localScale.y;
                            }
                            break;
                        case UIWidget.Pivot.Right:
                            {
                                left = -w.width * wt.localScale.x;
                                right = 0;
                                top = +w.height * wt.localScale.y;
                                bottom = 0;
                            }
                            break;
                        case UIWidget.Pivot.Top:
                            {
                                left = -w.width / 2 * wt.localScale.x;
                                right = +w.width / 2 * wt.localScale.x;
                                top = 0;
                                bottom = -w.height * wt.localScale.y;
                            }
                            break;
                        case UIWidget.Pivot.TopLeft:
                            {
                                left = 0;
                                right = +w.width * wt.localScale.x;
                                top = 0;
                                bottom = -w.height * wt.localScale.y;
                            }
                            break;
                        case UIWidget.Pivot.TopRight:
                            {
                                left = -w.width * wt.localScale.x;
                                right = 0;
                                top = 0;
                                bottom = -w.height * wt.localScale.y;
                            }
                            break;
                    }


                    lt.x = lt.x < left ? lt.x : left;
                    lt.y = lt.y > top ? lt.y : top;
                    lt.z = 0;

                    rt.x = rt.x > right ? rt.x : right;
                    rt.y = rt.y > top ? rt.y : top;
                    rt.z = 0;

                    lb.x = lb.x < left ? lb.x : left;
                    lb.y = lb.y < bottom ? lb.y : bottom;
                    lb.z = 0;

                    rb.x = rb.x > right ? rb.x : right;
                    rb.y = rb.y < bottom ? rb.y : bottom;
                    rb.z = 0;

                    left_top.localPosition = lt;
                    left_bottom.localPosition = lb;
                    right_top.localPosition = rt;
                    right_bottom.localPosition = rb;
                }

                SetParent(g.transform);
            }

            public void Dispose()
            {
                GameObject.Destroy(left_top.gameObject);
                GameObject.Destroy(right_bottom.gameObject);
                GameObject.Destroy(left_bottom.gameObject);
                GameObject.Destroy(right_top.gameObject);
            }
        }

        /// <summary>
        /// g의 maxdepth와 mindepth 가 차지하는 range를 구한다. (inclusive)
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public static int GetDepthRange(this GameObject g, params System.Type[] except_type)
        {
            return GetMaxDepthRecursively(g, except_type) - GetMinDepthRecursively(g, except_type) + 1;
        }

        public static Struct<int, int> GetMinMaxDepthRecursively_OnlyPanel(this GameObject g)
        {
            int mindepth = int.MaxValue;
            int maxdepth = int.MinValue;
            foreach(Transform t in g.transform)
            {
                var k = GetMinMaxDepthRecursively_OnlyPanel(t.gameObject);
                mindepth = (mindepth > k.v1 ? k.v1 : mindepth);
                maxdepth = (maxdepth < k.v2 ? k.v2 : maxdepth);
            }

            var gg = g.GetComponents<UIPanel>();
            if(gg != null)
            {
                foreach(var v in gg)
                {
                    var vd = v.depth;
                    mindepth = (mindepth > vd ? vd : mindepth);
                    maxdepth = (maxdepth < vd ? vd : maxdepth);
                }
            }

            return MakeTuple(mindepth, maxdepth);
        }

        public static void AddDepthRecusively_OnlyPanel(this GameObject g, int amount)
        {
            foreach (Transform t in g.transform)
            {
                AddDepthRecusively_OnlyPanel(t.gameObject, amount);
            }

            var gg = g.GetComponents<UIPanel>();
            if (gg != null)
            {
                foreach (var v in gg)
                {
                    v.depth += amount;
                }
            }
        }

        /// <summary>
        /// g의 가장 낮은 depth를 구함 (단 g가 uiwidget을 포함할 경우에만)
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public static int GetMinDepthRecursively(this GameObject g, params System.Type[] except_type)
        {
            int mindepth = int.MaxValue;
            foreach (Transform t in g.transform)
            {
                int k = GetMinDepthRecursively(t.gameObject, except_type);
                mindepth = (mindepth > k ? k : mindepth);
            }

            var gg = g.GetComponents<UIWidget>();

            if (gg != null)
            {
                foreach (var v in gg)
                {
                    if (except_type == null || !except_type.Contains(v.GetType()))
                    {
                        mindepth = (mindepth > v.depth ? v.depth : mindepth);
                    }
                }
            }

            return mindepth;
        }

        /// <summary>
        /// g의 가장 높은 depth를 구함 (단 g가 uiwidget을 포함할 경우에만)
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public static int GetMaxDepthRecursively(this GameObject g, params System.Type[] except_type)
        {
            if (g == null)
                return 0;
            else
            {
                int maxdepth = int.MinValue;
                foreach (Transform t in g.transform)
                {
                    int k = GetMaxDepthRecursively(t.gameObject, except_type);
                    maxdepth = (maxdepth < k ? k : maxdepth);
                }

                var gg = g.GetComponents<UIWidget>();
                if (gg != null)
                {
                    foreach (var v in gg)
                    {
                        if (except_type == null || !except_type.Contains(v.GetType()))
                        {
                            maxdepth = (maxdepth < v.depth ? v.depth : maxdepth);
                        }
                    }
                }

                return maxdepth;
            }
        }

        /// <summary>
        /// g의 가장 낮은 depth를 d로 만들어주고 다른 depth도 옮긴다.
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public static void SetMindepthRecursively(this GameObject g, int d, params System.Type[] except_type)
        {
            if (g != null)
            {
                int d_min = GetMinDepthRecursively(g, except_type);
                int amount = d - d_min;
                AddDepthRecursively(g, amount, except_type);
            }
        }

        /// <summary>
        /// g의 가장 높은 depth를 d로 만들어주고 다른 depth도 옮긴다.
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public static void SetMaxdepthRecursively(this GameObject g, int d, params System.Type[] except_type)
        {
            int d_max = GetMaxDepthRecursively(g, except_type);
            int amount = d - d_max;
            AddDepthRecursively(g, amount, except_type);
        }

        public static void AddDepthRecursively(this GameObject g, int amount, params System.Type[] except_type)
        {
            foreach (Transform t in g.transform)
            {
                AddDepthRecursively(t.gameObject, amount, except_type);
            }
            var gg = g.GetComponents<UIWidget>();
            if (gg != null)
            {
                foreach (var v in gg)
                {
                    if (except_type == null || !except_type.Contains(v.GetType()))
                        v.depth += amount;
                }
            }
        }


        public static List<T> SubtractList_WriteResultToMe<T>(List<T> from, List<T> addr)
        {
            List<T> left = new List<T>();
            foreach (T ft in from)
            {
                if (!addr.Contains(ft))
                    left.Add(ft);
            }

            from.Clear();
            foreach (T t in left)
            {
                from.Add(t);
            }
            return from;
        }


        public static void ChangeColorRecursive(this GameObject g, Color c)
        {
            //리커시브하게 자식들의 컬러를 체인지.
            foreach (Transform t in g.transform)
            {
                ChangeColorRecursive(t.gameObject, c);
            }
            //자기자신의 색상도 변경함
            var w = g.GetComponent<UIWidget>();
            if (w != null) w.color = c;
        }

        public static void ChangeAlphaRecursive(this GameObject g, float val)
        {
            //리커시브하게 자식들의 알파를 체인지.
            foreach (Transform t in g.transform)
            {
                ChangeAlphaRecursive(t.gameObject, val);
            }
            //자기자신의 알파도 변경함
            ChangeAlphaRecursive(g.GetComponents<UIWidget>(), val);
        }

        private static void ChangeAlphaRecursive(this UIWidget w, float val)
        {
            w.alpha = val;
        }

        private static void ChangeAlphaRecursive(UIWidget[] w, float val)
        {
            foreach (UIWidget uw in w)
            {
                uw.alpha = val;
            }
        }

        public static void SetAlphaRecursive(this Component g, float val)
        {
            SetAlphaRecursive(g.gameObject, val);
        }

        public static void SetAlphaRecursive(this GameObject g, float val)
        {
            foreach (Transform t in g.transform)
            {
                SetAlphaRecursive(t.gameObject, val);
            }
            SetAlphaRecursive(g.GetComponents<UIWidget>(), val);
        }

        private static void SetAlphaRecursive(UIWidget[] w, float val)
        {
            foreach (UIWidget uw in w)
            {
                uw.alpha = val;
            }
        }

        public static void AddAlphaRecursive(this GameObject g, float val)
        {
            foreach (Transform t in g.transform)
            {
                AddAlphaRecursive(t.gameObject, val);
            }
            AddAlphaRecursive(g.GetComponents<UIWidget>(), val);
        }

        private static void AddAlphaRecursive(UIWidget[] w, float val)
        {
            foreach (UIWidget uw in w)
            {
                uw.alpha += val;
            }
        }

        public static T FindInHierarchyRecuvely<T>(this Component c, System.Predicate<T> pCondition = null) where T : Component
        {
            return FindInHierarchyRecuvely<T>(c.gameObject, pCondition);
        }

        public static T FindInHierarchyRecuvely<T>(this GameObject c, System.Predicate<T> pCondition = null) where T : Component
        {
            var ls = FindAllInHierarchyRecuvely<T>(c);
            if (ls.Count == 0)
            {
                return null;
            }
            else
            {
                if (pCondition == null) return ls[0];
                else
                {
                    return ls.Find(pCondition);
                }
            }
        }

        public static List<T> FindAllInHierarchyRecuvely<T>(this Component c, List<T> usethis = null) where T : Component
        {
            if (c != null)
            {
                return FindAllInHierarchyRecuvely(c.gameObject, usethis);
            }
            else
            {
                if (usethis == null) return new List<T>();
                else return usethis;
            }
        }

        public static List<T> FindAllInHierarchyRecuvely<T>(this GameObject c, List<T> usethis = null) where T : Component
        {
            if (usethis == null) usethis = new List<T>();

            if (c != null)
            {
                var mine = c.GetComponents<T>();
                if (mine != null) foreach (var v in mine) usethis.Add(v);

                var tt = c.transform;
                foreach (Transform t in tt)
                    FindAllInHierarchyRecuvely<T>(t, usethis);
            }

            return usethis;
        }
        
        /// <summary>
        /// g와 모든 자식들의 알파를 begin~end까지 time시간동안 트윈한다.
        /// </summary>
        /// <param name="g">목표 오브젝트</param>
        /// <param name="begin">시작 알파값</param>
        /// <param name="end">종료시 알파값</param>
        /// <param name="time">트윈 시간</param>
        /// <param name="graphForm_0normal_1Accel_2Frict">0:직선형그래프, 1:가속형그래프(이차함수에가까움), 2:감속형그래프(사인파에가까움)</param>
        /// <returns></returns>
        public static IEnumerator AlphaTweenCoroutineRecursively(this GameObject g, float begin, float end, float time, int graphForm_0normal_1Accel_2Frict)
        {
            ChangeAlphaRecursive(g, begin);
            float accumtime = 0;
            do
            {
                accumtime += Time.deltaTime;
                switch (graphForm_0normal_1Accel_2Frict)
                {
                    default:
                    case 0:
                        //등속
                        ChangeAlphaRecursive(g, begin + (end - begin) * (accumtime / time));
                        break;
                    case 1:
                        //가속(첨에느리다빨라짐)
                        ChangeAlphaRecursive(g, begin + (end - begin) * (accumtime / time) * (accumtime / time));
                        break;
                    case 2:
                        //감속(첨에빠르다느려짐... 사인파에 가장 가까움)
                        ChangeAlphaRecursive(g, begin + (end - begin) * Mathf.Sqrt(accumtime / time));
                        break;
                }
                yield return null;
                if (g == null) yield break;
            } while (accumtime < time);

            ChangeAlphaRecursive(g, end);
        }
        public static int HexToInt(char hex)
        {
            switch (hex)
            {
                case '0': return 0;
                case '1': return 1;
                case '2': return 2;
                case '3': return 3;
                case '4': return 4;
                case '5': return 5;
                case '6': return 6;
                case '7': return 7;
                case '8': return 8;
                case '9': return 9;
                case 'A': return 10;
                case 'B': return 11;
                case 'C': return 12;
                case 'D': return 13;
                case 'E': return 14;
                case 'F': return 15;
                case 'a': return 10;
                case 'b': return 11;
                case 'c': return 12;
                case 'd': return 13;
                case 'e': return 14;
                case 'f': return 15;
                default: return 0;
            }
        }

        public static Color RGBAColorFromString(string HexVal)
        {
            var r = HexVal.Substring(0, 2);
            var g = HexVal.Substring(2, 2);
            var b = HexVal.Substring(4, 2);
            var a = HexVal.Substring(6, 2);

            return new Color(
                (HexToInt(r[0]) * 16 + HexToInt(r[1])) / 255.0f,
                (HexToInt(g[0]) * 16 + HexToInt(g[1])) / 255.0f,
                (HexToInt(b[0]) * 16 + HexToInt(b[1])) / 255.0f,
                (HexToInt(a[0]) * 16 + HexToInt(a[1])) / 255.0f
                );
        }


        public static string ColorToNguiLabel(Color c)
        {
            int r = (int)(255 * c.r);
            int g = (int)(255 * c.g);
            int b = (int)(255 * c.b);
            if (r > 255) r = 255;
            if (g > 255) g = 255;
            if (b > 255) b = 255;
            string rtv = "[" + r.ToString("X2") + g.ToString("X2") + b.ToString("X2") + "]";
            return rtv;
        }

        public static IEnumerator ColorTweenCoroutineRecursively(this GameObject g, Color begin, Color end, float time)
        {
            ChangeColorRecursive(g, begin);
            float accumtime = 0;
            do
            {
                accumtime += Time.deltaTime;
                ChangeColorRecursive(g, begin + (end - begin) * (accumtime / time));
                yield return null;
                if (g == null) yield break;
            } while (accumtime < time);

            ChangeColorRecursive(g, end);
        }


        public static string ToString(Vector2 v)
        {
            return "Vector2(" + v.x + ", " + v.y + ")";
        }

        public static string ToString(Vector3 v)
        {
            return "Vector3(" + v.x + ", " + v.y + ", " + v.z + ")";
        }

        public static string ToString(Vector4 v)
        {
            return "Vector4(" + v.x + ", " + v.y + ", " + v.z + ", " + v.w + ")";
        }


        /// <summary>
        /// Transform을 특정 위치로 시간만에 옮기는 함수
        /// </summary>
        /// <param name="t">옮길애</param>
        /// <param name="time">시간</param>
        /// <param name="target">목표</param>
        /// <param name="graph_0normal_1frict_2accel">그래프 0:기본, 1:사인, 2:점점빨라짐</param>
        /// <returns></returns>
        public static IEnumerator MoveToLocalPosition(Transform t, float time, Vector3 target, int graph_0normal_1frict_2accel = 1)
        {
            Vector3 first_pos = t.transform.localPosition;
            Vector3 amount = target - first_pos;
            float acctime = 0;

            while (acctime < time)
            {
                acctime += Time.deltaTime;

                //accel
                if (graph_0normal_1frict_2accel == 2)
                {
                    t.transform.localPosition = first_pos + amount * (1.0f - Mathf.Cos((acctime / time) * (Mathf.PI / 2)));
                }
                //frict
                if (graph_0normal_1frict_2accel == 1)
                {
                    t.transform.localPosition = first_pos + amount * Mathf.Sin((acctime / time) * (Mathf.PI / 2));
                }
                //normal
                else
                {
                    t.transform.localPosition = first_pos + amount * (acctime / time);
                }
                yield return null;
                if (t == null) yield break;
            }



            t.transform.localPosition = target;

            yield break;
        }

        public static IEnumerator ScaleLocalTo(GameObject g, Vector3 toScale, float time = 0.5f, int curve_0normal_1sin_2oneminuscos = 0, bool ignore = false)
        {
            Vector3 fromscale = g.transform.localScale;
            Vector3 dist = toScale - fromscale;
            CWait wait = new CWait(time);
            wait.IgnorePause = ignore;
            while (wait.NotYet)
            {
                switch (curve_0normal_1sin_2oneminuscos)
                {
                    case 0:
                        g.transform.localScale = fromscale + dist * wait.FlowedRatio;
                        break;
                    case 1:
                        g.transform.localScale = fromscale + dist * wait.FlowedRatioSin;
                        break;
                    case 2:
                        g.transform.localScale = fromscale + dist * wait.FlowedRatio1MinusCos;
                        break;
                }
                yield return null;
                if (g == null)
                    yield break;
            }
            g.transform.localScale = toScale;
        }

        public delegate void DelegateVoidStringParam(string str);
        public delegate void DelegateVoidNoParam();
        public delegate void DelegateVoid0Param();
        public delegate void DelegateVoidIntParam(int i);
        public delegate IEnumerator DelegateIEnumeratorNoParam();
        public delegate void DelegateVoidBoolParam(bool b);
        public delegate bool DelegateBoolNoParam();
        public delegate void DelegateVoid1Param<T>(T t);
        public delegate void DelegateVoidParam<T1>(T1 t1);
        public delegate void DelegateVoidParam<T1, T2>(T1 t1, T2 t2);
        public delegate void DelegateVoidParam<T1, T2, T3>(T1 t1, T2 t2, T3 t3);
        public delegate void DelegateVoidParam<T1, T2, T3, T4>(T1 t1, T2 t2, T3 t3, T4 t4);
        public delegate RTV DelegateParam<RTV>();
        public delegate RTV DelegateParam<RTV, T1>(T1 t1);
        public delegate RTV DelegateParam<RTV, T1, T2>(T1 t1, T2 t2);
        public delegate RTV DelegateParam<RTV, T1, T2, T3>(T1 t1, T2 t2, T3 t3);
        public delegate RTV DelegateParam<RTV, T1, T2, T3, T4>(T1 t1, T2 t2, T3 t3, T4 t4);

        [System.Diagnostics.Conditional("ENABLE_LOG")]
        public static void Log(object str)
        {
#if UNITY_EDITOR
            Debug.Log(str);
#endif
        }

        [System.Diagnostics.Conditional("ENABLE_LOG")]
        public static void LogError(object str)
        {
#if UNITY_EDITOR
            Debug.LogError(str);
#endif
            //scScreenLog.LogError(str);
        }
        

        public static Boxer<T> Boxing<T>(T from)
        {
            return new Boxer<T>(from);
        }


        public static T GetComponentInChildrenRecusively<T>(this Component c, bool includeinactive = false) where T : Component
        {
            return GetComponentInChildrenRecusively<T>(c.gameObject, includeinactive);
        }

        public static T GetComponentInChildrenRecusively<T>(this GameObject g, bool includeinactive = false) where T : Component
        {
            var tr = g.transform;
            foreach (Transform t in tr)
            {
                var ls = t.GetComponentsInChildren<T>();
                foreach (var v in ls)
                {
                    if (includeinactive || v.gameObject.activeSelf)
                        return v;
                }
            }
            return null;
        }

        public static List<T> GetComponentsInChildrenRecusively<T>(this Component c, bool includeinactive = false, List<T> usethis = null) where T : Component
        {
            return GetComponentsInChildrenRecusively<T>(c.gameObject, includeinactive, usethis);
        }

        public static List<T> GetComponentsInChildrenRecusively<T>(this GameObject g, bool includeinactive = false, List<T> usethis = null) where T : Component
        {
            if (usethis == null) usethis = new List<T>();
            if (g != null)
            {
                var tr = g.transform;
                var ls = tr.GetComponents<T>();

                foreach (var v in ls)
                    if (includeinactive || v.gameObject.activeSelf) usethis.Add(v);

                foreach (Transform t in tr)
                    GetComponentsInChildrenRecusively(t.gameObject, includeinactive, usethis);
            }
            return usethis;
        }


        /// <summary>
        /// 부모중에 해당 컴퍼넌트가 있는지 검색해서 리턴 (자기자신포함)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static T GetNearestParent<T>(this Transform t) where T : Component
        {
            while (t != null && t.GetComponent<T>() == null)
            {
                t = t.parent;
            }
            if (t == null) return null;
            return t.GetComponent<T>();
        }

        public static T GetNearestParent<T>(this Component c) where T : Component
        {
            return GetNearestParent<T>(c.transform);
        }

        public static T GetNearestParent<T>(this GameObject g) where T : Component
        {
            return GetNearestParent<T>(g.transform);
        }

        /// <summary>
        /// 부모중에 해당 컴퍼넌트가 있는지 검색해서 리턴 (자기자신 제외)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static T GetNearestParentExceptBySelf<T>(this Transform t) where T : Component
        {
            t = t.parent;
            while (t != null && t.GetComponent<T>() == null)
            {
                t = t.parent;
            }
            if (t == null) return null;
            return t.GetComponent<T>();
        }

        public static T GetNearestParentExceptBySelf<T>(this Component c) where T : Component
        {
            return GetNearestParentExceptBySelf<T>(c.transform);
        }

        public static T GetNearestParentExceptBySelf<T>(this GameObject g) where T : Component
        {
            return GetNearestParentExceptBySelf<T>(g.transform);
        }

        /// <summary>
        /// 리스트에서 리스트를 뺸다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="from"></param>
        /// <param name="sub"></param>
        public static void SubtractToOriginal<T>(List<T> from, List<T> sub)
        {
            from.RemoveAll((T t) => sub.Contains(t));
        }

        /// <summary>
        /// 리스트에서 리스트를 빼되 원본 사본은 다 남겨두고 new 로 만들어서 쓴다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="from"></param>
        /// <param name="sub"></param>
        public static List<T> SubtractClone<T>(List<T> from, List<T> sub)
        {
            List<T> newList = new List<T>();
            foreach (var v in from)
            {
                if (!sub.Contains(v)) newList.Add(v);
            }
            return newList;
        }

        static public void GetAllFilePath(System.IO.DirectoryInfo root, string extension, List<string> result)
        {
            string current_path_name = MakeValidPathName(root.FullName);

            System.IO.FileInfo[] files = root.GetFiles("*." + extension);
            foreach (var f in files)
            {
                string file_name = f.Name;
                result.Add(current_path_name + file_name);
            }

            System.IO.DirectoryInfo[] lower = root.GetDirectories();
            foreach (var d in lower)
            {
                GetAllFilePath(d, extension, result);
            }
        }

        static string MakeValidPathName(string full_path)
        {
            string rtv = string.Empty;

            var paths = full_path.Split('\\');

            for (int i = paths.Length - 1; i >= 0; i--)
            {
                string temp = paths[i];
                if (temp != "Assets")
                {
                    rtv = rtv.Insert(0, temp + "/");
                }
                else
                {
                    rtv = rtv.Insert(0, temp + "/");
                    break;
                }
            }
            return rtv;
        }


        public static string ZipString(string str)
        {
            var compressed = Ionic.Zlib.ZlibStream.CompressString(str);
            var stringed = System.Convert.ToBase64String(compressed);
            return stringed;
        }

        public static string UnzipString(string zipped)
        {
            //빈문자열이면 걍 돌려줌
            if (string.IsNullOrEmpty(zipped)) return zipped;

            try
            {
                var compressed = System.Convert.FromBase64String(zipped);
                string uncompressed = Ionic.Zlib.ZlibStream.UncompressString(compressed);
                return uncompressed;
            }
            catch(System.Exception e)
            {
                Debug.LogWarning("String decompressing is been failed");
                return zipped;
            }
        }

        public static T Convert<T>(this object o)
        {
            var s = o.ToString();
            return (T)System.Convert.ChangeType(s, typeof(T));
        }

        public static string DatetimeToTimestamp(System.DateTime dt)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(dt.Year.ToString("D4"));
            sb.Append("-");
            sb.Append(dt.Month.ToString("D2"));
            sb.Append("-");
            sb.Append(dt.Day.ToString("D2"));
            sb.Append(" ");
            sb.Append(dt.Hour.ToString("D2"));
            sb.Append(":");
            sb.Append(dt.Minute.ToString("D2"));
            sb.Append(":");
            sb.Append(dt.Second.ToString("D2"));
            return sb.ToString();
        }

        public static string DateTimeToEnglish(string str)
        {
            str = str.Replace("오전", "AM");
            str = str.Replace("오후", "PM");
            return str;
        }


        public static System.DateTime DateTimeFromTimestamp(string stamp)
        {
            if (string.IsNullOrEmpty(stamp)) return System.DateTime.MinValue;

            var eng = DateTimeToEnglish(stamp);
            try
            {
                return System.Convert.ToDateTime(eng);
            }
            catch (System.Exception e)
            {
                return System.DateTime.MinValue;
            }
        }

        //screenPosition 을 노말영역으로 옮겨준다
        public static Vector3 TransformScreenPositionToNormalizedPosition(Vector2 input_screen_pos)
        {
            float height = Screen.width;
            float Factor = Screen.height / Screen.height;
            float xpos = ((input_screen_pos.x) / (float)Screen.height) * height;
            float ypos = ((input_screen_pos.y) / (float)Screen.height) * height;

            float xmax = (Screen.width * height) / Screen.height;
            float ymax = (Screen.height * height) / Screen.height;

            xpos -= xmax / 2;
            ypos -= ymax / 2;

            return new Vector3(xpos / (height / 2.0f), ypos / (height / 2.0f), 0);
        }
        

    }

}