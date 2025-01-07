using Logging;
using System;
using System.Diagnostics;

namespace SecurityManager
{
    public class Audit : IDisposable
    {
        private static EventLog customLog = null;
        const string SourceName = "Database.EntityInsert";
        const string LogName = "PubSubEngineLog";

        static Audit()
        {
            try
            {
                if (!EventLog.SourceExists(SourceName))
                {
                    EventLog.CreateEventSource(SourceName, LogName);
                }
                customLog = new EventLog(LogName, Environment.MachineName, SourceName);
            }
            catch (Exception e)
            {
                customLog = null;
                Console.WriteLine("Error while trying to create log handle. Error = {0}", e.Message);
            }
        }

        public static void DataInserted(string timestamp, string databaseName, string entityId, string digitalSignature, string publicKey)
        {
            if (customLog != null)
            {
                string message = AuditEvents.LogDatabaseEntry(timestamp, databaseName, entityId, digitalSignature, publicKey);
                customLog.WriteEntry(message, EventLogEntryType.Information);
            }
            else
            {
                throw new ArgumentException($"Error while trying to write event to event log.");
            }
        }

        public void Dispose()
        {
            if (customLog != null)
            {
                customLog.Dispose();
                customLog = null;
            }
        }
    }
}
