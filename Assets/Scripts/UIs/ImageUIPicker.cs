using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageUIPicker : MonoBehaviour {

	static public ImageUIPicker _instance;
	void Awake() {
		if(_instance == null) _instance = this;
	}



	//Functions Delegate for UI
	delegate void UpdateUIList(float dt);
	UpdateUIList updateUIList;

	// Use this for initialization
	void Start () {
		updateUIList = null;
	}
	
	// Update is called once per frame
	void Update () {	
		updateUI (Time.deltaTime);
		if(updateUIList != null) updateUIList(Time.deltaTime);
	}


	void updateUI(float dt) {
		Debug.Log (ImageUITouch.Status);
	}

	void updateUISwipe(float dt) {
	}

	void updateUIPicked(float dt) {
	}



	//logic to do default
	public GameObject imagePrefab;	//Image Prefab
	public GameObject imageSlot;	//Image Swipe Controller

	private GameObject[] imageObject;

	void setup(Texture2D[] images) {
		//init Objects via image's count
		imageObject = new GameObject[images.Length];

		//Create each Objects about textures
		for (int idx = 0; idx < imageObject.Length; idx++) {
			//step 1. create image object
			imageObject[idx] = Instantiate (imagePrefab, Camera.main.transform);

			//step 2. set images

			//step 3. enable image canvas logic

		}
	}


	//inerfaces for using

	public static void Setup(Texture2D[] images) {
		_instance.setup(images);
	}

	
}
