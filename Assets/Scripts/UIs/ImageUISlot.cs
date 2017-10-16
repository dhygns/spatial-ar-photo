using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageUISlot : MonoBehaviour {

	//imageWrapper Objects
	public ImageUIWrapper[] imageWrapper;

	//imageWarpper Release Status
	enum ReleasedStatus { Inertia, Default };
	ReleasedStatus releasedStatus;

	//Function Delegate For ImageWrapper
	delegate void UpdateImageWrapper(float dt);
	UpdateImageWrapper updateImageWrapper;

	void Awake() {
	}

	// Use this for initialization
	void Start () {
//		this.transform.parent = Camera.main.transform;
//		this.transform.localScale = Vector3.one;
		this.transform.localEulerAngles = Vector3.zero;
//		this.transform.localPosition = new Vector3 (0.0f,-0.04f, -0.03f);
	}
	
	// Update is called once per frame
	void Update () {

		//Sperator Event
		if (ImageUITouch.isPreased == true) {
			if(ImageUITouch.NormalDownPosition.y < 0.2){
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
		foreach(ImageUIWrapper obj in imageWrapper) {
			
		}
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
	private Queue<float> speedx = new Queue<float> ();

	//init for claculating a SlotRotatingSpeed
	private void clearCalcSlotRotateSpeed() {
		prevx = 0.0f;
		currx = 0.0f;
		speedx.Clear ();
	}

	//calculating Slot Rotating Speed
	private float calcSlotRotateSpeed() {
		float speed = 0.0f;
		prevx = currx;
		currx = ImageUITouch.ElaspedNormalPosition.x;
		if (speedx.Count > 8)
			speedx.Dequeue ();
		speedx.Enqueue (currx - prevx);

		for(int i = 0 ; i < speedx.Count ; i ++) {
			speed += speedx.ToArray () [i] / speedx.Count;
		}
		return speed * 20.0f;
	}

	private float calcSlotDeltaPosition() {
		return (currx - prevx) * 20.0f;
	}


	private bool isPicked = false;
	public void PickUp() {
		if (isPicked)
			return;
		isPicked = true;
		foreach(ImageUIWrapper obj in imageWrapper) {
			obj.rotationTarget.z -= 4.5f * 0.5f;
		}
	}

	public void PickDown() {
		if (!isPicked)
			return;
		isPicked = false;
		foreach(ImageUIWrapper obj in imageWrapper) {
			obj.rotationTarget.z += 4.5f * 0.5f;
		}
	}

	public void PickInit() {
		isPicked = false;
	}

	public Transform GetCurrentCenterWrapper() {
		Transform wrapper = null;
		foreach(ImageUIWrapper obj in imageWrapper) {
			if (obj.ID == 3 || obj.ID == 4)
				wrapper = obj.gameObject.transform;
		}
		return wrapper;
	}
}
