using UnityEngine;

namespace DigitalSalmon.C360 {
	[AddComponentMenu("Complete360Tour/Hotspots/HotspotReactor")]
	public class HotspotReactor : MonoBehaviour {
		//-----------------------------------------------------------------------------------------
		// Events:
		//-----------------------------------------------------------------------------------------

		public event EventHandler<Hotspot> OnTriggered;

		//-----------------------------------------------------------------------------------------
		// Inspector Variables:
		//-----------------------------------------------------------------------------------------

		[SerializeField]
		protected Hotspot hotspotTemplate;

		//-----------------------------------------------------------------------------------------
		// Private Fields:
		//-----------------------------------------------------------------------------------------

		private HotspotPool hotspotPool;

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		protected void Awake() {
			if (hotspotTemplate == null) {
				Debug.LogWarning("No Hotspot template assigned. Please assign a Hotspot template.");
				return;
			}

			hotspotPool = new HotspotPool(hotspotTemplate, transform);
		}

		protected void OnEnable() { Complete360Tour.MediaSwitch += C360_MediaSwitch; }

		protected void OnDisable() { Complete360Tour.MediaSwitch -= C360_MediaSwitch; }

		//-----------------------------------------------------------------------------------------
		// Event Handlers:
		//-----------------------------------------------------------------------------------------

		protected void C360_MediaSwitch(MediaSwitchStates state, NodeData data) {
			if (state == MediaSwitchStates.BeforeSwitch) SwitchElements(null);
			if (state == MediaSwitchStates.Switch && data != null) SwitchElements(data.GetElements<HotspotElement>());
		}

		private void Hotspot_OnTrigger(Hotspot hotspot) {
			if (hotspot == null || hotspot.Element == null)  return;

			OnTriggered.InvokeSafe(hotspot);
		}

		//-----------------------------------------------------------------------------------------
		// Private Methods:
		//-----------------------------------------------------------------------------------------

		private void SwitchElements(HotspotElement[] elements) {
			if (hotspotPool == null) return;

			foreach (Hotspot hotspot in hotspotPool.Hotspots) {
				hotspot.SwitchElement(null);
				hotspot.OnTrigger -= Hotspot_OnTrigger;
			}

			if (elements == null) return;

			for (int i = 0; i < elements.Length; i++) {
				Hotspot hotspot = hotspotPool.Get();

				hotspot.SwitchElement(elements[i]);
				hotspot.OnTrigger += Hotspot_OnTrigger;
			}
		}
	}
}