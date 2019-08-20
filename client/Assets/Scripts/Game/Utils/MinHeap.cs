using System;
using System.Collections.Generic;

namespace AM.Game {
    public class MinHeap<T> where T : IComparable<T> {
        private List<T> _array = new List<T>();

        public List<T> List {
            get { return _array; }
        }

        public T Min() {
            if (_array.Count > 0) {
                return _array[0];
            }
            return default(T);
        }

        public T Pop() {
            var len = _array.Count;
            if (len == 0) {
                return default(T);
            }
            var top = _array[0];
            _array[0] = _array[len - 1];
            _array.RemoveAt(len - 1);
            ShiftDown(0);
            return top;
        }
        public void Push(T e) {
            _array.Add(e);
            ShiftUp(_array.Count - 1);
        }

        public void Fix(int i) {
            ShiftUp(i);
            ShiftDown(i);
        }

        private void ShiftUp(int i) {
            while (i > 0) {
                var j = (i - 1) / 2;
                if (_array[i].CompareTo(_array[j]) < 0) {
                    var temp = _array[i];
                    _array[i] = _array[j];
                    _array[j] = temp;
                    i = j;
                } else {
                    return;
                }
            }
        }

        private void ShiftDown(int i) {
            var len = _array.Count;
            while (i < len) {
                var j = i * 2 + 1;
                var k = j + 1;
                if (j >= len) {
                    return;
                }
                if (k < len && _array[k].CompareTo(_array[j]) < 0) {
                    j = k;
                }
                if (_array[j].CompareTo(_array[i]) < 0) {
                    var temp = _array[i];
                    _array[i] = _array[j];
                    _array[j] = temp;
                    i = j;
                } else {
                    return;
                }
            }
        }

    }
}
