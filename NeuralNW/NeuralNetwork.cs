using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNW
{
    class NeuralNetwork
    {
        public Layer[] Layers { get; }
        public int Inputs { get; }
        public int Outputs { get; }
        public double LearningSpeed { get; }

        public NeuralNetwork(int inputs,int outputs,params int[] layers)
        {
            Inputs = inputs;
            Outputs = outputs;
            Layers = new Layer[layers.Length + 2];

            Neuron[] inputNeurons = new Neuron[Inputs];
            for (int i = 0; i < Inputs; i++)
            {
                Neuron neuron = new Neuron(1);
                inputNeurons[i] = neuron;
            }
            Layer inputLayer = new Layer(inputNeurons);
            Layers[0] = inputLayer;

            for (int i = 0; i < layers.Length; i++)
            {
                Neuron[] Neurons = new Neuron[layers[i]];
                for (int j = 0; j < layers[i]; j++)
                {
                    Neuron neuron = new Neuron(Layers[i].NeuronCount);
                    Neurons[j] = neuron;
                }
                Layer layer = new Layer(Neurons);
                Layers[i + 1] = layer;
            }

            Neuron[] outputNeurons = new Neuron[outputs];
            for (int i = 0; i < Outputs; i++)
            {
                Neuron neuron = new Neuron(Layers[Layers.Length-2].NeuronCount);
                outputNeurons[i] = neuron;
            }
            Layer outputLayer = new Layer(outputNeurons);
            Layers[Layers.Length-1] = outputLayer;
        }

        public double[] GetResult(params double[] input)
        {
            for (int i = 0; i < Layers[0].NeuronCount; i++)
            {
                double[] inp = new double[] { input[i] };
                Layers[0].Neurons[i].GetResult(inp);
            }

            for (int i = 1; i < Layers.Length; i++)
            {
                Layer currentLayer = Layers[i];
                double[] prevLayer = Layers[i - 1].GetResult();

                foreach (Neuron neuron in currentLayer.Neurons)
                {
                    neuron.GetResult(prevLayer);
                }
            }

            return Layers.Last().GetResult();
        }
    }
}
