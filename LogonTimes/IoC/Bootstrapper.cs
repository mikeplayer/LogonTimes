using System;
using StructureMap;
using StructureMap.Graph;
using LogonTimes.People;
using LogonTimes.DataModel;
using LogonTimes.TimeControl;
using LogonTimes.Logging;
using LogonTimes.DateHandling;
using LogonTimes.Applications;

namespace LogonTimes.IoC
{
    public static class Bootstrapper
    {
        #region Fields
        private static readonly object SyncObj = new object();
        private static bool isBootStrapped;
        private static ILogger logger;
        #endregion Fields

        #region Methods
        private static ILogger Logger
        {
            get
            {
                if (logger == null)
                {
                    logger = IocRegistry.GetInstance<ILogger>();
                }
                return logger;
            }
        }

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
                        DoBootStrapping<StructureMapRegistry>(typeExclusion);
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
        private static void DoBootStrapping<T>(Func<Type, bool> typeExclusion) where T : IStructureMapRegistry, new()
        {
            var iocRegistry = InitialseIocRegistry<T>();

            try
            {
                IContainer iocContainer = iocRegistry.Container;

                iocContainer.Configure(config => config.Scan(scanner =>
                {
                    scanner.TheCallingAssembly();
                    scanner.AddAllTypesOf<IUserManagement>();
                    scanner.AddAllTypesOf<ITimeManagement>();
                    scanner.AddAllTypesOf<IEventManagement>();
                    scanner.AddAllTypesOf<IFileServices>();
                    scanner.AddAllTypesOf<IApplicationManagement>();
                    scanner.AddAllTypesOf<IDataAccess>();
                    scanner.AddAllTypesOf<IDates>();
                    scanner.WithDefaultConventions();
                }));
                iocContainer.Configure(_ =>
                {
                    _.ForSingletonOf<IDataAccess>().Use<DataAccess>();
                    _.Forward<IDataAccess, ITestServiceRunningData>();
                    _.Forward<IDataAccess, ILogonTimesConfigurationData>();
                    _.Forward<IDataAccess, IWorkingItemsData>();
                    _.Forward<IDataAccess, ITimeManagementData>();
                    _.Forward<IDataAccess, IUserManagementData>();
                    _.Forward<IDataAccess, IApplicationManagementData>();
                    _.ForSingletonOf<ILogger>().Use<Logger>();
                });
            }
            catch (Exception ex)
            {
                Logger.LogException(string.Format("BaseDir is {0}", AppDomain.CurrentDomain.BaseDirectory), DebugLevels.Error, ex);
                throw;
            }
        }

        /// <summary>
        ///   Initialises the IOC registry.
        /// </summary>
        private static T InitialseIocRegistry<T>() where T : IStructureMapRegistry, new()
        {
            var result = new T();
            IocRegistry.Instance = result;

            return result;
        }

        #endregion Methods
    }
}
