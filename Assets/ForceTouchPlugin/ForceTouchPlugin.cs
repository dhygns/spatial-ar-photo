using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Globalization;
using System;
using System.IO;

/// <summary>
/// 1. IncompatibleOS for all platforms except iOS and everything bellow iOS 9.0
/// 2. Unknown - is returned if you request TouchForceState too early (Awake of first scene is bad idea)
/// 3. Unavailable - for all devices that do not support it (at the moment of writting everything except iPhone 6s/6s+ and iPad Pro) and for devices that have Force Touch disabled in accessibility settings.
/// 4. Available - for devices that have hardware to support Force Touch and have it enabled in Accessibility Settings.
/// </summary>
public enum ForceTouchState
{
	Available,
	Unavailable,
	Unknown,
	IncompatibleOS
}
	
/// <summary>
/// Class used to store touch object including force and radius properites.
/// </summary>
public class NativeTouch
{
	public int id;
	public Vector2 pos;
	public Vector2 delta;
	public TouchPhase phase;
	public float force;
	public float maxforce;
	public float radius;
	public float radiusTolerance;

	public NativeTouch()
	{
		id = -1;
		pos = Vector2.zero;
		delta = Vector2.zero;

		force = 1f;
		maxforce = -1f;
		radius = -1f;
		radiusTolerance = 0;
	}

	public override string ToString()
	{
		return string.Format ("id: {0}, pos: {1}, force: {2:F2}, maxForce: {3:F2}, radius: {4:F2}, radiusTolerance: {5:F2}", id, pos, force, maxforce, radius, radiusTolerance);
	}
}

/// <summary>
/// Class used to store only force and radius properties of the touch.
/// </summary>
public class NativeTouchExtraData
{
	public int id;
	public float force;
	public float maxforce;
	public float radius;
	public float radiusTolerance;

	public NativeTouchExtraData()
	{
		id = -1;

		force = 1f;
		maxforce = -1f;
		radius = -1f;
		radiusTolerance = 0;
	}

	public override string ToString()
	{
		return string.Format ("id: {0}, force: {1:F2}, maxForce: {2:F2}, radius: {3:F2}, radiusTolerance: {4:F2}", id, force, maxforce, radius, radiusTolerance);
	}
}

/// <summary>
/// Class holding static methods interacting with native side of plugin code.
/// </summary>
public class ForceTouchPlugin
{
	#if UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern int getForceTouchState();

	[DllImport ("__Internal")]
	private static extern float getScaleFactor(float unityScreenSize);

	[DllImport ("__Internal")]
	private static extern int getNativeTouches(out IntPtr dataPtr, float unityScreenSize, int touchCount);

	[DllImport ("__Internal")]
	private static extern int getForceData(out IntPtr dataPtr, int touchCount);

	[DllImport ("__Internal")]
	private static extern void setCallbackMethod (string gameObject, string methodName);

	[DllImport ("__Internal")]
	private static extern void removeCallbackMethod ();

	[DllImport ("__Internal")]
	private static extern bool supportsTouchRadius ();
	#endif


	private static float prevTime = 0;
	private static List<NativeTouch> touches;
	private static Dictionary<int, NativeTouchExtraData> touchesExtraData;


	private const int TOUCH_BINARY_SMALL_SIZE = 20;
	private const int TOUCH_BINARY_SIZE = 36;


	/// <summary>
	/// Retrieves only force/radius data from native side of the code 
	/// </summary>
	/// <returns>The extra data for the Touch object.</returns>
	/// <param name="fingerId">fingerId of a Touch object.</param>          
	public static NativeTouchExtraData GetTouchExtraData(int fingerId)
	{
		#if UNITY_EDITOR
			return new NativeTouchExtraData();
		#elif UNITY_IOS
			//only one call per frame. 
			if (Time.realtimeSinceStartup - prevTime > (Time.deltaTime/2))
			{
				GetForceData();
				prevTime = Time.realtimeSinceStartup;
			}

			//return extra data
			if (touchesExtraData.ContainsKey (fingerId)) 
			{
				return touchesExtraData[fingerId];
			}
			else
			{
				Debug.Log("Point missing: " + fingerId);
				return new NativeTouchExtraData();
			}
		#else
			return new NativeTouchExtraData();
		#endif
	}

	/// <summary>
	/// DO NOT CALL THIS EVER. Internal use only. Retrieves only force/radius data from native side of the code. 
	/// </summary>
	/// <returns>The force/radius data for all current touches.</returns>
	public static Dictionary<int, NativeTouchExtraData> GetForceData()
	{
		if (touchesExtraData == null)
			touchesExtraData = new Dictionary<int, NativeTouchExtraData>();
		//in this method we don't clear existing touches, because we need to properly handle ended touches which no longer exist in ojb-c



		#if UNITY_EDITOR
			//do nothing
		#elif UNITY_IOS
			//Create a memory pointer
			IntPtr unmanagedPtr;

			//Receive binary data from native code. Native code also allocates memory for this data.
			int size = getForceData(out unmanagedPtr, Input.touchCount);

			//if touches are available
			if (size > 0)
			{
				//copy binary data from unamanged memory to managed memory
				byte[] managedData = new byte[size];
				Marshal.Copy(unmanagedPtr, managedData, 0, size);

				int touchCount = (int)size/TOUCH_BINARY_SMALL_SIZE; //each touch is serialized into 20 bytes of binary data
				for (var i = 0; i < touchCount; i++)
				{
					int id = BitConverter.ToInt32 (managedData, i * TOUCH_BINARY_SMALL_SIZE);
					if (touchesExtraData.ContainsKey (id)) //update touch
					{
						touchesExtraData [id].force = BitConverter.ToSingle (managedData, i * TOUCH_BINARY_SMALL_SIZE + 4);
						touchesExtraData [id].maxforce = BitConverter.ToSingle (managedData, i * TOUCH_BINARY_SMALL_SIZE + 8);
						touchesExtraData [id].radius = BitConverter.ToSingle (managedData, i * TOUCH_BINARY_SMALL_SIZE + 12);
						touchesExtraData [id].radiusTolerance = BitConverter.ToSingle (managedData, i * TOUCH_BINARY_SMALL_SIZE + 16);
					} 
					else //add touch
					{
						touchesExtraData.Add (id, new NativeTouchExtraData () 
						{
							id = BitConverter.ToInt32 (managedData, i * TOUCH_BINARY_SMALL_SIZE),
							force = BitConverter.ToSingle (managedData, i * TOUCH_BINARY_SMALL_SIZE + 4),
							maxforce = BitConverter.ToSingle (managedData, i * TOUCH_BINARY_SMALL_SIZE + 8),
							radius = BitConverter.ToSingle (managedData, i * TOUCH_BINARY_SMALL_SIZE + 12),
							radiusTolerance = BitConverter.ToSingle (managedData, i * TOUCH_BINARY_SMALL_SIZE + 16)
						});
					}
				}

				//release unamanaged memory
				Marshal.FreeHGlobal(unmanagedPtr);
			}
		#else
			//do nothing
		#endif

		return touchesExtraData;
	}


	/// <summary>
	/// Retrieves touch data from native code.
	/// </summary>
	/// <returns>The force/radius data for all current touches. This data also includes position and deltaPosition properties</returns>
	[System.Obsolete("User Touch class extension methods instead (Touch.GetForce(), Touch.GetMaxForce(), Touch.GetRadius(), Touch.GetRadiusTolerance()")]
	public static List<NativeTouch> GetNativeTouches()
	{
		if (touches == null)
			touches = new List<NativeTouch>();
		else
			touches.Clear ();

		#if UNITY_EDITOR
			//do nothing
		#elif UNITY_IOS
			//Create a memory pointer
			IntPtr unmanagedPtr;

			//Receive binary data from native code. Native code also allocates memory for this data.
			int size = getNativeTouches(out unmanagedPtr, Mathf.Max(Screen.width, Screen.height), Input.touchCount);

			//if touches are available
			if (size > 0)
			{
				//copy binary data from unamanged memory to managed memory
				byte[] managedData = new byte[size];
				Marshal.Copy(unmanagedPtr, managedData, 0, size);

				int touchCount = (int)size/TOUCH_BINARY_SIZE; //each touch is serialized into 36 bytes of binary data
				for (var i = 0; i < touchCount; i++)
				{

					var temp = new NativeTouch()
					{
						id = BitConverter.ToInt32(managedData, i * TOUCH_BINARY_SIZE),
						pos = new Vector2(BitConverter.ToSingle(managedData, i * TOUCH_BINARY_SIZE + 4), BitConverter.ToSingle(managedData, i * TOUCH_BINARY_SIZE + 8)),
						delta = new Vector2(BitConverter.ToSingle(managedData, i * TOUCH_BINARY_SIZE + 12), BitConverter.ToSingle(managedData, i * TOUCH_BINARY_SIZE + 16)),
						force = BitConverter.ToSingle(managedData, i * TOUCH_BINARY_SIZE + 20),
						maxforce = BitConverter.ToSingle(managedData, i * TOUCH_BINARY_SIZE + 24),
						radius = BitConverter.ToSingle(managedData, i * TOUCH_BINARY_SIZE + 28),
						radiusTolerance = BitConverter.ToSingle(managedData, i * TOUCH_BINARY_SIZE + 32)
					};

					touches.Add(temp);
				}

				//release unamanaged memory
				Marshal.FreeHGlobal(unmanagedPtr);
			}
		#else
			//do nothing
		#endif
		return touches;
	}

	/// <summary>
	/// Retrieve iOS device scale factor. Though plugin returns
	/// touch data in full screen resolution, iOS operates using low DPI space
	/// and using scale factor you can transform touch data back to 
	/// iOS space. For non retina devices this will be 1; for retina devices
	/// this will be 2; There's an exception for iPhone 6 Plus and iPhone 6s Plus,
	/// where native code sees screen size as 2208x1242, input area resolution
	/// is 736x413, but Unity sees it as 1920x1080 (this is hardware resolution
	/// of screen panel used in these devices). Therefore, even though the getScaleFactor
	/// for iPhone 6+/6s+ should be 3 (and it is 3 for iOS apps), for applications
	/// using Metal and Direct3d it is 2.608.
	/// </summary>
	/// <returns>Screen scale factor.</returns>
	public static float GetScaleFactor()
	{
		#if UNITY_EDITOR
			return 1;
		#elif UNITY_IOS
			return getScaleFactor(Mathf.Max(Screen.width, Screen.height));
		#else
			return 1;
		#endif
	}


	/// <summary>
	/// Tests device capabilites for ForceTouch. See overload method for more info.
	/// https://developer.apple.com/library/prerelease/ios/documentation/UIKit/Reference/UITouch_Class/#//apple_ref/c/tdef/UIForceTouchCapability
	/// </summary>
	/// <returns>Force touch capablity.</returns>
	public static ForceTouchState GetForceTouchState()
	{
		#if UNITY_EDITOR
			return ForceTouchState.IncompatibleOS;
		#elif UNITY_IOS
			return GetForceTouchState(getForceTouchState());
		#else
			return ForceTouchState.IncompatibleOS;
		#endif
	}

	/// <summary>
	/// Returns current ForceTouchState. Possible values:
	/// 1. IncompatibleOS for all platforms except iOS and everything bellow iOS 9.0
	/// 2. Unknown - is returned if you request TouchForceState too early (Awake of first scene is bad idea)
	/// 3. Unavailable - for all devices that do not support it (at the moment of writting everything except iPhone 6s/6s+ and iPad Pro) and for devices that have Force Touch disabled in accessibility settings.
	/// 4. Available - for devices that have hardware to support Force Touch and have it enabled in Accessibility Settin
	/// </summary>
	/// <returns>Force touch capablity.</returns>
	/// <param name="state">Int representation of force touch capability returned by native side of the plugin.</param>
	public static ForceTouchState GetForceTouchState(int state)
	{
		#if UNITY_EDITOR
			return ForceTouchState.IncompatibleOS;
		#elif UNITY_IOS
			switch(state)
			{
			case 1:
				return ForceTouchState.Available;
			case 2:
				return ForceTouchState.Unavailable;
			case 3:
				return ForceTouchState.Unknown;
			case 4:
				return ForceTouchState.IncompatibleOS;
			default:
				return ForceTouchState.IncompatibleOS;
			}
		#else
			return ForceTouchState.IncompatibleOS;
		#endif
	}

	/// <summary>
	/// Register callback method to be triggered when user changes ForceTouch 
	/// settings in Accessibility settings. GameObject has to be in hierarchy and method has to have signature:
	/// public void MethodName(string message). See InputManager.cs for example
	/// Only last method/GO subscribed will be called.
	/// </summary>
	/// <param name="GameObjectName">Name of the GameObject within hierarchy.</param>
	/// <param name="MethodName">Name of the method within that GameObject.</param>
	public static void SetCallbackMethod(string GameObjectName, string MethodName)
	{
		#if UNITY_EDITOR
		// 
		#elif UNITY_IOS
			setCallbackMethod(GameObjectName, MethodName);
		#else
		//
		#endif
	}

	/// <summary>
	/// Remove callback for ForceTouch state change event
	/// </summary>
	public static void RemoveCallbackMethod()
	{
		#if UNITY_EDITOR
		// 
		#elif UNITY_IOS
			removeCallbackMethod();
		#else
		//
		#endif
	}



	/// <summary>
	/// Returns whether current device supports touch radius.
	/// </summary>
	/// <returns><c>true</c>, for iOS 8 and above, <c>false</c> otherwise.</returns>
	public static bool SupportsTouchRadius()
	{
		#if UNITY_EDITOR
			return false;
		#elif UNITY_IOS
			return supportsTouchRadius();
		#else
			return false;
		#endif
	}


	/// <summary>
	/// Shortcut method for checking ForceTouch state. Returns whether current device supports force touch.
	/// </summary>
	/// <returns><c>true</c>, for iPhone 6s and iPhone 6s+ and iPad Pro, <c>false</c> otherwise.</returns>
	public static bool SupportsForceTouch()
	{
		#if UNITY_EDITOR
			return false;
		#elif UNITY_IOS
			return getForceTouchState() == 1;
		#else
			return false;
		#endif
	}	

}