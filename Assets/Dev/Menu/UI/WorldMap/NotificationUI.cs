using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationUI : MonoBehaviour
{
    public GameObject panel;
    public GameEvent notificationEvent;
    public TMPro.TextMeshProUGUI msg;
    float timer;
    // Start is called before the first frame update
    void Start()
    {
        notificationEvent.Subcribe(OnEvent);
    }
    void OnDestroy()
    {
        notificationEvent.Unsubcribe(OnEvent);
    }

    private void OnEvent(params object[] values) {
        if (values.Length > 0) {
            msg.text = values[0].ToString();
            timer = 5.0f;
            panel.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0) {
                panel.SetActive(false);
            }
        }
        
    }
}
