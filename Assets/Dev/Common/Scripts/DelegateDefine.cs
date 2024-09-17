using UnityEngine;


public delegate void FunPointer();

public delegate void FunPointerGameObject(GameObject target);
public delegate void FunPointerString(string input);
public delegate void FunPointerInt(int index);
public delegate void FunPointerUlong(ulong index);
public delegate void FunPointerFloat(float time);
public delegate void FunPointerBool(bool enable);
public delegate void FunPointerLong(long index);