using System.Collections;
using UnityEngine;

namespace DigitalSalmon.C360 {
	public class AutoTour : MonoBehaviour {
		//-----------------------------------------------------------------------------------------
		// Events:
		//-----------------------------------------------------------------------------------------

		public event EventHandler Complete;

		//-----------------------------------------------------------------------------------------
		// Inspector Variables:
		//-----------------------------------------------------------------------------------------

		[Tooltip("The names, in order, of the nodes this AutoTour should traverse.")]
		[SerializeField]
		protected string[] nodeNames;

		[Tooltip("If true the AutoTour will begin as soon as you press Play.")]
		[SerializeField]
		protected bool autoStart;

		[Tooltip("If true the AutoTour will loop back to the first node when it reaches the end")]
		[SerializeField]
		protected bool loop;

		[Tooltip("The length of time to spend in each node")]
		[SerializeField]
		protected float nodeDuration;

		//-----------------------------------------------------------------------------------------
		// Private Fields:
		//-----------------------------------------------------------------------------------------

		private Complete360Tour complete360Tour;

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		protected void Awake() { complete360Tour = GetComponent<Complete360Tour>(); }

		protected IEnumerator Start() {
			// Delay a frame to let C360 initialise.
			yield return null;
			if (autoStart) BeginAutoTour();
		}

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		public void BeginAutoTour() { StartCoroutine(AutoTourCoroutine()); }

		public void StopAutoTour() { StopAllCoroutines(); }

		//-----------------------------------------------------------------------------------------
		// Private Methods:
		//-----------------------------------------------------------------------------------------

		private IEnumerator AutoTourCoroutine() {


			WaitForSeconds wait = new WaitForSeconds(nodeDuration);
			int index = 0;
			while (true) {
				
				string nextNode = nodeNames[index];
				complete360Tour.GoToMedia(nextNode);

				index = GetNextIndex(index);
				if (index == 0) {
					if (Complete != null) Complete();
					if (!loop) break;
				}

				yield return wait;
			}
		}

		private int GetNextIndex(int index) {
			if (index >= nodeNames.Length - 1) return 0;
			return index + 1;
		}
	}
}