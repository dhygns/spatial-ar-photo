using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageUITouch : MonoBehaviour {
	public Camera UICamera;

	//UI Touch Status
	public enum TouchStatus { swipe, pick, idle } 


	//Signleton Instance
	public static ImageUITouch _instance;
	void Awake() { 
		if (_instance == null)
			_instance = this;
	}

	//Functions Delegate for Tick 
	delegate void UpdateTickList(float dt);
	UpdateTickList updateTickList;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		//Update Main Tick Logic Function
		updateTick(Time.deltaTime);
		updateStatus (Time.deltaTime);



		//Update Delgate Functions
		if(updateTickList != null) updateTickList(Time.deltaTime);

	}

	//Touch Management

	//Methods for Pixel Position 
	private Vector2 pixelTouchingPosition;	//move position
	private Vector2 pixelTouchDownPosition;	//down position
	private Vector2 pixelTouchUpPosition;	//up position

	//Methods for Normal Position
	private Vector2 normalTouchingPosition;	//move position
	private Vector2 normalTouchDownPosition;//down position
	private Vector2 normalTouchUpPosition;	//up position

	//status for touch
	private TouchStatus touchStatus;

	//Touch Ray
	private Ray touchRay;
	private RaycastHit touchHit;


	//FUNCTIONS ABOUT TICK 

	//get picked object
	void _pickObject(float dt) {
		touchRay = UICamera.ScreenPointToRay (this.pixelTouchingPosition);
		Physics.Raycast (touchRay, out touchHit);

		if (touchHit.collider == null) {
			touchRay = Camera.main.ScreenPointToRay (this.pixelTouchingPosition);
			Physics.Raycast (touchRay, out touchHit);
		}
	}

	//set position for Touch
	void updateTick(float dt) {
		//will seperate down, move, up
		if(Input.GetMouseButtonDown(0)) {
			updateTickList = _updateDownTick;
		} else if(Input.GetMouseButton(0)) {
			updateTickList = _updateMoveTick;
		} else if(Input.GetMouseButtonUp(0)) {
			updateTickList = _updateUpTick;
		} else {
			updateTickList = null;
		}
	}

	//set status for TouchStatus (swipe, pick, idle)
	void updateStatus(float dt) {
		//step 0. check touch status idle
		if (touchStatus == TouchStatus.idle) {

			//step 1. check normal distance 
			if (_normalDistance > 0.05) {
				//step 2. compare x with y
				if (Mathf.Abs(_elapsedNormalPosition.x) > Mathf.Abs(_elapsedNormalPosition.y)) {
					touchStatus = TouchStatus.swipe;
				} else {
					touchStatus = TouchStatus.pick;
				}
			} else {
				touchStatus = TouchStatus.idle;
			}
		}

		//step 3. release
		if (updateTickList == null) {
			touchStatus = TouchStatus.idle;
		}
	}



	//touch or mouse position To Tick Position
	void _ToTick(ref Vector2 pixel, ref Vector2 normal, Vector2 pos) {
		pixel.x = pos.x;
		pixel.y = pos.y;
		normal.x = pos.x / Screen.width;
		normal.y = pos.y / Screen.height;
	}
	void _updateDownTick(float dt) {
		#if UNITY_EDITOR
		_ToTick(ref pixelTouchDownPosition, ref normalTouchDownPosition, Input.mousePosition);
		#elif UNITY_IPHONE 
		_ToTick(ref pixelTouchDownPosition, ref normalTouchDownPosition, Input.GetTouch(0).position);
		#endif
		_updateMoveTick (dt);
		_pickObject (dt);
	}
	void _updateUpTick(float dt) {
		#if UNITY_EDITOR
		_ToTick(ref pixelTouchUpPosition, ref normalTouchUpPosition, Input.mousePosition);
		#elif UNITY_IPHONE 
		_ToTick(ref pixelTouchUpPosition, ref normalTouchUpPosition, Input.GetTouch(0).position);
		#endif
		_instance.touchHit = new RaycastHit ();
	}
	void _updateMoveTick(float dt) {
		#if UNITY_EDITOR
		_ToTick(ref pixelTouchingPosition, ref normalTouchingPosition, Input.mousePosition);
		#elif UNITY_IPHONE
		_ToTick(ref pixelTouchingPosition, ref normalTouchingPosition, Input.GetTouch(0).position);
		#endif
	}

	//Interface For Local Script

	//get Distance from down Position
	private float _pixelDistance {
		get { 
			if (updateTickList == _updateMoveTick)
				return Mathf.Abs(Vector2.Distance (pixelTouchDownPosition, pixelTouchingPosition));
			else
				return -1.0f;
		}
	}
	private float _normalDistance {
		get { 
			if (updateTickList == _updateMoveTick)
				return Mathf.Abs(Vector2.Distance (normalTouchDownPosition, normalTouchingPosition));
			else
				return -1.0f;
		}
	}

	//get Elapsed Position from down Position
	private Vector2 _elapsedPixelPosition {
		get {
			return pixelTouchingPosition - pixelTouchDownPosition;
		}
	}
	private Vector2 _elapsedNormalPosition {
		get {
			return normalTouchingPosition - normalTouchDownPosition;
		}
	}
		

	//Interface For Singleton

	//get Distance from down Position
	static public float PixelDistance {
		get { return _instance._pixelDistance; }
	}
	static public float NormalDistance {
		get { return _instance._normalDistance; }
	}

	//get Elapsed Position from down Position
	static public Vector2 ElapsedPixelPosition {
		get { return _instance._elapsedPixelPosition; }
	}
	static public Vector2 ElaspedNormalPosition {
		get { return _instance._elapsedNormalPosition; }
	}

	//get Status
	static public TouchStatus Status{
		get { return _instance.touchStatus; }
	}

	//is Preased?
	static public bool isPreased {
		get {
			if (_instance.updateTickList == _instance._updateMoveTick ||
			    _instance.updateTickList == _instance._updateDownTick)
				return true;
			else
				return false;
		}
	}

	//get Touching Position
	static public Vector2 PixelPosition {
		get { 
			if (_instance.updateTickList == _instance._updateMoveTick ||
			    _instance.updateTickList == _instance._updateDownTick) {
				return _instance.pixelTouchingPosition; 
			} else {
				return _instance.pixelTouchUpPosition;
			}
		}
	}

	static public Vector2 NormalPosition {
		get { 
			if (_instance.updateTickList == _instance._updateMoveTick ||
				_instance.updateTickList == _instance._updateDownTick) {
				return _instance.normalTouchingPosition; 
			} else {
				return _instance.pixelTouchUpPosition;
			}
		}
	}

	static public Vector2 PixelDownPosition {
		get {
			if (_instance.updateTickList == _instance._updateMoveTick ||
				_instance.updateTickList == _instance._updateDownTick) {
				return _instance.pixelTouchDownPosition; 
			} else {
				return Vector2.zero;
			}
		}
	}

	static public Vector2 NormalDownPosition {
		get {
			if (_instance.updateTickList == _instance._updateMoveTick ||
				_instance.updateTickList == _instance._updateDownTick) {
				return _instance.normalTouchDownPosition; 
			} else {
				return Vector2.zero;
			}
		}
	}		

	//get Touched GameObject
	static public GameObject HitObject {
		get { 
			if (_instance.touchHit.collider != null) {
				return _instance.touchHit.collider.gameObject; 
			} else {
				return null;
			}
		}
	}



	static public float Force {
		get { 
			if (Input.touchCount != 0)
				return Input.GetTouch (0).GetForce ();
			else
				return 0.0f;
		}
	}
}
