using System;
using System.Collections.Generic;
using ahbsd.Synaptic.lib2.Events;
using ahbsd.Synaptic.lib2.Interfaces;
using Serilog;

namespace ahbsd.Synaptic.lib2
{
    /// <summary>
    /// Class representing an axon terminal, which is one of the end points of an axon that can fire signals.
    /// </summary>
    public class AxonTerminal : SynapticObject, IAxonTerminal
    {
        private readonly IList<IDendrit> _connectedDendrites;

        /// <summary>
        /// Internal constructor for creating an axon terminal.
        /// </summary>
        /// <param name="axon">The base axon</param>
        /// <param name="name">[optional] a given name</param>
        internal AxonTerminal(IAxon axon, string name = null) : base(name)
        {
            Logger = Log.ForContext<AxonTerminal>();
            Axon = axon;
            _connectedDendrites = new List<IDendrit>();
            Logger.Verbose("[{Name}] created; ID: {Id}; Connected Axon: {Axon}", Name, Id, Axon);
        }

        #region implementation of IAxonTerminal

        /// <inheritdoc />
        public IAxon Axon { get; }

        /// <inheritdoc />
        public bool FireSignal()
        {
            var result = false;
            Logger.Debug("[{Name}] Try fire signal; ID: {Id} Signal to {DendritCount} dendrites",
                Name, Id, ConnectedDendrites.Count);
            var signal = 1.0 / (Axon.Terminals.Count + ConnectedDendrites.Count);
            Logger.Verbose("[{Name}] Signal strength: {SignalStrength}", Name, signal);
            OnSignal?.Invoke(this);
            var e = new SendEventArgs(signal, this);

            foreach (var dendrit in ConnectedDendrites)
            {
                Logger.Debug("[{Name}] Try send {E} to dendrit: {Dendrit}", Name, e, dendrit);
                result |= dendrit.ReceiveSignal(e);
            }

            return result;
        }

        /// <inheritdoc />
        public event Action<IAxonTerminal> OnSignal;

        /// <inheritdoc />
        public IReadOnlyList<IDendrit> ConnectedDendrites => (IReadOnlyList<IDendrit>)_connectedDendrites;

        /// <inheritdoc />
        public bool AddDendrit(IDendrit dendrit)
        {
            bool result;

            if (dendrit != null)
            {
                if (_connectedDendrites.Contains(dendrit))
                {
                    Logger.Warning("[{Name}] AddDendrit failed: dendrit {Dendrit} is already connected", Name,dendrit.Name);
                    result = false;
                }
                else
                {
                    try
                    {
                        ((Dendrit)dendrit).ConnectedAxonTerminal = this;
                        _connectedDendrites.Add(dendrit);
                        Logger.Debug("[{Name}] Dendrit {Dendrit} added", Name, dendrit.Name);
                        result = true;
                    }
                    catch (Exception e)
                    {
                        Logger.Debug(e, "[{Name}] Error while trying to add dendrit: {Exception}", Name, e);
                        result = false;
                    }
                }
            }
            else
            {
                Logger.Debug("[{Name}] AddDendrit failed: dendrit cannot be null", Name);
                throw new ArgumentNullException(nameof(dendrit), "Dendrit cannot be null");
            }

            return result;
        }

        #endregion

        /// <inheritdoc />
        protected override void ReleaseUnmanagedResources()
        {
            foreach (var dendrit in _connectedDendrites)
            {
                try
                {
                    dendrit.Dispose();
                }
                catch (Exception e)
                {
                    Logger.Error("[{Name}] Error while releasing dendrit {Dendrit}: {Exception}", Name, dendrit?.Name ?? "already disposed dendrit", e);
                }
            }
            _connectedDendrites.Clear();
            Logger.Debug("[{Name}] Released unmanaged resources", Name);
        }
    }
}