using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Genome {
	private Dictionary<string,Chromosome> chromosomes;
	private int fitness;
	public WolfController manager;
	
	// Generates a blank genome
	public Genome(WolfController manager)
	{
		//Fitness always begins at 0
		fitness = 0;
		this.manager = manager;
		chromosomes = new Dictionary<string, Chromosome>();
	}
	
	// Generates a mutated genome from an existing one
	public Genome(Genome genome, float mutationRate, WolfController manager)
	{
		this.manager = manager;
		fitness = 0;
		chromosomes = genome.copyChromosomes();
		foreach(Chromosome gene in chromosomes.Values) {
			gene.mutate(mutationRate);
		}
	}
	
	public void incrementFitness(int amount)
	{
		fitness += amount;
		//GenePool.instance.setBestGenome(this.name,this);
	}
	
	public int Fitness()
	{
		return fitness;
	}
	
	public Dictionary<string,Chromosome> copyChromosomes()
	{
		Dictionary<string,Chromosome> d = new Dictionary<string, Chromosome>();
		
		foreach (string k in chromosomes.Keys)
		{
			d[k] = chromosomes[k].Clone();
		}
		
		return d;
	}
	
	public void mutate(float mutationRate)
	{
		foreach (Chromosome c in chromosomes.Values)
		{
			c.mutate(mutationRate);
		}
	}
	
	public double getGene(string name)
	{
		if (!chromosomes.ContainsKey(name))
		{
			Debug.LogError("Genome does not contain gene: " + name);
			return 0;
		}
		return chromosomes[name].getValue ();
	}
	
	public void addGene(string name, Chromosome value) {
		chromosomes.Add(name, value);
	}
}
