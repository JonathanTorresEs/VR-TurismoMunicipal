namespace DigitalSalmon.C360
{
    public interface IMappedPrefab {
        //-----------------------------------------------------------------------------------------
        // Methods:
        //-----------------------------------------------------------------------------------------

        void UpdateState(MediaSwitchStates state);
        void UpdateData(PrefabElement element, NodeData nodeData);
    }
}