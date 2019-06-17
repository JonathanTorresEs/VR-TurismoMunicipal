using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace DigitalSalmon.C360 {
	public class ContextualVideo : Popup {
		//-----------------------------------------------------------------------------------------
		// Inspector Variables:
		//-----------------------------------------------------------------------------------------

		[Header("Video")]
		[Tooltip("This video clip you'd like this prefab to play")]
		[SerializeField]
		protected VideoClip videoClip;

		[Subheader("Resolution Control")]
		[Tooltip("If true the video will be played at it's native resolution, ignoring the resolution override")]
		[SerializeField]
		protected bool nativeResolution;

		[Tooltip("If native resolution is false, this resolution is used for playback (Useful for performance improvement)")]
		[SerializeField]
		protected Vector2Int resolutionOverride;

		//-----------------------------------------------------------------------------------------
		// Private Fields:
		//-----------------------------------------------------------------------------------------

		private VideoPlayer videoPlayer;
		private RenderTexture renderTexture;
		private RawImage rawImage;
		private AudioSource audioSource;

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		protected override void Awake() {
			base.Awake();
			videoPlayer = GetComponentInChildren<VideoPlayer>();
			rawImage = GetComponentInChildren<RawImage>();
			audioSource = GetComponentInChildren<AudioSource>();

			videoPlayer.clip = videoClip;

			if (!ValidateClip()) return;

			InitialiseRenderTexture();
			InitialiseAudio();
		}

		//-----------------------------------------------------------------------------------------
		// Protected Methods:
		//-----------------------------------------------------------------------------------------

		protected override void HoveredChanged(bool hovered) {
			base.HoveredChanged(hovered);
			if (hovered) {
				videoPlayer.Play();
			}
			else {
				videoPlayer.Stop();
			}
		}

		//-----------------------------------------------------------------------------------------
		//-----------------------------------------------------------------------------------------
		// Private Methods:
		//-----------------------------------------------------------------------------------------

		private bool ValidateClip() {
			if (videoClip == null) {
				Debug.LogWarning("No VideoClip found on ContextualVideo, removing prefab from scene");
				if (Application.isPlaying) Destroy(gameObject);
				return false;
			}
			return true;
		}

		private void InitialiseAudio() { videoPlayer.SetTargetAudioSource(0, audioSource); }

		private void InitialiseRenderTexture() {
			if (!Application.isPlaying) return;
			if (nativeResolution) {
				CreateRenderTexture(new Vector2Int((int) videoClip.width, (int) videoClip.height));
			}
			else {
				CreateRenderTexture(resolutionOverride);
			}
		}

		private void CreateRenderTexture(Vector2Int resolution) {
			renderTexture = new RenderTexture(resolution.x, resolution.y, 24);
			renderTexture.antiAliasing = 2;
			videoPlayer.renderMode = VideoRenderMode.RenderTexture;
			videoPlayer.targetTexture = renderTexture;
			rawImage.texture = renderTexture;
		}
	}
}