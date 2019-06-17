using UnityEngine;

public class AcSphereColliderRadius : MonoBehaviour, IAnimatedComponent {
	[SerializeField]
	protected float radiusA;

	[SerializeField]
	protected float radiusB;

	private SphereCollider _sphereCollider;
	private SphereCollider SphereCollider { get { return _sphereCollider == null ? (_sphereCollider = GetComponent<SphereCollider>()) : _sphereCollider; } }

	void IAnimatedComponent.OnAlphaChanged(float alpha) { SphereCollider.radius = Mathf.Lerp(radiusA, radiusB, alpha); }
}