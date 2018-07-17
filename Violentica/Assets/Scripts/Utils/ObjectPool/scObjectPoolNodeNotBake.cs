using UnityEngine;
using System.Collections;

namespace ZO
{
    public sealed class scObjectPoolNodeNotBake : MonoBehaviour
    {

        public string[] NotBakeName = new string[] { "Awake", "Start", "OnDestroy" };
        public bool AffectRecusively = true;

    }

}