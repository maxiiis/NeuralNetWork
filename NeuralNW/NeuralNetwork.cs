using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNW
{
    public class NeuralNetwork
    {
        public Layer[] Layers { get; }
        public int Inputs { get; }
        public int Outputs { get; }
        public double LearningSpeed { get; }

        public NeuralNetwork(int inputs,int outputs,double learnSpeed,params int[] layers)
        {
            Inputs = inputs;
            Outputs = outputs;
            Layers = new Layer[layers.Length + 2];
            LearningSpeed = learnSpeed;

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
                Neuron neuron = new Neuron(Layers[Layers.Length - 2].NeuronCount);
                outputNeurons[i] = neuron;
            }
            Layer outputLayer = new Layer(outputNeurons);
            Layers[Layers.Length - 1] = outputLayer;
        }

        public Neuron GetResult(params double[] input)
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

            Neuron result = Layers.Last().Neurons.OrderByDescending(x => x.Output).ToArray()[0];

            return result;
        }

        public double Learn(double[] expected,double[,] inputs,int epoch)
        {
            double error = 0.0;
            for (int i = 0; i < epoch; i++)
            {
                for (int j = 0; j < expected.Length; j++)
                {
                    error += ErrorBack(expected[j],GetRow(inputs,j));
                }
            }
            error = error / epoch;
            return error;
        }

        public static double[] GetRow(double[,] input,int row)
        {
            double[] result = new double[input.GetLength(1)];

            for (int i = 0; i < result.Length; i++)
                result[i] = input[row,i];

            return result;
        }

        private double ErrorBack(double output,double[] inputs)
        {
            double actual = GetResult(inputs).Output;

            double difference = actual - output;

            Layer last = Layers[Layers.Length - 1];
            for (int i = 0; i < last.NeuronCount; i++)
            {
                last.Neurons[i].Learn(difference,LearningSpeed);
            }

            for (int i = Layers.Length - 2; i >= 0; i--)
            {
                Layer layer = Layers[i];
                Layer prevLayer = Layers[i + 1];

                for (int j = 0; j < layer.NeuronCount; j++)
                {
                    Neuron neuron = layer.Neurons[j];

                    for (int k = 0; k < prevLayer.NeuronCount; k++)
                    {
                        Neuron prevNeuron = prevLayer.Neurons[k];
                        double error = prevNeuron.Weights[j] * prevNeuron.Delta;
                        neuron.Learn(error,LearningSpeed);
                    }
                }
            }

            double newdifference = GetResult(inputs).Output - output;

            return difference * difference;
        }
    }
}
