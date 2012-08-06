//Immutable, stores both dB for human readibility and Watts per m^2
public struct Volume {
	//Will relent with capitalization. I still hate it. Only doing it because
	//they're readonly properties and don't want them confused with variables.
	public double Decibels { get; private set; } // dB
	public double Intensity { get; private set; } // W m-2
	
	//Threshold of human hearing in W m-2. Converts to 0 dB
	public const double HUMAN_THRESHOLD = 1e-12;
	//No need for this to be public/editable because it is based on physics.
	private const double INTENSITY_DROPOFF_CONSTANT = 1 / (4 * System.Math.PI);
	
	//Factory Pattern
	public static Volume fromDecibels(double decibels) {
		return new Volume(System.Math.Pow (10.0,decibels/10.0 - 12.0),
			              decibels);
	}
	public static Volume fromIntensity(double intensity) {
		return new Volume(intensity,
			10.0 * System.Math.Log10 (intensity/Volume.HUMAN_THRESHOLD));
	}
	
	private Volume(double intensity,double decibels) {
		this.Intensity = intensity;
		this.Decibels = decibels;
	}
	
	//distance squared used to remove any need to sqare root
	public Volume volumeFromDistanceSquared(double distanceSquared /*metres squared*/) {
		return Volume.fromIntensity ((this.Intensity/distanceSquared)*INTENSITY_DROPOFF_CONSTANT);
	}
}
