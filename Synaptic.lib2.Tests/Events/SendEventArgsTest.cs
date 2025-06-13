using System;
using ahbsd.Synaptic.lib2;
using ahbsd.Synaptic.lib2.Events;
using ahbsd.Synaptic.lib2.Interfaces;
using JetBrains.Annotations;
using Serilog;
using Xunit;

namespace Synaptic.lib2.Tests.Events
{
    [TestSubject(typeof(SendEventArgs))]
    public class SendEventArgsTest : BaseTest
    {
        public SendEventArgsTest()
        {
            Logger = Log.ForContext<SendEventArgsTest>();
            Logger.Information("[{Name}] Starting ...", Name);
        }

        [Fact]
        public void TestConstructor1()
        {
            Logger.Debug("[{Name}] Testing default constructor", Name);
            var args = new SendEventArgs();
            Assert.NotNull(args);
            Assert.Equal(0d, args.SignalStrength);
            Assert.Null(args.Sender);
            Assert.NotEqual(Guid.Empty, args.Id);
            Assert.StartsWith("SendEventArgs_", args.Name);
        }

        [Theory]
        [InlineData(double.MinValue)]
        [InlineData(double.NaN)]
        [InlineData(0.5)]
        [InlineData(1.0)]
        [InlineData(double.MaxValue)]
        public void TestConstructor2(double signalStrength)
        {
            Logger.Debug("[{Name}] Testing constructor with signal strength {SignalStrength}", Name, signalStrength);
            var args = new SendEventArgs(signalStrength);
            Assert.NotNull(args);
            Assert.Equal(signalStrength, args.SignalStrength);
            Assert.Null(args.Sender);
            Assert.NotEqual(Guid.Empty, args.Id);
            Assert.StartsWith("SendEventArgs_", args.Name);
        }

        [Theory]
        [InlineData(double.MinValue, "Sender1")]
        [InlineData(double.NaN, "Sender2")]
        [InlineData(0.5, "Sender3")]
        [InlineData(1.0, "Sender4")]
        [InlineData(double.MaxValue, "Sender5")]
        public void TestConstructor3(double signalStrength, string name)
        {
            Logger.Debug("[{Name}] Testing constructor with signal strength {SignalStrength} and sender {SenderName}", Name, signalStrength, name);
            ISynapticObject sender = new Neuron(name);
            var args = new SendEventArgs(signalStrength, sender);
            Assert.NotNull(args);
            Assert.Equal(signalStrength, args.SignalStrength);
            Assert.Equal(sender, args.Sender);
            Assert.NotEqual(Guid.Empty, args.Id);
            Assert.StartsWith("SendEventArgs_", args.Name);
        }
    }
}