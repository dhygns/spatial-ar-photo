using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageUIObject : MonoBehaviour {

	//Objects Common Root
	private ImageUISlot UISlot;

	private Transform objectRoot;
	private Transform objectWrapper;

	private Rigidbody objectRigidbody;


	//Objects trasnforms
	private Vector3 touch;
	private Vector3 position;
	private Vector3 rotation;
	private Vector3 velocity;

	void Awake() {
		//Get Root Transform.
		UISlot = GameObject.Find ("UISlot").GetComponent<ImageUISlot> ();
		objectRoot = GameObject.Find ("ObjectRoot").transform;
		objectWrapper = this.transform.parent;
		objectRigidbody = this.GetComponent<Rigidbody> ();
//		objectRigidbody.Sleep ();
		motion = UIMotion;
	}

	// Use this for initialization
	void Start () {
		
	}


	delegate void Motion (float dt);
	Motion motion;

	// Update is called once per frame
	void Update () {
		if(motion != null) motion (Time.deltaTime);
	}

	void UIMotion(float dt) {
		if (ImageUITouch.Status == ImageUITouch.TouchStatus.pick &&
			ImageUITouch.HitObject == this.gameObject) {
			motion = GrabMotion;
		} 
		position.Set (0.0f, 2.0f, 0.0f);

		rotation = this.transform.localEulerAngles;
		rotation.z = rotation.z > 180.0f ? rotation.z - 360.0f : rotation.z;
		rotation.z += (0.0f - rotation.z) * dt * 10.0f;

		this.transform.localPosition += (position - this.transform.localPosition) * dt * 10.0f;
		this.transform.localEulerAngles = rotation;

		objectWrapper = this.transform.parent;
	}

	void ReleaseMotion(float dt) {

	}

	void GrabMotion(float dt) {
		const float limit = 0.2f;

		if (ImageUITouch.Status == ImageUITouch.TouchStatus.idle) {
			//need one more condition. (speed or area)
			if (ImageUITouch.ElaspedNormalPosition.y > limit) {
				this.transform.parent = objectRoot;
				motion = ReleaseMotion;
			} else {
				motion = UIMotion;
			}
			UISlot.PickInit ();
		} else {
			if (ImageUITouch.ElaspedNormalPosition.y > limit) {
				this.transform.parent = objectRoot;
				UISlot.PickUp ();
			} else {
				this.transform.parent = objectWrapper;
				UISlot.PickDown ();
			}
		}

		//calculating to get depth.
		touch = Camera.main.WorldToScreenPoint (this.transform.position);

		//calculating to get position x, y.
		touch.x = ImageUITouch.PixelPosition.x;
		touch.y = ImageUITouch.PixelPosition.y;
		position = Camera.main.ScreenToWorldPoint (touch);

		//set position
		this.transform.position += (position - this.transform.position) * dt * 20.0f;
	}

	//private clac 
	private Vector2 prev = Vector2.zero;
	private Vector2 curr = Vector2.zero;
	private Queue<Vector2> spdq = new Queue<Vector2> ();

	//init for claculating a SlotRotatingSpeed
	private void clearCalcSlotRotateSpeed() {
		prev = Vector2.zero;
		curr = Vector2.zero;
		spdq.Clear ();
	}

	//calculating Slot Rotating Speed
	private Vector2 calcSlotRotateSpeed() {
		Vector2 speed = Vector2.zero;
		prev = curr;
		curr = ImageUITouch.ElaspedNormalPosition;
		if (spdq.Count > 8)
			spdq.Dequeue ();
		spdq.Enqueue (curr - prev);

		for(int i = 0 ; i < spdq.Count ; i ++) {
			speed += spdq.ToArray () [i] / spdq.Count;
		}
		return speed * 10.0f;
	}

	private Vector2 calcSlotDeltaPosition() {
		return (curr - prev) * 10.0f;
	}

}
