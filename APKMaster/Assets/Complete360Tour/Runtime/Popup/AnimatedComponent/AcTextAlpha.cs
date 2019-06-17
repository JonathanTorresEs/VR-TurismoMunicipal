using DigitalSalmon.C360.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class AcTextAlpha : MonoBehaviour, IAnimatedComponent {
	[SerializeField]
	protected float alphaA;

	[SerializeField]
	protected float alphaB;

	private Text _image;
	private Text Image { get { return _image == null ? (_image = GetComponent<Text>()) : _image; } }

	void IAnimatedComponent.OnAlphaChanged(float alpha) { Image.color = Color.Lerp(Image.color.WithAlpha(alphaA), Image.color.WithAlpha(alphaB), alpha); }
}