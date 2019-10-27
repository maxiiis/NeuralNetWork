using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace NeuralNW
{
    public class NeuralNetwork
    {
        public Layer[] Layers { get; }
        public int Inputs { get; }
        public int Outputs { get; }
        public double LearningSpeed { get; private set;}

        public NeuralNetwork(int inputs,int outputs,params int[] layers)
        {
            Inputs = inputs;
            Outputs = outputs;
            Layers = new Layer[layers.Length + 2];

            CreateInputLayer();
            for (int i = 0; i < layers.Length; i++)
                CreateHiddenLayers(i + 1,layers[i]);
            CreateOutputLayer();
        }

        private void CreateOutputLayer(double[] weights = null)
        {
            Neuron[] outputNeurons = new Neuron[Outputs];
            for (int i = 0; i < Outputs; i++)
            {
                Neuron neuron;
                int inputCount = Layers[Layers.Length - 2].NeuronCount;

                if (weights == null)
                    neuron = new Neuron(inputCount);
                else
                    neuron = new Neuron(inputCount,weights.Skip(i * inputCount).Take(inputCount).ToArray());
                outputNeurons[i] = neuron;
            }
            Layer outputLayer = new Layer(outputNeurons);
            Layers[Layers.Length - 1] = outputLayer;
        }

        private void CreateHiddenLayers(int layerNum,int count,double[] weights = null)
        {
            Neuron[] Neurons = new Neuron[count];
            for (int j = 0; j < count; j++)
            {
                Neuron neuron;
                int inputCount = Layers[layerNum - 1].NeuronCount;
                if (weights == null)
                    neuron = new Neuron(inputCount);
                else
                    neuron = new Neuron(inputCount,weights.Skip(j * inputCount).Take(inputCount).ToArray());
                Neurons[j] = neuron;
            }
            Layer layer = new Layer(Neurons);
            Layers[layerNum] = layer;
        }

        private void CreateInputLayer()
        {
            Neuron[] inputNeurons = new Neuron[Inputs];
            for (int i = 0; i < Inputs; i++)
            {
                Neuron neuron = new Neuron(1);
                inputNeurons[i] = neuron;
            }
            Layer inputLayer = new Layer(inputNeurons);
            Layers[0] = inputLayer;
        }

        public NeuralNetwork(string path)
        {
            byte[] read = File.ReadAllBytes(path);

            List<double> settings = new List<double>();

            for (int i = 0; i < read.Length; i += 8)
                settings.Add(BitConverter.ToDouble(read,i));

            double layersCount = settings[0];

            Layers = new Layer[Convert.ToInt32(layersCount)];

            Inputs = (int)settings[1];

            CreateInputLayer();

            int neuronsCountIndex = 2;
            int weightsCount = 0;

            for (int i = 1; i <= Layers.Length - 2; i++)
            {
                weightsCount = (int)(Layers[i - 1].NeuronCount * settings[neuronsCountIndex]);
                CreateHiddenLayers(i,(int)settings[neuronsCountIndex],settings.GetRange(neuronsCountIndex + 1,weightsCount).ToArray());
                neuronsCountIndex += weightsCount;
            }

            Outputs = (int)settings[neuronsCountIndex+1];

            weightsCount = (int)(Layers[Layers.Length - 2].NeuronCount * settings[neuronsCountIndex+1]);
            CreateOutputLayer(settings.GetRange(neuronsCountIndex+2,weightsCount).ToArray());
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

            double[] rezult = Layers.Last().GetResult();

            return rezult;
        }

        public double Learn(double[][] expected,double[,] inputs,int epoch, double learnSpeed)
        {
            LearningSpeed = learnSpeed;
            double error = 0.0;
            for (int i = 0; i < epoch; i++)
            {
                for (int j = 0; j < expected.Length; j++)
                {
                    //
                    Application.DoEvents();
                    //
                    error += ErrorBack(expected[j],GetRow(inputs,j));
                }
            }
            error = error / epoch;
            return error;
        }

        private static double[] GetRow(double[,] input,int row)
        {
            double[] result = new double[input.GetLength(1)];

            for (int i = 0; i < result.Length; i++)
                result[i] = input[row,i];

            return result;
        }

        private double ErrorBack(double[] output,double[] inputs)
        {
            double[] actual = GetResult(inputs);

            double[] difference = new double[actual.Length];

            double error = 0;

            Layer last = Layers[Layers.Length - 1];
            for (int i = 0; i < last.NeuronCount; i++)
            {
                error = actual[i] - output[i];
                last.Neurons[i].Learn(error,LearningSpeed);
            }

            for (int i = Layers.Length - 2; i > 0; i--)
            {
                Layer layer = Layers[i];
                Layer prevLayer = Layers[i + 1];

                for (int j = 0; j < layer.NeuronCount; j++)
                {
                    Neuron neuron = layer.Neurons[j];

                    for (int k = 0; k < prevLayer.NeuronCount; k++)
                    {
                        Neuron prevNeuron = prevLayer.Neurons[k];
                        error = prevNeuron.Weights[j] * prevNeuron.Delta;
                        neuron.Learn(error,LearningSpeed);
                    }
                }
            }

            return error * error;
        }

        public void Save(string path)
        {
            List<double> saveDouble = new List<double>();
            saveDouble.Add(Layers.Count());
            saveDouble.Add(Layers[0].NeuronCount);

            for (int l = 1; l < Layers.Count(); l++)
            {
                saveDouble.Add(Layers[l].NeuronCount);

                int prevLayerNeurons = Layers[l - 1].NeuronCount;

                for (int n = 0; n < Layers[l].NeuronCount; n++)
                {
                    saveDouble.AddRange(Layers[l].Neurons[n].Weights);
                }
            }

            List<byte> saveByte = new List<byte>();

            foreach (double d in saveDouble)
            {
                saveByte.AddRange(BitConverter.GetBytes(d));
            }

            File.WriteAllBytes(path,saveByte.ToArray());
        }
    }
}
