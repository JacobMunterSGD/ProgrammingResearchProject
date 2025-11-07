using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class ChunkLoader : MonoBehaviour
{

    [SerializeField] TerrainGeneration terGenScript;

    Vector3 currentChunkOffset;

	[SerializeField] List<Vector3> createdChunkPositions = new();

	[SerializeField] Transform chunkPosSignifer;
	[SerializeField] Vector3 signifierOffset;

	int chunkWidth;
	int chunkLength;

	private void Start()
	{
		chunkWidth = terGenScript.MapWidth;
		chunkLength = terGenScript.MapLength;
		chunkPosSignifer.position = currentChunkOffset + signifierOffset + new Vector3(chunkWidth / 2, 0, chunkLength / 2);
	}

	void Update()
    {
		if (Input.GetKeyDown(KeyCode.Space))
		{
			createdChunkPositions.Clear();
			currentChunkOffset = Vector3.zero;
			createdChunkPositions.Add(currentChunkOffset);
			terGenScript.GenerateCubes(true, currentChunkOffset);

			chunkPosSignifer.position = currentChunkOffset + signifierOffset + new Vector3(chunkWidth / 2, 0, chunkLength / 2);
		}

		if (Input.GetKeyDown(KeyCode.RightArrow))
        {
			CreateNewChunkAtPoint(new Vector3(chunkWidth, 0, 0));			
		}
		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			CreateNewChunkAtPoint(new Vector3(-chunkWidth, 0, 0));
		}
		if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			CreateNewChunkAtPoint(new Vector3(0, 0, -chunkLength));
		}
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			CreateNewChunkAtPoint(new Vector3(0, 0, chunkLength));
		}
	}

	void CreateNewChunkAtPoint(Vector3 directionMoving)
	{
		currentChunkOffset += directionMoving;

		bool isChunkOverlapping = false;

		foreach(Vector3 chunkPos in createdChunkPositions)
		{
			if (chunkPos == currentChunkOffset) isChunkOverlapping = true;
		}

		if (!isChunkOverlapping)
		{
			createdChunkPositions.Add(currentChunkOffset);

			terGenScript.GenerateCubes(false, currentChunkOffset);
		}

		chunkPosSignifer.position = currentChunkOffset + signifierOffset + new Vector3(chunkWidth/2, 0, chunkLength/2);


	}

}
