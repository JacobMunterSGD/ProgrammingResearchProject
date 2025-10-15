using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PickupCubes : MonoBehaviour
{

    [Header("Runtime Variables")]
    [SerializeField] List<GameObject> selection = new();

    [SerializeField] bool hasSelection;
    [SerializeField] Vector3 averageSelectionPosition;

    Vector3 lastMousePos;

    [Header("Parameters")]
    [SerializeField] int selectedRaiseHeight;
    [SerializeField] float minMouseMovement;

    [SerializeField] float selectionRadius;

    [SerializeField] float minSelectionRadius;
    [SerializeField] float maxSelectionRadius;

    [SerializeField] LayerMask selectionLayer;

    Transform currentRaycastHitSpot;

	[Header("Set Up References")]
    [SerializeField] Slider selectionRadiusSlider;

	private void Start()
	{
		hasSelection = false;
        selectionRadiusSlider.minValue = minSelectionRadius;
        selectionRadiusSlider.maxValue = maxSelectionRadius;
	}

	void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {

			GameObject hitGameObject = hit.transform.gameObject;

            // select new geometry
			if (Vector3.Distance(lastMousePos, Input.mousePosition) > minMouseMovement)
            {
                if (!hasSelection)
                {
					lastMousePos = Input.mousePosition;
					HighlightSelection(hitGameObject);
				}
                else
                {
                    MoveSelection(hitGameObject.transform.position);
				}
			}

			if (Input.GetMouseButtonDown(0))
            {
                if (selection.Count <= 0) return;

                if (hasSelection) PlaceSelection();
                else GrabSelection();
			}
            
		}
    }

    void GrabSelection()
    {
        hasSelection = true;

		foreach (GameObject GO in selection)
        {
            GO.transform.position += new Vector3(0, selectedRaiseHeight, 0);
        }
    }

    void PlaceSelection()
    {
		foreach (GameObject GO in selection)
		{
			RaycastHit _hit;
			float yValue = GO.transform.position.y;
			if (Physics.Raycast(GO.transform.position, Vector3.down, out _hit, Mathf.Infinity, selectionLayer))
			{
				yValue = _hit.transform.position.y + 1;
			}

			GO.transform.position = new Vector3(GO.transform.position.x, yValue, GO.transform.position.z);

		}

		hasSelection = false;
	}

    void MoveSelection(Vector3 _moveToSpot)
    {
        averageSelectionPosition = GetAverageSelectionPosition();

		Vector3 moveBy = _moveToSpot - averageSelectionPosition;
        moveBy.y = 0;

        foreach (GameObject GO in selection)
        {
            float yValue = GO.transform.position.y;
			RaycastHit _hit;
			if (Physics.Raycast(GO.transform.position, Vector3.down, out _hit, Mathf.Infinity, selectionLayer))
			{
				yValue = _hit.transform.position.y + selectedRaiseHeight;
			}

			GO.transform.position += moveBy;
            GO.transform.position = new Vector3(GO.transform.position.x, yValue, GO.transform.position.z);
            
		}
    }

    void HighlightSelection(GameObject hitGameObject)
    {
        if (selection.Count >= 0)
        {
            foreach (GameObject GO in selection)
            {
                ChangeHighlight(false, GO);
			}

            selection.Clear();
        }

        RaycastHit[] allSpheres = Physics.SphereCastAll(hitGameObject.transform.position, selectionRadius, Vector3.forward, selectionRadius, selectionLayer);
        currentRaycastHitSpot = hitGameObject.transform;

		foreach (RaycastHit hit in allSpheres)
		{
			GameObject _tempCube = hit.transform.gameObject;
			selection.Add(_tempCube);
			ChangeHighlight(true, _tempCube);
		}

		averageSelectionPosition = GetAverageSelectionPosition();
	}

    Vector3 GetAverageSelectionPosition()
    {
        Vector3 _tempAveragePos = Vector3.zero;

		foreach (GameObject GO in selection)
        {
            _tempAveragePos += GO.transform.position;
		}

		_tempAveragePos /= selection.Count;

        return _tempAveragePos;
	}

    void ChangeHighlight(bool isBeingHighlighted, GameObject _gameObject)
    {
        Color originalColour = _gameObject.GetComponent<CubeInfoGameobject>().cubeInfo.Colour;

        MeshRenderer mr = _gameObject.GetComponent<MeshRenderer>();

		if (isBeingHighlighted)
        {
            mr.material.color = new Color(originalColour.r + .2f, originalColour.g + .2f, originalColour.b + .2f);
		}
        else
        {
            mr.material.color = originalColour;
		}

    }

    public void ChangeSelectionRadius(float sliderValue)
    {
        selectionRadius = sliderValue;
    }

}