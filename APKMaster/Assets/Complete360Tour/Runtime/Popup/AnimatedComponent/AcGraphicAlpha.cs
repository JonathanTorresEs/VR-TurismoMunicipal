using DigitalSalmon.C360.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class AcGraphicAlpha : MonoBehaviour, IAnimatedComponent {
	[SerializeField]
	protected float alphaA;

	[SerializeField]
	protected float alphaB;

	private Graphic _graphic;
	private Graphic Graphic { get { return _graphic == null ? (_graphic = GetComponent<Graphic>()) : _graphic; } }

	void IAnimatedComponent.OnAlphaChanged(float alpha) { Graphic.color = Color.Lerp(Graphic.color.WithAlpha(alphaA), Graphic.color.WithAlpha(alphaB), alpha); }
}