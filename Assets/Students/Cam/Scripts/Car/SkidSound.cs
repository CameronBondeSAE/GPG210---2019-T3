using UnityEngine;
using System.Collections;

public class SkidSound : MonoBehaviour
{
	public Wheel[] wheels;
	public AudioSource AudioSource;

	// Use this for initialization
	void Start()
	{
		AudioSource = GetComponent<AudioSource>();
	}

	// Update is called once per frame
	void Update()
	{
		float totalSkid = 0;
		foreach (Wheel wheel in wheels)
		{
			if (!wheel.inAir)
			{
				totalSkid += Mathf.Abs(wheel.localVelocity.x/20f);
			}
		}

		AudioSource.volume = totalSkid;
		AudioSource.pitch = 0.85f+totalSkid/10f;

	}
}
