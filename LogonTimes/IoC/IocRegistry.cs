using System;
using System.Collections.Generic;

namespace LogonTimes.IoC
{
    /// <summary>
    /// Static IOC Registry interface.
    /// </summary>
    public static class IocRegistry
    {
        #region Fields

        private static IStructureMapRegistry registry;

        /// <summary>
        /// Singleton instance of the IocRegistry
        /// </summary>
        public static IStructureMapRegistry Instance
        {
            get
            {
                if (registry == null)
                {
                    registry = new StructureMapRegistry();
                }
                return registry;
            }
            set
            {
                registry = value;
            }
        }

        #endregion Fields

        #region Methods

        /// <summary>
        /// Retrieves all instances of the specified type.
        /// </summary>
        /// <typeparam name="T"> The type of instance to retrieve. </typeparam>
        /// <returns> </returns>
        public static IEnumerable<T> GetAllInstances<T>()
        {
            return Instance.GetAllInstances<T>();
        }

        /// <summary>
        /// Retrieves an instance of the specified type.
        /// </summary>
        /// <typeparam name="T"> The type of instance to retrieve. </typeparam>
        /// <returns> The retrieved instance. </returns>
        public static T GetInstance<T>()
        {
            return Instance.GetInstance<T>();
        }

        /// <summary>
        /// Retrieves an instance of the specified type.
        /// </summary>
        /// <param name="type">The type of instance to retrieve.</param>
        /// <returns>The retrieved instance.</returns>
        public static object GetInstance(Type type)
        {
            return Instance.GetInstance(type);
        }

        #endregion Methods
    }
}
