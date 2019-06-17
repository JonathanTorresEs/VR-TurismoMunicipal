using DigitalSalmon.C360.Extensions;
using UnityEngine;

public class AcRectTransformAnchoredPosition : MonoBehaviour, IAnimatedComponent {
	[SerializeField]
	protected float positionA;

	[SerializeField]
	protected float positionB;

	[SerializeField]
	protected Axis2D axis;

	private RectTransform _rectTransform;
	private RectTransform RectTransform { get { return _rectTransform == null ? (_rectTransform = GetComponent<RectTransform>()) : _rectTransform; } }

	void IAnimatedComponent.OnAlphaChanged(float alpha) {
		switch (axis) {
			case Axis2D.X:
				RectTransform.anchoredPosition = RectTransform.anchoredPosition.WithX(Mathf.Lerp(positionA, positionB, alpha));
				break;
			case Axis2D.Y:
				RectTransform.anchoredPosition = RectTransform.anchoredPosition.WithY(Mathf.Lerp(positionA, positionB, alpha));
				break;
		}
	}
}