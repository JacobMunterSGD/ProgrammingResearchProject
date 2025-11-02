using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AllBiomeData", menuName = "ScriptableObjects/AllBiomeDataScriptableObjects", order = 1)]
public class AllBiomeData : ScriptableObject
{
    public List<BiomeData> BiomeList;
}

[Serializable]
public class BiomeData
{
    [SerializeField] string name;

    [SerializeField] private Biomes biome;
    public Biomes Biome
    {
        set { biome = value; }
        get { return biome; }
    }

    [SerializeField][Range(0, .2f)] private float increment;
    public float Increment
    {
        get { return increment; }  
        set { increment = value; }
    }

    [SerializeField][Range(-20, 50)] private int heightDifference;
    public int HeightDifference
    {
        get { return heightDifference; }
        set { heightDifference = value; }
    }

    [SerializeField] Color color;
	public Color Color
	{
		get { return color; }
		set { color = value; }
	}

}

public enum Biomes
{
    noBiome,
    plains,
    mountains,
    wasteland,
    ocean,
    forest
}
