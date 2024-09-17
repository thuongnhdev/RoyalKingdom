using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioClipVar", menuName = "ScriptableObjectSystem/AudioClipVariable")]
public class AudioClipVariable : BaseScriptableObjectVariable<AudioClip>
{
    protected override bool IsSetNewValue(AudioClip value)
    {
        return value.GetInstanceID() != Value.GetInstanceID();
    }
}
