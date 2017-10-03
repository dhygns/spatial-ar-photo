using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TouchItem : MonoBehaviour {
	
	public int id;
	TouchVisualization holder;
	
	public void Initialize(TouchVisualization holder, int id)
	{
		this.id = id;
		this.holder = holder;
		
		LateUpdate();
	}
	
	void LateUpdate () 
	{
		//itterate all current touches
		foreach(var p in InputManager.instance.touches)
		{
			//look for touch with this.id
			if(p.id == id)
			{
				var worldScreenHeight = InputManager.instance.GetComponent<Camera>().orthographicSize;
				var worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;
				
				//position item space
				transform.localPosition = new Vector2(
					(p.pos.x / (InputManager.instance.GetComponent<Camera>().pixelWidth / 2) - 1) * worldScreenWidth,
					(p.pos.y / (InputManager.instance.GetComponent<Camera>().pixelHeight / 2) - 1) * worldScreenHeight
					);
				
				//if maxforce is not available
				if (p.maxforce < 0) 
				{
					if (p.radius > 0)						
					{
						//Using normalized value for radiuses between 4 and 10
						var rad = Mathf.Clamp((p.radius / p.radiusTolerance) - 4f, 0f, 6f) / 6f;
						transform.localScale = Vector3.Lerp(Vector3.one*0.1f, Vector3.one, rad);
					}
					else
						transform.localScale = Vector3.one / 2;
					
				}
				else
					transform.localScale = Vector3.one * p.force/p.maxforce; //for iphone 6s max force is 6.66667, average touch force is ~1.
				
				return;
			}
		}
		
		//if id not found on the list, the touch has ended and item should be destroyed.
		DestroyObject(gameObject);
		holder.OnTouchDestroyed(id);
	}
}
