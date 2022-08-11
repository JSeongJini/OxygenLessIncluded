using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PopupManager : MonoBehaviour
{
    private SceneController sceneCtrl = null;

   
    [SerializeField] private Image popupBuildGo = null;  //빌드팝업 레이아웃의 오브젝트
    [SerializeField] private Image popupMenuGo = null; // 메뉴팝업 레이아웃의 오브젝트
    [SerializeField] private Image confirmLayout = null; // 확인팝업 레이아웃의 오브젝트
    [SerializeField] private SetButton setbutton = null;
    [SerializeField] private Text confirmTxt = null;

    private GameObject Popup = null;

    private Stack<GameObject> PopupList = new Stack<GameObject>();
    private int selectNum = 0;
    private Button[] confirmButton = null;

    private void Awake()
    {
        sceneCtrl = GameObject.Find("SceneController")?.GetComponent<SceneController>();
        confirmButton = confirmLayout.gameObject.GetComponentsInChildren<Button>();
    }
   
    public void ToggleBuildPopup(GameObject _obj)
    {
        
        if(PopupList.Count != 0)
            ClosePopup();
        else
            ActiveBuildPopup(_obj);
        return;
    }
    public void ToggleMenuPopup(GameObject _obj)
    {
        if (PopupList.Count != 0)
            ClosePopup();
        else
            ActiveMenuPopup(_obj);
        return;
    }

    

    public void SelectButton()
    {
        if (selectNum == 0)
        {
            sceneCtrl.LoadIntroScene();
            Time.timeScale = 1f;
        }
        else if(selectNum == 1)
        {
            sceneCtrl.ExitGame();
        }
    }
    public void CloseLayOut()
    {
        confirmLayout.gameObject.SetActive(false);
    }

    public void ClosePopup()
    {
        if (PopupList.Count > 0)
        {
            Popup = PopupList.Pop();
            Popup.SetActive(false);
            return;
        }
        return;
    }

    private void ActiveBuildPopup(GameObject _obj)
    {
        popupBuildGo.gameObject.SetActive(true);
        Popup = popupBuildGo.gameObject;
        Popup.transform.SetParent(_obj.transform);
        Popup.transform.position = _obj.transform.position + new Vector3(0f, 100f, 0f);
        setbutton.transform.SetParent(Popup.transform);
        PopupList.Push(Popup);
    }
    public void ActiveIcon(int _type)
    {
        setbutton.ActiveIcon(_type);
    }


    private void ActiveMenuPopup(GameObject _obj)
    {
        popupMenuGo.gameObject.SetActive(true);
        Popup = popupMenuGo.gameObject;
        Popup.transform.SetParent(_obj.transform);
        Popup.transform.position = new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2);
        PopupList.Push(Popup);
    }
    public void ActiveConfirmPopup(int _type)
    {
        confirmLayout.gameObject.SetActive(true);
        Popup = confirmLayout.gameObject;
        confirmLayout.transform.position = new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2);
        PopupList.Push(Popup);
        if (_type == 0)
        {
            confirmTxt.text = "첫 화면으로 돌아가시겠습니까?";
            selectNum = 0;
        }
        else if (_type == 1)
        {
            confirmTxt.text = "게임을 종료하시겠습니까?";
            selectNum = 1;
        }
        else if(_type == 2)
        {
            confirmTxt.text = "해당 기능은 업데이트 예정입니다.";
            foreach(Button button in confirmButton)
            {
                button.gameObject.SetActive(false);
            }
            selectNum = 2;
            return;
        }
        else if(_type == 3)
        {
            confirmTxt.text = "해당 기능은 업데이트 예정입니다.";
            foreach (Button button in confirmButton)
            {
                button.gameObject.SetActive(false);
            }
            selectNum = 3;
            return;
        }
        foreach (Button button in confirmButton)
        {
            button.gameObject.SetActive(true);
        }
        return;
    }






}
