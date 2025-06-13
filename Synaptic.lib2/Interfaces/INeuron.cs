using System.Collections.Generic;
using ahbsd.Synaptic.lib2.Events;

namespace ahbsd.Synaptic.lib2.Interfaces
{
    /// <summary>
    /// Interface of a neuron.
    /// </summary>
    public interface INeuron : ISynapticObject
    {
        /// <summary>
        /// Gets the dendrites of the neuron.
        /// </summary>
        /// <value>The dendrites of the neuron</value>
        IReadOnlyList<IDendrit> Dendrites { get; }

        /// <summary>
        /// Gets the axon of the neuron.
        /// </summary>
        /// <value>The axon of the neuron</value>
        IAxon Axon { get; }

        /// <summary>
        /// Gets the signal buffer of the neuron.
        /// </summary>
        /// <value>The signal buffer of the neuron</value>
        double SignalBuffer { get; }

        /// <summary>
        /// Happens, when the signal buffer of the neuron changes.
        /// </summary>
        event ChangeEventHandler<double> SignalBufferChanged;

        /// <summary>
        /// Happens, if the <see cref="Axon.HillSize"/> has changed.
        /// </summary>
        event ChangeEventHandler<double> AxonHillSizeChanged;

        /// <summary>
        /// Connects the neuron to an axon terminal, allowing it to receive signals.
        /// </summary>
        /// <param name="terminal">The given axon terminal</param>
        /// <exception cref="System.ArgumentNullException">If the given <paramref name="terminal"/> is <c>null</c></exception>
        void ConnectTo(IAxonTerminal terminal);

        /// <summary>
        /// Adds a dendrit to the neuron.
        /// </summary>
        /// <param name="dendrit">The dendrit to add</param>
        /// <returns>Was it successful to add the given dendrit?</returns>
        bool AddDendrit(IDendrit dendrit);

        /// <summary>
        /// Gets the axon terminals of the neuron.
        /// </summary>
        /// <value>The axon terminals of the neuron</value>
        IReadOnlyList<IAxonTerminal> AxonTerminals { get; }

        /// <summary>
        /// Gets the default axon terminal of the neuron.
        /// </summary>
        /// <value>The default axon terminal of the neuron</value>
        IAxonTerminal DefaultAxonTerminal { get; }
    }
}