using System;
using ahbsd.Synaptic.lib2;
using JetBrains.Annotations;
using Xunit;

namespace Synaptic.lib2.Tests
{
    [TestSubject(typeof(Dendrit))]
    public class DendritTest : BaseTest
    {

        [Fact]
        public void SimpleTest1()
        {
            var dendrit = new Dendrit();
            Assert.NotNull(dendrit);
            dendrit.Disposed += Dendrit_OnDisposed;
            Assert.StartsWith("Dendrit", dendrit.Name);
            Assert.Null(dendrit.Neuron);
        }

        [Fact]
        public void SimpleTest2()
        {
            var neuron = new Neuron("TestNeuron");
            var dendrit = new Dendrit(neuron);
            dendrit.Disposed += Dendrit_OnDisposed;
            Assert.NotNull(dendrit);
            Assert.Equal("Dendrit of TestNeuron", dendrit.Name);
            Assert.Equal(neuron, dendrit.Neuron);
        }

        private static void Dendrit_OnDisposed(object sender, EventArgs e)
        {
            Assert.NotNull(sender);
            Assert.IsType<Dendrit>(sender);
            Assert.Equal(EventArgs.Empty, e);
        }
    }
}