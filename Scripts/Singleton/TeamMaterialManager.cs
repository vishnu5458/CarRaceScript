using UnityEngine;
using System.Collections;

[System.Serializable]
public class ModelClass
{
    public Material[] carTexture;
//    public Material[] garageMaterial;
}


[System.Serializable]
public class TeamMaterialClass
{
    public ModelClass[] modelClass;

}

public class TeamMaterialManager : MonoBehaviour 
{
	
	#region Singleton
	private static TeamMaterialManager _instance;
	public static TeamMaterialManager instance {
		get {
			if (_instance == null) {
				_instance = GameObject.FindObjectOfType<TeamMaterialManager> ();
			}
			return _instance;
		}
	}
	#endregion

	public TeamMaterialClass[] teamTextureArray;
}
