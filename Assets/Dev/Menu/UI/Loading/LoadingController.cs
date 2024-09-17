using System;
using System.Collections;
using System.Collections.Generic;
using DataCore;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingController : MonoSingleton<LoadingController>
{
    [SerializeField] private UILoading _loading;
    [SerializeField] private SceneLoader _sceneLoader;
    // Start is called before the first frame update
    private void Start()
    {
        InitData();
        ShowLoading();
    }
    private void InitData()
    {
        _loading.Init();
        InitCalendarCrashFix();    
    }

    public static void InitCalendarCrashFix()
    {
        try
        {
            // Two Letter ISO Language
            string strTwoLetterISOLanguage = System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            Debug.Log($"[CurrentCulture.strTwoLetterISOLanguage] {strTwoLetterISOLanguage}");
            if (strTwoLetterISOLanguage == "ar")
            {
                new System.Globalization.UmAlQuraCalendar();
            }
            else if (strTwoLetterISOLanguage == "th") {
                new System.Globalization.ThaiBuddhistCalendar();
            }
        }
        catch (Exception ex)
        {
            Debug.Log($"Failed InitCalendarCrashFix. {ex.Message}");
        }
    }

    private void ShowLoading()
    {
        _loading.Open();                
        AssetManager.Instance.Init(progress: (progress) => {
            var value = progress * 0.2f;
            _loading.UpdateProgress(value);
        }, completed: () =>
        {
            Debug.Log("Start Load ");
            StartCoroutine(LoadScene());
        });
    }

  
    IEnumerator LoadScene()
    {
        yield return new WaitForEndOfFrame();        
        
        var timer = 0;
        while (timer < 20)
        {
            timer += 1;
            var value = 0.2f + timer * 0.01f;
            _loading.UpdateProgress(value);
            yield return new WaitForSeconds(0.1f);
        }

        timer = 0;
        while (timer < 20)
        {
            timer += 1;
            var value = 0.4f + timer * 0.01f;
            _loading.UpdateProgress(value);
            yield return new WaitForSeconds(0.1f);
        }

        AsyncOperation asyncOperation = _sceneLoader.LoadRequestedSceneAsync();
        asyncOperation.allowSceneActivation = false;
        while (!asyncOperation.isDone)
        {
            var value = asyncOperation.progress * 0.2f + 0.6f;
            _loading.UpdateProgress(value);

            if (asyncOperation.progress >= 0.9f)
            {
                _loading.UpdateProgress(1f);
                yield return new WaitForSeconds(0.1f);
                asyncOperation.allowSceneActivation = true;
                yield break;                
            }

            yield return null;
        }
    }

}
