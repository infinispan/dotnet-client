namespace Infinispan.Hotrod
{
    internal interface ILogHandler
    {
        public void Log(LogLevel type, string message);
    }
}
