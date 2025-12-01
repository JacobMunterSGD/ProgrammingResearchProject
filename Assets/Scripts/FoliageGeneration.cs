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
	[SerializeField] GameObject house1Prefab;

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

        print(currentCubes.Count);

        foreach (CubeInfo cube in currentCubes)
        {
            if (!DoesBiomeContainFoliage(cube.Biome)) continue;

            FoliageTypes folType = FoliageTypes.none;

			GameObject _foliageGO = Instantiate(GetFoliagePrefab(cube.Position, cube.Biome, out folType), cube.CubeGameObject.transform.position + new Vector3(0, 1, 0), Quaternion.identity);
          
            if (folType == FoliageTypes.house)
            {
                RandomizeHouseRotation(_foliageGO);
			}
            else
            {
				RandomizeColour(_foliageGO);
			}

            foliageList.Add(new CubeInfo(_foliageGO, _foliageGO.transform.position, new(), cube.Biome));
            
        }

		return foliageList;

        // NESTED FUNCTIONS

        GameObject GetFoliagePrefab(Vector3 position, Biomes biome, out FoliageTypes foliageType)
        {
			int index = (int)(Mathf.PerlinNoise(
                (position.x + perlinOffset.x) * foliageSpacialIncrement, (position.z + perlinOffset.y) * foliageSpacialIncrement)
                * (numberOfPrefabs + spawnChanceModifier));

            BiomeData currentBiome = GetBiomeDataFromBiome(biome);

			foliageType = FoliageTypes.none;

			switch (index)
            {
                case 2:
                    if (!CheckIfBiomeHasFoliage(currentBiome, FoliageTypes.grass)) return new();
                    foliageType = FoliageTypes.grass;
					return grassPrefab;
                case 3:
					if (!CheckIfBiomeHasFoliage(currentBiome, FoliageTypes.flower)) return new();
					foliageType = FoliageTypes.flower;
					return flowerPrefab;
                case 8:
					if (!CheckIfBiomeHasFoliage(currentBiome, FoliageTypes.bush)) return new();
					foliageType = FoliageTypes.bush;
					return bushPrefab;
                case 9:
                    if (Random.Range(0, 4) != 1) return new();
					if (!CheckIfBiomeHasFoliage(currentBiome, FoliageTypes.house)) return new();
                    foliageType = FoliageTypes.house;
					return house1Prefab;

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

        void RandomizeColour(GameObject cubeGO)
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

        void RandomizeHouseRotation(GameObject houseGO)
        {
            int ran = Random.Range(1, 4);

            switch (ran)
            {
                case 1:
                    houseGO.transform.eulerAngles = new Vector3(0, 0, 0);
                    break;
				case 2:
					houseGO.transform.eulerAngles = new Vector3(0, 90, 0);
					break;
				case 3:
					houseGO.transform.eulerAngles = new Vector3(0, 180, 0);
					break;
				case 4:
					houseGO.transform.eulerAngles = new Vector3(0, 270, 0);
					break;
			}
        }
    }

}
