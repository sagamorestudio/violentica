using UnityEngine;
using System.Collections;

namespace ZO
{
    public class scResetAllTweensWhenRecycled : scAbsObjectPoolIgnoreNotBake
    {

        void OnDestroy()
        {
            this.ResetAllTweensAndPlay();
        }

    }

}