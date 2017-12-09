using System.Collections;
using System.Collections.Generic;

namespace Xamarin.Forms.Platform.SkiaSharp.Controls
{
    public class ControlCollection : ICollection<Control>, IList<Control>
    {
        private ControlManager _container;
        private List<Control> _items;

        internal ControlCollection(ControlManager container)
        {
            _container = container;
            _items = new List<Control>();
        }

        public Control this[int index]
        {
            get { return _items[index]; }
            set
            {
                _items[index] = value;
                SetParent(value);
                Invalidate();
            }
        }

        public int Count => _items.Count;

        public bool IsReadOnly => false;

        public void Add(Control item)
        {
            _items.Add(item);
            SetParent(item);
            Invalidate();
        }

        public void Clear()
        {
            RemoveParent(_items);
            _items.Clear();
            Invalidate();
        }

        public bool Contains(Control item)
        {
            return _items.Contains(item);
        }

        public void CopyTo(Control[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public IEnumerator<Control> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public int IndexOf(Control item)
        {
            return _items.IndexOf(item);
        }

        public void Insert(int index, Control item)
        {
            _items.Insert(index, item);
            SetParent(item);
            Invalidate();
        }

        public bool Remove(Control item)
        {
            if (_items.Remove(item))
            {
                RemoveParent(item);
                Invalidate();
                return true;
            }

            return false;
        }

        public void RemoveAt(int index)
        {
            RemoveParent(_items[index]);
            _items.RemoveAt(index);
            Invalidate();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        private void SetParent(Control item)
        {
            item.Parent = _container;
        }

        private void RemoveParent(IList<Control> items)
        {
            foreach (var item in items)
            {
                RemoveParent(item);
            }
        }

        private void RemoveParent(Control item)
        {
            item.Parent = null;
        }

        private void Invalidate()
        {
            _container?.Invalidate();
        }
    }
}