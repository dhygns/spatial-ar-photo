using UnityEngine;
using System.Collections;

public class FrameRateManager : MonoBehaviour {

	void Awake()
	{
		//I hate the fact that iOS default framerate
		//is set to 30 :(.
		Application.targetFrameRate = 60;
	}
}
