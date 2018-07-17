using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZO;

public class LocalCache : SingleTone<LocalCache> {


    static WrapPlayerPrefs _effectoff = new WrapPlayerPrefs("_effectoff");
    public static bool EffectOff
    {
        get
        {
            return _effectoff.ValueToIntDefaultIs0 == 0;
        }
        set
        {
            _effectoff.Value = value ? "1" : "0";
        }
    }
	
}
