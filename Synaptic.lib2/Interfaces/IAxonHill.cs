using System;
using ahbsd.Synaptic.lib2.Events;

namespace ahbsd.Synaptic.lib2.Interfaces
{
    /// <summary>
    /// Interface for an axon hill, which is the part of a neuron where the axon begins and where signals are integrated before being sent down the axon.
    /// </summary>
    public interface IAxonHill : ISynapticObject
    {
        /// <summary>
        /// Input of a single signal.
        /// </summary>
        /// <param name="signalStrength">The input level strength</param>
        /// <returns>Was the input level strength high enough to fire?</returns>
        bool InputSignal(double signalStrength);

        /// <summary>
        /// Gets the size of the current Axon hill.
        /// </summary>
        /// <value>The size of the current Axon hill</value>
        double HillSize { get; }

        /// <summary>
        /// Happens, when aa signal was fired.
        /// </summary>
        event EventHandler<SendEventArgs> SignalFired;

        /// <summary>
        /// Happens, when the <see cref="HillSize"/> has changed.
        /// </summary>
        event ChangeEventHandler<double> HillSizeChanged;

    }
}