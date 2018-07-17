using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZO;

public class scSpineRenderStageLinker : MonoBehaviour {

    public scSpineRenderStage Stage;

    void Awake()
    {
        if(!GameInitializer.NowLoading)
        {
            Stage = scSpineRenderStage.Create();
            Stage.name = "SpineStage" + Util.GetHierarchyNameUntil<UIPanel>(this);
        }
    }

    void OnDestroy()
    {
        if(Stage != null)
            Destroy(Stage.gameObject);
    }
        
    public UITexture TargetTexture;

    void Start()
    {
        ResetTexture();
    }
    public void ResetTexture()
    {
        TargetTexture.mainTexture = Stage.OutTexture;
        
        //세로를 720에 맞춘다
        
        TargetTexture.width = (int)((Stage.OutTexture.width / (float)Stage.OutTexture.height) * 720);
        TargetTexture.height = 720;

        TargetTexture.transform.position = Vector3.zero;
    }

    void Update()
    {
        ResetTexture();
    }
    
}
