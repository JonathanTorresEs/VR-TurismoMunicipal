using DigitalSalmon.C360.Extensions;
using UnityEngine;

public class AcRectTransformSize : MonoBehaviour, IAnimatedComponent {
	[SerializeField]
	protected float sizeA;

	[SerializeField]
	protected float sizeB;

	[SerializeField]
	protected Axis2D axis;

	private RectTransform _rectTransform;
	private RectTransform RectTransform { get { return _rectTransform == null ? (_rectTransform = GetComponent<RectTransform>()) : _rectTransform; } }

	void IAnimatedComponent.OnAlphaChanged(float alpha) {
		switch (axis) {
			case Axis2D.X:
				RectTransform.sizeDelta = RectTransform.sizeDelta.WithX(Mathf.Lerp(sizeA, sizeB, alpha));
				break;
			case Axis2D.Y:
				RectTransform.sizeDelta = RectTransform.sizeDelta.WithY(Mathf.Lerp(sizeA, sizeB, alpha));
				break;
		}
	}
}