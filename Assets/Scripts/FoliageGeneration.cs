using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class FoliageGeneration : MonoBehaviour
{

	[Header("Grass")]

	[SerializeField] GameObject grassPrefab;

	[SerializeField] float grassSpacialIncrement;
    [Tooltip("The higher this number, the less grass will spawn")]
    [SerializeField] float grassChance;

	[SerializeField] float grassColourIncrement;
	[SerializeField] float grassColourNoiseMultiplier;
    [SerializeField] float grassRandomColourRange;

	public List<CubeInfo> MakeFoliage(List<CubeInfo> currentCubes, Vector2 perlinOffset)
    {
        List<CubeInfo> grassList = new();

        foreach(CubeInfo cube in currentCubes)
        {
            if (cube.Biome == Biomes.plains)
            {
                if (!IsGrassHere(cube.Position)) continue;

                GameObject _tempGrass = Instantiate(grassPrefab, cube.CubeGameObject.transform.position + new Vector3(0, 1, 0), Quaternion.identity);

				ColourBladesOfGrass(_tempGrass);

				Color _color = Color.green;

				grassList.Add(new CubeInfo(_tempGrass.gameObject, _tempGrass.gameObject.transform.position, _color, cube.Biome));


			}
		}

        bool IsGrassHere(Vector3 position)
		{
			int grassIndex = (int)(Mathf.PerlinNoise((position.x + perlinOffset.x) * grassSpacialIncrement * grassChance, (position.z + perlinOffset.y) * grassSpacialIncrement) * grassChance);

            if (grassIndex == 1) return true;
            else return false;
        }

        void ColourBladesOfGrass(GameObject cubeGO)
        {
            Vector3 position = cubeGO.transform.position;

            // find all the mesh renderer children of the parent object, then colour them randomly
            foreach (Transform child in cubeGO.transform)
            {
                if (child.TryGetComponent(out MeshRenderer _mr))
                {
                    float colourMultiplier = Mathf.PerlinNoise((position.x + perlinOffset.x) * grassColourIncrement, (position.z + perlinOffset.y) * grassColourIncrement) * grassColourNoiseMultiplier;

                    float ranColourMulitplier = Random.Range(0, grassRandomColourRange) - grassRandomColourRange/2;

                    Color startColour = _mr.material.color;

                    _mr.material.color = new Color(startColour.r * (colourMultiplier + ranColourMulitplier),
                                                   startColour.g * (colourMultiplier + ranColourMulitplier),
                                                   startColour.b * (colourMultiplier + ranColourMulitplier));
                }
            }
        }

		return grassList;
    }

}
