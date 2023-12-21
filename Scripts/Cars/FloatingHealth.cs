using UnityEngine;
using System.Collections;

public class FloatingHealth : MonoBehaviour
{
	public Camera carCamera;
	public Camera guiCamera;
	public Transform target;
	public Renderer[] healthRenderer;

	private bool isEnabled;
	private bool isVisible;
	private Vector3 initPos;
	private Transform trans;
	private UIProgressBar healthBar;

	void Awake()
	{
		trans = transform;
		initPos = trans.position;
		healthBar = GetComponentInChildren<UIProgressBar>();
		healthRenderer = GetComponentsInChildren<Renderer>();
	}

	void Update ()
	{
		if(!target || !isEnabled)
			return;

		Vector3 wantedPos = guiCamera.ViewportToWorldPoint(carCamera.WorldToViewportPoint (target.position));
		if(wantedPos.z > 40 || !isVisible || wantedPos.z < -10)
		{
			foreach(Renderer _ren in healthRenderer)
				_ren.enabled = false;
		}
		else
		{
			foreach(Renderer _ren in healthRenderer)
				_ren.enabled = true;
			
			//		Debug.Log(wantedPos);
			wantedPos.y += 1.5f;
			trans.position = Vector3.Lerp(trans.position, wantedPos, Time.fixedDeltaTime * 10);
//			trans.position =(wantedPos);
		}


	}

	public void SetVisibility(bool status)
	{
		isVisible = status;
	}

	public void SetHealth(float percent)
	{
		healthBar.Percentage = percent;
	}

    public void InitFloatingHealth(CarBase _target)
	{
		_target.floatingHealthBar = this;
		isVisible = true;
		foreach(Renderer _ren in healthRenderer)
			_ren.enabled = true;
		isEnabled = true;

		if(_target.isDelayedStart)
		{
			isVisible = false;
		}
		SetHealth(1.0f);
		target = _target.trans;
	}

	public void Reset()
	{
		foreach(Renderer _ren in healthRenderer)
			_ren.enabled = false;

		SetHealth(0.0f);

		target = null;
		isEnabled = false;
		trans.position = initPos;
	}
}
