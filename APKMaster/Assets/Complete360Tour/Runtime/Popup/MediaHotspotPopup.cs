using DigitalSalmon.C360;
using UnityEngine;

public class MediaHotspotPopup : Popup {
	//-----------------------------------------------------------------------------------------
	// Unity Lifecycle:
	//-----------------------------------------------------------------------------------------

	[Header("Media Hotspot")]
	[Subheader("Target")]
	[SerializeField]
	protected string targetMediaName;

	//-----------------------------------------------------------------------------------------
	// Protected Methods:
	//-----------------------------------------------------------------------------------------

	protected override void Trigger() {
		base.Trigger();
		DigitalSalmon.C360.Complete360Tour c360 = FindObjectOfType<DigitalSalmon.C360.Complete360Tour>();
		if (c360 != null) c360.GoToMedia(targetMediaName);
	}
	

}