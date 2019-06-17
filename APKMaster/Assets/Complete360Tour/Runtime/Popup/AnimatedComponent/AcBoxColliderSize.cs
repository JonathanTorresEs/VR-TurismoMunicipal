using System.Collections;
using System.Collections.Generic;
using DigitalSalmon.C360.Extensions;
using UnityEngine;

public class AcBoxColliderSize : MonoBehaviour, IAnimatedComponent
{
	[SerializeField]
	protected float sizeA;

	[SerializeField]
	protected float sizeB;

	[SerializeField]
	protected Axis3D axis;

	private BoxCollider _boxCollider;
	private BoxCollider BoxCollider { get { return _boxCollider == null ? (_boxCollider = GetComponent<BoxCollider>()) : _boxCollider; } }

	void IAnimatedComponent.OnAlphaChanged(float alpha)
	{
		switch (axis)
		{
			case Axis3D.X:
				BoxCollider.size = BoxCollider.size.WithX(Mathf.Lerp(sizeA, sizeB, alpha));
				break;
			case Axis3D.Y:
				BoxCollider.size = BoxCollider.size.WithY(Mathf.Lerp(sizeA, sizeB, alpha));
				break;
			case Axis3D.Z:
				BoxCollider.size = BoxCollider.size.WithZ(Mathf.Lerp(sizeA, sizeB, alpha));
				break;
		}
	}
}