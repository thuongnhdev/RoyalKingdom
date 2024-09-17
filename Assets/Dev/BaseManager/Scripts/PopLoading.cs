using UnityEngine;
using UnityEngine.UI;
using Network.Common.Static;

public class PopLoding : MonoBehaviour
{
    public Text TxTipContent;
    public GameObject[] Contents;
    public GameObject[] ContentsBg;
    private bool IS_OPEND = false;
    private bool IS_DONE = false;
    private bool IS_TITLE = false;

    private FunPointer ActOpend;
    private FunPointer ActClosed;

    private GameObject CurContent;

    private void uae_loading_opend()
    {
        if (ActOpend != null)
        {
            ActOpend();
            ActOpend = null;
        }
        IS_OPEND = true;
        if (IS_DONE)
        {
            CallCloseTrigger();
        }
        else
        {
            GetComponent<Animator>().SetTrigger("loding");
        }
    }

    private void uae_loading_closed()
    {
        if (ActClosed != null)
        {
            ActClosed();
            ActClosed = null;
        }
        IS_OPEND = false;
        IS_DONE = false;
        gameObject.SetActive(false);
        CurContent.SetActive(false);
    }

    public void ResetAction()
    {
        ActOpend = null;
        ActClosed = null;
    }

    public void SetActiveObj(bool isTitle, FunPointer actOpend = null)
    {
        ActOpend = actOpend;
        IS_OPEND = false;
        IS_DONE = false;
        IS_TITLE = isTitle;

        gameObject.SetActive(true);
        GetComponent<Animator>().SetTrigger("open");

        if (IS_TITLE)
        {
            CurContent = Contents[0];
            ContentsBg[0].SetActive(true);
            ContentsBg[1].SetActive(false);
        }
        else
        {
            CurContent = Contents[Random.Range(1, Contents.Length)];
            ContentsBg[0].SetActive(false);
            ContentsBg[1].SetActive(true);
        }
        CurContent.SetActive(true);
    }

    public void Complete(FunPointer actClosed = null)
    {
        if (StatesGlobal.DEBUG_IGNORE_LOADING)
        {
            ActClosed = actClosed;
            uae_loading_closed();
        }
        else
        {
            ActClosed = actClosed;
            IS_DONE = true;
            if (IS_OPEND)
            {
                CallCloseTrigger();
            }
        }
    }

    private void CallCloseTrigger()
    {
        GetComponent<Animator>().SetTrigger("close");
        CurContent.GetComponent<Animator>().SetTrigger("close");
    }
}
