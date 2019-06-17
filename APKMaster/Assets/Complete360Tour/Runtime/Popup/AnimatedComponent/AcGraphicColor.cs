using DigitalSalmon.C360.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class AcGraphicColor : MonoBehaviour, IAnimatedComponent {
	[SerializeField]
	protected Color colorA;

	[SerializeField]
	protected Color colorB;

	private Graphic _graphic;
	private Graphic Graphic { get { return _graphic == null ? (_graphic = GetComponent<Image>()) : _graphic; } }

	void IAnimatedComponent.OnAlphaChanged(float alpha) { Graphic.color = Color.Lerp(colorA.WithAlpha(Graphic.color.a), colorB.WithAlpha(Graphic.color.a), alpha); }
}