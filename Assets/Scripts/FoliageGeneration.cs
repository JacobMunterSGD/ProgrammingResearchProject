using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class FoliageGeneration : MonoBehaviour
{
    [Header("References")]
    [SerializeField] AllBiomeData biomeData;

    [Header("General Parameters")]
	[SerializeField] float foliageSpacialIncrement;

    [Header("Prefabs")]
	[SerializeField] GameObject grassPrefab;
	[SerializeField] GameObject flowerPrefab;
	[SerializeField] GameObject bushPrefab;

	[Tooltip("The higher this number, the less foliage will spawn")]
	[SerializeField] float spawnChanceModifier;
	[SerializeField] float numberOfPrefabs;

	[Header("Grass")]
	[SerializeField] float grassColourIncrement;
	[SerializeField] float grassColourNoiseMultiplier;
    [SerializeField] float grassRandomColourRange;

    public List<CubeInfo> MakeFoliage(List<CubeInfo> currentCubes, Vector2 perlinOffset)
    {
        List<CubeInfo> foliageList = new();

        foreach (CubeInfo cube in currentCubes)
        {
            if (!DoesBiomeContainFoliage(cube.Biome)) continue;

			GameObject _tempGrass = Instantiate(GetFoliagePrefab(cube.Position, cube.Biome), cube.CubeGameObject.transform.position + new Vector3(0, 1, 0), Quaternion.identity);

            ColourFoliageOfGrass(_tempGrass);

            Color _color = Color.green;

            foliageList.Add(new CubeInfo(_tempGrass, _tempGrass.transform.position, _color, cube.Biome));
            
        }

		return foliageList;

        // NESTED FUNCTIONS

        GameObject GetFoliagePrefab(Vector3 position, Biomes biome)
        {
			int index = (int)(Mathf.PerlinNoise(
                (position.x + perlinOffset.x) * foliageSpacialIncrement, (position.z + perlinOffset.y) * foliageSpacialIncrement)
                * (numberOfPrefabs + spawnChanceModifier));

            BiomeData currentBiome = GetBiomeDataFromBiome(biome);

			switch (index)
            {
                case 3:
                    if (!CheckIfBiomeHasFoliage(currentBiome, FoliageTypes.grass)) return new();
                    return grassPrefab;
                case 4:
					if (!CheckIfBiomeHasFoliage(currentBiome, FoliageTypes.flower)) return new();
					return flowerPrefab;
                case 5:
					if (!CheckIfBiomeHasFoliage(currentBiome, FoliageTypes.bush)) return new();
					return bushPrefab;

                default:
					return new();

			}

            bool CheckIfBiomeHasFoliage(BiomeData biomeData, FoliageTypes folType)
            {
                bool containsThisFoliage = false;

                foreach (FoliageTypes _folType in biomeData.FoliageTypes)
                {
                    if (_folType == folType)
                    {
                        containsThisFoliage = true;
                        break;
					}
                }

                return containsThisFoliage;
			}
			
		}

        void ColourFoliageOfGrass(GameObject cubeGO)
        {
            Vector3 position = cubeGO.transform.position;

            // find all the mesh renderer children of the parent object, then colour them randomly
            foreach (Transform child in cubeGO.transform)
            {
                if (child.TryGetComponent(out MeshRenderer _mr))
                {
                    float colourMultiplier = Mathf.PerlinNoise((position.x + perlinOffset.x) * grassColourIncrement, (position.z + perlinOffset.y) * grassColourIncrement) * grassColourNoiseMultiplier;

                    float ranColourMulitplier = Random.Range(0, grassRandomColourRange) - grassRandomColourRange / 2;

                    Color startColour = _mr.material.color;

                    _mr.material.color = new Color(startColour.r * (colourMultiplier + ranColourMulitplier),
                                                   startColour.g * (colourMultiplier + ranColourMulitplier),
                                                   startColour.b * (colourMultiplier + ranColourMulitplier));
                }
            }
        }

        bool DoesBiomeContainFoliage(Biomes _currentBiome)
        {
            BiomeData _currentBiomeData = GetBiomeDataFromBiome(_currentBiome);
            return _currentBiomeData.ContainsFoliage;
		}

        BiomeData GetBiomeDataFromBiome(Biomes _wantedBiome)
        {
            foreach(BiomeData _biomeData in biomeData.BiomeList)
            {
                if (_biomeData.Biome == _wantedBiome) return _biomeData;
			}

            return biomeData.BiomeList[0];
		}
    }

}
