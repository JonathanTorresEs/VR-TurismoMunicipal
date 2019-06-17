using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DigitalSalmon.C360 {
	[AddComponentMenu("Complete360Tour/Input/GazeInputRaycaster")]
	public class MouseInputRaycaster : MonoBehaviour {
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

		private IPointerHandler lastInteractable; //The last interactive item
		private new Camera camera;

		//-----------------------------------------------------------------------------------------
		// Public Properties:
		//-----------------------------------------------------------------------------------------

		// Utility for other classes to get the current interactive item
		public IPointerHandler CurrentInteractable { get; private set; }

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		protected void Awake() { camera = GetComponent<Camera>(); }

		protected void Start() { StartCoroutine(RaycastLoopCoroutine()); }

		protected void Update() {
			if (Input.GetMouseButtonDown(0)) {
				if (CurrentInteractable != null) {
					CurrentInteractable.OnPointerClick(new PointerEventData(EventSystem.current));
				}
			}
		}

		//-----------------------------------------------------------------------------------------
		// Private Methods:
		//-----------------------------------------------------------------------------------------

		private IEnumerator RaycastLoopCoroutine() {
			while (true) {
				yield return new WaitForEndOfFrame();
				Raycast();
			}
		}

		private void Raycast() {
			Ray ray = camera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			// Do the raycast forweards to see if we hit an interactive item
			if (Physics.Raycast(ray, out hit, rayLength)) {
				IPointerHandler interactible = hit.collider.GetComponentInChildren<IPointerHandler>(); //attempt to get the VRInteractiveItem on the hit object
				CurrentInteractable = interactible;

				// If we hit an interactive item and it's not the same as the last interactive item, then call Over
				if (interactible != null && interactible != lastInteractable) interactible.OnPointerEnter(new PointerEventData(EventSystem.current));

				// Deactive the last interactive item 
				if (interactible != lastInteractable) DeactiveLastInteractible();

				lastInteractable = interactible;

				if (OnRaycasthit != null) OnRaycasthit(hit);
			}
			else {
				// Nothing was hit, deactive the last interactive item.
				DeactiveLastInteractible();
				CurrentInteractable = null;
			}
		}

		private void DeactiveLastInteractible() {
			if (lastInteractable == null) return;

			lastInteractable.OnPointerExit(new PointerEventData(EventSystem.current));
			lastInteractable = null;
		}
	}
}