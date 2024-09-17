using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network.Common.Static;

public class AddressableMainLoad : AddressableLoad// Editor 에서 Maian Scene에서 시작 할 때  addressable 관련 부분 실행을 위해 추가  
{
    protected override void Awake()
    {

#if UNITY_EDITOR
        if(!StatesGlobal.IS_TITLE_START)
        {
            //addressable load를 위해 한번 call한다.
            DataMgCommonPref commomPreMgr = DataMgCommonPref.Instance;
            SoundManager sounfMgr = SoundManager.Instance;

            if (DataMgSprite.Instance == null)
            {
                base.Awake();

                if (AddressableLoadManager.Instance != null)
                {
                    AddressableLoadManager.Instance.DownLoadComplete();
                }
            }
        }
#endif


    }
}
