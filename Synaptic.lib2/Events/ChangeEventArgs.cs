using System;

namespace ahbsd.Synaptic.lib2.Events
{
    /// <summary>
    /// Event arguments for change events.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of the changing Value</typeparam>
    public class ChangeEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Constructor with a given old and new value.
        /// </summary>
        /// <param name="oldValue">The given old value</param>
        /// <param name="newValue">The given new value</param>
        /// <param name="info">[optional] Additional info about the change</param>
        public ChangeEventArgs(T oldValue, T newValue, string info = null)
        {
            OldValue = oldValue;
            NewValue = newValue;
            Info = info;
        }

        /// <summary>
        /// Gets the old value.
        /// </summary>
        /// <value>The old value</value>
        public T OldValue { get; }

        /// <summary>
        /// Gets the new value.
        /// </summary>
        /// <value>The new value</value>
        public T NewValue { get; }

        /// <summary>
        /// Gets additional information about the change.
        /// </summary>
        /// <value>Additional information about the change</value>
        /// <returns>May be <c>null</c></returns>
        public string Info { get; }
    }
}