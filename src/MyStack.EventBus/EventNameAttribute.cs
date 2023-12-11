using System;

namespace Microsoft.Extensions.EventBus
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EventNameAttribute : Attribute
    {
        public string Name { get; private set; }
        public EventNameAttribute(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name", "The name of event cannot be empty or null");
            Name = name;
        }
    }
}
