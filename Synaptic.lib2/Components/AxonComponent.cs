using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using ahbsd.Synaptic.lib2.Events;
using ahbsd.Synaptic.lib2.Interfaces;
using Serilog;

namespace ahbsd.Synaptic.lib2.Components
{
    /// <summary>
    /// Component of an Axon, which is a part of a neuron responsible for transmitting signals.
    /// </summary>
    public partial class AxonComponent : Component, IAxon
    {

        private readonly IAxon _axon;

        /// <summary>
        /// Default constructor for an AxonComponent.
        /// </summary>
        public AxonComponent()
        {
            _axon = new Axon(null);
            InitializeComponent();
        }

        /// <summary>
        /// Constructor for an AxonComponent that takes an IContainer.
        /// </summary>
        /// <param name="container">The given parent container</param>
        public AxonComponent(IContainer container)
        {
            container.Add(this);
            _axon = new Axon(null);

            InitializeComponent();
        }

        private void OnNeuronChanged(object sender, ChangeEventArgs<INeuron> e)
        {
            if (e.NewValue != null)
            {
                if (e.OldValue is Neuron neuron)
                {
                    foreach (var dendrit in neuron.Dendrites)
                    {
                        dendrit.Dispose();
                    }
                }

                Name = Axon.GetAxonName(e.NewValue, this);
                Axon.AddNewTerminal($"Default Terminal of {e.NewValue.Name}'s Axon", this);
            }
        }

        /// <inheritdoc />
        public void GetObjectData(SerializationInfo info, StreamingContext context)
            => _axon.GetObjectData(info, context);

        /// <inheritdoc />
        public bool Equals(ISynapticObject x, ISynapticObject y)
            => _axon.Equals(x, y);

        /// <inheritdoc />
        public int GetHashCode(ISynapticObject obj)
            => _axon.GetHashCode(obj);

        /// <inheritdoc />
        public ILogger Logger => _axon.Logger;

        /// <inheritdoc />
        public Guid Id => _axon.Id;

        /// <inheritdoc />
        public string Name
        {
            get => _axon.Name;
            protected internal set
            {
                if (_axon is Axon axon)
                {
                    axon.Name = value;
                }
            }
        }

        /// <inheritdoc />
        public ulong Number => _axon.Number;

        /// <inheritdoc />
        public bool InputSignal(double signalStrength) => _axon.InputSignal(signalStrength);

        /// <inheritdoc />
        public double HillSize => _axon.HillSize;

        /// <inheritdoc />
        public event EventHandler<SendEventArgs> SignalFired
        {
            add => _axon.SignalFired += value;
            remove => _axon.SignalFired -= value;
        }

        /// <inheritdoc />
        public event ChangeEventHandler<double> HillSizeChanged
        {
            add => _axon.HillSizeChanged += value;
            remove => _axon.HillSizeChanged -= value;
        }

        /// <inheritdoc />
        public IReadOnlyList<IAxonTerminal> Terminals => _axon.Terminals;

        /// <inheritdoc />
        public event EventHandler<SendEventArgs> AxonFired
        {
            add => _axon.AxonFired += value;
            remove => _axon.AxonFired -= value;
        }

        /// <inheritdoc />
        public IAxonTerminal AddNewTerminal() => _axon.AddNewTerminal();

        /// <summary>
        /// Happens, when the neuron of the axon changes.
        /// </summary>
        public event ChangeEventHandler<INeuron> NeuronChanged;

        /// <inheritdoc />
        public INeuron Neuron
        {
            get => _axon.Neuron;
            set
            {
                if (_axon is Axon axon && _axon.Neuron != value)
                {
                    var e = new ChangeEventArgs<INeuron>(_axon.Neuron, value);
                    axon.Neuron = value;
                    NeuronChanged?.Invoke(this, e);
                }
            }
        }
    }
}