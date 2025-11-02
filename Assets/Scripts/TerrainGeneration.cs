using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// This script makes the terrain
public class TerrainGeneration : MonoBehaviour
{

	public GameObject cubePrefab;

    [Header("Grid")]

    [SerializeField] int mapWidth;
	[SerializeField] int mapLength;

	[SerializeField][Range(0, .2f)] float increment;

	[SerializeField] [Range(0, 50)] int heightDifference;

	[SerializeField] List<CubeInfo> cubes = new();

    [SerializeField] LayerMask cubeLayerMask;

    [Header("BiomeData")]
    public AllBiomeData biomeData;
	[SerializeField] BiomeData biomeGenerationData;

    private int amountOfBiomes;

	private void Start()
	{
        amountOfBiomes = biomeData.BiomeList.Count;

		//GenerateCubes();
	}

	void GenerateCubes()
	{
		ClearCurrentCubes();

		Vector2 RandOffset = GetRandomOffset();

		cubes = CreateCubes();

        cubes = CreateVerticalCubes(cubes);

        ColourCubes();

		// NESTED FUNCTIONS FROM HERE

		void ClearCurrentCubes()
		{
            foreach (CubeInfo c in cubes)
            {
                Destroy(c.CubeGameObject);
            }

            cubes.Clear();
        }

		Vector2 GetRandomOffset()
		{
            int xRandOffset = Random.Range(-100, 100);
            int yRandOffset = Random.Range(-100, 100);

			return new Vector2(xRandOffset, yRandOffset);
        }

        List<CubeInfo> CreateCubes()
        {
            List<CubeInfo> tempCubes = new();

            for (int _width = 0; _width < mapWidth; _width++)
            {
                for (int _length = 0; _length < mapLength; _length++)
                {

                    //new Vector3(_width + RandOffset.x, _length + RandOffset.y, 0);

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

                    int height = (int)(Mathf.PerlinNoise((_width + RandOffset.x) * _tempIncrement, (_length + RandOffset.y) * _tempIncrement) * _tempHeightDifference);

                    Vector3 position = new Vector3(_width, height, _length);

					GameObject _cube = Instantiate(cubePrefab, position, Quaternion.identity);

                    CubeInfo newCubeInfo = new(_cube, position, _tempColor, _tempBiome);
					_cube.AddComponent<CubeInfoGameobject>().cubeInfo = newCubeInfo;

					tempCubes.Add(newCubeInfo);

					//float _colorMultiplier = 1 / (float)heightDifference;
					//mr.material.color = new Color(height * _colorMultiplier, height * _colorMultiplier, height * _colorMultiplier);
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
						print(i + " " + distanceToNextCube + " " + hit.point);

						cubesToAdd = distanceToNextCube;
                        //print(hit.point + " " + cubesToAdd);
                        
					}

					//if (hit.point == Vector3.zero) cubesToAdd = 0;
				}

				cubesToAddList.Add(cubesToAdd);

            }

			for (int i = 0; i < _cubes.Count; i++)
            {

                print(i + " " + cubesToAddList[i]);

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

            int biomeIndex = (int)(Mathf.PerlinNoise((pos.x + RandOffset.x) * biomeGenerationData.Increment, (pos.y + RandOffset.y) * biomeGenerationData.Increment) * amountOfBiomes);

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

        void ColourCubes()
        {
            foreach(CubeInfo cube in cubes)
            {
				MeshRenderer mr = cube.CubeGameObject.GetComponent<MeshRenderer>();
				mr.material.color = cube.Colour;
			}			
		}

    }

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			GenerateCubes();
		}
	}

}
