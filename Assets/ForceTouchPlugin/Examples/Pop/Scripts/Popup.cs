using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Popup : MonoBehaviour {

	public GameObject PreviewBg;
	public GameObject OpenBg;

	public Text description;

	private CanvasGroup cg;
	private bool push;
	private bool pop;

	private float offset;

	private string placeholder = "Button #{0} \nPopup Content";

	void Awake()
	{
		cg = this.GetComponent<CanvasGroup> ();
		Hide ();
	}


	public void Hide()
	{
		cg.alpha = 0;
		gameObject.SetActive (false);

		PreviewBg.gameObject.SetActive (false);
		PreviewBg.transform.localScale = Vector3.one;
			
		OpenBg.gameObject.SetActive (false);

		push = false;
		pop = false;
	}

	public void Reset(int button)
	{
		description.text = string.Format (placeholder, button);
	}

	public void Click()
	{
		cg.alpha = 1;
		gameObject.SetActive (true);

		PreviewBg.SetActive (false);
		PreviewBg.transform.localScale = Vector3.one;

		OpenBg.gameObject.SetActive (true);
		pop = true;
	}

	public bool IsPop()
	{
		return pop;
	}

	public void SetForce(float force, float maxForce)
	{
		//if popup is already in pop state ignore the force
		if (pop)
			return;

		gameObject.SetActive (true);
		PreviewBg.SetActive (true);

		//while we push, we change the opacity of popup
		if (!push) 
		{
			PreviewBg.transform.localScale = Vector3.one;

			var value = Mathf.Clamp (force / (maxForce / 2f), 0f, 1f); //while force is less than 0.5f normalized value
			cg.alpha = value;

			if (value >= 0.99f)
				push = true;


			//This is quite awesome behavior seen in native apps. Threshold for Push is 0.5f
			//Threshold for Pop is 1f. Dynamic range for Pop by default is [0.5f; 1f], however if
			//user reduces the pressure after Push threshhold is reached, the dynamic range for Pop
			//will increase to [min_pressure; 1f];
			offset = force;
		}
		//while we pop we change the scale of background
		else
		{
			cg.alpha = 1f;

			if (force < offset)
				offset = force;

			var value = Mathf.Clamp ((force - offset) / (maxForce - offset), 0f, 1f); //while force is less than 1f normalized value (we only track pop progress above 0.5f).
			PreviewBg.transform.localScale = Vector3.one * Mathf.Lerp(1f, 1.1f, value); 

			if (value >= 0.99f)
				Click(); //click takes us to the same state like pop
		}
	}
}
