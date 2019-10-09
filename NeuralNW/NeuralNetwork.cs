using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNW
{
    class NeuralNetwork
    {
        public List<Layer> Layers { get; }
        public int Inputs { get; }
        public int Outputs { get; }

        public NeuralNetwork(int inputs,int outputs,params int[] layers)
        {
            Inputs = inputs;
            Outputs = outputs;

            List<Neuron> inputNeurons = new List<Neuron>();
            for (int i = 0; i < Inputs; i++)
            {
                Neuron neuron = new Neuron(1);
                inputNeurons.Add(neuron);
            }
            Layer inputLayer = new Layer(inputNeurons);
            Layers.Add(inputLayer);

            for (int i = 0; i < layers.Length; i++)
            {
                List<Neuron> Neurons = new List<Neuron>();
                for (int j = 0; i < layers[i]; j++)
                {
                    Neuron neuron = new Neuron(Layers.Last().NeuronCount);
                    Neurons.Add(neuron);
                }
                Layer layer = new Layer(Neurons);
                Layers.Add(layer);
            }

            List<Neuron> outputNeurons = new List<Neuron>();
            for (int i = 0; i < Outputs; i++)
            {
                Neuron neuron = new Neuron(Layers.Last().NeuronCount);
                outputNeurons.Add(neuron);
            }
            Layer outputLayer = new Layer(outputNeurons);
            Layers.Add(outputLayer);
        }

        public List<double> GetResult(params double[] input)
        {
            for (int i = 0; i < Layers[0].NeuronCount; i++)
            {
                List<double> inp = new List<double> { input[i] };
                Layers[0].Neurons[i].GetResult(inp);
            }

            for (int i = 1; i < Layers.Count; i++)
            {
                Layer currentLayer = Layers[i];
                List<double> prevLayer = Layers[i - 1].GetResult();

                foreach (Neuron neuron in currentLayer.Neurons)
                {
                    neuron.GetResult(prevLayer);
                }
            }

            return Layers.Last().GetResult();
        }
    }
}
