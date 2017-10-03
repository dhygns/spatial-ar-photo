using UnityEngine;
using System.Collections;

public class BGStretch : MonoBehaviour 
{
	void Start()
	{
		Resize ();
	}
		
	/// <summary>
	/// Resize the background image to cover whole camera view
	/// </summary>
	void Resize()
	{
		SpriteRenderer bg = GetComponent<SpriteRenderer> ();
		if (bg == null)
			return;

		transform.localScale = Vector3.one;
		
		var width = bg.sprite.bounds.size.x;
		var height = bg.sprite.bounds.size.y;
		
		var worldScreenHeight = InputManager.instance.GetComponent<Camera>().orthographicSize * 2.0;
		var worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;
		
		var scale = Vector3.one;
		scale.x = (float)(worldScreenWidth / width);
		scale.y = (float)(worldScreenHeight / height);
		
		transform.localScale = scale;
	}

	void Update () 
	{	
		Resize ();
	}
}
