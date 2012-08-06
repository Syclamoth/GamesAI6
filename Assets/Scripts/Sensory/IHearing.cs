using UnityEngine;
using System.Collections;

public interface IHearing : ILocatable {
	Volume getHearingThreshold();
}
