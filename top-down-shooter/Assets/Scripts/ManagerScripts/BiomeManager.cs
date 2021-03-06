using Assets.Scripts.Components;
using ProceduralNoiseProject;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeManager : MonoBehaviour
{
	public enum Biome
	{
		Void, Planes, Swamp, Forest, Hell
	}
	public EnvironmentGenerationManager evManager;
	public float spawnSafeRadius = 100;
	[Header("Simplex Noise")]
	public float pFrequency = 5f;
	public float pAmplitude = 2f;
	public float pScale = 1f;
	public Vector2 pOffset;
	[Header("Biome Thresholds")]
	public float hellThresh = 1.3f;
	public float forestThresh = 0.3f;
	public float planesThresh = -1f;
	public float swampThresh = -2f;
	[Header("Other variables")]
	public float biomeVariation = 1;
	[ReadOnly] [SerializeField] private float maxNoiseValue = float.MinValue;
	[ReadOnly] [SerializeField] private float minNoiseValue = float.MaxValue;
	// Procedural generators
	private SimplexNoise simplexNoise;
	void Awake()
	{
		UnityEngine.Random.InitState(GlobalVariables.seed);
		// regionCount = Enum.GetNames(typeof(Biome)).Length;
		simplexNoise = new SimplexNoise(GlobalVariables.seed, pFrequency, pAmplitude);
	}

	public Biome GetBiomeAt(Vector3 position)
	{
		// Convert x and y to noise space
		float xCoord = position.x / evManager.textureSize.x;
		float zCoord = position.z / evManager.textureSize.y;
		// Get noise sample
		float noiseSample = simplexNoise.Sample2D((xCoord + pOffset.x) * pScale, (zCoord + pOffset.y) * pScale);
		// Check for range
		if (noiseSample < minNoiseValue)
			minNoiseValue = noiseSample;
		if (noiseSample > maxNoiseValue)
			maxNoiseValue = noiseSample;
		// Lerp from position magnitude to noise values
		float lerpValue = Mathf.Lerp(0, noiseSample, position.magnitude.Map(0, spawnSafeRadius * 2, 0, 1));
		// Determine biome
		if (lerpValue > hellThresh)
			return Biome.Hell;
		if (lerpValue > forestThresh)
			return Biome.Forest;
		if (lerpValue > planesThresh)
			return Biome.Planes;
		if (lerpValue >= swampThresh)
			return Biome.Swamp;
		return Biome.Planes;
	}
}
