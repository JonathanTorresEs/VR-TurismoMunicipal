using UnityEngine;

namespace DigitalSalmon.C360 {
	public class SwitchYaw : MonoBehaviour {
		//-----------------------------------------------------------------------------------------
		// Type Definitions:
		//-----------------------------------------------------------------------------------------

		public enum EntryYawModes {
			Absolute,
			Dynamic
		}

		//-----------------------------------------------------------------------------------------
		// Inspector Variables:
		//-----------------------------------------------------------------------------------------

		[SerializeField]
		[Tooltip("Absolute - The world is aligned to the entry yaw defined by the node editor. Dynamic - The camera will always be looking at the entry yaw line when the node is entered")]
		protected EntryYawModes entryYawMode;

		[SerializeField]
		protected Transform targetTransform;

		protected void Awake() {
			if (targetTransform == null) Debug.LogWarning("No target transform. This component is obselete without a target; assign one, or remove the component.");
		}

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		protected void OnEnable() { Complete360Tour.MediaSwitch += C360_MediaSwitch; }

		protected void OnDisable() { Complete360Tour.MediaSwitch += C360_MediaSwitch; }

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		public void ExitMedia() { }

		protected void C360_MediaSwitch(MediaSwitchStates state, NodeData data) {
			MediaNodeData mediaNodeData = data as MediaNodeData;
			if (mediaNodeData == null) return;
			if (state == MediaSwitchStates.Switch) UpdateEntryYaw(mediaNodeData.EntryYaw);
		}

		//-----------------------------------------------------------------------------------------
		// Private Methods:
		//-----------------------------------------------------------------------------------------

		private void UpdateEntryYaw(float entryYaw) {
			if (targetTransform == null) return;
			switch (entryYawMode) {
				case EntryYawModes.Absolute:
					targetTransform.localEulerAngles = new Vector3(0, (entryYaw * 360), 0);
					break;
				case EntryYawModes.Dynamic:
					float childOffset = targetTransform.GetChild(0).localEulerAngles.y;
					targetTransform.localEulerAngles = new Vector3(0, (entryYaw * 360) - childOffset, 0);
					break;
			}
		}
	}
}