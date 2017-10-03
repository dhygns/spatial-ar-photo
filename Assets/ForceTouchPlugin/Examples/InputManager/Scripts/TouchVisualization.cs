using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TouchVisualization : MonoBehaviour {

    public GameObject pointerPrefab;

	private Dictionary<int, TouchItem> touches = new Dictionary<int, TouchItem>();

	void LateUpdate () 
	{
		//query current touches
		var inputTouches = InputManager.instance.touches;

		//if touch hasn't been added to display list, one should be added
		foreach(var p in inputTouches) 
		{
			if (!touches.ContainsKey(p.id))
				AddTouch(p.id);
		}
	}

    private void AddTouch(int id)
    {
		var touch = (Instantiate(pointerPrefab) as GameObject).GetComponent<TouchItem>();
        touch.Initialize(this, id);
        touches[id] = touch;
    }


	//called by TouchItem
    public void OnTouchDestroyed(int id)
    {
        touches.Remove(id);
    }
}
