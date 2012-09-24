using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Genome {
	private float mutationRate;
	private Dictionary<string,Chromosome> chromosomes;
	private int fitness;
	private string name;
	
	public Genome(string name, float mutationRate)
	{
		//Fitness always begins at 0
		fitness = 0;
		chromosomes = new Dictionary<string, Chromosome>();
		this.mutationRate = mutationRate;
		this.name = name;
	}
	
	public Genome(Genome genome,float mutationRate)
	{
		fitness = 0;
		chromosomes = genome.copyChromosomes();
		this.mutationRate = mutationRate;
		this.name = genome.getName ();
	}
	
	public void incrementFitness(int amount)
	{
		fitness += amount;
		GenePool.instance.setBestGenome(this.name,this);
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
	
	public void mutate()
	{
		foreach (Chromosome c in chromosomes.Values)
		{
			c.mutate(this.mutationRate);
		}
	}
	
	public double getGene(string name)
	{
		if (chromosomes.ContainsKey(name))
			chromosomes[name] = new Chromosome(name,1,this.mutationRate);
		return chromosomes[name].getValue ();
	}
	
	public string getName()
	{
		return this.name;
	}
}
