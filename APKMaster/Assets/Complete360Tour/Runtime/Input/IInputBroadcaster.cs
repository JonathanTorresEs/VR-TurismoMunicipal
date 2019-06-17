using System;

namespace DigitalSalmon.C360
{
    public interface IInputBroadcaster {
        //-----------------------------------------------------------------------------------------
        // Properties:
        //-----------------------------------------------------------------------------------------

        Action OnInputBegin { get; set; }
        Action OnInputEnd { get; set; }
		Action OnInputSubmit { get; set; }
    }
}