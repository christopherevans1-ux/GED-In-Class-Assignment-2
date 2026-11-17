using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
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
        transitionPanel.SetActive(true);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("RunNGun");
    }
}