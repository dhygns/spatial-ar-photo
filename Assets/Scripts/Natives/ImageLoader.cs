using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;


using System.Runtime.InteropServices;

public class ImageLoader : MonoBehaviour {

	//it would be private image list
	public Dictionary<int, Texture2D> imageDics = new Dictionary<int, Texture2D>();

	static public ImageLoader _instance = null;

	void Awake() {
		if(_instance == null) _instance = this;
	}


	[DllImport("__Internal")]
	private extern static void _RequestGalleryImage();

	[DllImport("__Internal")]
	private extern static void _GetImageBytes(int idx, out IntPtr data);

	[DllImport("__Internal")]
	private extern static void _GetImageSize(int idx, out int width, out int height);


	// Use this for initialization
	void Start () {
		#if UNITY_EDITOR

		imageDics[0] = Resources.Load ("images/dummy1") as Texture2D;
		imageDics[1] = Resources.Load ("images/dummy2") as Texture2D;
		imageDics[2] = Resources.Load ("images/dummy3") as Texture2D;
		imageDics[3] = Resources.Load ("images/dummy4") as Texture2D;
		imageDics[4] = Resources.Load ("images/dummy5") as Texture2D;
		imageDics[5] = Resources.Load ("images/dummy1") as Texture2D;
		imageDics[6] = Resources.Load ("images/dummy2") as Texture2D;
		imageDics[7] = Resources.Load ("images/dummy3") as Texture2D;
		imageDics[8] = Resources.Load ("images/dummy4") as Texture2D;
		imageDics[9] = Resources.Load ("images/dummy5") as Texture2D;
		ImagesLoad ("10");

		#elif UNITY_IPHONE
		_RequestGalleryImage ();
		#endif
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private int totalImageCount = 0;

	//Get Datas from Native
	void ImagesLoad(string count) {
		totalImageCount = System.Int32.Parse (count);
		ImageUIObjectsManager.Setup (totalImageCount);
	}

	public Texture2D getTexture(int idx) {
		#if UNITY_EDITOR
		return imageDics[idx];
		#elif UNITY_IPHONE 
		if (totalImageCount == 0)
		return null;

		idx = idx % totalImageCount;

		int width, height;
		_GetImageSize (idx, out width, out height);

		IntPtr unmanagedPtr;
		_GetImageBytes (idx, out unmanagedPtr);

		Texture2D tex = new Texture2D (width, height, TextureFormat.RGBA32, false);
		tex.LoadRawTextureData(unmanagedPtr, width * height * 4);
		tex.Apply ();

		return tex;
		#endif
	}

	//Interface 
	static public Texture2D GetTexture(int idx) {
		return _instance.getTexture (idx);
	}
}
