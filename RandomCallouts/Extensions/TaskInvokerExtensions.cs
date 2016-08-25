using System.Reflection;
using Rage;

namespace RandomCallouts.Extensions
{
    public static class TaskInvokerExtensions
    {
        public static Ped GetInstancePed(this TaskInvoker taskInvoker)
        {
            PropertyInfo p = taskInvoker.GetType().GetProperty("Ped", BindingFlags.NonPublic | BindingFlags.Instance);
            if (p != null)
            {
                Ped instancePed = (Ped)p.GetMethod.Invoke(taskInvoker, null);
                return instancePed;
            }
            return null;
        }
    }
}
