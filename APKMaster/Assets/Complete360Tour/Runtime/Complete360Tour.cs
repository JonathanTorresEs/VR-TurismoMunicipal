using System.Collections.Generic;
using UnityEngine;

namespace DigitalSalmon.C360 {
	public enum MediaSwitchStates {
		BeforeSwitch,
		Switch,
		AfterSwitch
	}

	[AddComponentMenu("Complete360Tour/Complete360Tour")]
	public class Complete360Tour : MonoBehaviour {
		//-----------------------------------------------------------------------------------------
		// Type Definitions:
		//-----------------------------------------------------------------------------------------

		public enum Transitions {
			None,
			DipToBlack
		}

		//-----------------------------------------------------------------------------------------
		// Events:
		//-----------------------------------------------------------------------------------------

		public static EventHandler<MediaSwitchStates, NodeData> MediaSwitch;

		//-----------------------------------------------------------------------------------------
		// Inspector Variables:
		//-----------------------------------------------------------------------------------------

		[Header("Tour Settings")]
		[Subheader("Primary Tour")]
		[SerializeField]
		[Tooltip("The tour data file you created using the 360 Tour panel.")]
		protected TextAsset tourData;

		[SerializeField]
		[Tooltip("Should the tour begin as soon as the scene loads.")]
		protected bool autoBeginTour = true;

		[SerializeField]
		[Tooltip("The name or uid of the node you would like to start your tour with.")]
		protected string entryId;

		[Header("Transition Settings")]
		[Subheader("Style")]
		[SerializeField]
		protected Transitions transitionStyle;

		[Header("Assignment")]
		[SerializeField]
		protected HotspotReactor hotspotReactor;

		//-----------------------------------------------------------------------------------------
		// Backing Fields:
		//-----------------------------------------------------------------------------------------

		private ScreenTint _screenTint;

		//-----------------------------------------------------------------------------------------
		// Private Fields:
		//-----------------------------------------------------------------------------------------

		private Tour tour;

		public List<NodeData> NodeData { get; private set; }
		public Dictionary<int, NodeData> NodeDataFromUid { get; private set; }
		public Dictionary<string, NodeData> NodeDataFromName { get; private set; }

		//-----------------------------------------------------------------------------------------
		// Protected Properties:
		//-----------------------------------------------------------------------------------------

		protected ScreenTint ScreenTint {
			get {
				if (_screenTint == null) {
					_screenTint = FindObjectOfType<ScreenTint>();
				}
				return _screenTint;
			}
		}

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		protected void Awake() {
			if (ScreenTint == null && transitionStyle == Transitions.DipToBlack) {
				Debug.LogWarning("Transition style requires ScreenTint to be in the scene. Please add ScreenTint to your camera.");
			}
			if (ScreenTint != null && transitionStyle == Transitions.None) {
				Debug.LogWarning(
					"ScreenTint found in scene but transition style does not use it. If clicking 'Play' results in a black screen, remove the ScreenTint component from your camera.");
			}

			NodeData = new List<NodeData>();
			NodeDataFromUid = new Dictionary<int, NodeData>();
			NodeDataFromName = new Dictionary<string, NodeData>();
		}

		protected void OnEnable() { hotspotReactor.OnTriggered += HotspotReactor_HotspotTriggered; }

		protected void Start() {
			if (tourData == null) {
				Debug.LogWarning("No TourData file. Ensure a TourData TextAsset has been assigned in the Complete360Tour object.");
				return;
			}

			tour = Tour.GetTourFromJson(tourData.text);

			if (tour == null) {
				Debug.LogWarning("Failed to load tour. TourData file could be damaged.");
				return;
			}

			InitialiseNodeDataLookups();

			if (autoBeginTour) {
				BeginTour();
			}
		}

		protected void OnDisable() { hotspotReactor.OnTriggered -= HotspotReactor_HotspotTriggered; }

		//-----------------------------------------------------------------------------------------
		// Event Handlers:
		//-----------------------------------------------------------------------------------------

		protected void HotspotReactor_HotspotTriggered(Hotspot hotspot) { GoToMedia(hotspot.Element.TargetNodeData); }

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		/// <summary>
		/// Instructs the MediaSwitch to move to a given NodeData.
		/// </summary>
		public void GoToMedia(NodeData nodeData) {
			if (nodeData == null) {
				Debug.LogWarning("GoToMedia called but requested nodeData is null.");
			}

			switch (transitionStyle) {
				case Transitions.None:
					InformSwitchReactors(nodeData, MediaSwitchStates.BeforeSwitch);
					InformSwitchReactors(nodeData, MediaSwitchStates.Switch);
					InformSwitchReactors(nodeData, MediaSwitchStates.AfterSwitch);
					break;
				case Transitions.DipToBlack:
					InformSwitchReactors(nodeData, MediaSwitchStates.BeforeSwitch);
					ScreenTint.Dip(1, () => InformSwitchReactors(nodeData, MediaSwitchStates.Switch), () => InformSwitchReactors(nodeData, MediaSwitchStates.AfterSwitch));
					break;
			}
		}

		/// <summary>
		/// Switches the MediaSwitch to the first piece of media.
		/// </summary>
		public void BeginTour() {
			NodeData firstNodeData = GetFirstMedia();
			GoToMedia(firstNodeData);
		}

		/// <summary>
		/// Returns the NodeData from the current tour, located by name.
		/// </summary>
		public NodeData GetNodeDataByName(string niceName) {
			if (NodeDataFromName.ContainsKey(niceName)) {
				return NodeDataFromName[niceName];
			}
			return null;
		}

		/// <summary>
		/// Returns the NodeData from the current tour, located by uid.
		/// </summary>
		public NodeData GetNodeDataByUid(int uid) {
			if (NodeDataFromUid.ContainsKey(uid)) {
				return NodeDataFromUid[uid];
			}
			return null;
		}

		/// <summary>
		/// Instructs the MediaSwitch to move to a given NodeData, located by node name.
		/// </summary>
		public void GoToMedia(string nodeName) {
			GoToMedia(GetNodeDataByName(nodeName));
		}

		//-----------------------------------------------------------------------------------------
		// Protected Methods:
		//-----------------------------------------------------------------------------------------

		/// <summary>
		/// Calls our MediaSwitch event and resets all canvases
		/// </summary>
		protected void InformSwitchReactors(NodeData nodeData, MediaSwitchStates state) {
			MediaSwitch(state, nodeData);

			if (state == MediaSwitchStates.Switch) {
				Canvas[] canvases = FindObjectsOfType<Canvas>();
				foreach (Canvas canvas in canvases) {
					canvas.enabled = false;
					canvas.enabled = true;
				}
			}
		}

		//-----------------------------------------------------------------------------------------
		// Private Methods:
		//-----------------------------------------------------------------------------------------

		private void InitialiseNodeDataLookups() {
			NodeData = tour.NodeDataList;
			foreach (NodeData nodeData in NodeData) {
				if (!NodeDataFromUid.ContainsKey(nodeData.Uid)) {
					NodeDataFromUid.Add(nodeData.Uid, nodeData);
				}
				else {
					Debug.LogWarning("Duplicate uid found on nodes [" + nodeData.Uid + "]");
				}
				if (!NodeDataFromName.ContainsKey(nodeData.NiceName)) {
					NodeDataFromName.Add(nodeData.NiceName, nodeData);
				}
				else {
					Debug.LogWarning("Duplicate name found on nodes [" + nodeData.NiceName + "]");
				}
			}
		}

		private NodeData GetFirstMedia() {
			if (tour == null) {
				Debug.LogWarning("Failed to find first media. TourData files must contain atleast one node!");
				return null;
			}

			NodeData namedNodeData = GetNodeDataByName(entryId);
			if (namedNodeData != null) return namedNodeData;
			int uid;
			if (int.TryParse(entryId, out uid)) {
				return GetNodeDataByUid(uid);
			}

			return null;
		}
	}
}