using System;
using ahbsd.Synaptic.lib2.Interfaces;

namespace ahbsd.Synaptic.lib2.Events
{
    /// <summary>
    /// Event argument for an added <see cref="ISynapticObject"/>
    /// </summary>
    public class AddedEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor with a given <see cref="ISynapticObject"/>.
        /// </summary>
        /// <param name="synapticObject">The given synaptic object</param>
        public AddedEventArgs(ISynapticObject synapticObject) => SynapticObject = synapticObject;

        /// <summary>
        /// Gets the <see cref="ISynapticObject"/> that was added.
        /// </summary>
        /// <value>The synaptic object</value>
        public ISynapticObject SynapticObject { get; }
    }
}