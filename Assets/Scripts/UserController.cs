using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class UserController : MonoBehaviour
{
    [SerializeField] private MapManager mapManager = null;
    [SerializeField] private NPCManager npcManager = null;
    [SerializeField] private WorkManager workManager = null;
    [SerializeField] private GameObject[] mouseCursors = null;
    [SerializeField] private PopupManager popupManager = null;

    [Header("Popup")]
    [SerializeField] private GameObject resourceInfoPopup = null;
    [SerializeField] private GameObject npcInfoPopup = null;
    [SerializeField] private GameObject tutorialPopup = null;

    private GameObject curCursor;
  

    public enum EInputState
    {
        Idle = 0, Dig, Tile, Ladder, Destroy, Cancel, Rocket
    }
    private EInputState state = EInputState.Idle;
    private Vector2Int downPos;
    private Vector2Int upPos;

    private void Start()
    {
        StartCoroutine("ShowInfoPopup");
    }

    private void Update()
    {
        CursorControl();

        if (Input.GetKeyDown(KeyCode.G))        //굴착
            SetState(1);
        if (Input.GetKeyDown(KeyCode.T))        //타일
            SetState(2);
        if (Input.GetKeyDown(KeyCode.L))        //사다리
            SetState(3);
        if (Input.GetKeyDown(KeyCode.D))        //파괴
            SetState(4);
        if (Input.GetKeyDown(KeyCode.C))        //취소
            SetState(5);

        if (Input.GetMouseButtonDown(0))
            downPos = GetMousePos();

        if (Input.GetMouseButtonDown(1))
        {
            SetState(0);
            popupManager.ClosePopup();
        }
        if (Input.GetKeyDown(KeyCode.Space))
            Camera.main.transform.position = new Vector3(96f, 54f, -10f);
        if (Input.GetKeyDown(KeyCode.F1))
            tutorialPopup.gameObject.SetActive(!tutorialPopup.activeSelf);

            if (Input.GetMouseButtonUp(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            upPos = GetMousePos();

            int bigX, bigY, smallX, smallY;
            if (upPos.x > downPos.x)
            {
                bigX = upPos.x;
                smallX = downPos.x;
            }
            else
            {
                bigX = downPos.x;
                smallX = upPos.x;
            }

            if (upPos.y > downPos.y)
            {
                bigY = upPos.y;
                smallY = downPos.y;
            }
            else
            {
                bigY = downPos.y;
                smallY = upPos.y;
            }

            Vector2Int[] targetPos = new Vector2Int[(bigX - smallX + 1) * (bigY - smallY + 1)];
            int index = 0;
            for(int y = smallY; y <= bigY; y++) 
                for(int x = smallX; x <=bigX; x++)
                    targetPos[index++] = new Vector2Int(x, y);

            for (int i = 0; i < targetPos.Length; i++)
            {
                if (mapManager.IsValidPos(targetPos[i]))
                {
                    //TODO : 작업과 상태를 일치하면 하나로 통일될 듯
                    switch (state)
                    {
                        case EInputState.Idle:
                            break;
                        case EInputState.Dig:
                            workManager.RequestDig(targetPos[i]);
                            break;
                        case EInputState.Tile:
                            workManager.RequestBuild(targetPos[i], 0);
                            break;
                        case EInputState.Ladder:
                            workManager.RequestBuild(targetPos[i], 1);
                            break;
                        case EInputState.Rocket:
                            workManager.RequestRocket(targetPos[i]);
                            break;
                        case EInputState.Destroy:
                            workManager.RequestDestroy(targetPos[i]);
                            break;
                        case EInputState.Cancel:
                            workManager.RequestCancel(targetPos[i]);
                            break;
                    }
                }
            }
        }
    }

    private void CursorControl(){
        if (curCursor == null) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        curCursor.transform.position = new Vector3(
            Mathf.Round(mousePos.x), Mathf.Round(mousePos.y), 0f
            );
    }

    private Vector2Int GetMousePos()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return new Vector2Int((int)Mathf.Round(mousePos.x), (int)Mathf.Round(mousePos.y));
    }

    public void SetState(int _type)
    {
        if(_type == (int)state)
        {
            state = EInputState.Idle;
            curCursor?.SetActive(false);
            curCursor = null;
            return;
        }
        else
        {
            state = (EInputState)_type;
            curCursor?.SetActive(false);
            if (_type != 0)
            {
                curCursor = mouseCursors[_type - 1];
                curCursor.SetActive(true);
            }
        }
    }

    private IEnumerator ShowInfoPopup()
    {
        float resourcePopupSizeY = 0f;
        float npcPopupSizeY = 0f;
        while (true)
        {
            if (state == EInputState.Idle)
            {
                resourceInfoPopup.SetActive(true);
                RectTransform rt = resourceInfoPopup.GetComponent<RectTransform>();
                Vector2 mousePos = Input.mousePosition;

                resourcePopupSizeY = rt.sizeDelta.y * 2.2f;
                rt.anchoredPosition = mousePos + new Vector2(30f, -resourcePopupSizeY);

                ResourceBase rb = mapManager.GetResourceByPos(GetMousePos());
                if (rb)
                {
                    Text[] texts = resourceInfoPopup.GetComponentsInChildren<Text>();
                    if (rb.GetType() == typeof(ResourceSandStone))
                        texts[0].text = "사암";
                    else if (rb.GetType() == typeof(ResourceGold))
                        texts[0].text = "금";
                    else if (rb.GetType() == typeof(ResourceAir))
                        texts[0].text = "산소";
                    else
                        resourceInfoPopup.SetActive(false);

                    texts[1].text = "남은 량 : " + rb.GetAmount().ToString("F1") + "g";
                }

                NPC npc = npcManager.GetNPCByPos(GetMousePos());
                if (npc)
                {
                    npcInfoPopup.SetActive(true);
                    rt = npcInfoPopup.GetComponent<RectTransform>();
                    npcPopupSizeY = rt.sizeDelta.y * 2.2f;
                    rt.anchoredPosition = mousePos + new Vector2(30f, -(resourcePopupSizeY + npcPopupSizeY));


                    Text[] texts = npcInfoPopup.GetComponentsInChildren<Text>();
                    texts[0].text = npc.name;
                    texts[1].text = "산소 : " + npc.GetOxygenRate().ToString("F1") + "%";
                }
                else
                {
                    npcInfoPopup.SetActive(false);
                }

            }
            else
            {
                resourceInfoPopup.SetActive(false);
                npcInfoPopup.SetActive(false);
            }

            yield return new WaitForSeconds(0.3f);
        }
    }

    public void GameOver()
    {
        StopAllCoroutines();
        resourceInfoPopup.SetActive(false);
        npcInfoPopup.SetActive(false);
        SetState(0);
    }
}
