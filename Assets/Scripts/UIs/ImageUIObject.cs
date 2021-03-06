﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageUIObject : MonoBehaviour {
	
	private Camera UICamera;
	private ImageUISlot UISlot;

	private Transform objectSlot;
	private Transform objectRoot;
	private Transform objectWrapper;

	//Objects trasnforms
	private Vector3 touch;
	private Vector3 position;
	private Vector3 rotation;
	private Vector3 velocity;
	private Vector3 scale;


	//UI Moition Updator Manager
	delegate void UpdateMotion (float dt);
	UpdateMotion updateMotion;

	void Awake() {
		//Get Root Transform.
		objectSlot = GameObject.Find("UISlot").transform;
		UICamera = GameObject.Find ("UICamera").GetComponent<Camera>();

		UISlot = GameObject.Find ("UISlot").GetComponent<ImageUISlot> ();
		objectRoot = GameObject.Find ("ObjectRoot").transform;
		objectWrapper = this.transform.parent;

		scale = this.transform.localScale;
//		objectRigidbody.Sleep ();
		updateMotion = updateUIMotion;

	}

	// Use this for initialization
	void Start () {
		Debug.Log ("Created");
		if (this.transform.parent != null) {
			Create (this.transform.parent);
		}
	}


	// Update is called once per frame
	void Update () {
		if(updateMotion != null) updateMotion (Time.deltaTime);

	}



	// update Function for UI MODE
	void updateUIMotion(float dt) {
		if (ImageUITouch.Status == ImageUITouch.TouchStatus.pick &&
			ImageUITouch.HitObject == this.gameObject) {
			updateMotion = updateGrabMotion;
		} 

		position.Set (0.0f, 2.0f, 0.0f);
		rotation.Set (0.0f, 0.0f, 0.0f);

		rotation.z = rotation.z > 180.0f ? rotation.z - 360.0f : rotation.z;
		rotation.z += (0.0f - rotation.z) * dt * 10.0f;

		this.transform.localPosition += (position - this.transform.localPosition) * dt * 10.0f;
		this.transform.localEulerAngles = rotation;
		this.transform.localScale += (scale - this.transform.localScale) * dt * 10.0f;

		objectWrapper = this.transform.parent;
	}

	//update Function for Released MODE
	void updateReleaseMotion(float dt) {
		//it was supposed to check
		if (ImageUITouch.HitObject == this.gameObject) {
			#if UNITY_EDITOR
			attachToUI();
			scale.x = 0.15f;
			scale.y = 0.19f;
			scale.z = 0.03f;

			#elif UNITY_IPHONE
			if (ImageUITouch.Force < 5.0f) {
				scale.x = 0.15f * Mathf.Max(1.0f, ImageUITouch.Force) * 0.5f;
				scale.y = 0.19f * Mathf.Max(1.0f, ImageUITouch.Force) * 0.5f;
				scale.z = 0.03f;
			} else {
				attachToUI();
				scale.x = 0.15f;
				scale.y = 0.19f;
				scale.z = 0.03f;
			}
			#endif
		} else {
			scale.x = 0.15f;
			scale.y = 0.19f;
			scale.z = 0.03f;
		}
		if (this.transform.localPosition.y < -1.0) {
			this.Remove ();
			ImageUIObjectsManager.SetLeft (this.gameObject);
		}
		this.transform.localScale += (scale - this.transform.localScale) * dt * 10.0f;
	}

	//update Function for Grabed MODE
	void updateGrabMotion(float dt) {
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
				attachToWorld ();
				updateMotion = updateReleaseMotion;
			} else {
				
				updateMotion = updateUIMotion;
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
		spdm = speed * 5.0f;
	}

	private Vector2 calcSlotDeltaPosition() {
		return (curr - prev) * 5.0f;
	}


	//change parent 
	private void attachToUI() {
		objectWrapper = UISlot.GetCurrentCenterWrapper ();

		this.transform.parent = Camera.main.transform;
		position = this.transform.localPosition;

		this.transform.parent = UICamera.transform;
		this.transform.localPosition = position;

		this.transform.parent = objectWrapper;
		updateMotion = updateUIMotion;

		disableRigidBody();
	}

	private void attachToWorld() {
		this.transform.parent = UICamera.transform;
		Vector3 eular = this.transform.localEulerAngles;

		this.transform.parent = Camera.main.transform;
		this.transform.localEulerAngles = eular;

		this.transform.parent = objectRoot;
		this.transform.position = Camera.main.ScreenToWorldPoint (touch);

		enableRigidBody ();

        Texture2D tex = this.GetComponent<MeshRenderer>().material.mainTexture as Texture2D;
        byte[] pixels = tex.EncodeToPNG();
        StartCoroutine(sendTexture(pixels, tex.width, tex.height));
	}
		
    //temp send texture
    IEnumerator sendTexture(byte[] pixels, int width, int height) {//Temp 
        int pixelSize = pixels.Length;

        for (int step = 0; step < pixels.Length / 1024; step++)
        {
            CoSpatialProtocol.GetImage msg = new CoSpatialProtocol.GetImage();

            msg.width = width;
            msg.height = height;
            msg.maxstep = (int)Mathf.Ceil(pixels.Length / 1024);
            msg.step = step;

            int len = Mathf.Min(pixelSize, 1024);
            msg.pixels = new byte[len];
            for (int i = 0; i < len; i++)
            {
                msg.pixels[i] = pixels[step * 1024 + i];
            }
            CoSpatial.Network.Send(CoSpatialProtocol.Type.GetImage, msg);
            yield return null;
            pixelSize -= 1024;
        }
    }

	//Enable Rigidbody
	private void enableRigidBody() {
		Vector2 dir = spdm;
		Vector3 rot = Random.rotation.eulerAngles * 0.00005f;
		Rigidbody rigidbody = GetComponent<Rigidbody> ();
		float mas = rigidbody.mass;


		dir.y = Mathf.Min (0.7f, dir.y);
		rot *= mas; rot.y = 0.0f; rot.x = 0.0f;

		rigidbody.constraints = RigidbodyConstraints.None;
		rigidbody.useGravity = true;
		rigidbody.AddForce (
			this.transform.up * dir.y * 300.0f * mas + 
			this.transform.right * dir.x * 10.0f * mas
		);
		rigidbody.AddTorque (rot);
	}


	//Disable Rigidbody
	private void disableRigidBody() {
		Rigidbody rigidbody = GetComponent<Rigidbody> ();
		rigidbody.constraints = RigidbodyConstraints.FreezeAll;
		rigidbody.useGravity = false;

	}


	//public create interface
	public int imageID;
	public void Init(int imgID) {
		imageID = imgID;
	}

	public void Create(Transform parent) {
		//Create Images
		Texture2D tex = ImageLoader.GetTexture (imageID);
		float ratio = 1.0f;//tex.width / tex.height;
		this.GetComponent<MeshRenderer> ().material.mainTextureScale = new Vector2 (1.0f * ratio, 1.0f);
		this.GetComponent<MeshRenderer> ().material.mainTexture = tex;
		//Set transform

		position.Set (0.0f, 2.0f, 0.0f);
		rotation.Set (0.0f, 0.0f, 0.0f);

		//Set Parent
		this.transform.parent = parent;

		this.transform.localPosition = position;
		this.transform.localEulerAngles = rotation;
		this.transform.localScale = scale;

	}

	public void Remove() {
		//Remove Texture for memory
		Texture2D tex = this.GetComponent<MeshRenderer> ().material.mainTexture as Texture2D;
		this.GetComponent<MeshRenderer> ().material.mainTexture = null;
		#if UNITY_EDITOR

		#elif UNITY_IPHONE
		Destroy(tex);
		#endif

		//Remove Parent
		this.transform.parent = null;
	}
}
