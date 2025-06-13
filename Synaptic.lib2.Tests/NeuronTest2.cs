using System.Collections.Generic;
using ahbsd.Synaptic.lib2;
using ahbsd.Synaptic.lib2.Events;
using ahbsd.Synaptic.lib2.Interfaces;
using JetBrains.Annotations;
using Serilog;
using Xunit;

namespace Synaptic.lib2.Tests
{
    [TestSubject(typeof(Neuron))]
    public class NeuronTest2 : BaseTest
    {

        private readonly IList<INeuron> _neurons;

        private readonly IDictionary<INeuron, IList<IAxonTerminal>> _neuronDict;

        public NeuronTest2()
        {
            Logger = Log.ForContext<NeuronTest2>();
            Logger.Information("Starting NeuronTest2...");
            _neurons = new List<INeuron>
            {
                new Neuron("Neuron1"),
                new Neuron("Neuron2"),
                new Neuron("Neuron3")
            };

            foreach (var t in _neurons)
            {
                t.SignalBufferChanged += OnSignalBufferChanged;
                t.AxonHillSizeChanged += OnSignalBufferChanged;
            }

            _neuronDict = new Dictionary<INeuron, IList<IAxonTerminal>>();

            for (var i = 0; i < 50; i++)
            {
                var j = i % _neurons.Count;
                var t = _neurons[j].Axon.AddNewTerminal();
                if (!_neuronDict.ContainsKey(_neurons[j]))
                {
                    _neuronDict[_neurons[j]] = new List<IAxonTerminal>();
                }
                _neuronDict[_neurons[j]].Add(t);
            }

            var k = 0;

            IAxonTerminal previousTerminal = null;
            foreach (var pair in _neuronDict)
            {
                if (previousTerminal != null)
                {
                    pair.Key.ConnectTo(previousTerminal);
                }
                pair.Key.ConnectTo(_neurons[k % _neurons.Count].Axon.Terminals[0]);
                previousTerminal = pair.Value[k++ % pair.Value.Count];
                previousTerminal.OnSignal += PreviousTerminalOnOnSignal;
            }
        }

        private static void PreviousTerminalOnOnSignal(IAxonTerminal obj)
        {
            Assert.NotNull(obj);
        }

        private static void OnSignalBufferChanged(object sender, ChangeEventArgs<double> e)
        {
            Assert.NotEqual(e.OldValue, e.NewValue);
        }

        [Fact]
        public void SimpleNeuronTest()
        {
            Logger.Information("Running SimpleNeuronTest...");
            var starterNeuron = new Neuron("Starter Neuron");

            foreach (var neuron in _neurons)
            {
                Logger.Information("Testing neuron: {NeuronName}", neuron.Name);
                Assert.NotNull(neuron);
                neuron.AxonHillSizeChanged += Neuron_OnAxonHillSizeChanged;
                neuron.SignalBufferChanged += Neuron_OnAxonHillSizeChanged;

                neuron.ConnectTo(starterNeuron.DefaultAxonTerminal);
            }

            var dendrit1 = new Dendrit(starterNeuron);
            dendrit1.InputSignal += Dendrit1_OnInputSignal;
            if (starterNeuron.AddDendrit(dendrit1))
            {
                var testSignal = new SendEventArgs(10.0, starterNeuron.DefaultAxonTerminal);
                for (var i = 0; i < 50; i++)
                {
                    foreach (var dendrit in starterNeuron.Dendrites)
                    {
                        dendrit.ReceiveSignal(testSignal);
                    }
                }
            }
        }

        private static void Neuron_OnAxonHillSizeChanged(object sender, ChangeEventArgs<double> e)
        {
            Assert.NotEqual(e.OldValue, e.NewValue);
        }

        private static void Dendrit1_OnInputSignal(object sender, SendEventArgs e)
        {
            Assert.NotNull(e);
        }
    }
}