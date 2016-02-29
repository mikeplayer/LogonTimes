using System.Reflection;

namespace LogonTimes
{
    public partial class LogonTimesDataContext
    {
        public void RefreshCache()
        {
            const BindingFlags FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var method = GetType().GetMethod("ClearCache", FLAGS);
            method.Invoke(this, null);
        }
    }
}
