using System.Configuration;

namespace Geta.VippyModule
{
    public static class VippyConfiguration
    {
        public static string ApiKey
        {
            get { return ConfigurationManager.AppSettings["Vippy:ApiKey"]; }
        }

        public static string SecretKey
        {
            get { return ConfigurationManager.AppSettings["Vippy:SecretKey"]; }
        }

        public static string ArchiveNumber
        {
            get { return ConfigurationManager.AppSettings["Vippy:ArchiveNumber"]; }
        }
    }
}