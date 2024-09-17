using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;


public class CameraManager : MonoBehaviour
{
    #region Singleton
    public static CameraManager Instance;
    void Awake()
    {
        Instance = this;
    }
    #endregion
    
    public Camera CamMain;
    public Camera CamUI_Base;
    public Camera CamUI_Back;

    public Camera CamBattleEffect;
    public Camera CamCharacter;
    

    
    private Camera CurrentHeroCam;

    private MoveCamInfo Target;

    private List<Camera> SkillCamList = new List<Camera>();

    private Transform ShakeCamTrans;
    private Transform SkillCamTrans;

    private FunPointer RetAction;
    private Coroutine CrMove;
    private DG.Tweening.Core.TweenerCore<Vector3, DG.Tweening.Plugins.Core.PathCore.Path, DG.Tweening.Plugins.Options.PathOptions> DoMove = null;

    private Vector3 OriCharCamPosForShake;
    private Quaternion OriCharCamRotForShake;
    private Coroutine CrShakeCamCharacter = null;    
    

    private void Start()
    {
        CurrentHeroCam = CamCharacter;
    }



    public void MovePosition(MoveCamInfo targetPos, float time, AnimationCurve aniCurve = null, FunPointer retAction = null)
    {
        RetAction = retAction;

        if (Target == targetPos)
        {
            return;
        }

        Target = targetPos;
        AnimationCurve AniCurve = null;
        if (aniCurve == null)
        {
            AniCurve = CustomDataManager.Instance.GetAniCurveData(0);
        }
        else
        {
            AniCurve = aniCurve;
        }

        if (DoMove != null)
        {
            DoMove.Kill(false);
            DoMove = null;
        }


        Vector3[] tempPos = new Vector3[2];

        tempPos[0] = transform.position;
        tempPos[1] = Target.Position;

        if (Target.LookAtTarget != null && AniCurve != null)
        {
            DoMove = transform.DOPath(tempPos, time, PathType.CatmullRom).SetEase(AniCurve).SetLookAt(Target.LookAtTarget, true).OnComplete(CompleteMove);
        }
        else
        {
            if (Target.LookAtTarget == null && AniCurve == null)
            {
                DoMove = transform.DOPath(tempPos, time, PathType.CatmullRom).OnComplete(CompleteMove);
            }
            else if(Target.LookAtTarget == null)
            {
                DoMove = transform.DOPath(tempPos, time, PathType.CatmullRom).SetEase(AniCurve).OnComplete(CompleteMove);
            }
            else if (AniCurve == null)
            {
                DoMove = transform.DOPath(tempPos, time, PathType.CatmullRom).SetLookAt(Target.LookAtTarget, true).OnComplete(CompleteMove);
            }
        }
    }
    public void StopMove()
    {
        if (DoMove != null)
        {
            DoMove.Kill(false);
            DoMove = null;
        }
        RetAction = null;
        Target = null;
    }
    private void CompleteMove()
    {
        if (RetAction != null) RetAction();
        if (DoMove != null)
        {
            DoMove.Kill(false);
            DoMove = null;
        }
    }


    public void ShakeCamChar(float trauma, Transform source, Transform target, bool isSingle = true)
    {
        Vector3 maximumTranslationShake = target.position - source.position;
        maximumTranslationShake = Vector3.Normalize(maximumTranslationShake);

        if (SkillCamTrans == null)
        {
            ShakeCamTrans = CamCharacter.transform;
        }
        else
        {
            ShakeCamTrans = SkillCamTrans.parent;
        }
        if (CrShakeCamCharacter != null)
        {
            StopCoroutine(CrShakeCamCharacter);
            CrShakeCamCharacter = null;
            CamCharacter.transform.localPosition = OriCharCamPosForShake;
            CamCharacter.transform.localRotation = OriCharCamRotForShake;

            if (SkillCamTrans != null && SkillCamTrans.parent != null)
            {
                if (SkillCamTrans.parent.name.Contains("Parent"))
                {
                    SkillCamTrans.parent.transform.localPosition = new Vector3(0f, 0f, 0f);
                    SkillCamTrans.parent.transform.localRotation = Quaternion.identity;
                    SkillCamTrans.parent.transform.localRotation = Quaternion.Euler(-90, 0, 0);
                }
                else
                {
                    SkillCamTrans.transform.localPosition = new Vector3(0f, 0f, 0f);
                    SkillCamTrans.transform.localRotation = Quaternion.identity;
                    SkillCamTrans.transform.localRotation = Quaternion.Euler(-90, 0, 0);
                }
            }
        }

        OriCharCamPosForShake = CamCharacter.transform.localPosition;
        OriCharCamRotForShake = CamCharacter.transform.localRotation;


        maximumTranslationShake = ShakeCamTrans.localPosition + maximumTranslationShake;
        maximumTranslationShake += new Vector3(2.5f, 2.5f, 2.5f);

        if (isSingle)
        {
            CrShakeCamCharacter = StartCoroutine(CoShakeCamChar_Single(trauma, maximumTranslationShake, OnComplete =>
            {
                if (OnComplete)
                {
                    CrShakeCamCharacter = null;
                }
            }));
        }
        else
        {
            CrShakeCamCharacter = StartCoroutine(CoShakeCamChar_Multi(trauma, maximumTranslationShake, OnComplete =>
            {
                if (OnComplete)
                {
                    CrShakeCamCharacter = null;
                }
            }));
        }

    }

    private IEnumerator CoShakeCamChar_Single(float trauma, Vector3 maximumTranslationShake, System.Action<bool> OnComplete)
    {
        float frequency = 25;
        float seed = Random.value;    // ¸Å È÷Æ® ½Ã º¯°æ

        Vector3 maximumAngularShake = Vector3.one * 2;

        float recoverySpeed = 1.5f;
        float traumaExponent = 2;
        float shake = Mathf.Pow(trauma, traumaExponent);
        while (shake != 0)
        {
            shake = Mathf.Pow(trauma, traumaExponent);
            ShakeCamTrans.localPosition = new Vector3(
                maximumTranslationShake.x * (Mathf.PerlinNoise(seed, Time.time * frequency) * 2 - 1),
                maximumTranslationShake.y * (Mathf.PerlinNoise(seed + 1, Time.time * frequency) * 2 - 1),
                maximumTranslationShake.z * (Mathf.PerlinNoise(seed + 2, Time.time * frequency) * 2 - 1)
                ) * shake;
            trauma = Mathf.Clamp01(trauma - recoverySpeed * Time.deltaTime);
            yield return null;
        }

        OnComplete(true);
    }
    private IEnumerator CoShakeCamChar_Multi(float trauma, Vector3 maximumTranslationShake, System.Action<bool> OnComplete)
    {
        float frequency = 25;
        float seed = Random.value;    // ¸Å È÷Æ® ½Ã º¯°æ

        Vector3 maximumAngularShake = Vector3.one * 2;

        float recoverySpeed = 1.5f;
        float traumaExponent = 2;
        float shake = Mathf.Pow(trauma, traumaExponent);

        while (shake != 0)
        {
            shake = Mathf.Pow(trauma, traumaExponent);
            ShakeCamTrans.localPosition = new Vector3(
                maximumTranslationShake.x * (Mathf.PerlinNoise(seed, Time.time * frequency) * 2 - 1),
                maximumTranslationShake.y * (Mathf.PerlinNoise(seed + 1, Time.time * frequency) * 2 - 1),
                maximumTranslationShake.z * (Mathf.PerlinNoise(seed + 2, Time.time * frequency) * 2 - 1)
                ) * shake;

            ShakeCamTrans.localRotation = Quaternion.Euler(new Vector3(
                maximumAngularShake.x * (Mathf.PerlinNoise(seed + 3, Time.time * frequency) * 2 - 1),
                maximumAngularShake.y * (Mathf.PerlinNoise(seed + 4, Time.time * frequency) * 2 - 1),
                maximumAngularShake.z * (Mathf.PerlinNoise(seed + 5, Time.time * frequency) * 2 - 1)
                ) * shake);

            trauma = Mathf.Clamp01(trauma - recoverySpeed * Time.deltaTime);
            yield return null;
        }

        OnComplete(true);
    }    
}
