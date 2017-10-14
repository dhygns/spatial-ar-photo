using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;


using System.Runtime.InteropServices;

public class ImageLoader : MonoBehaviour {
	public GameObject targetCube;


	[DllImport("__Internal")]
	private extern static void _RequestGalleryImage();

	[DllImport("__Internal")]
	private extern static void _GetImageBytes(int idx, out IntPtr data);

	[DllImport("__Internal")]
	private extern static void _GetImageSize(int idx, out int width, out int height);


	// Use this for initialization
	void Start () {
		#if UNITY_EDITOR
		ImagesLoad ("assets-library://asset/asset.jpeg?id=582F0DEA-3355-47C0-BA32-FCBC4F0828A0&ext=jpeg");
		#elif UNITY_IPHONE
		_RequestGalleryImage ();
		#endif
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private Dictionary<string, WWW> fileReq;
	private Texture2D _tex;
//	IEnumerator RequestImage(string path) {
//		fileReq [path] = new WWW ("file://" + path);
//	}
//
//	IEnumerator LoadImageFromPath(string path) {
//		_tex = new Texture2D (1, 1);
//
//		if (File.Exists (path)) {
//			Debug.Log ("EXIST :: " + path);
//			yield return StartCoroutine (RequestImage (path));
//		}
//	}

	void ImagesLoad(string count) {
		int cnt = System.Int32.Parse (count);

		int width, height;
		_GetImageSize (cnt - 1, out width, out height);

		IntPtr unmanagedPtr;
		_GetImageBytes (cnt - 1, out unmanagedPtr);

		byte[] mangedData = new byte[width * height * 4];
		Marshal.Copy(unmanagedPtr, mangedData, 0, width * height * 4);
		//don't forget to free the unmanaged memory
		Marshal.FreeHGlobal(unmanagedPtr);

		Debug.Log ("B" + mangedData.GetLength(0));
		Debug.Log ("B" + mangedData.Length);
		Debug.Log ("B" + mangedData.LongLength);
		Debug.Log ("B" + mangedData.ToString());

//		targetCube.GetComponent<Renderer> ().material.mainTexture = file.textureNonReadable;
	}
}
