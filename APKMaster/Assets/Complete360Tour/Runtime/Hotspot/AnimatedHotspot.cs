using System.Collections;
using UnityEngine;

namespace DigitalSalmon.C360 {
	[AddComponentMenu("Complete360Tour/Hotspots/AnimatedUIHotspot")]
	public class AnimatedHotspot : Hotspot {
		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------
		[Header("Hotspot")]

		[Subheader("Animation")]

		[SerializeField]
		protected float triggerDuration = 1.5f;

		//-----------------------------------------------------------------------------------------
		// Private Fields:
		//-----------------------------------------------------------------------------------------

		private Animator animator;
		private float hoveredAlpha;
		private bool triggerLocked;

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		protected override void Awake() {
			base.Awake();
			animator = GetComponent<Animator>();
		}

		protected virtual void OnEnable() { StartCoroutine(HoverCoroutine()); }

		protected virtual void Update() {
			if (Input.GetKeyDown(KeyCode.E)) {
				SetVisible(!IsVisible);
			}
		}

		protected virtual void OnDisable() { StopAllCoroutines(); }

		//-----------------------------------------------------------------------------------------
		// Protected Methods:
		//-----------------------------------------------------------------------------------------

		protected override void VisiblityChanged(bool visible) {
			base.VisiblityChanged(visible);
			const string VISIBILITY_PARAM = "Visible";
			if (animator != null) animator.SetBool(VISIBILITY_PARAM, visible);
		}

		protected override void HoveredChanged(bool hovered) {
			base.HoveredChanged(hovered);
			triggerLocked = false;
		}

		protected virtual void OnHoveredAlphaUpdate(float alpha) {
			if (renderer != null) renderer.material.SetFloat("_FillValue", alpha);
		}

		private IEnumerator HoverCoroutine() {
			

			while (true) {
				bool alphaChanged = false;

				if (IsHovered && hoveredAlpha < 1) {
					hoveredAlpha += Time.deltaTime / triggerDuration;
					if (hoveredAlpha > 1) hoveredAlpha = 1;
					alphaChanged = true;
				}
				if (!IsHovered && hoveredAlpha > 0) {
					hoveredAlpha -= Time.deltaTime / triggerDuration;
					if (hoveredAlpha < 0) hoveredAlpha = 0;
					alphaChanged = true;
				}

				if (IsHovered && hoveredAlpha == 1 && !triggerLocked) {
					Trigger();
					triggerLocked = true;
				}

				if (alphaChanged) OnHoveredAlphaUpdate(hoveredAlpha);

				yield return null;
			}
		}
	}
}