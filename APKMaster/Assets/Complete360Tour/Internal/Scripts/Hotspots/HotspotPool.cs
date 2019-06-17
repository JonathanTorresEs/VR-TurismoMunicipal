using System.Collections.Generic;
using UnityEngine;

namespace DigitalSalmon.C360
{
    public class HotspotPool {
        //-----------------------------------------------------------------------------------------
        // Private Fields:
        //-----------------------------------------------------------------------------------------

        private List<Hotspot> hotspots = new List<Hotspot>();

        private readonly Hotspot hotspotTemplate;
        private readonly ObjectPool<Hotspot> pool;
        private readonly Transform root;

        public List<Hotspot> Hotspots { get { return hotspots; } protected set { hotspots = value; } }

        //-----------------------------------------------------------------------------------------
        // Constructors:
        //-----------------------------------------------------------------------------------------

        public HotspotPool(Hotspot template, Transform root) {
            hotspotTemplate = template;
            this.root = root;

            pool = new ObjectPool<Hotspot>(CreateNew);
            pool.OnObjectCreated += OnObjectCreated;
        }

        //-----------------------------------------------------------------------------------------
        // Public Methods:
        //-----------------------------------------------------------------------------------------

        public Hotspot Get() { return pool.Get(); }

        private void OnObjectCreated(IPooledObject<Hotspot> hotspot) { Hotspots.Add((Hotspot) hotspot); }

        private Hotspot CreateNew() {
            Hotspot hotspot = Object.Instantiate(hotspotTemplate);
            hotspot.transform.SetParent(root, false);
            return hotspot;
        }
    }
}