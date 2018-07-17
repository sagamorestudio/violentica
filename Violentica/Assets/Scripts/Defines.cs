using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZO
{
    public static class Defines
    {
        public const int AppVersion = 1;
        public static string EditorAccessID
        {
            get
            {
                return SystemInfo.deviceUniqueIdentifier;
            }
        }

    }
}