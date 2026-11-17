using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingSceneMain : MonoBehaviour
{
    public GameObject transitionPanel;

    void Update()
    {
        if (Input.anyKeyDown)
            StartCoroutine(Transition());
    }

    private IEnumerator Transition()
    {
        enabled = false;
        if (transitionPanel != null) transitionPanel.SetActive(true);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Main Menu");
    }
}