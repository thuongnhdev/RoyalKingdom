using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameEvent", menuName = "ScriptableObjectSystem/GameEvent")]
public class GameEvent : ScriptableObject
{
#if UNITY_EDITOR
    [TextArea(1, 10)]
    public string _eventExplanation;
    public bool _logWhenRaise;
    public List<string> _subcribers;
    public List<EditorEventParam> _eventParams;
    [System.Serializable]
    public class EditorEventParam
    {
        public string className;
        [TextArea(1,10)]
        public string valueOrJson;
    }
#endif

    public delegate void EventActionDel(params object[] eventParam);
    private event EventActionDel _eventAction;

    public void Subcribe(EventActionDel action)
    {
        _eventAction += action;
#if UNITY_EDITOR
        var st = new System.Diagnostics.StackTrace();
        var stackFrame = st.GetFrame(1);
        _subcribers.Add(stackFrame.GetMethod().DeclaringType.Name);
#endif
    }

    public void Unsubcribe(EventActionDel action)
    {
        _eventAction -= action;
#if UNITY_EDITOR
        var st = new System.Diagnostics.StackTrace();
        var stackFrame = st.GetFrame(1);
        _subcribers.Remove(stackFrame.GetMethod().DeclaringType.Name);
#endif
    }

    public void Raise(params object[] eventParam)
    {
        _eventAction?.Invoke(eventParam);

#if UNITY_EDITOR
        if (_logWhenRaise)
        {
            Debug.Log($"Event [{name}] Raised", this);
        }
#endif
    }

    public void Raise()
    {
        _eventAction?.Invoke();
#if UNITY_EDITOR
        if (_logWhenRaise)
        {
            Debug.Log($"Event [{name}] Raised", this);
        }
#endif
    }

    public void RaiseAfter(float delay)
    {
        Delay_Raise(delay).Forget();
    }
    private async UniTaskVoid Delay_Raise(float delay)
    {
        await UniTask.Delay(System.TimeSpan.FromSeconds(delay));
        Raise();
    }

#if UNITY_EDITOR
    private void OnDisable()
    {
        _subcribers.Clear();
    }

    public void SubcribeEditor(EventActionDel action, string subcriber)
    {
        _eventAction += action;
        _subcribers.Add(subcriber);
    }

    public void UnsubcribeEditor(EventActionDel action, string subcriber)
    {
        _eventAction -= action;
        _subcribers.Remove(subcriber);
    }
#endif
}
