using LogonTimes.IoC;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap.AutoMocking;
using Rhino.Mocks;

namespace LogonTimesTesting
{
    /// <summary>
    /// Base class for untyped tests. Exposes Registry property and initializes it in getter. Also Assigns a static IocRegistry.Instance.
    /// Uses Automocked container and no bootstrapping. Cleans up global static container on [AfterScenario] and [TestCleanup]. 
    /// </summary>
    public abstract class BaseTest
    {
        private StructureMapRegistry registry;

        private readonly StringBuilder instanceLog = new StringBuilder();

        private static readonly object SyncLock = new object();

        #region Properties

        protected StringBuilder InstanceLog
        {
            get { return instanceLog; }
        }

        /// <summary>
        ///   The test IOC registry.
        /// </summary>
        protected StructureMapRegistry Registry
        {
            get { return CreateRegistryIfNotExistsOrReturnExisting(); }
            private set { registry = value; }
        }

        #endregion Properties

        #region Methods

        private StructureMapRegistry CreateRegistryIfNotExistsOrReturnExisting()
        {
            return registry ?? (registry = CreateRegistryAndAssignIoCRegistry(GetIoCContainer()));
        }

        protected void AddLogMessage(string fmt, params object[] args)
        {
            var dt = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.ffff");
            var msg = string.Format(fmt, args);
            var logMsg = string.Format("[{0}]\t- {1}", dt, msg);
            InstanceLog.AppendLine(logMsg);

            //Debug.WriteLine("*** " + logMsg);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private static StructureMapRegistry CreateRegistryAndAssignIoCRegistry(IContainer ioContainer)
        {
            var mapRegistry = new StructureMapRegistry(ioContainer);

            lock (SyncLock)
            {
                if (IocRegistry.Instance != null)
                {
                    IocRegistry.Instance.Dispose();
                }

                IocRegistry.Instance = mapRegistry;
            }

            return mapRegistry;
        }

        /// <summary>
        /// Cleans up a test.
        /// </summary>
        [TestCleanup]
        public virtual void CleanUp()
        {
            ReleaseRegistry();
        }

        private void ReleaseRegistry()
        {
            lock (SyncLock)
            {
                if (IocRegistry.Instance == null)
                {
                    return;
                }

                if (IocRegistry.Instance != null)
                {
                    IocRegistry.Instance.Dispose();
                    //Console.WriteLine("*** Container disposed");
                }

                IocRegistry.Instance = null;
            }

            Registry = null;
        }


        /// <summary>
        /// Gets the container to use for the tests.
        /// </summary>
        /// <returns>
        /// The IOC container to use.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        protected virtual IContainer GetIoCContainer()
        {
            return CreateAutomockedContainer();
        }

        protected static IContainer CreateContainer()
        {
            return new Container();
        }

        protected static IContainer CreateAutomockedContainer()
        {
            return new AutoMockedContainer(new RhinoMocksAAAServiceLocator());
        }

        /// <summary>
        ///   Creates Registry if not exists.
        /// </summary>
        protected void Initialise()
        {
            Initialise(x => { });
        }

        /// <summary>
        /// Wires up container. Creates Registry if not exists
        /// </summary>
        /// <param name = "dependenciesAction">
        ///   Action used to specify dependencies.
        /// </param>
        protected void Initialise(Action<DependencySpecifier> dependenciesAction)
        {
            if (dependenciesAction == null)
                throw new ArgumentNullException("dependenciesAction");

            PreInitialise();

            CreateRegistryIfNotExistsOrReturnExisting();

            var dependencySpecifier = new DependencySpecifier();
            dependenciesAction(dependencySpecifier);

            foreach (var dependency in dependencySpecifier.Dependencies)
            {
                Registry.Container.Inject(
                    dependency.Key,
                    dependency.Value);
            }

            PostInitialise();
            AddLogMessage("registry recreated");
        }

        /// <summary>
        ///   Creates a mock of the specified type.
        /// </summary>
        /// <typeparam name = "TMock">
        ///   The type of mock to retrieve.
        /// </typeparam>
        /// <param name = "argumentsForConstructor">
        ///   The arguments to use in construction.
        /// </param>
        /// <returns>
        ///   The mocked object.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public static TMock Mock<TMock>(params object[] argumentsForConstructor)
            where TMock : class
        {
            return MockRepository.GenerateMock<TMock>(argumentsForConstructor);
        }

        /// <summary>
        /// Post-initialise.
        /// </summary>
        protected virtual void PostInitialise()
        {
        }

        /// <summary>
        /// Pre-initialise.
        /// </summary>
        protected virtual void PreInitialise()
        {
        }

        /// <summary>
        ///   Creates a stub of the specified type.
        /// </summary>
        /// <remarks>
        ///   A stub is very similar to a mock except for:
        ///   1) Stubs do not have verifyable behaviour (you cannot perform assertions on them)
        ///   2) Stubs automatically behave as though all properties are auto-properties
        /// 
        ///   Generally the rule is: use stubs for data passed into a test method, use mocks for classes
        ///   with logic/behaviour.
        /// </remarks>
        /// <typeparam name = "TStub">
        ///   The type of stub to retrieve.
        /// </typeparam>
        /// <returns>
        ///   The stubbed object.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public static TStub Stub<TStub>()
            where TStub : class
        {
            var result = MockRepository.GenerateStub<TStub>();
            return result;
        }

        /// <summary>
        ///   Creates a stub of the specified type.
        /// </summary>
        /// <remarks>
        ///   A stub is very similar to a mock except for:
        ///   1) Stubs do not have verifiable behaviour (you cannot perform assertions on them)
        ///   2) Stubs automatically behave as though all properties are auto-properties
        /// 
        ///   Generally the rule is: use stubs for data passed into a test method, use mocks for classes
        ///   with logic/behaviour.
        /// </remarks>
        /// <typeparam name = "TStub">
        ///   The type of stub to retrieve.
        /// </typeparam>
        /// <param name="setup">
        ///   Setup actions to perform (primarily to allow initialiser-like nested syntax).
        /// </param>
        /// <returns>
        ///   The stubbed object.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        protected TStub Stub<TStub>(Action<TStub> setup)
            where TStub : class
        {
            if (setup == null) throw new ArgumentNullException("setup");

            var result = MockRepository.GenerateStub<TStub>();
            setup(result);

            return result;
        }
        #endregion Methods

        #region Nested Types

        /// <summary>
        ///   Provides a nice means to specify injectable dependencies for tests.
        /// </summary>
        protected class DependencySpecifier
        {
            #region Fields

            /// <summary>
            ///   The dependencies.
            /// </summary>
            private readonly IDictionary<Type, object> _dependencies = new Dictionary<Type, object>();

            #endregion Fields

            #region Properties

            /// <summary>
            ///   The dependencies.
            /// </summary>
            public IEnumerable<KeyValuePair<Type, object>> Dependencies
            {
                get { return _dependencies; }
            }

            #endregion Properties

            #region Methods

            /// <summary>
            ///   Adds a dependency.
            /// </summary>
            /// <typeparam name = "TDependency">
            ///   The type of the dependency.
            /// </typeparam>
            /// <param name = "dependency">
            ///   The dependency to add.
            /// </param>
            public void AddDependency<TDependency>(TDependency dependency)
            {
                AddDependency(
                    typeof(TDependency),
                    dependency);
            }

            /// <summary>
            ///   Adds a dependency.
            /// </summary>
            /// <param name = "dependencyType">
            ///   The type of the dependency.
            /// </param>
            /// <param name = "dependency">
            ///   The dependency to add.
            /// </param>
            public void AddDependency(
                Type dependencyType,
                object dependency)
            {
                _dependencies.Add(
                    dependencyType,
                    dependency);
            }

            #endregion Methods
        }

        #endregion Nested Types
    }

    /// <summary>
    ///   Base class for typed tests.
    /// </summary>
    /// <typeparam name = "T">
    ///   The type of class being tested.
    /// </typeparam>
    public abstract class BaseTest<T> : BaseTest
        where T : class
    {
        #region Fields

        /// <summary>
        ///   The auto-mocking container used to support the tests.
        /// </summary>
        private RhinoAutoMocker<T> autoMocker = new RhinoAutoMocker<T>(MockMode.AAA);

        #endregion Fields

        #region Properties

        /// <summary>
        ///   The instance being tested.
        /// </summary>
        protected T TestInstance
        {
            get;
            private set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Cleans up after a test.
        /// </summary>
        [TestCleanup]
        public override void CleanUp()
        {
            base.CleanUp();
            autoMocker = null;
        }

        /// <summary>
        ///   Retrieves the dependency initialised for the test.
        /// </summary>
        /// <remarks>
        ///   This method can potentially be used to retrieved non-dependency mock object;
        ///   this is strongly discouraged and <see cref = "BaseTest.Mock{TMock}" /> should be used instead for
        ///   this purpose.
        /// </remarks>
        /// <typeparam name = "TDependency">
        ///   The type of dependency to retrieve.
        /// </typeparam>
        /// <returns>
        ///   The retrieved dependency.
        /// </returns>
        protected TDependency Get<TDependency>()
            where TDependency : class
        {
            return autoMocker.Get<TDependency>();
        }

        /// <summary>
        /// Gets the container to use for the tests.
        /// </summary>
        /// <returns>
        /// The IOC container to use.
        /// </returns>
        protected override IContainer GetIoCContainer()
        {
            return (autoMocker != null) ? autoMocker.Container : null;
        }

        /// <summary>
        /// Post-initialise.
        /// </summary>
        protected override void PostInitialise()
        {
            TestInstance = autoMocker.ClassUnderTest;
        }

        ///// <summary>
        ///// Pre-initialise
        ///// </summary>
        //protected override void PreInitialise()
        //{
        //    autoMocker = new RhinoAutoMocker<T>(MockMode.AAA);
        //}

        #endregion Methods
    }
}
