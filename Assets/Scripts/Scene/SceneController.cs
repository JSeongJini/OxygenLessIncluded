using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*
�� ��Ʈ�ѷ�, ��Ʈ��, �ε�, ���� �� ��ȯ�� �����ϴ� ��ũ��Ʈ  
Ư�̻��� : �ε���� �־��µ� �ε��� �ʹ� ���� ����� �۵� ���� 
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
        loadingCanvasGo.transform.SetParent(transform);                   // ��üȭ�� ĵ������ �θ� ����Ʈ�ѷ��� ����
        loadingBar = loadingCanvasGo.GetComponentsInChildren<Image>()[3]; // ĵ�������� 4��°�� �ε��ٰ� ���� 
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


        while (!asyncScene.isDone) // MainScene�� �ε��Ǳ� ������ 
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
