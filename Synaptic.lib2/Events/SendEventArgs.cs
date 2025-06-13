using System;
using ahbsd.Synaptic.lib2.Interfaces;
using Serilog;

namespace ahbsd.Synaptic.lib2.Events
{
    /// <summary>
    /// Event arguments for a send signal.
    /// </summary>
    public class SendEventArgs : EventArgs
    {
        private readonly ILogger _logger = Log.ForContext<SendEventArgs>();

        /// <summary>
        /// Default constructor initializing the signal strength to 0.
        /// </summary>
        public SendEventArgs()
        {
            Sender = null;
            SignalStrength = 0d;
            _logger.Verbose("[{Name}] created with default values; SignalStrength: {SignalStrength}, no Sender", Name, SignalStrength);
        }

        /// <summary>
        /// Constructor with a given signal strength.
        /// </summary>
        /// <param name="signalStrength">The given signal strength</param>
        public SendEventArgs(double signalStrength)
        {
            Sender = null;
            SignalStrength = signalStrength;
            _logger.Verbose("[{Name}] created with given SignalStrength; SignalStrength: {SignalStrength}, no sender", Name, SignalStrength);
        }

        /// <summary>
        /// Constructor with a given signal strength and sender.
        /// </summary>
        /// <param name="signalStrength">The given signal strength</param>
        /// <param name="sender">The given sender</param>
        public SendEventArgs(double signalStrength, ISynapticObject sender)
        {
            Sender = sender;
            SignalStrength = signalStrength;
            _logger.Verbose("[{Name}] created with given values; SignalStrength: {SignalStrength}, Sender: {Sender}", Name, SignalStrength, Sender);
        }

        /// <summary>
        /// Gets the unique identifier of this event handler.
        /// </summary>
        /// <value>The unique identifier of this event handler</value>
        public Guid Id { get; } = Guid.NewGuid();

        /// <summary>
        /// Gets the name of this <see cref="EventArgs"/>.
        /// </summary>
        /// <value>The name of this event args</value>
        public string Name => $"SendEventArgs_{Id}";

        /// <summary>
        /// Gets the signal strength.
        /// </summary>
        /// <value>The signal strength</value>
        public double SignalStrength { get; }

        /// <summary>
        /// Gets the sender of the signal.
        /// </summary>
        /// <value>The sender of the signal</value>
        public ISynapticObject Sender { get; }

        /// <inheritdoc />
        public override string ToString() =>
            $"SendEventArgs: SignalStrength = {SignalStrength}, Sender = {Sender?.Name ?? "null"}; Id = {Id}";
    }
}