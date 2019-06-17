using System;
using System.Collections.Generic;

namespace DigitalSalmon.C360
{
    public abstract class ObjectPool {
    }

    public class ObjectPool<T> : ObjectPool where T : IPooledObject {
        //-----------------------------------------------------------------------------------------
        // Events:
        //-----------------------------------------------------------------------------------------

        public Action<IPooledObject<T>> OnObjectCreated = delegate { };

        //-----------------------------------------------------------------------------------------
        // Private Fields:
        //-----------------------------------------------------------------------------------------

        private readonly Func<T> createNew;
        private readonly Stack<T> stack = new Stack<T>();

        //-----------------------------------------------------------------------------------------
        // Public Properties:
        //-----------------------------------------------------------------------------------------

        public int Count { get; private set; }
        public int CountActive { get { return Count - CountInactive; } }
        public int CountInactive { get { return stack.Count; } }

        //-----------------------------------------------------------------------------------------
        // Constructors:
        //-----------------------------------------------------------------------------------------

        public ObjectPool(Func<T> createNew) { this.createNew = createNew; }

        //-----------------------------------------------------------------------------------------
        // Public Methods:
        //-----------------------------------------------------------------------------------------

        public T Get() { return (T) GetPooledObject(); }

	    public void Return(T element) { if(!stack.Contains(element)) stack.Push(element); }

        //-----------------------------------------------------------------------------------------
        // Public Methods:
        //-----------------------------------------------------------------------------------------

        protected IPooledObject<T> GetPooledObject() {
            IPooledObject<T> element;

            if (stack.Count == 0) {
                element = (IPooledObject<T>) createNew();

                if (element == null) return null;

                element.OnCreatedByPool();
                OnObjectCreated(element);
                Count++;
            }
            else {
                element = (IPooledObject<T>) stack.Pop();
            }

            element.OnRemovedFromPool();

            element.ReturnToPool -= Return;
            element.ReturnToPool += Return;

            return element;
        }
    }
}