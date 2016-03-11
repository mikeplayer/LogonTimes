using System;
using StructureMap;
using StructureMap.Graph;
using LogonTimes.People;
using LogonTimes.DataModel;
using LogonTimes.TimeControl;
using LogonTimes.Logging;

namespace LogonTimes.IoC
{
    public static class Bootstrapper
    {
        #region Fields

        /// <summary>
        ///   Object used for internal static synchronisation.
        /// </summary>
        private static readonly object SyncObj = new object();

        /// <summary>
        ///   Whether the system has boot strapped.
        /// </summary>
        private static bool isBootStrapped;

        #endregion Fields

        #region Methods

        /// <summary>
        ///   Boot straps the system.
        /// </summary>
        public static void BootStrap()
        {
            BootStrap(null);
        }

        /// <summary>
        ///   Boot straps the system.
        /// </summary>
        /// <param name="typeExclusion"> Function to exclude specific types. </param>
        public static void BootStrap(Func<Type, bool> typeExclusion)
        {
            if (!isBootStrapped)
            {
                lock (SyncObj)
                {
                    if (!isBootStrapped)
                    {
                        DoBootStrapping<Registry>(typeExclusion);
                        isBootStrapped = true;
                    }
                }
            }
        }

        /// <summary>
        ///   Removes bootstrapped configuration.
        /// </summary>
        public static void Dispose()
        {
            if (isBootStrapped)
            {
                lock (SyncObj)
                {
                    if (isBootStrapped)
                    {
                        IocRegistry.Instance = null;
                        isBootStrapped = false;
                    }
                }
            }
        }

        /// <summary>
        ///   Performs boot strapping.
        /// </summary>
        /// <typeparam name="T"> The type of IOC Registry to boot strap for. </typeparam>
        private static void DoBootStrapping<T>(Func<Type, bool> typeExclusion) where T : IRegistry, new()
        {
            var iocRegistry = InitialseIocRegistry<T>();

            try
            {
                IContainer iocContainer = iocRegistry.Container;

                iocContainer.Configure(config => config.Scan(scanner =>
                {
                    scanner.AssemblyContainingType<IUserManagement>();
                    scanner.AssemblyContainingType<ITimeManagement>();
                    scanner.WithDefaultConventions();
                }));
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(string.Format("BaseDir is {0}", AppDomain.CurrentDomain.BaseDirectory), DebugLevels.Error, ex);
                throw;
            }
        }

        /// <summary>
        ///   Initialises the IOC registry.
        /// </summary>
        private static T InitialseIocRegistry<T>() where T : IRegistry, new()
        {
            var result = new T();
            IocRegistry.Instance = result;

            return result;
        }

        #endregion Methods
    }
}
