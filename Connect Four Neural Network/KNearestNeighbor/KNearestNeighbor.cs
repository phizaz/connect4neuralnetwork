using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using NeuralNet;

namespace KNearestNeighbor
{

[Serializable]
public class KNearestNeighbor
{
	public int k;
	public List<Example> savedExamples;

	private KNearestNeighbor() { }  

	public KNearestNeighbor(int k)
	{
		this.k = k;
		this.savedExamples = new List<Example>();
	}

	public void LearnOneExample(Example example)
	{
		savedExamples.Add(example);
	}

	private double distance_sq(Example e1, Example e2)
	{
		double sum = 0.0;
		Debug.Assert(e1.Features.Count == e2.Features.Count);
		for (int i = 0; i < e1.Features.Count; i++)
		{
			double x1 = e1.Features[i];
			double x2 = e2.Features[i];
			sum += Math.Pow(x1 - x2, 2.0);
		}
		return sum;
	}

	private class BestExample : IComparable
	{
		public Example example;
		public double distance;

		public BestExample(Example e, double d)
		{
			example = e;
			distance = d;
		}

		public int CompareTo(object o)
		{
			return distance.CompareTo(((BestExample) o).distance);
		}
	}

	public void PropogateInput(Example example)
	{
		List<BestExample> best = new List<BestExample>();
		double maxBest = 0.0;

		foreach (Example e in savedExamples)
		{
			double d = distance_sq(e, example);
			if (best.Count < k)
			{
				best.Add(new BestExample(e,d));
				if (d > maxBest) { maxBest = d; }
			}
			else if (d < maxBest)
			{
				// at this point, the Best list contains k elements
				best.Sort();
				maxBest = best[k-1].distance;
				best[k-1] = new BestExample(e, d);
			}
		}

		//TODO: perform a weighted average
		example.Predictions = best[0].example.Labels;
	}

  
}//end KNearestNeighbor class

}//end KNearestNeighbor namespace
