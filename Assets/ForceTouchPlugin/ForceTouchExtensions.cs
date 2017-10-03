using UnityEngine;
using System.Collections;

/// <summary>
/// Extension methods for Unity's built in Touch Class.
/// </summary>
public static class ForceTouchExtensions 
{
	/// <summary>
	/// Extension method for Touch class. Used to retrieve force and radius data.
	/// </summary>
	/// <returns>NativeTouchExtraData object for this touch containing force and radius data</returns>
	/// <b>Example</b> 
	/// <code>
	/// Input.touches[0].GetExtraData();
	/// </code>
	public static NativeTouchExtraData GetExtraData(this Touch touch)
	{
		return ForceTouchPlugin.GetTouchExtraData(touch.fingerId);
	}

	/// <summary>
	/// Extension method for Touch class. Used to retrieve force of this touch.
	/// </summary>
	/// <returns>Touch force.</returns>
	/// <b>Example</b> 
	/// <code>
	/// Input.touches[0].GetForce();
	/// </code>
	public static float GetForce(this Touch touch)
	{
		return ForceTouchPlugin.GetTouchExtraData(touch.fingerId).force;
	}

	/// <summary>
	/// Extension method for Touch class. Used to retrieve maximum possible force of this touch. MaxForce might be different for different devices.
	/// </summary>
	/// <returns>Maximum possible force of this touch.</returns>
	/// <b>Example</b> 
	/// <code>
	/// Input.touches[0].GetMaxForce();
	/// </code>
	public static float GetMaxForce(this Touch touch)
	{
		return ForceTouchPlugin.GetTouchExtraData(touch.fingerId).maxforce;
	}

	/// <summary>
	/// Extension method for Touch class. Used to retrieve radius of this touch.
	/// </summary>
	/// <returns>Touch radius.</returns>
	/// <b>Example</b> 
	/// <code>
	/// Input.touches[0].GetRadius();
	/// </code>
	public static float GetRadius(this Touch touch)
	{
		return ForceTouchPlugin.GetTouchExtraData(touch.fingerId).radius;
	}

	/// <summary>
	/// Extension method for Touch class. Used to retrieve radius tolerance of this touch. Radius tolerance is differs among devices.
	/// </summary>
	/// <returns>Touch radius tolerance.</returns>
	/// <b>Example</b> 
	/// <code>
	/// Input.touches[0].GetRadiusTolerance();
	/// </code>
	public static float GetRadiusTolerance(this Touch touch)
	{
		return ForceTouchPlugin.GetTouchExtraData(touch.fingerId).radiusTolerance;
	}
}
