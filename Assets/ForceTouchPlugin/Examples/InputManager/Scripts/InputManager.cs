using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class InputManager : MonoBehaviour
{
	public static InputManager instance;
	
	private int mouseId = -1;
	private Vector2 lastMousePos;

	public float nativeTouchScale = 1;
	public List<NativeTouch> touches;
	public bool supportsTouchRadius;
	public ForceTouchState forceTouchState;

	public void Awake()
    {
		instance = this;
		Application.targetFrameRate = 60; //iOS by default runs 30fps

		//retrieve screen scale (iOS operates in lowDPI mode)
		nativeTouchScale = ForceTouchPlugin.GetScaleFactor();
		
		//checks whether current device supports TouchRadius (iOS 8 and above)
		supportsTouchRadius = ForceTouchPlugin.SupportsTouchRadius ();

		//prepare to store touches
		touches = new List<NativeTouch> ();	
    }

	public void Start()
	{
		//during awake of first scene force touch state is not yet available. If InputManager is initialized later it's ok to call this in Awake.
		//For platforms other than iOS ForceTouchState.IncompatibleOS will be returned.
		forceTouchState = ForceTouchPlugin.GetForceTouchState(); 

		//if (forceTouchState != ForceTouchState.IncompatibleOS)
		//	AddCallback ();
	}

	/*
	 * Method that will be called by native end if ForceTouch state changes. You must
	 * subscribe to that event by calling AddCallback method. For this method to be called,
	 * InputManager component has to be in hierarchy. 
	 * See AddCallbackMethod in ForceTouchPlugin.cs for more info
	 */
	public void UpdateForceTouch(string message)
	{
		if (GetComponent<StatusUI>()) //this is only required for StatusUI.
			GetComponent<StatusUI>().callbackMessage = message;
			
		forceTouchState = ForceTouchPlugin.GetForceTouchState (Int32.Parse(message));
	}
	
	/*
	 * Method used to subscribe this component as event handler for ForceTouch state change (use car disable/enable ForceTouch/3D touch in accessibility settings).
	 */
	public void AddCallback()
	{
		ForceTouchPlugin.SetCallbackMethod(this.name, "UpdateForceTouch"); //won't be executed on anything but iOS so no need to add compiler conditionals
	}
	
	/* 
	 * Removes this component as event handler for ForceTouch state change event. 
	 */ 
	public void RemoveCallback()
	{
		ForceTouchPlugin.RemoveCallbackMethod();
	}


	/* 
	 * Refresh touch data.
	 */
	public void Update () 
	{	
		if (GetComponent<StatusUI>()) //this is only required for StatusUI.
			GetComponent<StatusUI>().currentInput = -1;

		if (Input.touchCount > 0) //if touches are available
		{
			touches.Clear();
			for (int i=0; i<Input.touchCount; i++) 
			{
				var t = Input.touches [i];
				HandleInput (t.fingerId, t.phase, t.position, t.deltaPosition, t.GetForce(), t.GetMaxForce(), t.GetRadius(), t.GetRadiusTolerance());
			}

			if (GetComponent<StatusUI>()) //this is only required for StatusUI.
				GetComponent<StatusUI>().currentInput = 1;
		}
		else //if unity touch is unavailable fallback to mouse input
		{
			touches.Clear();
			var temp = new NativeTouchExtraData ();

			var mousePos = new Vector2(Input.mousePosition.x,Input.mousePosition.y);
			if(Input.GetMouseButtonDown(0))
				HandleInput(mouseId, TouchPhase.Began, mousePos, Vector2.zero, temp.force, temp.maxforce, temp.radius, temp.radiusTolerance);
			else if(Input.GetMouseButtonUp(0))
				HandleInput(mouseId, TouchPhase.Ended, mousePos, mousePos-lastMousePos, temp.force, temp.maxforce, temp.radius, temp.radiusTolerance);
			else if(Input.GetMouseButton(0))
			{
				var delta = mousePos-lastMousePos;
				if(delta!=Vector2.zero)
					HandleInput(mouseId, TouchPhase.Moved, mousePos, delta, temp.force, temp.maxforce, temp.radius, temp.radiusTolerance);
				else
					HandleInput(mouseId, TouchPhase.Stationary, mousePos, delta, temp.force, temp.maxforce, temp.radius, temp.radiusTolerance);
			}
			else
				return;

			lastMousePos = Input.mousePosition;

			if (GetComponent<StatusUI>()) //this is only required for StatusUI.
				GetComponent<StatusUI>().currentInput = 2;
		}
	}
	
	/*
	 * Add UnityTouch and Mouse events to the list
	 */
	private void HandleInput (int pointerId, TouchPhase phase, Vector2 position, Vector2 deltaPosition, float force, float maxForce, float radius, float radiusTolerance)
	{
        touches.Add (new NativeTouch ()
		{
			id = pointerId,
			pos = position,
			delta = deltaPosition,
			phase = phase,
			force = force,
			maxforce = maxForce,
			radius = radius,
			radiusTolerance = radiusTolerance
		});
	}
}