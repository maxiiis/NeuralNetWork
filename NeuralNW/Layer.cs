using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNW
{
    class Layer
    {
        public List<Neuron> Neurons { get; }
        public int NeuronCount { get; }

        public Layer(List<Neuron> neurons)
        {
            Neurons = neurons;
            NeuronCount = neurons.Count;
        }

        public List<double> GetResult()
        {
            List<double> result = new List<double>();

            foreach (Neuron neuron in Neurons)
            {
                result.Add(neuron.Output);
            }

            return result;
        }
    }
}
