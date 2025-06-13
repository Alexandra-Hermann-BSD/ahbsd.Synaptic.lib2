using System;
using System.Collections.Generic;

namespace ahbsd.Synaptic.lib2.Interfaces
{
    /// <summary>
    /// Interface for an axon terminal, which is one of the end points of an axon that can fire signals.
    /// </summary>
    public interface IAxonTerminal : ISynapticObject
    {
        /// <summary>
        /// Gets the axon associated with this terminal.
        /// </summary>
        /// <value>The axon associated with this terminal</value>
        IAxon Axon { get; }

        /// <summary>
        /// Fires a signal through the axon terminal.
        /// </summary>
        /// <returns>Did the signal reached at least one receiver?</returns>
        bool FireSignal();

        /// <summary>
        /// Happens, when a signal was send by the axon terminal.
        /// </summary>
        event Action<IAxonTerminal> OnSignal;

        /// <summary>
        /// Gets the connected dendrites of this axon terminal.
        /// </summary>
        /// <value>The connected dendrites of this axon terminal</value>
        IReadOnlyList<IDendrit> ConnectedDendrites { get; }

        /// <summary>
        /// Adds a dendrit to the axon terminal, allowing it to receive signals.
        /// </summary>
        /// <param name="dendrit">The given dendrit</param>
        /// <returns>Was it successful to add or was it already added</returns>
        /// <exception cref="ArgumentNullException">If the given <paramref name="dendrit"/> is <c>null</c></exception>
        bool AddDendrit(IDendrit dendrit);
    }
}