using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZO;

public class Reftable : scSingleToneForMB<Reftable> {

    WrapPlayerPrefs prefs;

    public Reftable()
    {
        prefs = new WrapPlayerPrefs("total_refdata");
    }
    
    public IEnumerator Initialize()
    {
        if (RefTable_google.HowMuchTimeFlowedLastDownloaded > System.TimeSpan.FromDays(1))
        {
            Boxer<TotalSpreadSheet> box = new Boxer<TotalSpreadSheet>(null);           
            yield return RefTable_google.GetDBFromWWW(box).VPlay();
            prefs.Value = Util.ZipString(box.value.ToString());
            total_data = box.value;
        }
        else
        {
            var unzip = Util.UnzipString(prefs.Value);
            var dic = MiniJSON.Json.Deserialize(unzip) as Dictionary<string, object>;
            total_data = new TotalSpreadSheet(dic);
        }
    }

    TotalSpreadSheet total_data;
}
