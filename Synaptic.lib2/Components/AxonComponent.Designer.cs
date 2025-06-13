using System.ComponentModel;
using ahbsd.Synaptic.lib2.Interfaces;

namespace ahbsd.Synaptic.lib2.Components
{
    partial class AxonComponent
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            this.NeuronChanged += new ahbsd.Synaptic.lib2.Events.ChangeEventHandler<INeuron>(this.OnNeuronChanged);
        }

        #endregion
    }
}