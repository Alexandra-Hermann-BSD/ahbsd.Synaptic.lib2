using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ahbsd.Synaptic.lib2.Events;
using ahbsd.Synaptic.lib2.Interfaces;
using Serilog;

namespace ahbsd.Synaptic.lib2
{
    /// <summary>
    /// Class representing an axon terminal, which is one of the end points of an axon that can fire signals.
    /// </summary>
    public partial class Axon : SynapticObject, IAxon
    {
        private readonly IAxonHill _hill;
        private static readonly IDictionary<IAxon, IList<IAxonTerminal>> AxonTerminals = new Dictionary<IAxon, IList<IAxonTerminal>>();

        /// <summary>
        /// The size that is added to the hill size of the axon each time a signal is sent.
        /// </summary>
        protected internal const double HillSizeAdd = 0.0001;

        /// <summary>
        /// Default constructor for creating an axon instance.
        /// </summary>
        /// <param name="neuron">The neuron this axon belongs to</param>
        internal Axon(INeuron neuron)
        {
            if (neuron != null) Name = GetAxonName(neuron, this);

            if (!AxonTerminals.ContainsKey(this))
            {
                AxonTerminals.Add(this, new List<IAxonTerminal>());
            }

            Logger = Log.ForContext<Axon>();
            Neuron = neuron;
            _hill = new AxonHill(this);
            _hill.HillSizeChanged += Hill_OnHillSizeChanged;
            Logger.Verbose("[{Name}] Axon created; ID: {Id}", Name, Id);
            if (Neuron != null)
            {
                AddNewTerminal($"Default Terminal of {neuron.Name}'s Axon", this);
            }

            SignalFired += OnSignalFired;
        }

        private void OnSignalFired(object sender, SendEventArgs e)
        {
            AxonFired?.Invoke(this, e);
        }

        private void Hill_OnHillSizeChanged(object sender, ChangeEventArgs<double> e)
        {
            AxonFired?.Invoke(this, new SendEventArgs(e.NewValue, this));
        }

        internal static string GetAxonName(INeuron neuron, IAxon axon)
        {
            var nameBuilder = new StringBuilder("Axon-");
            var count = 0;

            if (AxonTerminals.TryGetValue(axon, out var terminals))
            {
                count = terminals.Count;
            }

            count++;
            nameBuilder.Append(count.ToString("0000"));
            nameBuilder.Append(" of ");
            nameBuilder.Append(neuron.Name);
            return nameBuilder.ToString();
        }

        #region implementation of IAxon

        /// <inheritdoc />
        public IReadOnlyList<IAxonTerminal> Terminals => (IReadOnlyList<IAxonTerminal>) AxonTerminals[this];

        /// <inheritdoc />
        public event EventHandler<SendEventArgs> AxonFired;

        /// <inheritdoc />
        public IAxonTerminal AddNewTerminal()
        {
            var terminal = new AxonTerminal(this);
            AxonTerminals[this].Add(terminal);
            Logger.Debug("[{Name}] Adding new terminal {TerminalName}", Name,terminal.Name);
            return terminal;
        }

        /// <inheritdoc />
        public INeuron Neuron { get; internal set; }

        #endregion

        #region implementation of IAxonHill
        /// <inheritdoc />
        public bool InputSignal(double signalStrength) => _hill.InputSignal(signalStrength);

        /// <inheritdoc />
        public double HillSize => _hill.HillSize;

        /// <inheritdoc />
        public event EventHandler<SendEventArgs> SignalFired
        {
            add => _hill.SignalFired += value;
            remove => _hill.SignalFired -= value;
        }

        /// <inheritdoc />
        public event ChangeEventHandler<double> HillSizeChanged
        {
            add => _hill.HillSizeChanged += value;
            remove => _hill.HillSizeChanged -= value;
        }
        #endregion

        /// <summary>
        /// Adds a new terminal to the axon with a specified name.
        /// </summary>
        /// <param name="name">The given name</param>
        /// <param name="axon">The given Axon</param>
        internal static void AddNewTerminal(string name, IAxon axon)
        {
            var terminal = new AxonTerminal(axon, name);
            AxonTerminals[axon].Add(terminal);
        }

        /// <inheritdoc />
        protected override void ReleaseUnmanagedResources()
        {
            if (AxonTerminals != null && AxonTerminals.ContainsKey(this))
            {
                foreach (var terminal in AxonTerminals[this].Where(t => t != null))
                {
                    try
                    {
                        terminal.Dispose();
                    }
                    catch (Exception e)
                    {
                        Logger?.Error(e, "[{Name}] Error while disposing terminal {TerminalName}: {Exception}", Name, terminal.Name, e);
                    }
                }
                AxonTerminals.Remove(this);
            }

            _hill?.Dispose();
            Logger?.Debug("[{Name}] Unmanaged resources released", Name);
        }

        /// <summary>
        /// Deconstructor for the Axon class, which releases unmanaged resources.
        /// </summary>
        ~Axon()
        {
            ReleaseUnmanagedResources();
        }
    }
}