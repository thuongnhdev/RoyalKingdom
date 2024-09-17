using UnityEngine;

public class UIPopup : MonoBehaviour
{
	public Animator animator;

	public Canvas canvas;

	[SerializeField]
	protected bool bLoaded = true;

	public virtual void Init()
	{
		this.transform.localPosition = Vector3.zero;
		this.transform.localScale = Vector3.one;

		if (canvas) canvas.worldCamera = CameraManager.Instance.CamUI_Base;
	}

	public virtual void Show()
	{
		gameObject.SetActive(true);
	}

	public virtual void Hide()
	{
		gameObject.SetActive(false);
	}

	public virtual void SettingPopup()
    {

    }

	public bool IS_Loaded()
	{
		return bLoaded;
	}
}
