using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageUIWrapper : MonoBehaviour {
	public int ID;

	//Side Wrapper
	public Transform LeftWrapper;
	public Transform RightWrapper;

	//Image Object
	private GameObject ImagePrefab;
	public Transform ImageObject;

//	private float lastRotationStep = 0.0f;
//	private float[] allRotationSteps = new float[]{
//		-13.5f, -9.0f, -4.5f, 0.0f, 4.5f, 9.0f, 13.5f
//	};
	private float minLimit = -13.5f;
	private float maxLimit =  13.5f;
	private float sideLength = 31.5f;
	private float eachDistance = 4.5f;
		
	private Vector3 rotation = Vector3.zero;
	public Vector3 rotationTarget = Vector3.zero;

	private Vector3 position = new Vector3(0.0f, -2.5f, 1.0f);

	void Awake() {
		ImagePrefab = Resources.Load ("HitCube") as GameObject;

		rotationTarget = this.transform.localEulerAngles;
		rotationTarget.z = rotationTarget.z > 180.0f ? rotationTarget.z - 360.0f : rotationTarget.z;

//		lastRotationStep = rotation.z;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		position.y = -2.45f - Mathf.Abs(rotation.z) * 0.002f;
		position.z =  1.0f + Mathf.Abs(rotation.z) * 0.004f;

		rotation += (rotationTarget - rotation) * Time.deltaTime * 10.0f;

		this.transform.localEulerAngles = rotation;
		this.transform.localPosition = position;

		watchPosition ();
		watchChild ();
	}

	//Check that ImageObject is in this wrapper
	void watchChild() {
		if (this.transform.childCount == 0) {
			if (ID == 0 || ID == 6) {
				Instantiate (ImagePrefab, this.transform);
			} else {
				RightWrapper.GetChild (0).parent = this.transform;
			}
		} else if (this.transform.childCount == 2) {
			if (ID == 0 || ID == 6) {
				GameObject.Destroy (this.transform.GetChild (0).gameObject);
			} else {
				this.transform.GetChild (0).parent = RightWrapper;
			}
		}
	}

	private void watchPosition() {
		
		if (rotation.z > maxLimit + eachDistance * 0.5f) {
			rotation.z -= sideLength;
			rotationTarget.z -= sideLength;
		}

		if (rotation.z < minLimit - eachDistance * 0.5f) {
			rotation.z += sideLength;
			rotationTarget.z += sideLength;
		}

		float targ = Mathf.Round (rotation.z / 4.5f);
		ID = (int)(targ + 10) % 7;
	}


	//Interface 
	private float keepSpeed = 0.0f;

	public void Move(float spdpertime) {
//		float prevz = rotation.z;
//		float currz = rotation.z - spdpertime;

		keepSpeed = spdpertime;
		rotationTarget.z -= keepSpeed;

//
//		//limit indices
//		int s = 0, e = allRotationSteps.Length - 1;
//
//		if (spdpertime > 0.0f) {
//			float targ = Mathf.Floor (rotation.z / 4.5f);
//			ID = (int)(targ + 10) % 7;
//			lastRotationStep = targ * 4.5f;
//			if (lastRotationStep < allRotationSteps [s]) {
//				lastRotationStep = allRotationSteps [e];
//				rotation.z += sideLength;
//			}
//		}
//		else {
//			float targ = Mathf.Ceil (rotation.z / 4.5f);
//			ID = (int)(targ + 10) % 7;
//			lastRotationStep = targ * 4.5f;
//			if (lastRotationStep > allRotationSteps [e]) {
//				lastRotationStep = allRotationSteps [s];
//				rotation.z -= sideLength;
//			}
//		}
	}

	public void Keep(float spdpertime, float dt) {
		
		if (Mathf.Abs (spdpertime) < 0.1f) {
			keepSpeed += (0.0f - keepSpeed) * dt * 10.0f;
			rotationTarget.z -= keepSpeed;

		} else {
			keepSpeed = spdpertime;
			rotationTarget.z -= keepSpeed;
		}
//		rotation.z += (lastRotationStep - rotation.z) * dt * 2.0f;
	}

	public void setRotationTarget(float z) {
		rotationTarget.z = z;
	}
}
