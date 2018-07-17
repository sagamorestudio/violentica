using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZO;

public class scTester : MonoBehaviour {

	// Use this for initialization
	void Start () {
        test().VPlay();
	}

    IEnumerator test()
    {
        Boxer<TotalSpreadSheet> bout = new Boxer<TotalSpreadSheet>(null);
        yield return RefTable_google.GetDBFromWWW(bout).VPlay();
        var downloaded_spreadsheet = bout.value;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
