using System;
using System.Collections;
using UnityEngine;

namespace DigitalSalmon {
	[ExecuteInEditMode]
	public class ScreenTint : MonoBehaviour {
		//-----------------------------------------------------------------------------------------
		// Inspector Variables:
		//-----------------------------------------------------------------------------------------

		[SerializeField]
		protected Material tintMaterial;

		[SerializeField]
		protected Color tintOnAwake;

		//-----------------------------------------------------------------------------------------
		// Constants:
		//-----------------------------------------------------------------------------------------

		private static readonly int TINT_PROPERTY = Shader.PropertyToID("_Tint");

		//-----------------------------------------------------------------------------------------
		// Backing Fields:
		//-----------------------------------------------------------------------------------------

		private Color _tint;

		private IEnumerator screenCoroutine;

		//-----------------------------------------------------------------------------------------
		// Private Properties:
		//-----------------------------------------------------------------------------------------


		private Material Material { get { return tintMaterial; } }

		private Color Tint {
			get { return _tint; }
			set {
				_tint = value;
				UpdateMaterialProperty();
			}
		}

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		protected void Awake() {
			Tint = tintOnAwake;
		}

		protected void OnRenderImage(RenderTexture source, RenderTexture destination) {
			
			if (Material == null || Tint.a == 0) {
				Graphics.Blit(source, destination);
				return;
			}
			
			if (!Application.isPlaying) {
				Tint = Color.clear;
			}

			Graphics.Blit(source, destination, Material);
		}

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		public void Dip(float seconds = 1, Action onMidPoint = null, Action onComplete = null, Color? color = null) {
			if (screenCoroutine != null) StopCoroutine(screenCoroutine);
			screenCoroutine = ScreenDipCoroutine(seconds, onMidPoint, onComplete, color);
			StartCoroutine(screenCoroutine);
		}

		public void Fade(float seconds = 1, float alpha = 1, Action onComplete = null, Color? color = null) {
			if (screenCoroutine != null) StopCoroutine(screenCoroutine);
			screenCoroutine = ScreenFadeCoroutine(seconds, alpha, onComplete, color);
			StartCoroutine(screenCoroutine);
		}

		public void InstantFade(float alpha, Color? color = null) {
			if (screenCoroutine != null) StopCoroutine(screenCoroutine);
			Color targetTint = color ?? Color.black;
			targetTint.a = alpha;
			Tint = targetTint;
		}

		//-----------------------------------------------------------------------------------------
		// Private Methods:
		//-----------------------------------------------------------------------------------------

		private void UpdateMaterialProperty() {
			if (Material.HasProperty(TINT_PROPERTY)) Material.SetColor(TINT_PROPERTY, Tint);
		}

		private void SetAlpha(float alpha) {
			Color t = Tint;
			t.a = alpha;
			Tint = t;
		}

		private IEnumerator ScreenFadeCoroutine(float seconds, float alpha, Action onComplete, Color? color) {
			Color initialTint = Tint;
			Color targetTint = color ?? initialTint;
			targetTint.a = alpha;

			// if already invisible, set to target colour before fading to visible.
			if (initialTint.a == 0) {
				Tint = targetTint;
				SetAlpha(0);
			}
	
			float delta = 0;
			while (true) {
				delta += Time.unscaledDeltaTime / seconds;
				Tint = Color.Lerp(initialTint, targetTint, delta);

				if (delta >= 1) {
					Tint = targetTint;

					yield return new WaitForEndOfFrame();

					if (onComplete != null) {
						onComplete.Invoke();
					}
					break;
				}

				yield return new WaitForEndOfFrame();
			}
		}

		private IEnumerator ScreenDipCoroutine(float seconds, Action onMidPoint, Action onComplete, Color? color) {
			Color initialTint = Tint;
			Color targetTint = color ?? initialTint;
			targetTint.a = 1;
		
			// if already invisible, set to target colour before fading to visible.
			if (initialTint.a == 0) {
				Tint = targetTint;
				SetAlpha(0);
			}

			float delta = 0;
			bool forward = true;
			while (true) {
				delta += (Time.unscaledDeltaTime * (forward ? 1 : -1)) / seconds;

				Tint = Color.Lerp(initialTint, targetTint, delta);

				if (delta >= 1) {
					forward = !forward;
					initialTint.a = 0;

					Tint = targetTint;

					initialTint = new Color(targetTint.r, targetTint.g, targetTint.b, 0);

					yield return new WaitForEndOfFrame();

					if (onMidPoint != null) {
						onMidPoint.Invoke();
					}

					delta = 1;
				}

				if (delta < 0) {
					Tint = initialTint;
					SetAlpha(0);

					yield return new WaitForEndOfFrame();

					if (onComplete != null) {
						onComplete.Invoke();
					}

					break;
				}

				yield return new WaitForEndOfFrame();
			}
		}
	}
}