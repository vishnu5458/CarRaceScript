using UnityEngine;
using System.Collections;

public class GunScreenObject : MonoBehaviour 
{

	public int gunCost = 1000;

	private Renderer glowRenderer;
	// Use this for initialization
	void Start () 
	{
		GetComponentInChildren<TextMesh>().text = gunCost.ToString();
		glowRenderer = transform.GetChild(0).GetComponent<Renderer>();
	}

	public void OnSelected()
	{
		glowRenderer.enabled = true;
	}

	public void UnSelected()
	{
		glowRenderer.enabled = false;
	}

	public void SetCostRenderer(bool status)
	{
		GetComponentInChildren<TextMesh>().GetComponent<Renderer>().enabled = status;
	}
}
