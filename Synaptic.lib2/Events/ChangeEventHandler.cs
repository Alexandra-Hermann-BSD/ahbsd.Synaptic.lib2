using System;

namespace ahbsd.Synaptic.lib2.Events
{
    /// <summary>
    /// Event handler for change events.
    /// </summary>
    /// <param name="sender">The sending object</param>
    /// <param name="e">The change event arguments</param>
    /// <typeparam name="T">The <see cref="Type"/> of the changing Value</typeparam>
    public delegate void ChangeEventHandler<T>(object sender, ChangeEventArgs<T> e);
}