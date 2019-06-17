using UnityEngine;

public class AcAudioSourceVolume : MonoBehaviour, IAnimatedComponent {
	[SerializeField]
	protected float volumeA;

	[SerializeField]
	protected float volumeB;

	private AudioSource _audioSource;
	private AudioSource AudioSource { get { return _audioSource == null ? (_audioSource = GetComponent<AudioSource>()) : _audioSource; } }

	void IAnimatedComponent.OnAlphaChanged(float alpha) { AudioSource.volume = Mathf.Lerp(volumeA, volumeB, alpha); }
}