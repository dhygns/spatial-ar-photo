﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageUISlot : MonoBehaviour {


	//imageWrapper Objects
	public GameObject imageSlot;
	public ImageUIWrapper[] imageWrapper;


	//imageWarpper Release Status
	enum ReleasedStatus { Inertia, Default };
	ReleasedStatus releasedStatus;

	//Function Delegate For ImageWrapper
	delegate void UpdateImageWrapper(float dt);
	UpdateImageWrapper updateImageWrapper;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		//Sperator Event
		if (ImageUITouch.isPreased == true) {
			//
			switch (ImageUITouch.Status) {
			case ImageUITouch.TouchStatus.idle: 
				updateImageWrapper = updateImageWrapperGrap;
				break;
			case ImageUITouch.TouchStatus.pick:
				updateImageWrapper = updateImageWrapperPick;
				break;
			case ImageUITouch.TouchStatus.swipe:
				updateImageWrapper = updateImageWrapperSwipe;
				break;
			default :
				Debug.LogWarning ("Unknown Touch Status");
				break;
			}
		} else {
			updateImageWrapper = updateImageWrapperReleased;
		}

		if (updateImageWrapper != null)
			updateImageWrapper (Time.deltaTime);
		
	}

	//Slots Methods
	private float slotRotateSpeed = 0.0f;


	//Released Logics
	void updateImageWrapperReleased(float dt) {
		
		//case 1 : decrease speed
		if (Mathf.Abs(slotRotateSpeed) > 0.04f) {
			slotRotateSpeed += (0.0f - slotRotateSpeed) * 2.0f * dt;
			foreach(ImageUIWrapper obj in imageWrapper) {
				obj.Move (slotRotateSpeed);
			}
		}		
		//case 2 : come back to original position
		else {
			foreach(ImageUIWrapper obj in imageWrapper) {
				obj.Keep (slotRotateSpeed, dt);
			}
		}
			
	}

	//Grap Logics
	void updateImageWrapperGrap(float dt) {
		//all stop
		slotRotateSpeed = 0.0f;

		clearCalcSlotRotateSpeed ();

		foreach(ImageUIWrapper obj in imageWrapper) {
			obj.Move (slotRotateSpeed);
		}

	}

	//Pick Logics
	void updateImageWrapperPick(float dt) {
		
	}

	//Swipe Logics
	void updateImageWrapperSwipe(float dt) {
		slotRotateSpeed = calcSlotRotateSpeed();

		foreach(ImageUIWrapper obj in imageWrapper) {
			obj.Move (calcSlotDeltaPosition());
		}
	}


	//private clac 
	private float prevx = 0.0f;
	private float currx = 0.0f;
	private Queue<float> speeds = new Queue<float> ();

	//init for claculating a SlotRotatingSpeed
	private void clearCalcSlotRotateSpeed() {
		prevx = 0.0f;
		currx = 0.0f;
		speeds.Clear ();
	}

	//calculating Slot Rotating Speed
	private float calcSlotRotateSpeed() {
		float speed = 0.0f;
		prevx = currx;
		currx = ImageUITouch.ElaspedNormalPosition.x;
		if (speeds.Count > 8)
			speeds.Dequeue ();
		speeds.Enqueue (currx - prevx);

		for(int i = 0 ; i < speeds.Count ; i ++) {
			speed += speeds.ToArray () [i] / speeds.Count;
		}
		return speed * 10.0f;
	}

	private float calcSlotDeltaPosition() {
		return (currx - prevx) * 10.0f;
	}



}
