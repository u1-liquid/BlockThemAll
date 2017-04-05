using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlockThemAll.Base
{
    public class ThreadSafeHashSet<T> : HashSet<T>, IDisposable
    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        public ThreadSafeHashSet()
        {
        }

        public ThreadSafeHashSet(IEnumerable<T> collection) : base(collection)
        {
        }

        public ThreadSafeHashSet(IEqualityComparer<T> comparer) : base(comparer)
        {
        }

        public ThreadSafeHashSet(IEnumerable<T> collection, IEqualityComparer<T> comparer) : base(collection, comparer)
        {
        }

        protected ThreadSafeHashSet(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
        
        public new bool Add(T item)
        {
            _lock.EnterWriteLock();
            try
            {
                return base.Add(item);
            }
            finally
            {
                if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();
            }
        }

        public bool UnSafeAdd(T item) => base.Add(item);

        public new void Clear()
        {
            _lock.EnterWriteLock();
            try
            {
                base.Clear();
            }
            finally
            {
                if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();
            }
        }

        public void UnSafeClear() => base.Clear();

        public new bool Contains(T item)
        {
            _lock.EnterReadLock();
            try
            {
                return base.Contains(item);
            }
            finally
            {
                if (_lock.IsReadLockHeld) _lock.ExitReadLock();
            }
        }

        public bool UnSafeContains(T item) => base.Contains(item);

        public new bool Remove(T item)
        {
            _lock.EnterWriteLock();
            try
            {
                return base.Remove(item);
            }
            finally
            {
                if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();
            }
        }

        public bool UnSafeRemove(T item) => base.Remove(item);

        public new int Count
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return base.Count;
                }
                finally
                {
                    if (_lock.IsReadLockHeld) _lock.ExitReadLock();
                }
            }
        }

        public int UnSafeCount => base.Count;

        public void EnterReadLock() => _lock.EnterReadLock();
        public void ExitReadLock() => _lock.ExitReadLock();
        public void EnterWriteLock() => _lock.EnterWriteLock();
        public void ExitWriteLock() => _lock.ExitWriteLock();
        public bool IsReadLockHeld => _lock.IsReadLockHeld;
        public bool IsWriteLockHeld => _lock.IsWriteLockHeld;

        public void Dispose()
        {
            _lock?.Dispose();
        }
    }
}
