using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class ButtonScript : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
	private int touchId;
	private bool down = false;
	private bool over = false;

	private bool supportsForceTouch;
	private float downTime;

	public Popup popup;
	public int id;

	public void Start()
	{
		//this is important to differentiate behavior
		supportsForceTouch = ForceTouchPlugin.SupportsForceTouch ();
	}

	public void OnPointerDown (PointerEventData eventData)
	{
		//we don't want to disable multitouch for the whole project, but we want to avoid
		//trouble therefore we only listen for mouse (id always -1) and for first touch (id always 0) on the screen
		if (eventData.pointerId > 0)
			return;
		
		touchId = eventData.pointerId;
		down = true;
		downTime = Time.realtimeSinceStartup; //used to handle clicks

		popup.Reset (id); //set popup properties
		this.gameObject.transform.SetAsLastSibling (); //rearange buttons to make sure that active button is always on top of the other content
		popup.transform.SetAsLastSibling (); //make sure popup is always on top of everything
	}

	public void OnPointerUp (PointerEventData eventData)
	{
		down = false;
		this.gameObject.transform.localScale = Vector3.one;
			
		var duration = Time.realtimeSinceStartup - downTime;

		//if it's a click over the button
		if ((!supportsForceTouch || duration < 0.3f) && over)
			Click ();
		//if it was action involving 3d touch but user released the button in push state
		else if (!popup.IsPop())
			popup.Hide ();
	}

	public void OnPointerEnter (PointerEventData eventData)
	{
		over = true;
	}


	public void OnPointerExit (PointerEventData eventData)
	{
		over = false;
	}



	void Update()
	{
		//the code below is only executed on devices supporting 3d touch
		if (!supportsForceTouch)
			return;

		//if down and touch is not mouse
		if (down && (touchId != -1)) 
		{
			//use scale to provide visual feedback.
			//this is not crucial, because popup provides better visual feedback
			var t = ForceTouchPlugin.GetTouchExtraData(touchId); //is the same as Input.GetTouch(touchId).GetExtraData();
			this.gameObject.transform.localScale = Vector3.one * Mathf.Lerp (1f, 1.2f, Mathf.Clamp (t.force / (t.maxforce / 2f), 0f, 1f)); 

			//code above could be replaced with this as well, but above call has fewer function calls and is JUST A BIT more efficient
			//var t = Input.GetTouch (touchId);
			//this.gameObject.transform.localScale = Vector3.one * Mathf.Lerp (1f, 1.2f, Mathf.Clamp (t.GetForce() / (t.GetMaxForce() / 2f), 0f, 1f)); 

			popup.SetForce (t.force, t.maxforce);
		}
	}

	public void Click()
	{
		popup.Reset (id);
		popup.Click ();
	}
}
