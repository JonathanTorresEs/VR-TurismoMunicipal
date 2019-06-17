using System;
using System.Linq;
using UnityEngine;

namespace DigitalSalmon.C360 {
	[AddComponentMenu("Complete360Tour/Hotspots/Hotspot")]
	public class Hotspot : MonoBehaviour, IPooledObject<Hotspot> {
		//-----------------------------------------------------------------------------------------
		// Events:
		//-----------------------------------------------------------------------------------------

		public event EventHandler<Hotspot> OnTrigger;

		//-----------------------------------------------------------------------------------------
		// Inspector Variables:
		//-----------------------------------------------------------------------------------------

		protected new Renderer renderer;

		//-----------------------------------------------------------------------------------------
		// Backing Fields:
		//-----------------------------------------------------------------------------------------

		private bool _isVisible;
		private bool _isHovered;

		//-----------------------------------------------------------------------------------------
		// Private Fields:
		//-----------------------------------------------------------------------------------------

		private IInputBroadcaster[] inputBroadcasters;

		//-----------------------------------------------------------------------------------------
		// Interface Properties:
		//-----------------------------------------------------------------------------------------

		GameObject IPooledObject.GameObject { get; set; }

		Action<Hotspot> IPooledObject<Hotspot>.ReturnToPool { get; set; }

		//-----------------------------------------------------------------------------------------
		// Public Properties:
		//-----------------------------------------------------------------------------------------

		public HotspotElement Element { get; private set; }

		//-----------------------------------------------------------------------------------------
		// Protected Properties:
		//-----------------------------------------------------------------------------------------

		public bool IsVisible {
			get { return _isVisible; }
			protected set {
				if (_isVisible == value) return;
				_isVisible = value;
				VisiblityChanged(_isVisible);
			}
		}

		public virtual bool IsHovered {
			get { return _isHovered; }
			protected set {
				if (_isHovered == value) return;
				_isHovered = value;
				HoveredChanged(_isHovered);
			}
		}

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		protected virtual void Awake() {
			renderer = GetComponentInChildren<Renderer>();
			inputBroadcasters = GetComponentsInChildren<IInputBroadcaster>();
		}

		protected virtual void Start() {
			DefaultState();
			SetVisible(true);
			SetAllowInput(true);
		}

		//-----------------------------------------------------------------------------------------
		// Event Handlers:
		//-----------------------------------------------------------------------------------------

		protected virtual void InputBroadcaster_InputBegin() { SetHovered(true); }
		protected virtual void InputBroadcaster_InputEnd() { SetHovered(false); }
		protected virtual void InputBroadcaster_InputSubmit() { Trigger(); }

		//-----------------------------------------------------------------------------------------
		// Interface Methods:
		//-----------------------------------------------------------------------------------------

		void IPooledObject.OnCreatedByPool() { DefaultState(); }

		void IPooledObject.OnRemovedFromPool() { DefaultState(); }

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		/// <summary>
		/// Switches this Hotspots internal data and visual style to settings defined by 'element'.
		/// </summary>
		public virtual void SwitchElement(HotspotElement element) {
			Element = element;
			SetVisible(element != null);
			SetAllowInput(IsVisible);

			if (element == null) {
				SetHovered(false);
				IPooledObject<Hotspot> pooledHotspot = this;
				if (pooledHotspot.ReturnToPool != null) pooledHotspot.ReturnToPool(this);
				return;
			}

			AssignIcon(element.IconIndex);

			NodeDataElement.UpdateMappedElementPosition(transform, element);
		}

		//-----------------------------------------------------------------------------------------
		// Virtual Methods:
		//-----------------------------------------------------------------------------------------

		protected virtual void VisiblityChanged(bool visible) { }
		protected virtual void HoveredChanged(bool hovered) { }

		protected virtual void Trigger() {
			OnTrigger.InvokeSafe(this);
		}

		//-----------------------------------------------------------------------------------------
		// Protected Methods:
		//-----------------------------------------------------------------------------------------

		protected void SetVisible(bool visible) { IsVisible = visible; }

		protected void SetHovered(bool hovered) {
			if (hovered && !IsVisible) return;
			IsHovered = hovered;
		}

		protected void SetAllowInput(bool allowInput) {
			if (inputBroadcasters == null) return;

			foreach (IInputBroadcaster broadcaster in inputBroadcasters) {
				if (allowInput) {
					broadcaster.OnInputBegin += InputBroadcaster_InputBegin;
					broadcaster.OnInputEnd += InputBroadcaster_InputEnd;
					broadcaster.OnInputSubmit += InputBroadcaster_InputSubmit;
				}
				else {
					broadcaster.OnInputBegin -= InputBroadcaster_InputBegin;
					broadcaster.OnInputEnd -= InputBroadcaster_InputEnd;
					broadcaster.OnInputSubmit -= InputBroadcaster_InputSubmit;
				}
			}
		}

		//-----------------------------------------------------------------------------------------
		// Private Methods:
		//-----------------------------------------------------------------------------------------

		private void DefaultState() {
			SetVisible(false);
			SetHovered(false);
		}

		private void AssignIcon(int iconIndex) {
			const string ICON_PROPERTY = "_Icon";
			Texture2D icon = HotspotIcons.AllHotspotIcons.ElementAtOrDefault(iconIndex);
			renderer.material.SetTexture(ICON_PROPERTY, icon);
		}
	}
}