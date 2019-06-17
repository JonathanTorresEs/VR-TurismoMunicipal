using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DigitalSalmon.C360 {
	[AddComponentMenu("Complete360Tour/Input/GazeInputRaycaster")]
	public class GazeInputRaycaster : MonoBehaviour {
		//-----------------------------------------------------------------------------------------
		// Delegates:
		//-----------------------------------------------------------------------------------------

		public delegate void RaycastHitEventHandler(RaycastHit raycastHit);

		//-----------------------------------------------------------------------------------------
		// Events:
		//-----------------------------------------------------------------------------------------

		public event RaycastHitEventHandler OnRaycasthit; // This event is called every frame that the user's gaze is over a collider.

		//-----------------------------------------------------------------------------------------
		// Protected Fields:
		//-----------------------------------------------------------------------------------------

		protected LayerMask exclusionLayers = 0; // Layers to exclude from the raycast.
		protected float rayLength = 50f; // How far into the scene the ray is cast.

		//-----------------------------------------------------------------------------------------
		// Private Fields:
		//-----------------------------------------------------------------------------------------

		private IPointerHandler lastInteractible; //The last interactive item

		//-----------------------------------------------------------------------------------------
		// Public Properties:
		//-----------------------------------------------------------------------------------------

		// Utility for other classes to get the current interactive item
		public IPointerHandler CurrentInteractible { get; private set; }

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		protected void Start() {
			StartCoroutine(RaycastLoopCoroutine());
		}

		//-----------------------------------------------------------------------------------------
		// Private Methods:
		//-----------------------------------------------------------------------------------------

		private IEnumerator RaycastLoopCoroutine() {
			while (true) {
				yield return new WaitForEndOfFrame();
				EyeRaycast();
			}
		}

		private void EyeRaycast() {
			PhysicsRaycast();
		}

		private void PhysicsRaycast() {
			// Create a ray that points forwards from the camera.
			Ray ray = new Ray(transform.position, transform.forward);
			RaycastHit hit;

			// Do the raycast forweards to see if we hit an interactive item
			if (Physics.Raycast(ray, out hit, rayLength)) {
				IPointerHandler interactible = hit.collider.GetComponentInChildren<IPointerHandler>(); //attempt to get the VRInteractiveItem on the hit object
				CurrentInteractible = interactible;

				// If we hit an interactive item and it's not the same as the last interactive item, then call Over
				if ((interactible != null) && interactible != lastInteractible) interactible.OnPointerEnter(new PointerEventData(EventSystem.current));

				// Deactive the last interactive item 
				if (interactible != lastInteractible) DeactiveLastInteractible();

				lastInteractible = interactible;

				if (OnRaycasthit != null) OnRaycasthit(hit);
			}
			else {
				// Nothing was hit, deactive the last interactive item.
				DeactiveLastInteractible();
				CurrentInteractible = null;
			}
		}

		private void DeactiveLastInteractible() {
			if (lastInteractible == null) return;

			lastInteractible.OnPointerExit(new PointerEventData(EventSystem.current));
			lastInteractible = null;
		}
	}
}