using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LegacyTouchVisualization : MonoBehaviour {

    public GameObject pointerPrefab;

	private Dictionary<int, LegacyTouchItem> touches = new Dictionary<int, LegacyTouchItem>();

	void LateUpdate () 
	{
		//query current touches
		var inputTouches = LegacyInputManager.instance.touches;

		//if touch hasn't been added to display list, one should be added
		foreach(var p in inputTouches) 
		{
			if (!touches.ContainsKey(p.id))
				AddTouch(p.id);
		}
	}

    private void AddTouch(int id)
    {
		var touch = (Instantiate(pointerPrefab) as GameObject).GetComponent<LegacyTouchItem>();
        touch.Initialize(this, id);
        touches[id] = touch;
    }


	//called by TouchItem
    public void OnTouchDestroyed(int id)
    {
        touches.Remove(id);
    }
}
