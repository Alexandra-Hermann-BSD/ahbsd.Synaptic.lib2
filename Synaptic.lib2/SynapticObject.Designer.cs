using System.ComponentModel;

namespace ahbsd.Synaptic.lib2
{

    public abstract partial class SynapticObject
    {
        private System.ComponentModel.IContainer _container = new Container();

        private void Initialize()
        {
            // This method can be used to perform any initialization logic
            // that is common to all SynapticObject instances.
            Logger.Verbose("[{Name}] SynapticObject initialized; ID: {Id}", Name, Id);

            this.Site = new SynapticSite(this, this._container, this.Name+ "_Site");
        }
    }
}