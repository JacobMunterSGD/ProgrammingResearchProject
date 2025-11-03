using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AllBiomeData", menuName = "ScriptableObjects/AllBiomeDataScriptableObjects", order = 1)]
public class AllBiomeData : ScriptableObject
{
    public List<BiomeData> BiomeList;
}

[CreateAssetMenu(fileName = "AllSubBiomeData", menuName = "ScriptableObjects/AllSubBiomeDataScriptableObjects", order = 2)]
public class AllSubBiomeData : ScriptableObject
{
	public List<SubBiomeData> SubBiomeList;
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

[Serializable]
public class SubBiomeData
{
	[SerializeField] string name;

	[SerializeField] private SubBiomes subBiome;
	public SubBiomes SubBiome
	{
		set { subBiome = value; }
		get { return subBiome; }
	}

	[SerializeField][Range(-10, 10)] private int heightChange;
	public int HeightChange
	{
		get { return heightChange; }
		set { heightChange = value; }
	}

	[SerializeField] Vector3 colorDifference;
	public Vector3 ColorDifference
	{
		get { return colorDifference; }
		set { colorDifference = value; }
	}

	[SerializeField] float chance;
	public float Chance
	{
		get { return chance; }
		set { chance = value; }
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

public enum SubBiomes
{
	none,
    hill,
    trench,
    spire
}
