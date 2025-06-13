using System;
using ahbsd.Synaptic.lib2.Events;

namespace ahbsd.Synaptic.lib2.Interfaces
{
    /// <summary>
    /// Interface of a dendrit.
    /// </summary>
    public interface IDendrit : ISynapticObject
    {
        /// <summary>
        /// Happens, when a signal is received by the dendrit.
        /// </summary>
        event EventHandler<SendEventArgs> InputSignal;

        /// <summary>
        /// Receives a signal from an axon terminal.
        /// </summary>
        /// <param name="signal">The received signal</param>
        /// <returns>Was the receiver available?</returns>
        bool ReceiveSignal(SendEventArgs signal);

        /// <summary>
        /// Gets the neuron associated with this dendrit.
        /// </summary>
        /// <value>The neuron associated with this dendrit</value>
        INeuron Neuron { get; }

        /// <summary>
        /// Gets the connected axon terminal.
        /// </summary>
        /// <value>The connected axon terminal</value>
        IAxonTerminal ConnectedAxonTerminal { get; }

        /// <summary>
        /// Happens, when the connected axon terminal has changes.
        /// </summary>
        event ChangeEventHandler<IAxonTerminal> ConnectedAxonTerminalChanged;
    }
}