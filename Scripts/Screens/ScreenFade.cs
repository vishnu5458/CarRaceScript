using UnityEngine;
using System.Collections;

public class ScreenFade : MonoBehaviour 
{
	Material thisMaterial;

	public float fadeRate = 0.01f;
//	public Renderer[] fontRenderer;
	
	public static ScreenFade screenFade;
	
	void Start () 
	{
		thisMaterial = GetComponent<Renderer>().material;
		screenFade = this;
	}
	
	public void ShowFadeInEffect()
	{
		StartCoroutine(FadeIn());
	}
	
	IEnumerator FadeIn()
	{
//		foreach(Renderer ren in fontRenderer)
//			ren.enabled = false;
		GetComponent<BoxCollider>().enabled = true;

		while(thisMaterial.color.a < 1)
		{
			Color tempColor = thisMaterial.color;
			tempColor.a += fadeRate;
			thisMaterial.color = tempColor;
			yield return null;
		}
//		yield return new WaitForSeconds(3);
////		Debug.Log("call here");
//		StartCoroutine(FadeOut());
	}

	public void ShowFadeOutEffect()
	{
		StartCoroutine(FadeOut());
	}

	IEnumerator FadeOut()
	{
		while(thisMaterial.color.a > 0)
		{
			Color tempColor = thisMaterial.color;
			tempColor.a -= fadeRate;
			thisMaterial.color = tempColor;
			yield return null;
		}

		GetComponent<BoxCollider>().enabled = false;
//		foreach(Renderer ren in fontRenderer)
//			ren.enabled = true;
	}
}
