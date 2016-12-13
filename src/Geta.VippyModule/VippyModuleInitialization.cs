using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using StructureMap;

namespace Geta.VippyModule
{
    [ModuleDependency(typeof(ServiceContainerInitialization))]
    public class VippyModuleInitialization : IConfigurableModule
    {
        public void Initialize(InitializationEngine context)
        {
        }

        public void Uninitialize(InitializationEngine context)
        {
        }

        public void Preload(string[] parameters)
        {
        }

        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            context.Container.Configure(ConfigureContainer);
        }

        private static void ConfigureContainer(ConfigurationExpression container)
        {
            container.For<VippyWrapper.VippyWrapper>().Use<VippyWrapper.VippyWrapper>()
                .Ctor<string>("apiKey").Is(VippyConfiguration.ApiKey)
                .Ctor<string>("secretKey").Is(VippyConfiguration.SecretKey);
        }
    }
}