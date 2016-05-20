using System.Collections;
using System.Collections.Generic;

namespace XianDict
{

    public class CartesianListIterator<T> : IEnumerator<List<T>>
    {
        private List<List<T>> sets;
        private int[] cardinalities;
        private int[] indices;
        private bool started;

        public CartesianListIterator(List<List<T>> sets)
        {
            this.sets = sets;
            cardinalities = new int[sets.Count];
            indices = new int[sets.Count];
            int i = 0;
            foreach (var set in sets)
            {
                cardinalities[i] = sets[i].Count;
                indices[i] = 0;
                i++;
            }
            started = false;
        }

        public List<T> Current
        {
            get
            {
                List<T> result = new List<T>();
                int i = 0;
                foreach (var set in sets)
                {
                    if (cardinalities[i] > 0)
                    {
                        result.Add(set[indices[i]]);
                    }
                    i++;
                }
                return result;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public bool MoveNext()
        {
            if (!started)
            {
                started = true;
                return true;
            }

            bool carry = true;
            for (int i = indices.Length - 1; i >= 0; i--)
            {
                if (carry)
                {
                    if (cardinalities[i] > 0)
                    {
                        indices[i] = (indices[i] + 1) % cardinalities[i];
                    }
                    carry = (indices[i] == 0);
                }
                else
                {
                    break;
                }
            }
            return !carry;
        }

        public void Reset()
        {
            for (int i = 0; i < indices.Length; i++)
            {
                indices[i] = 0;
            }
            started = false;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~CartesianIterator() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }

    public class CartesianArrayIterator<T> : IEnumerator<T[]>
    {
        private List<T[]> sets;
        private int[] cardinalities;
        private int[] indices;
        private bool started;

        public CartesianArrayIterator(List<T[]> sets)
        {
            this.sets = sets;
            cardinalities = new int[sets.Count];
            indices = new int[sets.Count];
            int i = 0;
            foreach (var set in sets)
            {
                cardinalities[i] = sets[i].Length;
                indices[i] = 0;
                i++;
            }
            started = false;
        }

        public T[] Current
        {
            get
            {
                T[] result = new T[sets.Count];
                int i = 0;
                foreach (var set in sets)
                {
                    if (cardinalities[i] > 0)
                    {
                        result[i] = set[indices[i]];
                    }
                    i++;
                }
                return result;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public bool MoveNext()
        {
            if (!started)
            {
                started = true;
                return true;
            }

            bool carry = true;
            for (int i = indices.Length - 1; i >= 0; i--)
            {
                if (carry)
                {
                    if (cardinalities[i] > 0)
                    {
                        indices[i] = (indices[i] + 1) % cardinalities[i];
                    }
                    carry = (indices[i] == 0);
                }
                else
                {
                    break;
                }
            }
            return !carry;
        }

        public void Reset()
        {
            for (int i = 0; i < indices.Length; i++)
            {
                indices[i] = 0;
            }
            started = false;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~CartesianIterator() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }

}
