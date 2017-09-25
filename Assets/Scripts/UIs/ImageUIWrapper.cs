using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageUIWrapper : MonoBehaviour {
	public int ID;

	private float lastRotationStep = 0.0f;
	private float[] allRotationSteps = new float[]{
		-13.5f, -9.0f, -4.5f, 0.0f, 4.5f, 9.0f, 13.5f
	};
	private float sideLength = 27.0f;
	private float eachDistance = 4.5f;
		
	private Vector3 rotation = Vector3.zero;

	void Awake() {
		rotation = new Vector3(0.0f, 0.0f, allRotationSteps[ID]);
		lastRotationStep = rotation.z;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.localEulerAngles = rotation;
	}

	//Interface 

	public void Move(float spdpertime) {
		float prevz = rotation.z;
		float currz = rotation.z -= spdpertime;

		//Left Limit to Right
		int s = 0, e = allRotationSteps.Length - 1;

		//Checking Last Step
		for (int i = 0; i < allRotationSteps.Length; i++) {
			if (Mathf.Sign (allRotationSteps[i] - currz) != 
				Mathf.Sign (allRotationSteps[i] - prevz)) {
				int idx = i + (int)Mathf.Sign (-spdpertime);
				if (idx == s - 1) 
					lastRotationStep = allRotationSteps [s] - eachDistance;
				else if (idx == e + 1)
					lastRotationStep = allRotationSteps [e] + eachDistance;
				else lastRotationStep = allRotationSteps [idx];
			}
		}

		if (currz < allRotationSteps[s] - eachDistance) {
			lastRotationStep = allRotationSteps [e - 1];
			rotation.z += sideLength + eachDistance;
		}

		//Right Limit to Left
		if (currz > allRotationSteps[e] + eachDistance) {
			lastRotationStep = allRotationSteps [s + 1];
			rotation.z += -sideLength - eachDistance;
		}

	}

	public void Keep(float spdpertime, float dt) {
		rotation.z += (lastRotationStep - rotation.z) * dt * 2.0f;
	}
}
