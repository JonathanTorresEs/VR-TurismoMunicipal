using DigitalSalmon.C360.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class AcTextColor : MonoBehaviour, IAnimatedComponent {
	[SerializeField]
	protected Color colorA;

	[SerializeField]
	protected Color colorB;

	private Text _image;
	private Text Image { get { return _image == null ? (_image = GetComponent<Text>()) : _image; } }

	void IAnimatedComponent.OnAlphaChanged(float alpha) { Image.color = Color.Lerp(colorA.WithAlpha(Image.color.a), colorB.WithAlpha(Image.color.a), alpha); }
}