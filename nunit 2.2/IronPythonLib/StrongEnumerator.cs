using System;
using System.Collections;
using System.Collections.Generic;

namespace IronPythonLib
{
    /// <summary>
    /// Little helper class, used to make converting enumerable classes to a strongly
    /// typed enumerator.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StrongEnumerator<T> : IEnumerable<T>
    {
        private IEnumerable _enumerable;

        public StrongEnumerator(IEnumerable enumerable)
        {
            if (enumerable == null) throw new ArgumentNullException("enumerable");
            _enumerable = enumerable;            
        }

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            IEnumerator enumerator = _enumerable.GetEnumerator();
            while (enumerator.MoveNext())
            {
                yield return (T) enumerator.Current;
            }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _enumerable.GetEnumerator();
        }

        #endregion
    }
}