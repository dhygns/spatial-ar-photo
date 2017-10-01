using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageUIObject : MonoBehaviour {
	
	private Camera UICamera;
	private ImageUISlot UISlot;

	private Transform objectSlot;
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
		objectSlot = GameObject.Find("UISlot").transform;
		UICamera = GameObject.Find ("UICamera").GetComponent<Camera>();
		UISlot = GameObject.Find ("UISlot").GetComponent<ImageUISlot> ();
		objectRoot = GameObject.Find ("ObjectRoot").transform;
		objectWrapper = this.transform.parent;
		objectRigidbody = this.GetComponent<Rigidbody> ();
//		objectRigidbody.Sleep ();
		motion = UIMotion;

	}

	// Use this for initialization
	void Start () {
		string idx = "dummy" + Random.Range (1, 6) + "";
		float ratio = UISlot.imageDics [idx].width / UISlot.imageDics [idx].height;
		this.GetComponent<MeshRenderer> ().material.mainTextureScale = new Vector2 (1.0f * ratio, -1.0f);
		this.GetComponent<MeshRenderer> ().material.mainTexture = UISlot.imageDics [idx]; 
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

		//calculating to get depth.
		touch = UICamera.WorldToScreenPoint (this.transform.position);

		//calculating to get position x, y.
		touch.x = ImageUITouch.PixelPosition.x;
		touch.y = ImageUITouch.PixelPosition.y;
		position = UICamera.ScreenToWorldPoint (touch);

		calcSlotRotateSpeed ();
		if (ImageUITouch.Status == ImageUITouch.TouchStatus.idle) {
			//need one more condition. (speed or area)
			if (ImageUITouch.ElaspedNormalPosition.y > limit) {
				this.transform.parent = UICamera.transform;
				Vector3 eular = this.transform.localEulerAngles;
					
				this.transform.parent = Camera.main.transform;
				this.transform.localEulerAngles = eular;

				this.transform.parent = objectRoot;
				this.transform.position = Camera.main.ScreenToWorldPoint (touch);

				enableRigidBody ();
				motion = ReleaseMotion;
			} else {
				
				motion = UIMotion;
			}
			UISlot.PickInit ();
		} else {
			if (ImageUITouch.ElaspedNormalPosition.y > limit) {
				this.transform.parent = objectSlot;
				UISlot.PickUp ();
			} else {
				this.transform.parent = objectWrapper;
				UISlot.PickDown ();
			}


			//set position
			this.transform.position += (position - this.transform.position) * dt * 20.0f;
		}
	}

	//private clac 
	private Vector2 prev = Vector2.zero;
	private Vector2 curr = Vector2.zero;
	private Queue<Vector2> spdq = new Queue<Vector2> ();
	private Vector2 spdm = new Vector2();

	//init for claculating a SlotRotatingSpeed
	private void clearCalcSlotRotateSpeed() {
		prev = ImageUITouch.ElaspedNormalPosition;
		curr = ImageUITouch.ElaspedNormalPosition;
		spdq.Clear ();
	}

	//calculating Slot Rotating Speed
	private void calcSlotRotateSpeed() {
		Vector2 speed = Vector2.zero;
		prev = curr;
		curr = ImageUITouch.ElaspedNormalPosition;
		if (spdq.Count > 4)
			spdq.Dequeue ();
		spdq.Enqueue (curr - prev);

		for(int i = 0 ; i < spdq.Count ; i ++) {
			speed += spdq.ToArray () [i] / spdq.Count;
		}
		spdm = speed * 10.0f;
	}

	private Vector2 calcSlotDeltaPosition() {
		return (curr - prev) * 10.0f;
	}

	//Enable Rigidbody
	private void enableRigidBody() {
		Vector2 dir = spdm;
		dir.y = Mathf.Min (0.5f, dir.y);
		GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.None;
		GetComponent<Rigidbody> ().useGravity = true;
		GetComponent<Rigidbody> ().AddForce (
			this.transform.up * dir.y * 300.0f + 
			this.transform.right * dir.x * 10.0f
		);

		GetComponent<Rigidbody> ().AddTorque (Random.rotation.eulerAngles * 0.0005f);
	}

}
