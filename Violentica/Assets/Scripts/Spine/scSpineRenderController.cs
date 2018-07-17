using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;
using ZO;

public class scSpineRenderController : MonoBehaviour {

    public scSpineRenderStageLinker MyLinker;
    Spine.Unity.SkeletonAnimation Mine;

    string basic_animation;
    string CurrentPrefabName;
    Util.DelegateVoid1Param<scSpineRenderController> event_next_call;

    public Spine.Unity.SkeletonAnimation MySpine
    {
        get
        {
            return Mine;
        }
    }

    public void Clear()
    {
        if (Mine != null)
        {
            Destroy(Mine.gameObject);
            Mine = null;
        }
        MyLinker = null;
        CurrentPrefabName = basic_animation = string.Empty;
    }

    public void Create(string prefabName, string prefabskin, string basic_animation_name, float size)
    {
        Clear();

        CurrentPrefabName = prefabName;
        basic_animation = basic_animation_name;

        //가장 가까운 링커를 찾는다
        MyLinker = this.GetNearestParent<scSpineRenderStageLinker>();

        //생성
        Mine = MyLinker.Stage.MakeSpineObject(gameObject, prefabName, size);
        if (Mine != null)
        {
            Mine.initialSkinName = prefabskin;
            SetAnimationBasic(basic_animation, true);
            Mine.Reload();
            Mine.state.Complete += everyAnimationCompleted;
        }
    }

    void everyAnimationCompleted(Spine.TrackEntry t)
    {
        if(Mine != null)
        {
            //once ani end
            if(!Mine.loop)
            {
                Mine.skeleton.PoseWithAnimation(basic_animation, 0, true);
                Mine.AnimationName = basic_animation;
                Mine.loop = true;                
                Mine.Reload();
                Mine.state.Complete += everyAnimationCompleted;
            }

            //always call
            if (event_next_call != null)
            {
                event_next_call(this);
                event_next_call = null;
            }
        }
    }


//     public void Initialize(RefTable.CharacterDataRow cd, string _basic_animation, float size)
//     {
//         if(Is(cd))
//         {
//             SetAnimationBasic(_basic_animation);
//             MyLinker.Stage.SetMyScale(Mine, size);
//         }
//         else
//         {
//             if(cd != null)
//             {
//                 Create(cd.PrefabName, cd.PrefabSkin, _basic_animation, size);
//             }
//             else
//             {
//                 Clear();
//             }
//         }
//     }

//     public bool Is(RefTable.CharacterDataRow cd)
//     {
//         if (Mine == null) return false;
// 
//         if (CurrentPrefabName != cd.PrefabName) return false;
//         if (Mine.initialSkinName != cd.PrefabSkin) return false;
//         return true;
//     }

    public bool SetAnimationBasic(string animation_name, bool replace_now = false)
    {
        if (Mine == null) return false;

        if (Mine.skeletonDataAsset.GetSkeletonData(true).FindAnimation(animation_name) == null)
            return false;

        basic_animation = animation_name;
        if (replace_now)
        {
            Mine.skeleton.PoseWithAnimation(animation_name, 0, true);
            Mine.AnimationName = animation_name;
            Mine.loop = true;
            Mine.Reload();
            Mine.state.Complete += everyAnimationCompleted;
        }
        
        return true;
    }

    public bool HasAni(string aniname)
    {
        if (Mine != null)
        {
            var animations = Mine.skeleton.data.Animations;
            bool m_bHasAnimation = -1 != animations.FindIndex((a) => a.Name.ToLower() == aniname);
            return m_bHasAnimation;
        }
        else
            return false;
    }

    public void WhenAnimationOver(Util.DelegateVoid1Param<scSpineRenderController> finish_run)
    {
        event_next_call += finish_run;
    }

    public bool SetAnimationOnce(string animation_name)
    {
        if (Mine == null) return false;

        if (Mine.skeletonDataAsset.GetSkeletonData(true).FindAnimation(animation_name) == null)
            return false;

        Mine.skeleton.PoseWithAnimation(animation_name, 0, true);
        Mine.AnimationName = animation_name;
        Mine.loop = false;
        Mine.Reload();
        Mine.state.Complete += everyAnimationCompleted;
        return true;
    }

    void OnDestroy()
    {
        if(MyLinker != null && MyLinker.Stage != null)
        {
            MyLinker.Stage.Remove(this.gameObject);
        }
    }

    public void Sync()
    {
        if (MyLinker != null && MyLinker.Stage != null)
        {
            MyLinker.Stage.Sync(this.gameObject);
        }
    }

    public string CurrentAni
    {
        get
        {
            if (Mine == null) return string.Empty;
            return basic_animation;
        }
    }
}


public static class scSpineRenderController_ExtensionMethods
{
    public static scSpineRenderController FindSpineControllerInChild(this Component g)
    {
        return Util.FindInHierarchyRecuvely<scSpineRenderController>(g);
    }

    public static scSpineRenderController FindSpineControllerInChild(this GameObject g)
    {
        return Util.FindInHierarchyRecuvely<scSpineRenderController>(g);
    }
    public static Spine.Unity.SkeletonAnimation Reload(this Spine.Unity.SkeletonAnimation s)
    {
        try
        {
            s.Initialize(true);
        }
        catch (System.Exception e)
        {
            Util.LogError(e);
        }
        return s;
    }

}

