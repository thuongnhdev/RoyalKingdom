using UnityEngine;
using UnityEngine.Events;

public class UIMenu : MonoBehaviour
{
	public Animator animator;

	public RectTransform transBG;

	public Canvas backCanvas;
	public Canvas frontCanvas;


	[SerializeField]
	protected bool bLoaded = true;

	[SerializeField]
	protected UnityEvent _onEnableEvent;

	public virtual void Start()
	{
		if (transBG)
		{
			transBG.sizeDelta = new Vector2(0f, transBG.rect.width);
		}
	}

	public bool IS_Loaded()
	{
		return bLoaded;
	}

	public virtual void Init()
	{
        if (backCanvas) backCanvas.worldCamera = CameraManager.Instance.CamUI_Back;
        if (frontCanvas) frontCanvas.worldCamera = CameraManager.Instance.CamUI_Base;
    }


	public virtual void Show()
	{
		gameObject.SetActive(true);
		_onEnableEvent?.Invoke();
	}

	public virtual void Hide()
	{
		gameObject.SetActive(false);
	}

	public virtual void SettingMenu()
	{

	}

	public virtual void OnReciveAnimationEvent(GameObject ob, string evname)
	{

	}
}
