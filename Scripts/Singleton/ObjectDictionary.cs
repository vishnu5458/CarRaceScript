using UnityEngine;
using System.Collections;

public class ObjectDictionary : MonoBehaviour 
{
	private static ObjectDictionary _instance;
	public static ObjectDictionary instance
	{
		get
		{
			if(_instance == null)
				_instance = GameObject.FindObjectOfType<ObjectDictionary>();
			return _instance;
		}
	}

	public PowerUp[] _powerUpLibrary;
	public TrackPiece[] _trackPieceLibrary;
	public TrackProp[] _trackPropLibrary;

	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public GameObject GetPowerUpObject(string id)
	{
		foreach(PowerUp propPbj in _powerUpLibrary)
		{
			if(string.Compare(propPbj.powerUpId,id) == 0)
			{
				return propPbj.gameObject;
			}
		}
		return null;
	}

	public GameObject GetPropsForID(string id)
	{
		foreach(TrackProp propPbj in _trackPropLibrary)
		{
			if(string.Compare(propPbj.propId,id) == 0)
			{
				return propPbj.gameObject;
			}
		}
		return null;
	}

	public GameObject GetTrackPieceForID(string id)
	{
		foreach(TrackPiece def in _trackPieceLibrary)
		{
			if(string.Compare(def.trackId,id) == 0)
			{
				return def.gameObject;
			}
		}
		return null;
	} 
}
