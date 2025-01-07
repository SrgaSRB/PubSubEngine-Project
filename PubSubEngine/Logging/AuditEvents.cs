using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Logging
{
    public class AuditEvents
    {
        private static ResourceManager resourceManager = null;
        private static object resourceLock = new object();

        private static ResourceManager ResourceManager
        {
            get
            {
                if (resourceManager == null)
                {
                    lock (resourceLock)
                    {
                        if (resourceManager == null)
                        {
                            resourceManager = new ResourceManager(typeof(AuditEventFile).ToString(), Assembly.GetExecutingAssembly());
                        }
                    }
                }
                return resourceManager;
            }
        }
        public static string LogDatabaseEntry(string timestamp, string databaseName, string entityId, string digitalSignature, string publicKey)
        {
            return string.Format(ResourceManager.GetString("LogSuccess"), timestamp, databaseName, entityId, digitalSignature, publicKey);
        }

    }
}
