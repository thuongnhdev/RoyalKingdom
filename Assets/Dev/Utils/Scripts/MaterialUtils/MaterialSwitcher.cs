using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class MaterialSwitcher : MonoBehaviour
{
    [SerializeField]
    private List<Material> _materials = new();
    [SerializeField]
    private int _maxConfigMatCount = 2;
    private Renderer _rendererer;

    public void SwitchMaterial(int matIndex)
    {
        _rendererer.material = _materials[matIndex % _materials.Count];
    }

    public void AddConfigMat(Material mat)
    {
        if (_materials.Count <= _maxConfigMatCount)
        {
            return;
        }

        _materials.Add(mat);
    }

    private void Awake()
    {
        _rendererer = GetComponent<Renderer>();
    }
}
