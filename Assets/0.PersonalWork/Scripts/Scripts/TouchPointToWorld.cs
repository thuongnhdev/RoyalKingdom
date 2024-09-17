using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchPointToWorld : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    private Camera _mainCam;
    [SerializeField]
    private Vector3Variable _output;

    public void OnPointerDown(PointerEventData eventData)
    {
        Ray ray = _mainCam.ScreenPointToRay(eventData.position);
        Physics.Raycast(ray, out var hit, 500f);
        _output.Value = hit.point;
    }
}
