using System;
using System.Collections.Generic;

namespace UnityEngine.XR.iOS
{
	public class TouchHitManager : MonoBehaviour
	{
		public Transform ImageObjectPrarent;
		public GameObject ImageObjectPrefab;

		bool HitTestWithResultType (ARPoint point, ARHitTestResultType resultTypes)
		{
			List<ARHitTestResult> hitResults = UnityARSessionNativeInterface.GetARSessionNativeInterface ().HitTest (point, resultTypes);
			if (hitResults.Count > 0) {
				foreach (var hitResult in hitResults) {
					Debug.Log ("Got hit!");
					Vector3 target = UnityARMatrixOps.GetPosition (hitResult.worldTransform);
					this.createImageCanvas (target);
					return true;
				}
			}
			return false;
		}

		// Update is called once per frame
		void Update () {
			

			if (Input.touchCount > 0 && ImageObjectPrarent != null)
			{
				var touch = Input.GetTouch(0);
				if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved)
				{
					var screenPosition = Camera.main.ScreenToViewportPoint(touch.position);
					ARPoint point = new ARPoint {
						x = screenPosition.x,
						y = screenPosition.y
					};

					// prioritize reults types
					ARHitTestResultType[] resultTypes = {
						ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent, 
						// if you want to use infinite planes use this:
						//ARHitTestResultType.ARHitTestResultTypeExistingPlane,
						ARHitTestResultType.ARHitTestResultTypeHorizontalPlane, 
						ARHitTestResultType.ARHitTestResultTypeFeaturePoint
					}; 

					foreach (ARHitTestResultType resultType in resultTypes)
					{
						if (HitTestWithResultType (point, resultType))
						{
							return;
						}
					}
				}
			}
			//For Debug Mouse Click
			else if (Input.GetMouseButton (0)) {
				RaycastHit hit;
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				if (Physics.Raycast (ray, out hit)) {
					this.createImageCanvas (hit.point);
				}
			}
		}

		void createImageCanvas(Vector3 target) {
			//for checking hit position & rotation
			GameObject image = ImageLoader.PictureCanvasObject;
			if (image == null)
				return;
			Debug.Log (image);
			ImageObjectController imageController = image.GetComponent<ImageObjectController> ();
			Debug.Log (imageController);
			imageController.DoAction (target);
		}
	}


}

