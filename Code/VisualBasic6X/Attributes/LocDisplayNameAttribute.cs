using System;
using System.ComponentModel;

namespace VisualBasic6.Attributes
{
    /// <summary>
    /// Specifies the display name for a property, event, 
    /// or public void method which takes no arguments.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    sealed class LocDisplayNameAttribute : DisplayNameAttribute
    {
        readonly string name;

        /// <summary>
        /// Public constructor.
        /// </summary>
        /// <param name="name">Attribute display name.</param>
        public LocDisplayNameAttribute(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Gets attribute display name.
        /// </summary>
        public override string DisplayName
        {
            get
            {
                return Resources.GetString(name) ?? name;
            }
        }
    }
}