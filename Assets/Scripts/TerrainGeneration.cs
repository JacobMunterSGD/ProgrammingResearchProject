using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// This script makes the terrain
public class TerrainGeneration : MonoBehaviour
{

	public GameObject cubePrefab;

    Vector2 perlinNoiseOffset = new();

    [SerializeField] FoliageGeneration folGenScript;

    [Header("Grid")]

    [SerializeField] int mapWidth;
	public int MapWidth
	{
		set { mapWidth = value; }
		get { return mapWidth; }
	}
	[SerializeField] int mapLength;
	public int MapLength
	{
		set { mapLength = value; }
		get { return mapLength; }
	}

	[SerializeField][Range(0, .2f)] float increment;

	[SerializeField] [Range(0, 50)] int heightDifference;

	[SerializeField] List<CubeInfo> cubes = new();

    [SerializeField] LayerMask cubeLayerMask;

    [Header("BiomeData")]
    public AllBiomeData biomeData;
	[SerializeField] BiomeData biomeGenerationSpecification;

    [Header("Sub Biome Data")]
    public AllSubBiomeData subBiomeData;
	[SerializeField] BiomeData subBiomeGenerationSpecification;

	private int amountOfBiomes;

	private void Start()
	{
        amountOfBiomes = biomeData.BiomeList.Count;

		//GenerateCubes();
	}

	public void GenerateCubes(bool startingOver, Vector3 chunkOffset)
	{
        List<CubeInfo> generatedCubes = new();

        if (startingOver)
        {
			ClearCurrentCubes();
			perlinNoiseOffset = GetRandomOffset();
		}

		List<CubeInfo> horizontalCubes = CreateHorizontalCubes();
		ColourCubes(horizontalCubes);

		List<CubeInfo> foliageCubes = folGenScript.MakeFoliage(horizontalCubes, perlinNoiseOffset);

		List<CubeInfo> verticalCubes = CreateVerticalCubes(horizontalCubes);
		ColourCubes(verticalCubes);

		generatedCubes.AddRange(horizontalCubes);
        generatedCubes.AddRange(verticalCubes);
        generatedCubes.AddRange(foliageCubes);

		AddGeneratedCubesToMainList();

		// NESTED FUNCTIONS FROM HERE

		void ClearCurrentCubes()
		{
            foreach (CubeInfo c in cubes)
            {
                Destroy(c.CubeGameObject);
            }

            cubes.Clear();
        }

        void AddGeneratedCubesToMainList()
        {
            cubes.AddRange(generatedCubes);
        }

		Vector2 GetRandomOffset()
		{
            int xRandOffset = Random.Range(-100, 100);
            int yRandOffset = Random.Range(-100, 100);

			return new Vector2(xRandOffset, yRandOffset);
        }

        List<CubeInfo> CreateHorizontalCubes()
        {
            List<CubeInfo> tempCubes = new();

            for (int _width = (int)chunkOffset.x; _width < (int)chunkOffset.x + mapWidth; _width++)
            {
                for (int _length = (int)chunkOffset.z; _length < (int)chunkOffset.z + mapLength; _length++)
                {

                    // GET BIOME INFO

                    Biomes _tempBiome = GetBiome(new Vector2(_length, _width));

                    float _tempIncrement = increment;
                    int _tempHeightDifference = heightDifference;
                    Color _tempColor = Color.white;

                    foreach (BiomeData _BiomeData in biomeData.BiomeList)
                    {
                        if (_BiomeData.Biome == _tempBiome)
                        {
                            _tempIncrement = _BiomeData.Increment;
                            _tempHeightDifference = _BiomeData.HeightDifference;
                            _tempColor = _BiomeData.Color;
                            break;
						}
                    }

					// ADD SUBBIOME

					SubBiomeData _tempSubBiome = GetSubBiome(new Vector2(_length, _width));

                    int heightAddition = _tempSubBiome.HeightChange;
                    Vector3 colorDifference = _tempSubBiome.ColorDifference / 255;

					int height = (int)(Mathf.PerlinNoise((_width + perlinNoiseOffset.x) * _tempIncrement, (_length + perlinNoiseOffset.y) * _tempIncrement) * _tempHeightDifference) + heightAddition;

                    Vector3 position = new Vector3(_width, height, _length);

					GameObject _cube = Instantiate(cubePrefab, position, Quaternion.identity);

                    _tempColor = new Color(_tempColor.r + colorDifference.x, _tempColor.g + colorDifference.y, _tempColor.b + colorDifference.z);

                    CubeInfo newCubeInfo = new(_cube, position, _tempColor, _tempBiome);
					_cube.AddComponent<CubeInfoGameobject>().cubeInfo = newCubeInfo;

					tempCubes.Add(newCubeInfo);
				}
			}

            return tempCubes;
        }

        // go through each cube, check how far down the cubes next to it are, then create cubes below this one by how many that is.
        // This will ensure that there are no gaps below/above cubes

		List<CubeInfo> CreateVerticalCubes(List<CubeInfo> _cubes)
        {
            List<CubeInfo> _verticalCubes = new();

            List<int> cubesToAddList = new();

            for (int i = 0; i < _cubes.Count; i++)
            {
                CubeInfo cube = _cubes[i];

                int cubesToAdd = 0;

                for (int n = 0; n < 4; n++)
                {
                    float angleInRadians = n * 90 * Mathf.Deg2Rad;
                    Vector3 directionVector = new Vector3(Mathf.Cos(angleInRadians), 0, Mathf.Sin(angleInRadians));

                    Vector3 raycastStartPoint = cube.CubeGameObject.transform.position + directionVector;

                    RaycastHit hit;
                    Physics.Raycast(raycastStartPoint, Vector3.down, out hit, Mathf.Infinity, cubeLayerMask);

                    int distanceToNextCube = (int)Mathf.Abs(raycastStartPoint.y - hit.point.y) + 1;

                    if (distanceToNextCube > cubesToAdd && !(hit.point == Vector3.zero))
                    {
						//print(i + " " + distanceToNextCube + " " + hit.point);

						cubesToAdd = distanceToNextCube;
                        //print(hit.point + " " + cubesToAdd);
                        
					}

					//if (hit.point == Vector3.zero) cubesToAdd = 0;
				}

				cubesToAddList.Add(cubesToAdd);

            }

			for (int i = 0; i < _cubes.Count; i++)
            {

                //print(i + " " + cubesToAddList[i]);

				CubeInfo cube = _cubes[i];

				for (int n = 1; n < cubesToAddList[i] + 1; n++)
				{
					Vector3 positionOffset = new Vector3(0, -n, 0);

					GameObject _cube = Instantiate(cubePrefab, cube.CubeGameObject.transform.position + positionOffset, Quaternion.identity);

					CubeInfo newCubeInfo = new(_cube, _cube.transform.position, cube.Colour, cube.Biome);
					_cube.AddComponent<CubeInfoGameobject>().cubeInfo = newCubeInfo;

					_verticalCubes.Add(newCubeInfo);
				}

				foreach (CubeInfo _cube in _cubes)
				{
					_verticalCubes.Add(_cube);
				}
			}      

            return _verticalCubes;
        }

        Biomes GetBiome(Vector2 pos)
		{

            int biomeIndex = (int)(Mathf.PerlinNoise((pos.x + perlinNoiseOffset.x) * biomeGenerationSpecification.Increment, (pos.y + perlinNoiseOffset.y) * biomeGenerationSpecification.Increment) * amountOfBiomes);

            foreach (BiomeData _biomeData in biomeData.BiomeList)
            {
                if (biomeData.BiomeList[biomeIndex] == _biomeData)
                {
                    return _biomeData.Biome;
				}
            }

            print("something's not working if we're here");
            return Biomes.plains;
        }

		SubBiomeData GetSubBiome(Vector2 pos)
        {
			float subBiomeIndex = (Mathf.PerlinNoise((pos.x + perlinNoiseOffset.x) * subBiomeGenerationSpecification.Increment, (pos.y + perlinNoiseOffset.y) * subBiomeGenerationSpecification.Increment) * subBiomeData.SubBiomeList.Count);

			SubBiomeData _tempSubBiomeData = subBiomeData.SubBiomeList[(int)subBiomeIndex];

            return _tempSubBiomeData;
        }

        void ColourCubes(List<CubeInfo> currentCubes)
        {
            foreach(CubeInfo cube in currentCubes)
            {
                if (cube.CubeGameObject.TryGetComponent<MeshRenderer>(out MeshRenderer mr))
                {
					mr.material.color = cube.Colour;
				}
			}			
		}

    }

}
