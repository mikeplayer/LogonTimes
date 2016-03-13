using StructureMap;
using System;
using System.Collections.Generic;

namespace LogonTimes.IoC
{
    public interface IStructureMapRegistry : IDisposable
    {
        #region Methods

        /// <summary>
        ///   Retrieves all instances of the specified type.
        /// </summary>
        /// <typeparam name="T"> The type of instance to retrieve. </typeparam>
        /// <returns> </returns>
        IEnumerable<T> GetAllInstances<T>();

        /// <summary>
        ///   Retrieves an instance of the specified type.
        /// </summary>
        /// <typeparam name="T"> The type of instance to retrieve. </typeparam>
        /// <returns> The retrieved instance. </returns>
        T GetInstance<T>();

        /// <summary>
        ///   Retrieves an instance of the specified type.
        /// </summary>
        /// <param name="type"> The type of instance to retrieve. </param>
        /// <returns> The retrieved instance. </returns>
        object GetInstance(Type type);

        /// <summary>
        /// The IoC Container
        /// </summary>
        IContainer Container { get; }

        #endregion Methods
    }
}
