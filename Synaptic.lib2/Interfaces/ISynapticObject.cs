using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Serilog;

namespace ahbsd.Synaptic.lib2.Interfaces
{
    /// <summary>
    /// Interface of a synaptic object.
    /// </summary>
    public interface ISynapticObject : ISerializable, IEqualityComparer<ISynapticObject>, IComponent
    {
        /// <summary>
        /// Gets the logger for this object.
        /// </summary>
        /// <value>The logger for this object</value>
        ILogger Logger { get; }

        /// <summary>
        /// Gets the unique identifier of the synaptic object.
        /// </summary>
        /// <value>The unique identifier of the synaptic object</value>
        Guid Id { get; }

        /// <summary>
        /// Gets the name of the synaptic object.
        /// </summary>
        /// <value>The name of the synaptic object</value>
        string Name { get; }

        /// <summary>
        /// Gets the global number of the synaptic object.
        /// </summary>
        /// <value>The global number of the synaptic object</value>
        ulong Number { get; }
    }
}