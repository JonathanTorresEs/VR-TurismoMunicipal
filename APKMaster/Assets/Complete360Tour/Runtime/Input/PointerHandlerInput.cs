using UnityEngine;
using UnityEngine.EventSystems;
using System;

namespace DigitalSalmon.C360
{
    [AddComponentMenu("Complete360Tour/Input/PointerHandlerInput")]
    public class PointerHandlerInput : MonoBehaviour, IInputBroadcaster, IPointerHandler {
        //-----------------------------------------------------------------------------------------
        // Public Properties:
        //-----------------------------------------------------------------------------------------
        
        Action IInputBroadcaster.OnInputBegin { get; set; }
        Action IInputBroadcaster.OnInputEnd { get; set; }
		Action IInputBroadcaster.OnInputSubmit { get; set; }

		///-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
            if (((IInputBroadcaster)this).OnInputBegin != null) ((IInputBroadcaster)this).OnInputBegin();
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
            if (((IInputBroadcaster)this).OnInputEnd != null) ((IInputBroadcaster)this).OnInputEnd();
        }

		void IPointerClickHandler.OnPointerClick(PointerEventData eventData) {
			if (((IInputBroadcaster)this).OnInputSubmit != null) ((IInputBroadcaster)this).OnInputSubmit();
		}
    }
}