using System;
using System.Collections.Generic;

namespace GOAP
{
    public class PriorityQueue<TElement, TPriority> where TPriority : IComparable<TPriority>
    {
        private readonly List<(TElement element, TPriority priority)> _elements = new();

        public int Count => _elements.Count;

        public void Enqueue(TElement element, TPriority priority)
        {
            _elements.Add((element, priority));
            var current = _elements.Count - 1;

            while (current > 0 && _elements[current].priority.CompareTo(_elements[current - 1].priority) < 0)
            {
                var parent = (current - 1) / 2;

                Swap(current, parent);
                current = parent;
            }
        }

        public TElement Dequeue()
        {
            var element = _elements[0].element;
            _elements[0] = _elements[^1];
            _elements.RemoveAt(_elements.Count - 1);
            
            var current = 0;

            while (true)
            {
                var left = 2 * current + 1;
                var right = 2 * current + 2;
                var smallest = current;

                if (left < _elements.Count && _elements[left].priority.CompareTo(_elements[smallest].priority) < 0)
                {
                    smallest = left;
                }

                if (right < _elements.Count && _elements[right].priority.CompareTo(_elements[smallest].priority) < 0)    
                {
                    smallest = right;   
                }
                
                if(smallest == current) break;

                Swap(current, smallest);
                current = smallest;
            }
            
            return element;
        }
        
        public void Clear() => _elements.Clear();
        
        private void Swap(int index1, int index2)
        {
            (_elements[index1], _elements[index2]) = (_elements[index2], _elements[index1]);
        }
    }
}