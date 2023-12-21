using UnityEngine;
using System.Collections;

public class SimpleBlinker : MonoBehaviour 
{

	public bool isActive = false;
	public int blinkInterval = 3;
	
	private int blinkFlipTimer;
	private bool isInvisible = false;
	[SerializeField]
	private Renderer[] thisRenderers;
	
	void Start()
	{
		thisRenderers = GetComponentsInChildren<Renderer>();
	}
	
	void Update () 
	{
		if(isActive)
		{
			blinkFlipTimer--;
			if(blinkFlipTimer < 0)
			{
				if(isInvisible)
				{
					foreach(Renderer ren in thisRenderers)
						ren.enabled = true;
				
					isInvisible = false;
				}
				else
				{
					foreach(Renderer ren in thisRenderers)
						ren.enabled = false;
				
					isInvisible = true;
				}
				
				blinkFlipTimer = blinkInterval;
			}
		}
	}
	
	public void EnableBlinker()
	{
		if(isActive)
			return;

		blinkFlipTimer = blinkInterval;
		isActive = true;
	}
	
	public void DisableBlinker()
	{
		if(!isActive)
			return;

		isActive = false;
		foreach(Renderer ren in thisRenderers)
			ren.enabled = true;
	}
}
