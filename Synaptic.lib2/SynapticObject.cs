using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using ahbsd.Synaptic.lib2.Interfaces;
using Serilog;
// ReSharper disable NonReadonlyMemberInGetHashCode

namespace ahbsd.Synaptic.lib2
{
    /// <summary>
    /// Base class for all synaptic objects.
    /// </summary>
    [Serializable]
    public abstract partial class SynapticObject : Component, ISynapticObject, IEqualityComparer<SynapticObject>
    {
        private static ulong _nr = (ulong)0;

        /// <summary>
        /// Deserialization constructor for SynapticObject.
        /// </summary>
        /// <param name="info">The serialization info</param>
        /// <param name="context">The serialization context</param>
        protected SynapticObject(SerializationInfo info, StreamingContext context)
        {
            if (info != null)
            {
                Id = new Guid(info.GetString(nameof(Id)));
                Name = info.GetString(nameof(Name));
                Number = info.GetUInt64(nameof(Number));
            }
            else
            {
                Number = _nr++;
                Id = Guid.NewGuid();
                Name = $"{GetType().Name}{Number}";
            }

            Logger = Log.ForContext<SynapticObject>();
        }

        /// <summary>
        /// Default constructor for SynapticObject.
        /// </summary>
        protected SynapticObject()
        {
            Number = _nr++;
            Id = Guid.NewGuid();
            Name = GetName();

            Logger = Log.ForContext<SynapticObject>();

            Initialize();
        }

        /// <summary>
        /// Constructor for SynapticObject with a given name.
        /// </summary>
        /// <param name="name">The given name</param>
        protected SynapticObject(string name)
        {
            Number = _nr++;
            Id = Guid.NewGuid();
            Name = string.IsNullOrWhiteSpace(name) ? GetName() : name;

            Logger = Log.ForContext<SynapticObject>();

            Initialize();
        }

        private string GetName()
        {
            return $"{GetType().Name}{Number}";
        }

        #region implementation of ISynapticObject

        /// <inheritdoc />
        public ILogger Logger { get; protected set; }

        /// <inheritdoc />
        public Guid Id { get; }

        /// <inheritdoc />
        public string Name { get; internal set; }

        /// <inheritdoc />
        public ulong Number { get; }

        #endregion

        #region implementation of ISerializable

        /// <inheritdoc />
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Id), Id);
            info.AddValue(nameof(Name), Name);
            info.AddValue(nameof(Number), Number);
        }

        #endregion

        #region Implementation of IEquatable<ISynapticObject>

        /// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
        public bool Equals(SynapticObject other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id.Equals(other.Id) && Name == other.Name;
        }

        /// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
        public bool Equals(ISynapticObject other) => other != null && Equals((object)other);

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((SynapticObject)obj);
        }

        /// <inheritdoc />
        bool IEqualityComparer<SynapticObject>.Equals(SynapticObject x, SynapticObject y) => x?.Equals(y) ?? y is null;

        /// <inheritdoc />
        int IEqualityComparer<SynapticObject>.GetHashCode(SynapticObject obj) => obj?.GetHashCode() ?? 0;

        /// <inheritdoc />
        bool IEqualityComparer<ISynapticObject>.Equals(ISynapticObject x, ISynapticObject y) => x?.Equals(y) ?? y is null;

        /// <inheritdoc />
        int IEqualityComparer<ISynapticObject>.GetHashCode(ISynapticObject obj) => obj?.GetHashCode() ?? 0;

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                int? hashCode = null;
                try
                {
                    hashCode = (Id.GetHashCode() * 397) ^ Name.GetHashCode();
                }
                catch (Exception e)
                {
                    Logger?.Error(e, "GetHashCode() failed");
                }
                return hashCode.GetValueOrDefault(-1);
            }
        }

        /// <inheritdoc cref="op_Equality" />
        public static bool operator ==(SynapticObject left, SynapticObject right) => Equals(left, right);

        /// <inheritdoc cref="op_Inequality" />
        public static bool operator !=(SynapticObject left, SynapticObject right) => !Equals(left, right);

        #endregion

        /// <inheritdoc />
        public override string ToString() => $"{Name} (ID: {Id})";

        /// <summary>
        /// Releases unmanaged resources.
        /// </summary>
        protected virtual void ReleaseUnmanagedResources() { }

        /// <summary>
        /// Disposes the object, releasing resources.
        /// </summary>
        /// <param name="disposing">Is this object disposing?</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose managed resources here if any
                Logger?.Debug("[{Name}] Disposing managed resources", Name);
            }

            // Release unmanaged resources
            ReleaseUnmanagedResources();
            Logger?.Debug("[{Name}] Unmanaged resources released", Name);
        }

        /// <inheritdoc cref="IDisposable.Dispose()"/>
        public new virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Deconstructor for SynapticObject, which releases unmanaged resources.
        /// </summary>
        ~SynapticObject()
        {
            Dispose(false);
            Logger.Debug("[{Name}] Finalizer called, unmanaged resources released", Name);
            Name = null;
            Logger = null;
            Log.CloseAndFlush();
        }
    }
}