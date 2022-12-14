using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartMgr : MonoBehaviour
{
    [SerializeField] GameObject canvas;
    [SerializeField] GameObject canvasPause;
    [SerializeField] Material ceilingMat1;
    [SerializeField] MeshRenderer ceilingRenderer;
    [SerializeField] AudioClip audioRotateRoom;
    public static event System.Action roomRotateStart;
    public static event System.Action gameStarted;
    bool isPaused = false;

    // Start is called before the first frame update
    void Start()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString("savedLevel", currentScene);

        if (currentScene == "End")
        {
            PlayerPrefs.SetString("savedLevel", "Level 1");
        }

        Cursor.lockState = CursorLockMode.Locked;

        Rigidbody[] rbs = GameObject.FindObjectsOfType<Rigidbody>();     
        Time.timeScale = 1; 

        if (SceneManager.GetActiveScene().name == "Level 1")
        {   
            canvas.SetActive(false);  

            for(int i = 0; i < rbs.Length; i++)
            {
                if (rbs[i].gameObject.tag == "Player")
                {
                    continue;
                }
                
                rbs[i].isKinematic = true;
            }
            StartCoroutine(FreezeAfter(4.4F));
            StartCoroutine(UnfreezeAfter(6));
        }
        else
        {
            canvas.SetActive(true);
            for(int i = 0; i < rbs.Length; i++)
            {
                rbs[i].isKinematic = true;
            }
            StartCoroutine(UnfreezeAfter(2));
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            string name = SceneManager.GetActiveScene().name;
            TransitionMgr.LoadScene?.Invoke(name);
        }

        if (Input.GetButtonDown("Cancel"))
        {
            Pause();
        }
    }

    public void ClickedMenuButton()
    {
        Time.timeScale = 1;
        TransitionMgr.LoadScene?.Invoke("Menu");
    }

    IEnumerator FreezeAfter(float t)
    {
        yield return new WaitForSeconds(t);
        Rigidbody[] rbs = GameObject.FindObjectsOfType<Rigidbody>();
        for(int i = 0; i < rbs.Length; i++)
        {
            rbs[i].isKinematic = true;
            rbs[i].interpolation = RigidbodyInterpolation.None;
        }
        roomRotateStart?.Invoke();
        AudioSource.PlayClipAtPoint(audioRotateRoom, Vector3.zero);
    }

    IEnumerator UnfreezeAfter(float t)
    {
        yield return new WaitForSeconds(t);
        Rigidbody[] rbs = GameObject.FindObjectsOfType<Rigidbody>();
        for(int i = 0; i < rbs.Length; i++)
        {
            rbs[i].interpolation = RigidbodyInterpolation.Interpolate;
            rbs[i].isKinematic = false;
            canvas.SetActive(true);
        }
        gameStarted?.Invoke();

        if (SceneManager.GetActiveScene().name == "Level 1")
        {
            ceilingRenderer.material = ceilingMat1;
        }
    }

    void Pause()
    {
        if (!isPaused)
        {
            canvasPause.SetActive(true);
            isPaused = true;
            Cursor.lockState = CursorLockMode.None;

            Rigidbody[] rbs = GameObject.FindObjectsOfType<Rigidbody>();
            for(int i = 0; i < rbs.Length; i++)
            {
                rbs[i].isKinematic = true;
            }
        }
    }

    public void Resume()
    {
        if (isPaused)
        {
            
            Cursor.lockState = CursorLockMode.Locked;
            canvasPause.SetActive(false);
            isPaused = false;

            Rigidbody[] rbs = GameObject.FindObjectsOfType<Rigidbody>();
            for(int i = 0; i < rbs.Length; i++)
            {
                rbs[i].isKinematic = false;
            }
        }
    }
}
