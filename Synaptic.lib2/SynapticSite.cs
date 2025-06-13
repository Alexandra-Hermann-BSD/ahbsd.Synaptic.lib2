using System;
using System.ComponentModel;

namespace ahbsd.Synaptic.lib2
{
    /// <summary>
    /// Implementation of <see cref="ISite"/> for Synaptic objects.
    /// </summary>
    public class SynapticSite : ISite
    {

        /// <summary>
        /// Constructor for creating a SynapticSite instance.
        /// </summary>
        /// <param name="component">The given component</param>
        /// <param name="name">[optional] The given name</param>
        /// <exception cref="ArgumentNullException"></exception>
        public SynapticSite(IComponent component, string name = null)
        {
            Component = component ?? throw new ArgumentNullException(nameof(component), "Component cannot be null");
            Name = name ?? component.GetType().Name;
            Container = new Container();
        }

        /// <summary>
        /// Constructor for creating a SynapticSite instance.
        /// </summary>
        /// <param name="component">The given component</param>
        /// <param name="container">The given container</param>
        /// <param name="name">[optional] The given name</param>
        /// <exception cref="ArgumentNullException"></exception>
        public SynapticSite(IComponent component, IContainer container, string name = null)
        {
            Component = component ?? throw new ArgumentNullException(nameof(component), "Component cannot be null");
            Name = name ?? component.GetType().Name;
            Container = container ?? throw new ArgumentNullException(nameof(container), "Container cannot be null");
        }

        /// <inheritdoc />
        public new object GetService(Type serviceType) => null;

        /// <inheritdoc />
        public IComponent Component { get; }

        /// <inheritdoc />
        public IContainer Container { get; }

        /// <inheritdoc />
        public bool DesignMode { get; protected internal set; }

        /// <inheritdoc />
        public new string Name { get; set;}
    }
}