using System;
using System.Collections.Generic;
using System.Text;

namespace BetterSimpleLang
{
    // TODO: Reset Method
    public class Iterator<T>
    {
        private T _NULL;
        private T[] _data;
        private int _index = -1;

        public Iterator(T[] data, T NULL)
        {
            _data = data;
            _NULL = NULL;
        }

        public T Current()
        {
            if (_index >= 0 && _index < _data.Length)
                return _data[_index];
            return _NULL;
        }

        public T Next()
        {
            if (_index < _data.Length - 1)
            {
                _index++;
                return Current();
            }
            return _NULL;
        }

        public T LookNext()
        {
            if (_index < _data.Length - 1)
            {
                //_index++;
                return _data[_index + 1];
            }
            return _NULL;
        }

        public int GetIndex() => _index;
    }
}
