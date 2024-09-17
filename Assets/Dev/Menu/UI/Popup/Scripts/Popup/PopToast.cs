using UnityEngine;
using UnityEngine.UI;

public class PopToast : MonoBehaviour
{
    public Text TxContent;
     
    public void ShowToast(string value)
    {
        TxContent.text = value;
    }

    private void end()
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
