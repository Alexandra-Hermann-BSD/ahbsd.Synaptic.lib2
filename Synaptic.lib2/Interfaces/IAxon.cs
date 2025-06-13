using System;
using System.Collections.Generic;
using ahbsd.Synaptic.lib2.Events;

namespace ahbsd.Synaptic.lib2.Interfaces
{
    /// <summary>
    /// Interface of an axon.
    /// </summary>
    public interface IAxon : IAxonHill
    {
        /// <summary>
        /// Gets the terminals of the axon.
        /// </summary>
        /// <value>The terminals of the axon</value>
        IReadOnlyList<IAxonTerminal> Terminals { get; }

        /// <summary>
        /// Happens, when a signal was sent through the axon.
        /// </summary>
        event EventHandler<SendEventArgs> AxonFired;

        /// <summary>
        /// Adds a new axon terminal to the axon, which can be used to send signals.
        /// </summary>
        /// <returns>The new created axon terminal</returns>
        IAxonTerminal AddNewTerminal();

        /// <summary>
        /// Gets the neuron associated with this axon.
        /// </summary>
        /// <value>The neuron associated with this axon</value>
        INeuron Neuron { get; }
    }
}