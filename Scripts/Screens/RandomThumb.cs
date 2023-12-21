using UnityEngine;
using System.Collections;

public class RandomThumb : MonoBehaviour 
{
	Material thisMaterial;
	public Texture[] thumbnailTex;

	void Start()
	{
		thisMaterial = GetComponent<Renderer>().material;
	}

	public void RandomizeThumb()
	{
		thisMaterial.mainTexture = thumbnailTex[Random.Range(0, thumbnailTex.Length)];
	}

}
