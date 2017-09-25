using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageObjectController : MonoBehaviour {
	//Transform for world
	private Transform rootTransform;

	//Action List Manager Function
	public delegate void ActionList ();
	ActionList actionList;

	//common information
	private Vector3 position;
	private Vector3 rotation;
	private Vector3 scale;

	//information For Action
	private Vector3 targetPosition;
	private Vector3 targetRotation;
	private Vector3 targetScale; 


	//init action datas
	void Awake() {
		this.rootTransform = GameObject.FindGameObjectWithTag ("PictureRoot").transform;


		this.position.x = 0.0f;
		this.position.y = -0.3f;
		this.position.z = 0.4f;

		this.targetPosition.x = 0.0f;
		this.targetPosition.y = -0.1f;
		this.targetPosition.z = 0.3f;

		this.scale.x = this.targetScale.x = 0.06f;
		this.scale.y = this.targetScale.y = 0.06f;
		this.scale.z = this.targetScale.z = 0.001f;

		this.rotation.x = this.targetRotation.x = 0.0f;
		this.rotation.y = this.targetRotation.y = 0.0f;
		this.rotation.z = this.targetRotation.z = 180.0f;

		this.actionList = this.ReadyAction;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		//action update
		if (this.actionList != null)
			this.actionList ();
		
		//default update
		this.position.x += (this.targetPosition.x - this.position.x) * Time.deltaTime * 1.0f;
		this.position.y += (this.targetPosition.y - this.position.y) * Time.deltaTime * 1.0f;
		this.position.z += (this.targetPosition.z - this.position.z) * Time.deltaTime * 1.0f;

		this.scale.x += (this.targetScale.x - this.scale.x) * Time.deltaTime * 10.0f;
		this.scale.y += (this.targetScale.y - this.scale.y) * Time.deltaTime * 10.0f;
		this.scale.z += (this.targetScale.z - this.scale.z) * Time.deltaTime * 10.0f;

		this.rotation.x += (this.targetRotation.x - this.rotation.x) * Time.deltaTime * 10.0f;
		this.rotation.y += (this.targetRotation.y - this.rotation.y) * Time.deltaTime * 10.0f;
		this.rotation.z += (this.targetRotation.z - this.rotation.z) * Time.deltaTime * 10.0f;

		this.transform.localPosition = this.position;
		this.transform.localScale = this.scale;
		this.transform.localEulerAngles = this.rotation;
	}

	//Start Trigger
	public void DoAction(Vector3 targetpos) {
		Vector3 targetscl = new Vector3 (0.06f, 0.001f, 0.06f);
		//Setup information for Action
		this.transform.parent = this.rootTransform;

		this.targetPosition = targetpos;
		this.targetScale = targetscl;

		//init action position to camera position
		this.position.x = this.transform.localPosition.x;
		this.position.y = this.transform.localPosition.y;
		this.position.z = this.transform.localPosition.z;

		this.scale.x = this.transform.localScale.x;
		this.scale.y = this.transform.localScale.y;
		this.scale.z = this.transform.localScale.z;

		this.rotation.x = this.transform.localEulerAngles.x;
		this.rotation.y = this.transform.localEulerAngles.y;
		this.rotation.z = this.transform.localEulerAngles.z;

		//Action Logic Setup
		this.actionList = this.UpdateAction;
	}

	//before it throw away to world
	void ReadyAction() {
		
	}

	//Update Trigger
	void UpdateAction() {

		float dist = (this.position - this.targetPosition).magnitude;

		//rotating action
		this.targetRotation.y += dist * Time.deltaTime * 500.0f;
//		this.transform.Rotate (Vector3.up * dist * Time.deltaTime * 500.0f);
	}

}
