using System;
using System.Collections.Generic;
using System.Linq;

namespace ZeroDawn.Managers
{
    public class DecimaManagerCollection : IDisposable
    {
        public List<IDecimaManager> Managers { get; private set; } = new List<IDecimaManager>();

        public bool Disposed { get; private set; }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Add manager <typeparamref name="T" />
        /// </summary>
        /// <param name="manager"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool AddManager<T>(T manager) where T : IDecimaManager, new()
        {
            if (Managers.OfType<T>().Any()) return false;
            Managers.Add(manager);
            return true;
        }

        /// <summary>
        ///     flow safe get manager <typeparamref name="T" />
        /// </summary>
        /// <param name="manager"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool GetManager<T>(out T manager) where T : IDecimaManager, new()
        {
            manager = Managers.OfType<T>().FirstOrDefault();
            return manager != null;
        }

        /// <summary>
        ///     Get manager <typeparamref name="T" />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [Obsolete("Use GetOrCreateManager<T>")]
        public T GetManager<T>() where T : IDecimaManager, new()
        {
            return Managers.OfType<T>().FirstOrDefault();
        }

        /// <summary>
        ///     Get or create manager <typeparamref name="T" /> if not exist
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetOrCreateManager<T>() where T : IDecimaManager, new()
        {
            if (GetManager(out T manager)) return manager;
            manager = new T();
            AddManager(manager);
            return manager;
        }

        /// <summary>
        ///     Safely clean manager set
        /// </summary>
        public void Clean()
        {
            foreach (var manager in Managers)
                if (manager is IDisposable disposable)
                    disposable.Dispose();
            Managers.Clear();
            GC.Collect();
        }

        /// <summary>
        ///     GC safe dispose wrapper
        /// </summary>
        /// <param name="disposing"></param>
        public void Dispose(bool disposing)
        {
            if (Disposed) return;

            Clean();

            if (disposing) Managers = default;

            Disposed = true;
        }

        ~DecimaManagerCollection()
        {
            Dispose(false);
        }
    }
}
