using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnerFull : MonoBehaviour
{
	public int number;
	public GameObject prefab;
	public Vector3 velocitySpread;
	public Vector3 positionSpread;
	public MathsHelper.Range scaleRange;
	public MathsHelper.Range spinRange;
	public float sizeAnimSpeedScalar;
	public bool autoSpawn = true;
	public float spawnDelay;
	public bool autoDestroy = false;
	public float destroyDelay = 2;

	// Use this for initialization

	public void Start()
	{
		if(autoSpawn)
		{
			if(spawnDelay < 0.01f) // No delay, don't invoke
			{
				Spawn();
			}
			else
			{
				Invoke("Spawn", spawnDelay); //Spawn();
			}
		}
	}

	public void Spawn(object o, EventArgs e)
	{
		Spawn(GetComponent<Transform>());
	}
	public void Spawn()
	{
		Spawn(GetComponent<Transform>());
	}
	public void Spawn(Transform spawnerParent)
	{
		// HACK: Disable
//		return;

		for(int i = 0; i < number; i++)
		{
			var go = Instantiate(prefab, transform.position + spawnerParent.TransformDirection(new Vector3(i*2 + Random.Range(0f, positionSpread.x), Random.Range(-positionSpread.y, positionSpread.y), Random.Range(0, positionSpread.z))), transform.rotation) as GameObject;
			//			go.rigidbody.velocity = new Vector3(Random.Range(0, velocitySpread.x), Random.Range(0, velocitySpread.y), 0);

			go.transform.localScale = MathsHelper.RandomVector3(scaleRange.min, scaleRange.max); // Check: Probably doesn't result in nice glass shards

			float size; // Size is never higher than 1. So 1 is the biggest shard piece.
			size = go.transform.localScale.sqrMagnitude/2;
			if(size > 1)
				size = 1;

//			var randomAnimatorSpeed = go.GetComponent<RandomAnimatorSpeed>();
//			if(randomAnimatorSpeed)
//			{
//				randomAnimatorSpeed.RandomiseSpeed((1 - size)*sizeAnimSpeedScalar, (1 - size)*sizeAnimSpeedScalar); // Inverted the size because SMALL things should spin faster and vice versa
//			}

			var body = go.GetComponent<Rigidbody>();
			if(body)
			{
				float xVelocity = velocitySpread.x*(1 - size/1.25f);
				float yVelocity = Random.Range(0, velocitySpread.y*(1 - size));
				body.mass = size*10;
				body.velocity = new Vector3(xVelocity, yVelocity, 0);
				body.angularVelocity = new Vector3(0, 0, Random.Range(spinRange.min, spinRange.max)); // Check: Allocs
			}

			//			go.SetActive(false); // HACK to test why offscreen things are so slow.

			go.transform.parent = transform.parent;

			if (autoDestroy)
			{
				Destroy(go, destroyDelay);
			}
		}
	}
}
