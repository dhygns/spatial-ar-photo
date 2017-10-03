using UnityEngine;
using System.Collections;
using System.Linq;
using System.Text;

public class StatusUI : MonoBehaviour {

	public string callbackMessage = "No message received";
	public bool callbackAdded = false;
	public int currentInput = -1;
	
	void OnGUI() {
		var nativeTouchScale = InputManager.instance.nativeTouchScale;
		StringBuilder output = new StringBuilder ();
		
		switch (currentInput) {
			case 1:
			output.AppendLine( "<b>Input:</b> Unity Touch");
			break;
			case 2:
			output.AppendLine("<b>Input:</b> Mouse");
			break;
			default:
			output.AppendLine("<b>Input:</b> Undefined (Mouse / Unity Touch)");
			break;
		}

		output.AppendLine ("<b>Supports Touch Radius:</b> "+ InputManager.instance.supportsTouchRadius);

		switch (InputManager.instance.forceTouchState) 
		{
			case ForceTouchState.Available:
			output.AppendLine("<b>Force Touch State:</b> Available");
				break;
			case ForceTouchState.Unknown:
			output.AppendLine("<b>Force Touch State:</b> Unknown");
				break;
			case ForceTouchState.IncompatibleOS:
			output.AppendLine("<b>Force Touch State:</b> Incompatible OS");
				break;
			case ForceTouchState.Unavailable:
			output.AppendLine("<b>Force Touch State:</b> Unavailable");
				break;

		}

		output.AppendLine ("<b>Callback Message:</b> " + callbackMessage);


		if (InputManager.instance.touches.Count > 0) {
			output.AppendLine ("<b>Touches:</b>");
			for (int i = 0; i < InputManager.instance.touches.Count; i++) 
			{
				output.AppendLine (InputManager.instance.touches [i].ToString ());
			}
		}
		else 
		{
			output.AppendLine ("<b>Touches:</b> No touches available");
		}



		GUI.skin.label.alignment = TextAnchor.UpperLeft;
		GUI.color = Color.black;
		GUI.Label(new Rect (10 * nativeTouchScale, 10 * nativeTouchScale, Screen.width - 20 * nativeTouchScale, Screen.height - 20 * nativeTouchScale), "<size=" + 16 * nativeTouchScale +">" + output.ToString() + "</size>");




		GUI.color = Color.white;

		if (GUI.Button (new Rect (10 * nativeTouchScale, Screen.height - ((10 + 40) * nativeTouchScale), Screen.width - 20 * nativeTouchScale, 40 * nativeTouchScale), "<size=" + 16 * nativeTouchScale +">Request FT State</size>")) 
		{
			InputManager.instance.forceTouchState = ForceTouchPlugin.GetForceTouchState (); 
		}


		if (callbackAdded) {
			if (GUI.Button (new Rect (10 * nativeTouchScale, Screen.height - (100 * nativeTouchScale), Screen.width - 20 * nativeTouchScale, 40 * nativeTouchScale), "<size=" + 16 * nativeTouchScale +">Remove FT State Change Callback</size>")) 
			{
				callbackMessage = "No message received";
				InputManager.instance.RemoveCallback();
				callbackAdded = false;
			}

		} else {
			if (GUI.Button (new Rect (10 * nativeTouchScale, Screen.height - (100 * nativeTouchScale), Screen.width - 20 * nativeTouchScale, 40 * nativeTouchScale), "<size=" + 16 * nativeTouchScale +">Add FT State Change Callback</size>")) 
			{
				callbackMessage = "No message received";
				InputManager.instance.AddCallback();
				callbackAdded = true;
			}
		}

	}	
}
