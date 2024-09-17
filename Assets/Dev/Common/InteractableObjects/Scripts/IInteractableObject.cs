using UnityEngine;

public interface ISceneGameObjectPoiterDownHandler
{
    void OnPointerDown(Vector3 userPointPos);
}

public interface ISceneGameObjectPointerUpHandler
{
    void OnPointerUp(Vector3 userPointedPos);
}

public interface ISceneGameObjectClickHandler
{
    void OnPointerClicked(Vector3 clickPoint);
}

public interface ISceneGameObjectDragHandler
{
    void OnDragged(Vector3 userPointedPos);
    void OnEndDrag(Vector3 userPointedPos);
    bool IsDragActive();
}
