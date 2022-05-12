using System.Collections.Generic;

namespace BehaviourTree
{
    public partial class Blackboard
    {
        private Dictionary<string, object> blackboardData = new Dictionary<string, object>();

        public void SetValue<T>(string key, T value)
        {
            blackboardData[key] = value;
        }

        public bool GetValue<T>(string key, out T outValue)
        {
            if(blackboardData.ContainsKey(key))
            {
                object value = blackboardData[key];
                System.Diagnostics.Debug.Assert(value.GetType() == typeof(T), $"Inconsisten blackboard value type. Expected {typeof(T).Name}, but found {value.GetType().Name}");
                outValue = (T)value;
                return true;
            }
            outValue = default(T);
            return false;
        }

        public T GetValue<T>(string key)
        {
            T value;
            GetValue(key, out value);
            return value;
        }
    }
}
