using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenePool {
	//Singleton
	public static GenePool instance = new GenePool();
	private Dictionary<string,Genome> genomes = new Dictionary<string, Genome>();
	public GenePool()
	{
		
	}
	public Genome getBestGenome(string genomeName)
	{
		Genome g;
		if (!genomes.ContainsKey(genomeName))
		{
			genomes.Add (genomeName,g = new Genome(genomeName,1.0f));
			//Small optimization
			return g;
		}
		return genomes[genomeName];
	}
	
	public void setBestGenome(string genomeName, Genome genome)
	{
		Genome best = getBestGenome(genomeName);
		if (genome == best)
			return;
		if (genome.Fitness () > best.Fitness ())
		{
			genomes[genomeName] = genome;
		}
	}
}
