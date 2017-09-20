using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageObjectController : MonoBehaviour {
	

	//Action List Manager Function
	public delegate void ActionList ();
	ActionList actionList;

	//information For Action
	private Vector3 actionPosition;
	private Vector3 actionPositionTarget;


	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		//action update
		if (this.actionList != null)
			this.actionList ();
//		else
//			Debug.Log ("action list empty");

		//default update
		this.transform.position = this.actionPosition;
	}



	//Start Trigger
	public void DoAction(Vector3 target) {
		this.actionPositionTarget = target;
		//init action position to camera position
		this.actionPosition.Set (
			this.transform.position.x,
			target.y,
			this.transform.position.z);
		
		Debug.Log (this.actionPosition);
		Debug.Log (this.actionPositionTarget);

		this.actionList = this.UpdateAction;
	}

	//Update Trigger
	void UpdateAction() {
		this.actionPosition.x += (this.actionPositionTarget.x - this.actionPosition.x) * Time.deltaTime * 1.0f;
		this.actionPosition.y += (this.actionPositionTarget.y - this.actionPosition.y) * Time.deltaTime * 1.0f;
		this.actionPosition.z += (this.actionPositionTarget.z - this.actionPosition.z) * Time.deltaTime * 1.0f;

		float dist = (this.actionPosition - this.actionPositionTarget).magnitude;
		this.transform.Rotate (Vector3.up * dist * Time.deltaTime * 500.0f);
//		if (dist < 0.001f)
//			this.actionList = null;
	}

}
