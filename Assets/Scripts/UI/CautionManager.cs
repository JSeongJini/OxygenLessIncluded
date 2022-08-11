using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CautionManager : MonoBehaviour
{
    [SerializeField] private RectTransform TextRtr = null;
    [SerializeField] private SoundsManager soundsManager = null;
    [SerializeField] private List<Text> cautionText = new List<Text>();
    [SerializeField] private Queue<Text> showQueue = new Queue<Text>();

    private float colortime = 0f;

    public void ShowCaution(int _val)
    {
        if(showQueue.Count < cautionText.Count)
        {
            soundsManager.PlayAudio(9);
            cautionText[_val].gameObject.SetActive(true);
            showQueue.Enqueue(cautionText[_val]);
            cautionText[_val].rectTransform.position = new Vector3(TextRtr.position.x, TextRtr.position.y-(showQueue.Count*64));
            StartCoroutine("ChangeColorTime", _val);
            StartCoroutine("ShowTime", _val);
        }
    }

    private IEnumerator ShowTime(int _val)
    {

        while(true)
        {
            yield return new WaitForSeconds(5);
            if (showQueue.Count != 0)
            {
                showQueue.Dequeue().gameObject.SetActive(false);
                colortime = 0f;
            }
            if(showQueue.Count == 0)
            {
                StopCoroutine("ChangeColorTime");
                StopCoroutine("ShowTime");
            }
        }
    }
    private IEnumerator ChangeColorTime(int _val)
    {
        while(showQueue.Count !=0)
        {
            colortime += Time.deltaTime;
            cautionText[_val].color = new Color(1f - (colortime * 0.1f), 0f, 0f);
            yield return null;
        }

    }

}
