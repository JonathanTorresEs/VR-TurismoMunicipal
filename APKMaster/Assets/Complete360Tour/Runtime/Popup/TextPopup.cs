using UnityEngine;
using UnityEngine.UI;

namespace DigitalSalmon.C360 {
	public class TextPopup : Popup {
		//-----------------------------------------------------------------------------------------
		// Inspector Variables:
		//-----------------------------------------------------------------------------------------

		[Header("Assignment")]
		[SerializeField]
		protected Text titleText;

		[SerializeField]
		protected Text subtitleText;

		[SerializeField]
		protected Text bodyText;

		[Header("Infographic Text")]
		[SerializeField]
		protected string title;

		[SerializeField]
		protected string subtitle;

		[SerializeField]
		[TextArea]
		protected string body;

		//-----------------------------------------------------------------------------------------
		// Protected Properties:
		//-----------------------------------------------------------------------------------------

		protected override void OnValidate() {
			base.OnValidate();
			if (titleText != null) titleText.text = title;
			if (subtitleText != null) subtitleText.text = subtitle;
			if (bodyText != null) bodyText.text = body;
		}
	}
}