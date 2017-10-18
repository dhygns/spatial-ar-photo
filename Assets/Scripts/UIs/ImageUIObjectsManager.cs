using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageUIObjectsManager : MonoBehaviour {

	static private ImageUIObjectsManager instance = null;
	void Awake() {
		if (instance == null)
			instance = this;
	}

	public GameObject ImageUIObjectPrefab;
	private List<GameObject> imageObjectList = new List<GameObject>();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void _createObject(int id) {
		//Image Load At Wrapper
		GameObject imageUIObject = Instantiate (ImageUIObjectPrefab, null) as GameObject;
		ImageUIObject imageUIObjectScript = imageUIObject.GetComponent<ImageUIObject> ();
		imageUIObjectScript.Init (id);
		if (id < 7) {
			imageUIObjectScript.Create (ImageUISlot.GetWrapper (id));
		} else {
			imageObjectList.Add (imageUIObject);
		}
	}

	int RightID {
		get { return imageObjectList.Count - 1; }
	}

	int LeftID {
		get { return 0; }
	}


	void setup(int cnt) {
		//Image 6 already loaded via scene
		for (int i = 0; i < cnt; i++) {
			_createObject (i);
		}
	}

	GameObject getObject(int id, Transform parent) {
		if (imageObjectList.Count > id) {
			GameObject imageObject = imageObjectList.ToArray () [id];
			imageObjectList.Remove (imageObject);

			imageObject.GetComponent<ImageUIObject> ().Create (parent);
			return imageObject;
		} else {
			return null;
		}
	}

	void setObject(GameObject imageObject, int id) {
		if (imageObject == null)
			return;
		imageObject.GetComponent<ImageUIObject> ().Remove ();
		imageObjectList.Insert (id, imageObject);
	}

	//interface for singleton
	static public void Setup(int cnt) {
		instance.setup (cnt);
	}

	static public GameObject GetRight(Transform parent) {
		return instance.getObject (instance.RightID, parent);
	}

	static public GameObject GetLeft(Transform parent) {
		return instance.getObject (instance.LeftID, parent);
	}

	static public void SetRight(GameObject imageObject) {
		instance.setObject (imageObject, instance.RightID + 1);
	}

	static public void SetLeft(GameObject imageObject) {
		instance.setObject (imageObject, instance.LeftID);
	}
}
