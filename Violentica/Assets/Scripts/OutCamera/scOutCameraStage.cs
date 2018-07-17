using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZO;
using Spine;
using Spine.Unity;

public class scOutCameraStage : MonoBehaviour
{

    public GameObject Content;
    public Camera cam;
    private RenderTexture rt;
    static List<scOutCameraStage> total_stages = new List<scOutCameraStage>();
    List<Pair> lsObjsInOutCameras = new List<Pair>();
    int zero_time = 0;
    public float zero_z_pos_from_camera = 10;

    public SpriteRenderer BlackBackpanel;

    public static scOutCameraStage Create()
    {
        //trim
        var sp = scPrefabs.Create(scPrefabs.Instance.PrefabOutCameraStage);

        int positions = 0;
        total_stages.ForEach(v => v.transform.position = new Vector3(0, 0, positions += 10000));
        return sp;
    }

    public Texture OutTexture
    {
        get
        {
            return rt;
        }
    }

    void Awake()
    {
        total_stages.Add(this);
        rt = new RenderTexture(Screen.width, Screen.height, 24);
        cam.targetTexture = rt;
    }

    void Start()
    {
        maxdist = BlackBackpanel.transform.localPosition.z - BuffDist;
        mindist = cam.transform.localPosition.z + BuffDist;
    }

    float mindist;
    float maxdist;

    void OnDestroy()
    {
        total_stages.Remove(this);
        Destroy(rt);
    }

    public float GetMyScale(GameObject sk)
    {
        return lsObjsInOutCameras.Find(v => v.skt == sk.transform).GetSmart(v => v.size_ratio, 0);
    }
    public float GetMyScale(Component sk)
    {
        return lsObjsInOutCameras.Find(v => v.skt == sk.transform).GetSmart(v => v.size_ratio, 0);
    }

    public void SetMyScale(GameObject sk, float s)
    {
        lsObjsInOutCameras.Find(v => v.skt == sk.transform).IfNotNull(v => v.size_ratio = s);
    }
    public void SetMyScale(Component sk, float s)
    {
        lsObjsInOutCameras.Find(v => v.skt == sk.transform).IfNotNull(v => v.size_ratio = s);
    }

    public void RefreshWith(GameObject g)
    {
        var t = g.transform;
        var finded = lsObjsInOutCameras.Find(v => v.skt == t);
        if(finded != null)
        {
            updateObjPosition(finded);
        }
    }

    public GameObject MakeOutcamObject(GameObject tracing, GameObject created, float size_ratio = 1)
    {
        var p = new Pair();
        p.skt = created.transform;
        p.skt.parent = Content.transform;
        p.skt.position = new Vector3(tracing.transform.position.x, tracing.transform.position.y, Content.transform.position.z);
        p.skt.localScale = size_ratio * tracing.transform.localScale;
        p.tracing_target = tracing.transform;
        p.size_ratio = size_ratio;
        lsObjsInOutCameras.Add(p);
        updateObjPosition(p);
        return created;
    }

    public T MakeOutcamObject<T>(GameObject tracing, T created, float size_ratio = 1) where T : Component
    {
        var p = new Pair();
        p.skt = created.transform;
        p.skt.parent = Content.transform;
        p.skt.position = new Vector3(tracing.transform.position.x, tracing.transform.position.y, Content.transform.position.z);
        p.skt.localScale = size_ratio * tracing.transform.localScale;
        p.tracing_target = tracing.transform;
        p.size_ratio = size_ratio;
        lsObjsInOutCameras.Add(p);
        updateObjPosition(p);
        return created;
    }

    class Pair
    {
        public Transform skt;
        public Transform tracing_target;
        public float size_ratio;
    }

    public float BuffDist = 2;
    
    void updateObjPosition(Pair v)
    {
        var tp = v.tracing_target.position;

        //v.skt.localScale = v.tracing_target.lossyScale * v.size_ratio;

        //가상사이즈, xy평균크기로 ratio맞춰서한다.
        var szvec = (v.tracing_target.localScale * v.size_ratio);
        float sz = (szvec.x + szvec.y) / 2;
        if (sz <= 0)
            sz = 0.01f;

        //미리 구해놓는 sqrt3
        float sqrt3 = Mathf.Sqrt(3);

        //fovy60 에 대한 크기배율계산
        //배율에 따른 z 위치는 BuffDist(가장 큼) ~ BlackBackpanel.z - BuffDist(가장 작음)
        {
            //basepose_z을 1로보고 sz배율만큼 이동시킨다

            //sz overflow방지
            float dist1 = zero_z_pos_from_camera;
            var unculled_dist = zero_z_pos_from_camera / sz;
            if (unculled_dist < BuffDist)
                sz = zero_z_pos_from_camera / BuffDist;
            else if (unculled_dist > (BlackBackpanel.transform.localPosition.z - BuffDist))
            {
                sz = zero_z_pos_from_camera / (BlackBackpanel.transform.localPosition.z - BuffDist);
            }
            var finalz = zero_z_pos_from_camera / sz;

            //overflow방지 : 이젠안씀
            //if (position > maxdist) position = maxdist;
            //else if (position < mindist) position = mindist;

            float current_Sqrt3 = (zero_z_pos_from_camera / sz) / sqrt3;

            //height
            float y_normalized = tp.y / (scUIRoot.Instance.HeightInGlobal / 2);
            float ypos =
                (y_normalized) //노멀라이즈 (-1 ~ 1)
                * (current_Sqrt3)    //현재 z위치의 x위치로 변경
                ;

            //width
            float x_normalized = tp.x / (scUIRoot.Instance.WidthInGolbal / 2);
            float y_to_x = scUIRoot.Instance.WidthInGolbal / scUIRoot.Instance.HeightInGlobal;
            float xpos = x_normalized * current_Sqrt3 * y_to_x;
            v.skt.localPosition = new Vector3(xpos, ypos, finalz);
        }

        //회전
        v.skt.localEulerAngles = v.tracing_target.localEulerAngles;
    }
            
    void Update()
    {
        //trim
        var ls = lsObjsInOutCameras.ToArray();
        foreach (var v in ls)
        {
            if (v.skt == null || v.tracing_target == null)
            {
                if (v.skt != null)
                    scObjectPool.Instance.DestroyPooled(v.skt.gameObject);
                lsObjsInOutCameras.Remove(v);
            }
        }

        if (lsObjsInOutCameras.Count == 0)
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
            if (!cam.enabled)
                cam.enabled = true;

            //following target
            foreach (var v in lsObjsInOutCameras)
            {
                updateObjPosition(v);
            }
        }
    }
}
