using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ZO;

public class scParticleGenerator : MonoBehaviour
{

    public GameObject GeneratingPrefab;
    public List<GameObject> GeneratingPrefabOthers;

    public Util.DelegateVoid1Param<GameObject> WhenEachParticleGenerated;

    [SerializeField]
    private Vector2 MinVelo;
    [SerializeField]
    private Vector2 MaxVelo;
    [SerializeField]
    private Vector2 MinAccel;
    [SerializeField]
    private Vector2 MaxAccel;
    [SerializeField]
    private Vector2 ParticleScale = new Vector2(1, 1);

    [SerializeField]
    private int MinNumberOfParticlesOneTime = 1;
    [SerializeField]
    private int MaxNumberOfParticlesOneTime = 1;

    [SerializeField]
    private float GenerateMinRadius = 100;
    [SerializeField]
    private float GenerateMaxRadius = 50;

    [SerializeField]
    private float ParticleGenerateTerm = 0.1f;

    public int ParticleGenerateMaxCount = 0;
    [SerializeField]
    [ReadOnlyInInspector]
    private int CurrentGenerated = 0;

    public Transform ParentIfNullEffectfront;

    public bool DisableComponentInsteadOfDestroy = false;

    public bool ParticleMovesOnlyOuter = false;
    public bool ParticleUpToFirstSpeed = false;

    public float BeginDelay = 0;

    void Awake()
    {
    }

    public void Reset()
    {
        OnDisable();
        OnEnable();
    }

    CoroutineClass cc = null;

    void OnEnable()
    {
        if (cc == null)
            cc = scCoroutine.Instance.Begin(GeneratingParticles());
    }

    void OnDisable()
    {
        CurrentGenerated = 0;
        if (cc != null)
        {
            cc.Stop();
            cc = null;
        }
    }

    IEnumerator GeneratingParticles()
    {
        yield return null;

        if (LocalCache.EffectOff) yield break;

        if(GeneratingPrefab != null)
            GeneratingPrefabOthers.AddIfNotContains(GeneratingPrefab);

        CWait w = new CWait();
        w.IgnorePause = true;

        Vector2 velodist = MaxVelo - MinVelo;
        Vector2 acceldist = MaxAccel - MinAccel;
        float RadiusDistance = GenerateMaxRadius - GenerateMinRadius;

        if (!Mathf.Approximately(BeginDelay, 0))
            yield return CWait.ForSeconds_IgnorePause(BeginDelay);

        var parent = ParentIfNullEffectfront;

        if (parent == null)
        {
            parent = scEffectPanelFront.Instance.transform;
        }

        while (true)
        {
            if (!enabled) yield return null;

            //parent = ParentIfNullEffectfront == null ? scEffectPanelFront.Instance.transform : ParentIfNullEffectfront;

            int count = Util.GetRandomNumberIntInclusive(MinNumberOfParticlesOneTime, MaxNumberOfParticlesOneTime);

            for (int i = 0; i < count; ++i)
            {
                var gp = GeneratingPrefabOthers[i % GeneratingPrefabOthers.Count];
                var particle = scObjectPool.Instance.CreatedPooled(parent, gp);
                if (WhenEachParticleGenerated != null) WhenEachParticleGenerated(particle.gameObject);
                particle.transform.localScale = ParticleScale;
                Vector3 adder = new Vector3(
                    (GenerateMinRadius + Random.value * (RadiusDistance)) * Mathf.Cos(Random.value * 2 * Mathf.PI),
                    (GenerateMinRadius + Random.value * (RadiusDistance)) * Mathf.Sin(Random.value * 2 * Mathf.PI),
                    0);
                particle.transform.Put2DOnly(gameObject);
                particle.transform.localPosition += adder;
                particle.transform.ResetAllTweensAndPlay();
                var vel = particle.GetComponent<scVelocity>();
                if (vel != null)
                {
                    vel.velocity = new Vector3(
                        MinVelo.x + (velodist.x) * Random.value,
                        MinVelo.y + (velodist.y) * Random.value,
                        0);
                    vel.accel = new Vector3(
                        MinAccel.x + (acceldist.x) * Random.value,
                        MinAccel.y + (acceldist.y) * Random.value,
                        0);
                    vel.IgnorePause = true;
                    if (ParticleMovesOnlyOuter)
                    {
                        var m = vel.velocity.magnitude;
                        var pdist = vel.transform.position - transform.position;
                        pdist.z = 0;
                        vel.velocity = pdist.normalized * m;
                    }

                    if (ParticleUpToFirstSpeed)
                    {
                        vel.transform.up = vel.velocity;
                    }
                }
                ++CurrentGenerated;
                if (CurrentGenerated >= ParticleGenerateMaxCount)
                    break;
            }
            
            if (ParticleGenerateMaxCount > 0)
            {
                if (CurrentGenerated >= ParticleGenerateMaxCount)
                {
                    yield return null;   
                    if (DisableComponentInsteadOfDestroy)
                    {
                        enabled = false;
                    }
                    else
                        scObjectPool.Instance.DestroyPooled(this);
                    yield break;
                }
            }

            if (Mathf.Approximately(ParticleGenerateTerm, 0))
                yield return null;
            else
            {
                w.Reset(ParticleGenerateTerm);
                while (w.NotYet) yield return null;
            }
        }

        yield break;
    }

}
