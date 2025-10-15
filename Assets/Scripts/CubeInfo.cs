using UnityEngine;

[System.Serializable]
public class CubeInfo
{
    private GameObject cubeGameObject;
    [SerializeField] public GameObject CubeGameObject
    {
		set { cubeGameObject = value; }
		get { return cubeGameObject; }
	}

	private Vector3 position;
	public Vector3 Position
	{
		set { position = value; }
		get { return position; }
	}

	private Color colour;
	public Color Colour
	{
		set { colour = value; }
		get { return colour; }
	}

	private Biomes biome;
	public Biomes Biome
	{
		set { biome = value; }
		get { return biome; }
	}

	public CubeInfo(GameObject _cubeGameObject, Vector3 _position, Color _colour, Biomes _biome)
	{
		cubeGameObject = _cubeGameObject;
		position = _position;
		colour = _colour;
		biome = _biome;
	}
	
}

public class CubeInfoGameobject : MonoBehaviour
{
	public CubeInfo cubeInfo;
}

