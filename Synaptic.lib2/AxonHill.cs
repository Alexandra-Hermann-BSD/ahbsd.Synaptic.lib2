using System;
using System.Linq;
using ahbsd.Synaptic.lib2.Events;
using ahbsd.Synaptic.lib2.Interfaces;
using Serilog;

namespace ahbsd.Synaptic.lib2
{
    /// <summary>
    /// Implementation of the axon hill, which is the part of the neuron where the axon begins.
    /// </summary>
    internal class AxonHill : SynapticObject, IAxonHill
    {
        private double _hillSize;
        private readonly IAxon _axon;

        /// <summary>
        /// Constructor with a given axon.
        /// </summary>
        /// <param name="axon">The given axon</param>
        /// <exception cref="ArgumentNullException">If the given <paramref name="axon"/> is <c>null</c></exception>
        internal AxonHill(IAxon axon) : base($"Axon Hill of {axon.Neuron.Name}")
        {
            _axon = axon ?? throw new ArgumentNullException(nameof(axon), "Axon cannot be null");
            _hillSize = 1d;
            Logger = Log.ForContext<AxonHill>();
            Logger.Verbose("[{Name}] AxonHill created; ID: {Id}", Name, Id);
        }

        #region implementation of IAxonHill
        /// <inheritdoc />
        public bool InputSignal(double signalStrength)
        {
            var fired = MaybeFire(signalStrength);

            if (fired)
            {
                SignalFired?.Invoke(this, new SendEventArgs(signalStrength, this));
            }

            return fired;
        }

        private bool MaybeFire(double signalStrength)
        {
            var fired = false;
            if (FireReason(signalStrength))
            {
                Logger.Debug("[{Name}] Try fire; ID: {Id}; Signal to {TerminalCount} terminals", Name, Id, _axon.Terminals.Count);
                fired = _axon.Terminals.Aggregate(fired, (current, terminal) => current | terminal.FireSignal());

                HillSize += Axon.HillSizeAdd;
                Logger.Debug("[{Name}] HillSize increased to {HillSize}", Name, HillSize);
            }

            Logger.Debug("[{Name}] Fired: {Fired}; Signal Strength: {SignalStrength}", Name, fired ? "Yes" : "No", signalStrength);
            return fired;
        }

        private bool FireReason(double signalStrength)
        {
            return (10 - HillSize) > (10 - signalStrength);
        }

        /// <inheritdoc />
        public double HillSize
        {
            get => _hillSize;
            private set
            {
                if (Math.Abs(value - _hillSize) > 0.00001)
                {
                    var e = new ChangeEventArgs<double>(value, _hillSize);
                    _hillSize = value;
                    Logger.Debug("[{Name}] HillSize changed from {OldHillSize} to {NewHillSize}", Name, e.OldValue, e.NewValue);
                    HillSizeChanged?.Invoke(this, e);
                }
            }
        }

        /// <inheritdoc />
        public event EventHandler<SendEventArgs> SignalFired;

        /// <inheritdoc />
        public event ChangeEventHandler<double> HillSizeChanged;
        #endregion

        /// <inheritdoc />
        protected override void ReleaseUnmanagedResources()
        {
            base.ReleaseUnmanagedResources();
            _axon?.Dispose();
            Logger.Debug("[{Name}] Released unmanaged resources", Name);
        }
    }
}