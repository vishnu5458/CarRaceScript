using UnityEngine;
using System.Collections;

public enum PanelObjectType
{
	CarPaint,
	RimPaint
}

[RequireComponent (typeof (BoxCollider))]
public class CarPanelObject : MonoBehaviour 
{
	public PanelObjectType thisType;
	public string id;
	public Texture corresTexture;

}
