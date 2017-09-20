using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using AOT;
using System.IO;

using UnityEngine;

public class ImageLoader : MonoBehaviour {



	//Picture Canvas for rendering image
	public Transform PictureRoot;
	public GameObject PictureCanvasPrefab;

	static public GameObject PictureCanvasObject;
	private Material PictureCanvasImage;

	private WWW fileReq;
	private Texture2D _tex;


	[DllImport("__Internal")]
	private extern static void RequestCameraImage ();

	//
	void Awake() {
	}

	void Start() {
//		PictureCanvasImage = PictureCanvasObject.GetComponent<MeshRenderer>().material;
//		Debug.Log (PictureCanvasImage);
	}

	void Update() {
		//For Debug
		if (Input.GetKeyDown (KeyCode.Return)) {
			SetImage (null);
		}
	}



	public void UpdateImage(string FilePath) {
		Debug.Log ("UNITY :" + FilePath);
		Debug.Log ("UpdateImage" + this);
		StartCoroutine (this.LoadImageFromPath(FilePath));
	}

	IEnumerator LoadImageFromPath(string path) {
		_tex = new Texture2D (1, 1);
		if (File.Exists (path)) {
			Debug.Log ("LoadImageFromPath" + this);
			yield return StartCoroutine (this.RequestImage (path));
			if (fileReq.bytes.Length > 0) {
				Debug.Log ("File Exist & Loading");
				fileReq.LoadImageIntoTexture (_tex);
				//				Sprite newSprite = Sprite.Create (_tex, new Rect (0f, 0f, _tex.width, _tex.height), new Vector2 (0.5f, 0.5f), 400.0f);
				SetImage (_tex);
				//				targetCanvas.sprite = newSprite;
			} else {
				Debug.Log ("fileReq.bytes == 0");
				_tex = null;
			}
		} else {
			Debug.Log ("File Doesn't Exist");
			_tex = null;
		}
	}

	IEnumerator RequestImage(string path) {

		fileReq = new WWW ("file://" + path);
		while (!fileReq.isDone) {
			Debug.Log ("RequestImage" + this);
			yield return null;
		}
		yield break;
	}

	public void SetImage(Texture2D tex) {
		PictureCanvasObject = Instantiate (this.PictureCanvasPrefab, this.PictureRoot) as GameObject;
		Debug.Log (PictureCanvasObject);
		if (tex != null) {
			PictureCanvasObject.GetComponent<MeshRenderer> ().material.mainTexture = tex;
			PictureCanvasObject.transform.localScale = new Vector3 (0.1f, 0.01f, 0.1f * tex.width / tex.height);
		}
		PictureCanvasObject.transform.position = new Vector3 (Camera.main.transform.position.x, 0.0f, Camera.main.transform.position.z);
	}

	public void RunImagePicker() {
		try {
			RequestCameraImage ();
		} catch(Exception e) {
			Debug.LogWarning (e);

		}
	}

}
