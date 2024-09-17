using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleDamageUI : MonoBehaviour
{
    public GameEvent damgeEvent;
    public PoolService pool;
    // Start is called before the first frame update
    void Start() {
        damgeEvent.Subcribe(OnDamage);
    }

    private void OnDestroy()
    {
        damgeEvent.Unsubcribe(OnDamage);
    }

    void OnDamage(params object[] values) {
        Vector3 pos = (Vector3)values[0];
        int damage = (int)values[1];

        Transform t = pool.Get(0, transform);
        t.position = pos + Vector3.up;
        var text = t.GetComponent<TMPro.TextMeshPro>();
        text.text = $"{damage}";
        t.DOMove(t.position + Vector3.up, 1f).OnComplete(() => {
            pool.Release(0, t);
        });
    }

    // Update is called once per frame
    void Update() {
    }
}
