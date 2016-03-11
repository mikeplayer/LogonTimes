using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogonTimes.IoC
{
    public class Registry : IRegistry
    {
        #region Fields

        /// <summary>
        /// The IOC container.
        /// </summary>
        private readonly IContainer container;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StructureMapIocRegistry" /> class.
        /// </summary>
        /// <remarks>
        /// Uses the default container.
        /// </remarks>
        public Registry() : this(GetContainer()) { }

        private static IContainer GetContainer()
        {
            IContainer container = new Container();
            return container;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="StructureMapIocRegistry"/> class.
        /// </summary>
        ~Registry()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }

        #region IDisposable

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Container.Dispose();
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="StructureMapIocRegistry" /> class.
        /// </summary>
        /// <param name="container"> The IOC container. </param>
        public Registry(IContainer container)
        {
            this.container = container;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// The IOC container.
        /// </summary>
        public IContainer Container
        {
            get { return container; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Retrieves all instances of the specified type.
        /// </summary>
        /// <typeparam name="T"> The type of instance to retrieve. </typeparam>
        /// <returns> </returns>
        public IEnumerable<T> GetAllInstances<T>()
        {
            try
            {
                return container.GetAllInstances<T>();
            }
            catch (StructureMapException)
            {
                return default(IEnumerable<T>);
            }
        }

        /// <summary>
        /// Retrieves an instance of the specified type.
        /// </summary>
        /// <typeparam name="T"> The type of instance to retrieve. </typeparam>
        /// <returns> The retrieved instance. </returns>
        public T GetInstance<T>()
        {
            var container = this.container;

            if (container == null)
            {
                throw new InvalidOperationException("container is null");
            }
            try
            {
                return IsInterface(typeof(T)) ? container.TryGetInstance<T>() : container.GetInstance<T>();
            }
            catch (StructureMapException)
            {
                return default(T);
            }
        }

        private static bool IsInterface(Type t)
        {
            return t.IsInterface;
        }

        /// <summary>
        /// Retrieves an instance of the specified type.
        /// </summary>
        /// <param name="type">The type of instance to retrieve. </param>
        /// <returns> The retrieved instance. </returns>
        public object GetInstance(Type type)
        {
            try
            {
                return container.GetInstance(type);
            }
            catch (StructureMapException)
            {
                return default(Type);
            }
        }

        #endregion Methods
    }
}
