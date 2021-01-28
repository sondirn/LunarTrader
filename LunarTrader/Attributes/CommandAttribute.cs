using System;
using System.Reflection;

namespace LunarTrader.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandAttribute : Attribute
    {
        public string Name;
        public string Help;

        public CommandAttribute(string name, string help)
        {
            Name = name;
            Help = help;
        }
    }
}