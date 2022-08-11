using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*
씬 컨트롤러, 인트로, 로딩, 메인 씬 전환을 제어하는 스크립트  
특이사항 : 로딩장면 넣었는데 로딩이 너무 빨라서 제대로 작동 안함 
*/


public class SceneController : MonoBehaviour
{
    [SerializeField] private Canvas loadingCanvas = null;
    [SerializeField] private Image loadingBar = null;

    private GameObject loadingCanvasGo = null;
    private AsyncOperation asyncScene = null;
    private float loadingTimer = 0f;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene()
    {
        SceneManager.LoadSceneAsync("LoadingScene");
        loadingCanvasGo = Instantiate(loadingCanvas.gameObject); 
        loadingCanvasGo.transform.SetParent(transform);                   // 실체화한 캔버스의 부모를 씬컨트롤러에 세팅
        loadingBar = loadingCanvasGo.GetComponentsInChildren<Image>()[3]; // 캔버스에서 4번째에 로딩바가 있음 
        StartCoroutine("timecoroutine");
        StartCoroutine("SceneLoad");

    }

    public void LoadIntroScene()
    {
        SceneManager.LoadSceneAsync("IntroScene");
    }
    public void ExitGame()
    {
        Application.Quit();
    }

    private IEnumerator SceneLoad()
    {
        asyncScene = SceneManager.LoadSceneAsync("MainScene");
        asyncScene.allowSceneActivation = false;


        while (!asyncScene.isDone) // MainScene이 로딩되기 전까지 
        {
            yield return null;
            
            loadingBar.fillAmount = loadingTimer;

            if (loadingBar.fillAmount >= 0.9f)
            {
                loadingBar.fillAmount = 1f;

                if (loadingBar.fillAmount == 1.0f)
                {
                    asyncScene.allowSceneActivation = true;
                    if (asyncScene.isDone)
                    {
                        loadingCanvasGo.SetActive(false);
                        StopCoroutine("timecoroutine");
                    }
                }
            }
        }
        yield return null;
    }
  private IEnumerator timecoroutine()
  {
      while(true)
      {
          loadingTimer += Time.deltaTime;
          yield return null;
      }
  }
}
