using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Advent.A2024
{

    public static class SplitExentsion {
        public static IEnumerable<List<T>> SplitBy<T>(this IEnumerable<T> source, T separator, bool removeEmptyEntries = false, bool includeSeparator = false)
        {
            return new StreamSplitterEnumerable<T>(source, separator, removeEmptyEntries, includeSeparator);
        }
    }

    public class StreamSplitterEnumerable<T> : IEnumerable<List<T>>
    {
        private IEnumerable<T> _source;
        private T _separator;
        private bool _removeEmptyEntries;
        private bool _includeSeparator;

        public StreamSplitterEnumerable (IEnumerable<T> source, T separator, bool removeEmptyEntries, bool includeSeparator) {
            _source = source;
            _separator = separator;
            _removeEmptyEntries = removeEmptyEntries;
            _includeSeparator = includeSeparator;
        }

        public IEnumerator<List<T>> GetEnumerator()
        {
            return new StreamSplitter<T>(_source.GetEnumerator(), _separator, _removeEmptyEntries, _includeSeparator);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class StreamSplitter<T> : IEnumerator<List<T>>
    {
        private IEnumerator<T> _source;
        private T _separator;
        private bool _removeEmptyEntries;
        private bool _includeSeparator;

        public StreamSplitter(IEnumerator<T> source, T separator, bool removeEmptyEntries, bool includeSeparator)
        {
            _source = source;
            _separator = separator;
            _removeEmptyEntries = removeEmptyEntries;
            _includeSeparator = includeSeparator;
        }

        private List<T> _current = null;
        private List<T> _next = new List<T>();

        public List<T> Current
        {
            get
            {
                if (_current == null) {
                    throw new InvalidOperationException();
                }
                return _current;
            }
        }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            // Do nothing
        }

        public bool MoveNext()
        {
            while (_source.MoveNext()) {
                if (_source.Current.Equals(_separator)) {
                    if (_removeEmptyEntries && _next.Count == 0) {
                        // Skip
                        continue;
                    }
                    if (_includeSeparator) {
                        _next.Add(_source.Current);
                    }
                    _current = _next;
                    _next = new List<T>();
                    return true;
                }
                _next.Add(_source.Current);
            }

            // If we're removing emtpy entries, skip the final group if it's empty
            if (_next != null && _removeEmptyEntries && _next.Count == 0) {
                _next = null;
                _current = null;
                return false;
            }
            // Return final group, if there is one
            else if (_next != null) {
                _current = _next;
                _next = null;
                return true;
            }
            // No more to return
            _current = null;
            return false;
        }

        public void Reset()
        {
            throw new NotSupportedException();
        }
    }
}