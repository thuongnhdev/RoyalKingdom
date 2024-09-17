using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[System.Serializable]
public class PopupInfo
{
	public E_MenuName parentMenu;
	public E_PopupName showPopup;
}

public class UIManager : MonoBehaviour
{
	public static UIManager Instance = null;
	
	public bool bPreLoading = false;

	public List<E_MenuName> preLoadMenu;
	public List<E_PopupName> preLoadPopup;

	public E_MenuName startMenu;

	[Header("Menu")]
	public UIMenuDic uiMenuDic;

	[Header("Popup")]
	public UIPopupDic uiPopupDic;

	private E_MenuName curShowMenu = E_MenuName.None;
	private E_PopupName curShowPopup = E_PopupName.None;

	[SerializeField]
	private MainMenuBtn mainMenuBtn;
	[SerializeField]
	private List<PopupInfo> beforePopup = new List<PopupInfo>();
	[SerializeField]
	private List<E_MenuName> beforeMenu = new List<E_MenuName>();

	void Awake()
	{
		Instance = this;
	}
	void OnDestroy() 
	{ 
		Instance = null; 
	}
	
	public IEnumerator CoPreLoadMenu(E_MenuName MenuKind)
	{
		if (uiMenuDic.ContainsKey(MenuKind) == false)
		{
			AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(MenuKind.ToString(), transform);

			while (!handle.IsDone)
			{
				yield return null;
			}

			if (handle.Status == AsyncOperationStatus.Succeeded)
			{
				UIMenu loadMenu = handle.Result.GetComponent<UIMenu>();
				loadMenu.Init();

				yield return new WaitUntil(() => loadMenu.IS_Loaded() == true);

				loadMenu.Hide();
				uiMenuDic.Add(MenuKind, loadMenu);
			}
		}
	}

	public IEnumerator CoPreLoadPopup(E_PopupName PopupKind)
	{
		if (uiPopupDic.ContainsKey(PopupKind) == false)
		{
			AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(PopupKind.ToString(), transform);

			while (!handle.IsDone)
			{
				yield return null;
			}

			if (handle.Status == AsyncOperationStatus.Succeeded)
			{
				UIPopup loadPopup = handle.Result.GetComponent<UIPopup>();
				loadPopup.Init();

				yield return new WaitUntil(() => loadPopup.IS_Loaded() == true);

				loadPopup.Hide();
				uiPopupDic.Add(PopupKind, loadPopup);
			}
		}
	}

	public IEnumerator ShowMenu(E_MenuName MenuKind)
	{
		if (curShowMenu == MenuKind)
			yield break;

		curShowMenu = MenuKind;

		if (mainMenuBtn == null)
		{
			AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync("MainMenuBtn", transform);

			while (!handle.IsDone)
			{
				yield return null;
			}

			if (handle.Status == AsyncOperationStatus.Succeeded)
			{
				mainMenuBtn = handle.Result.GetComponent<MainMenuBtn>();
				mainMenuBtn.Init();
			}
		}

		if (uiMenuDic.ContainsKey(MenuKind) == false)
		{
			BaseManager.Instance.ActiveLockUI(true);

			AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(MenuKind.ToString(), transform);

			while (!handle.IsDone)
			{
				yield return null;
			}

			if (handle.Status == AsyncOperationStatus.Succeeded)
			{
				UIMenu loadMenu = handle.Result.GetComponent<UIMenu>();
				loadMenu.Init();
				uiMenuDic.Add(MenuKind, loadMenu);
			}

			BaseManager.Instance.ActiveLockUI(false);
		}

		UIMenu menu = uiMenuDic[MenuKind];
		menu.SettingMenu();

		for (int i = 0; i < beforeMenu.Count; ++i)
		{
			BeforeMenuHide(beforeMenu[i]);
		}
		
		if (MenuKind == E_MenuName.TempMenu_1 || MenuKind == E_MenuName.TempMenu_2)
		{
			beforeMenu.Clear();
			mainMenuBtn.Show();
		}
		else
		{
			mainMenuBtn.Hide();
		}

		if (beforeMenu.Contains(MenuKind) == false)
		{
			beforeMenu.Add(MenuKind);
		}

		menu.Show();
	}

	public void HideMenu(E_MenuName MenuKind)
	{
		int hideIndex = beforeMenu.IndexOf(MenuKind);
		beforeMenu.RemoveAt(hideIndex);

		if (uiMenuDic.ContainsKey(MenuKind))
		{
			uiMenuDic[MenuKind].Hide();
		}

		if (beforeMenu.Count > 0)
		{
			if (MenuKind == E_MenuName.TempMenu_1 && beforeMenu[beforeMenu.Count - 1] == E_MenuName.TempMenu_2)
			{
				//(uiMenuDic[beforeMenu[beforeMenu.Count - 1]] as InventoryMenu).bRefreshTab = true;

			}
			StartCoroutine(ShowMenu(beforeMenu[beforeMenu.Count - 1]));
		}
	}

	void BeforeMenuHide(E_MenuName MenuKind)
	{
		if (uiMenuDic.ContainsKey(MenuKind))
		{
			uiMenuDic[MenuKind].Hide();
		}
	}

	public IEnumerator ShowPopup(E_PopupName PopupKind)
	{
		//if (curShowPopup == PopupKind)
		//	yield break;

		curShowPopup = PopupKind;

		PopupInfo info = new PopupInfo();
		info.showPopup = PopupKind;
		info.parentMenu = curShowMenu;
		beforePopup.Add(info);


		if (uiPopupDic.ContainsKey(PopupKind) == false)
		{
			BaseManager.Instance.ActiveLoadingSimple(true);

			AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(PopupKind.ToString(), transform);

			while (!handle.IsDone)
			{
				yield return null;
			}

			if (handle.Status == AsyncOperationStatus.Succeeded)
			{
				UIPopup loadPopup = handle.Result.GetComponent<UIPopup>();
				loadPopup.Init();
				uiPopupDic.Add(PopupKind, loadPopup);
			}

			BaseManager.Instance.ActiveLoadingSimple(false);
		}

		UIPopup popup = uiPopupDic[PopupKind];
		popup.SettingPopup();

		popup.Show();
	}

	public void HidePopup(E_PopupName PopupKind)
	{
		if (PopupKind == curShowPopup)
		{
			if (beforePopup.Count > 0)
			{
				int hideIndex = -1;
				for (int i = 0; i < beforePopup.Count; ++i)
				{
					if (beforePopup[i].showPopup == PopupKind)
					{
						hideIndex = i;
						break;
					}
				}
				if (hideIndex != -1)
					beforePopup.RemoveAt(hideIndex);
			}
		}

		if (uiPopupDic.ContainsKey(PopupKind))
		{
			uiPopupDic[PopupKind].Hide();
		}
	}

	public void NowMenuHide()
	{
		HideMenu(curShowMenu);
	}

	public void AllMenuHide()
	{
		foreach (UIMenu menu in uiMenuDic.Values)
		{
			menu.Hide();
		}

		beforeMenu.Clear();

		if (mainMenuBtn) mainMenuBtn.Hide();

		curShowMenu = E_MenuName.None;
	}

	public T GetMenu<T>(E_MenuName eMenu) where T : UIMenu
	{
		if (uiMenuDic.ContainsKey(eMenu))
		{
			return uiMenuDic[eMenu] as T;
		}
		else
		{
			return null;
		}
	}

	public T GetPopup<T>(E_PopupName ePopup) where T : UIPopup
	{
		if (uiPopupDic.ContainsKey(ePopup))
		{
			return uiPopupDic[ePopup] as T;
		}
		else
		{
			return null;
		}
	}
}
