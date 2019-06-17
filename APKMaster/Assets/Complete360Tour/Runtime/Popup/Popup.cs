using DigitalSalmon.C360;
using UnityEngine;

namespace DigitalSalmon.C360 {
	public class Popup : AnimatedHotspot {
		//-----------------------------------------------------------------------------------------
		// Inspector Variables:
		//-----------------------------------------------------------------------------------------

		[Header("Popup")]

		[Subheader("Developer")]

		[SerializeField]
		protected bool showTransitionAlpha = false;

		[SerializeField]
		protected float transitionAlpha = 0;

		private IAnimatedComponent[] animatedComponents;

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		protected override void Awake() {
			base.Awake();
			animatedComponents = GetComponentsInChildren<IAnimatedComponent>();
		}

		protected override void Start() {
			base.Start();
			OnHoveredAlphaUpdate(0);
		}

		protected virtual void OnValidate() {
			if (Application.isPlaying) return;
			Awake();
			OnHoveredAlphaUpdate(showTransitionAlpha ? transitionAlpha : 0);
		}

		//-----------------------------------------------------------------------------------------
		// Protected Methods:
		//-----------------------------------------------------------------------------------------

		protected override void OnHoveredAlphaUpdate(float alpha) {
			base.OnHoveredAlphaUpdate(alpha);
			float delta = Easing.QuadEaseInOut(alpha);

			foreach (IAnimatedComponent animatedComponent in animatedComponents) {
				animatedComponent.OnAlphaChanged(delta);
			}
		}
	}
}