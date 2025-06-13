using System;
using System.Runtime.Serialization;
using ahbsd.Synaptic.lib2.Events;
using ahbsd.Synaptic.lib2.Interfaces;
using Serilog;

namespace ahbsd.Synaptic.lib2
{
    /// <summary>
    /// Class representing a dendrit in a neural network.
    /// </summary>
    public class Dendrit : SynapticObject, IDendrit
    {
        private IAxonTerminal _connectedAxonTerminal;

        /// <inheritdoc />
        protected Dendrit(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _connectedAxonTerminal = info?.GetValue(nameof(_connectedAxonTerminal), typeof(IAxonTerminal)) as IAxonTerminal;
            Neuron = info?.GetValue(nameof(Neuron), typeof(INeuron)) as INeuron;
        }

        /// <summary>
        /// Default constructor that initializes a dendrit without a neuron.
        /// </summary>
        public Dendrit()
        {
            Logger = Log.ForContext<Dendrit>();
            Neuron = null;
            Logger.Verbose("[{Name}] created without a neuron", Name);
        }

        /// <summary>
        /// Constructor with a given neuron.
        /// </summary>
        /// <param name="neuron">The given neuron</param>
        public Dendrit(INeuron neuron) : base($"Dendrit of {neuron.Name}")
        {
            Logger = Log.ForContext<Dendrit>();
            Neuron = neuron;
            Logger.Verbose("[{Name}] created with a given neuron ({Neuron})", Name, Neuron.Name);
        }

        #region implementation of IDendrit

        /// <inheritdoc />
        public event EventHandler<SendEventArgs> InputSignal;

        /// <inheritdoc />
        public bool ReceiveSignal(SendEventArgs signal)
        {
            Logger.Information("[{Name}] Received signal from {Sender} with strength {SignalStrength}",
                Name, signal.Sender?.Name ?? "unknown", signal.SignalStrength);
            InputSignal?.Invoke(this, signal);
            return InputSignal != null;
        }

        /// <inheritdoc />
        public INeuron Neuron { get; internal set; }

        /// <inheritdoc />
        public IAxonTerminal ConnectedAxonTerminal
        {
            get => _connectedAxonTerminal;
            internal set
            {
                ChangeEventArgs<IAxonTerminal> e = null;
                if (value != _connectedAxonTerminal && value?.Axon.Neuron != Neuron)
                {
                    e = new ChangeEventArgs<IAxonTerminal>(_connectedAxonTerminal, value);
                    _connectedAxonTerminal = value;
                    Logger.Debug("[{Name}] connected to axon terminal {AxonTerminal} of neuron {Neuron}",
                        Name, value.Name, Neuron.Name);
                }
                else if (Neuron != null && Neuron == value?.Axon.Neuron)
                {
                    Logger.Debug("[{Name}] Attempted to connect to axon terminal of the same neuron {Neuron}; Connection ignored",
                        Name, Neuron.Name);
                }

                if (e != null)
                {
                    ConnectedAxonTerminalChanged?.Invoke(this, e);
                }
                else
                {
                    var message = $"Cannot connect dendrit {Name} to axon terminal {value?.Name ?? "null"} of neuron {Neuron?.Name}";
                    Logger.Debug("[{Name}] {Message}", Name, message);
                    throw new ArgumentException(message, nameof(value));
                }
            }
        }

        /// <inheritdoc />
        public event ChangeEventHandler<IAxonTerminal> ConnectedAxonTerminalChanged;

        #endregion

        /// <inheritdoc />
        protected override void ReleaseUnmanagedResources()
        {
            _connectedAxonTerminal = null;
            Logger?.Verbose("[{Name}] released", Name);
        }

        /// <inheritdoc />
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(_connectedAxonTerminal), _connectedAxonTerminal);
            info.AddValue(nameof(Neuron), Neuron);
        }
    }
}