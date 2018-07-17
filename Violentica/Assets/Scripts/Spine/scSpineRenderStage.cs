using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZO;
using Spine;
using Spine.Unity;

public class scSpineRenderStage : MonoBehaviour {

    public GameObject Content;
    public Camera cam;

    static List<scSpineRenderStage> total_stages = new List<scSpineRenderStage>();
    public static scSpineRenderStage Create()
    {
        //trim
        var sp = scPrefabs.Create(scPrefabs.Instance.PrefabSpineRenderStage);
        
        int positions = 0;
        total_stages.ForEach(v => v.transform.position = new Vector3(0, 0, positions += 10));
        return sp;
    }

    public Texture OutTexture
    {
        get
        {
            return rt;
        }
    }

    public void Remove(GameObject c)
    {
        var p = spines.Find(v => v.tracing_target == c);
        if(p != null)
        {
            Destroy(p.skel.gameObject);
            spines.Remove(p);
        }
    }

    private RenderTexture rt;

	void Awake()
    {
        total_stages.Add(this);
        rt = new RenderTexture(Screen.width, Screen.height, 24);
        cam.targetTexture = rt;
    }

    int base_sorting_order = -1;

    void OnDestroy()
    {
        total_stages.Remove(this);
        Destroy(rt);
    }

    public float GetMyScale(SkeletonAnimation sk)
    {
        return spines.Find(v => v.skel == sk).GetSmart(v => v.size_ratio, 0);
    }
    public void SetMyScale(SkeletonAnimation sk, float s)
    {
        spines.Find(v => v.skel == sk).IfNotNull(v => v.size_ratio = s);
    }

    public SkeletonAnimation MakeSpineObject(GameObject tracing, string prefabname, float size_ratio = 1)
    {
        
        var spine = scPrefabs.CreateSpine(Content, prefabname);

        if (spine != null)
        {
            var p = new Pair();
            p.skel = spine;
            p.skt = spine.transform;
            p.tracing_target = tracing.transform;
            p.size_ratio = size_ratio;
            spines.Add(p);
            if (base_sorting_order < 0)
                base_sorting_order = p.skel.GetComponent<MeshRenderer>().sortingOrder;

            //동기화
            Sync(p);

        }
        return spine;
    }

    void Sync(Pair p)
    {
        if (p != null)
        {
            var v = p;
            var tp = v.tracing_target.position;
            v.skt.position = new Vector3(tp.x, tp.y, v.skt.position.z);
            v.skt.localScale = v.tracing_target.lossyScale * v.size_ratio;
        }
    }

    public void Sync(GameObject gFrom)
    {
        var gt = gFrom.transform;
        var p = spines.Find(v => v.tracing_target == gt);
        Sync(p);
    }

    class Pair
    {
        public SkeletonAnimation skel;
        public Transform skt;
        public Transform tracing_target;
        public float size_ratio;
        public override string ToString()
        {
            if (tracing_target == null) return "null";
            else
                return tracing_target.name;
        }
    }

    List<Pair> spines = new List<Pair>();

    int zero_time = 0;

    void Update()
    {
        //trim
        var ls = spines.ToArray();
        foreach (var v in ls)
        {
            if (v.skel == null || v.tracing_target == null)
            {
                if (v.skel != null) Destroy(v.skel.gameObject);
                spines.Remove(v);
            }
        }

        if(spines.Count == 0)
        {
            //3틱후 카메라 끈다
            ++zero_time;
            if (zero_time > 3)
            {
                if (cam.enabled)
                    cam.enabled = false;
            }
        }
        else
        {
            zero_time = 0;
            if(!cam.enabled)
                cam.enabled = true;

            //following target
            foreach (var v in spines)
            {
                Sync(v);
//                 var tp = v.tracing_target.position;
//                 v.skt.position = new Vector3(tp.x, tp.y, v.skt.position.z);
//                 v.skt.localScale = v.tracing_target.lossyScale * v.size_ratio;
                //v.skt.localEulerAngles = v.tracing_target.localEulerAngles;
            }
        }
    }

    public List<SkeletonAnimation> GetTotalSkeletons
    {
        get
        {
            return spines.VConvert(v => v.skel);
        }
    }
        
    public void Sort<T>(Util.DelegateParam<T, scSpineRenderController> sort_ordering) where T : System.IComparable<T>
    {
        var sp2 = spines.Clone();
        sp2.RemoveAll(v => v.tracing_target == null || v.skt == null);
        sp2.VSort(v => sort_ordering(v.tracing_target.GetComponent<scSpineRenderController>()));

        for(int i = 0; i < sp2.Count; ++i)
        {
            var sp = sp2[i];
            var mr = sp.skel.GetComponent<MeshRenderer>();
            mr.sortingOrder = base_sorting_order + i;        
        }
    }
    
}
