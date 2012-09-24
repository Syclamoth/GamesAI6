using UnityEngine;
using System.Collections;

[System.Serializable]
public class Chromosome {
	private string key;
	private double value;
	private float mutationRate;
	
	public Chromosome (string key, double value, float mutationRate)
	{
		this.key = key;
		this.value = value;
		this.mutationRate = mutationRate;
	}
	
	public void mutate(float mutationRate)
	{
		this.value += mutationRate*this.mutationRate*Random.Range (-1.0f, 1.0f);
	}
	
	public double getValue()
	{
		return this.value;
	}
	
	public Chromosome Clone()
	{
		return new Chromosome(key,value,mutationRate);
	}
}