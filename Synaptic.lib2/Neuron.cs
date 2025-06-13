using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ahbsd.Synaptic.lib2.Events;
using ahbsd.Synaptic.lib2.Interfaces;
using Serilog;

namespace ahbsd.Synaptic.lib2
{
    /// <summary>
    /// Class representing a neuron, which is a fundamental unit of the nervous system
    /// </summary>
    [Serializable]
    public class Neuron : SynapticObject, INeuron
    {
        private readonly IList<IDendrit> _dendrites;
        private double _signalBuffer;
        private bool _firing;

        /// <inheritdoc />
        protected Neuron(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            if (info != null)
            {
                _dendrites = (IList<IDendrit>)info.GetValue(nameof(_dendrites), typeof(IList<IDendrit>));
                _signalBuffer = info.GetDouble(nameof(_signalBuffer));
                Axon = info.GetValue(nameof(Axon), typeof(IAxon)) as IAxon;

                if (Axon == null)
                    throw new InvalidOperationException("Axon cannot be null. Ensure it is initialized properly.");
            }
            else
            {
                _dendrites = new List<IDendrit>();
                _signalBuffer = 0d;
                Axon = new Axon(this);
            }

            if (Axon != null) Axon.HillSizeChanged += Axon_OnHillSizeChanged;
            Logger = Log.ForContext<Neuron>();
            SignalBufferChanged += OnSignalBufferChanged;
            Logger.Verbose("[{Name}] deserialized Neuron with ID {Id} and SignalBuffer {SignalBuffer}", Name, Id, _signalBuffer);
        }

        /// <summary>
        /// Default constructor for a Neuron.
        /// </summary>
        public Neuron()
        {
            _dendrites = new List<IDendrit>();
            _signalBuffer = 0d;
            Axon = new Axon(this);
            Axon.HillSizeChanged += Axon_OnHillSizeChanged;
            Logger = Log.ForContext<Neuron>();
            SignalBufferChanged += OnSignalBufferChanged;
            Logger.Verbose("[{Name}] created Neuron with ID {Id}", Name, Id);
        }

        /// <summary>
        /// Constructor with a given name.
        /// </summary>
        /// <param name="name">The given name</param>
        public Neuron(string name) : base(name)
        {
            _dendrites = new List<IDendrit>();
            _signalBuffer = 0d;
            Axon = new Axon(this);
            Axon.HillSizeChanged += Axon_OnHillSizeChanged;
            Logger = Log.ForContext<Neuron>();
            SignalBufferChanged += OnSignalBufferChanged;
            Logger.Verbose("[{Name}] created Neuron with ID {Id} and Name {NeuronName}", Name, Id, name);
        }

        private void Axon_OnHillSizeChanged(object sender, ChangeEventArgs<double> e) => AxonHillSizeChanged?.Invoke(sender, e);

        private void Dendrit_OnInputSignal(object sender, SendEventArgs e)
        {
            if (e.Sender is IAxonTerminal)
            {
                SignalBuffer += e.SignalStrength;
            }
        }

        private void OnSignalBufferChanged(object sender, ChangeEventArgs<double> e)
        {
            if (Axon == null)
                throw new InvalidOperationException("Axon is not initialized. Cannot process signal buffer change.");

            if (!_firing)
            {
                _firing = true;
                if (Axon.InputSignal(e.NewValue))
                {
                    Logger.Information("[{Name}] fired with signal buffer {SignalBuffer}", Name, SignalBuffer);
                    _signalBuffer = 0d; // Reset signal buffer after firing
                }
                _firing = false;
            }
        }

        #region implementation of INeuron
        /// <inheritdoc />
        public IReadOnlyList<IDendrit> Dendrites => (IReadOnlyList<IDendrit>)_dendrites;

        /// <inheritdoc />
        public IAxon Axon { get; }

        /// <inheritdoc />
        public double SignalBuffer
        {
            get => _signalBuffer;
            protected internal set
            {
                if (Math.Abs(value - _signalBuffer) > 0.000001)
                {
                    var e = new ChangeEventArgs<double>(_signalBuffer, value);
                    _signalBuffer = value;
                    Logger.Verbose("[{Name}] SignalBuffer changed from {OldValue} to {NewValue}", Name, e.OldValue, e.NewValue);
                    SignalBufferChanged?.Invoke(this, e);
                }
            }
        }

        /// <inheritdoc />
        public event ChangeEventHandler<double> SignalBufferChanged;

        /// <inheritdoc />
        public event ChangeEventHandler<double> AxonHillSizeChanged;

        /// <inheritdoc />
        public void ConnectTo(IAxonTerminal terminal)
        {
            if (terminal == null)
                throw new ArgumentNullException(nameof(terminal), "The axon terminal cannot be null.");

            var dendrit = new Dendrit(this);

            Logger.Debug("[{Name}] Trying to connect to {Terminal}", Name, terminal.Name);
            if (AddDendrit(dendrit))
            {
                terminal.AddDendrit(dendrit);
            }
        }

        /// <inheritdoc />
        public bool AddDendrit(IDendrit dendrit)
        {
            Logger.Debug("[{Name}] Trying to add dendrit '{Dendrit}'", Name, dendrit);
            var result = false;

            if (dendrit is Dendrit d && !_dendrites.Contains(dendrit) && (d.Neuron == this as INeuron || d.Neuron == null))
            {
                Logger.Debug("[{Name}] Adding dendrit '{Dendrit}'", Name, dendrit.Name);
                dendrit.InputSignal += Dendrit_OnInputSignal;
                d.Neuron = this;
                _dendrites.Add(dendrit);
                result = true;
            }
            else if (_dendrites.Contains(dendrit))
            {
                Logger.Debug("[{Name}] Dendrit '{Dendrit}' already exists", Name, dendrit.Name);
                dendrit.InputSignal -= Dendrit_OnInputSignal;
                result = true;
                dendrit.InputSignal += Dendrit_OnInputSignal;
            }

            return result;
        }

        /// <inheritdoc />
        public IReadOnlyList<IAxonTerminal> AxonTerminals => Axon.Terminals;

        /// <inheritdoc />
        public IAxonTerminal DefaultAxonTerminal => AxonTerminals.FirstOrDefault();

        /// <inheritdoc />
        protected override void ReleaseUnmanagedResources()
        {
            base.ReleaseUnmanagedResources();
            foreach (var dendrit in _dendrites)
            {
                if (dendrit is Dendrit d)
                {
                    d.InputSignal -= Dendrit_OnInputSignal;
                    d.Neuron = null; // Clear the neuron reference
                    d.Dispose();
                }
            }
            _dendrites.Clear();
            Axon?.Dispose();
            Logger.Debug("[{Name}] Released unmanaged resources", Name);
        }

        /// <inheritdoc />
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(_signalBuffer), _signalBuffer);
            info.AddValue(nameof(Axon), Axon);
            info.AddValue(nameof(_dendrites), _dendrites);
        }

        #endregion
    }
}