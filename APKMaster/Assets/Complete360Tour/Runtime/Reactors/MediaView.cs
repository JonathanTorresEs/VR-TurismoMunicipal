using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace DigitalSalmon.C360 {
	[AddComponentMenu("Complete360Tour/Media/MediaView")]
	public class MediaView : MonoBehaviour {
		//-----------------------------------------------------------------------------------------
		// Constants:
		//-----------------------------------------------------------------------------------------

		private const string TEXTURE_PROPERTY = "_MainTex";
		private const string STEREO_PROPERTY = "_Stereoscopic";

        public string currentScene = "";

		//-----------------------------------------------------------------------------------------
		// Private Fields:
		//-----------------------------------------------------------------------------------------

		private Renderer mediaRenderer;
		private RenderTexture videoTexture;
		private VideoPlayer videoPlayer;
		private AudioSource audioSource;

		//-----------------------------------------------------------------------------------------
		// Unity Lifecycle:
		//-----------------------------------------------------------------------------------------

		protected void Awake() {
			mediaRenderer = GetComponentInChildren<Renderer>();
			videoPlayer = GetComponent<VideoPlayer>();
			if (videoPlayer == null) {
				Debug.LogWarning("Ensure a VideoPlayer component exists on the MediaView object.");
			}

			audioSource = GetComponent<AudioSource>();
			if (audioSource == null) {
				Debug.LogWarning("Ensure an AudioSource component exists on the MediaView object if you would like Videos to output audio.");
			}

			if (videoPlayer != null && audioSource != null && videoPlayer.audioOutputMode == VideoAudioOutputMode.AudioSource) {
				videoPlayer.SetTargetAudioSource(0, audioSource);
			}
		}

		protected void OnEnable() { Complete360Tour.MediaSwitch += C360_MediaSwitch; }

		protected void OnDisable() { Complete360Tour.MediaSwitch -= C360_MediaSwitch; }

		//-----------------------------------------------------------------------------------------
		// Event Handlers:
		//-----------------------------------------------------------------------------------------

		protected void C360_MediaSwitch(MediaSwitchStates state, NodeData data) {
			if (state == MediaSwitchStates.Switch) SwitchMedia(data);
		}

		//-----------------------------------------------------------------------------------------
		// Protected Methods:
		//-----------------------------------------------------------------------------------------

		protected void SwitchMedia(NodeData data) {
            currentScene = data.NiceName;
            Debug.Log("Current scene is: " + currentScene);

			ExitMedia();

			ImageMediaNodeData imageData = data as ImageMediaNodeData;
			if (imageData != null) SwitchMedia(imageData);

			VideoMediaNodeData videoData = data as VideoMediaNodeData;
			if (videoData != null) SwitchMedia(videoData);
		}

		//-----------------------------------------------------------------------------------------
		// Private Methods:
		//-----------------------------------------------------------------------------------------

		private void SwitchMedia(ImageMediaNodeData data) {
			if (data == null) {
				ClearView();
				return;
			}

            
			SetMedia(data.Image);
			SetStereoscopic(data.IsStereo);
		}

		private void SwitchMedia(VideoMediaNodeData data) {
			if (data == null || data.VideoClip == null) {
				ClearView();
				return;
			}

			if (videoTexture != null) {
				videoTexture.DiscardContents();
				Destroy(videoTexture);
			}

			videoTexture = new RenderTexture((int) data.VideoClip.width, (int) data.VideoClip.width, 0);
			videoPlayer.clip = data.VideoClip;
			videoPlayer.targetTexture = videoTexture;
            Debug.Log("Setting video scene");

            //Find all loading objects and make them visible
            GameObject[] loadingObjects = GameObject.FindGameObjectsWithTag("Loading");
            Color whiteColor = new Color(1, 1, 1, 1);
            for (int j = 0; j < loadingObjects.Length; j++)
                loadingObjects[j].gameObject.GetComponent<Text>().color = whiteColor;

            //Disappear all loading texts
            StartCoroutine(DisappearLoadingText(loadingObjects));

            SetMedia(videoTexture);
			videoPlayer.Play();
			SetStereoscopic(data.IsStereo);
		}

		private void ExitMedia() {
			ClearView();
			videoPlayer.Stop();
		}

		private void SetMedia(Texture texture) { mediaRenderer.material.SetTexture(TEXTURE_PROPERTY, texture); }

		private void SetStereoscopic(bool stereoscopic) { mediaRenderer.material.SetFloat(STEREO_PROPERTY, stereoscopic ? 1 : 0); }

		private void ClearView() {
			SetMedia(null);
			SetStereoscopic(false);
		}

        IEnumerator DisappearLoadingText(GameObject[] loadingObject)
        {
            yield return new WaitForSeconds(1f);
            for (int i = 0; i < loadingObject.Length; i++)
            {
                Color transparentColor = new Color(1, 1, 1, 0);
                loadingObject[i].gameObject.GetComponent<Text>().color = transparentColor;
            }
        }
    }
}