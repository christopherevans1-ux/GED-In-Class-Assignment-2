using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CompletedLevel : MonoBehaviour
{
    bool lvlCom;
    public GameObject carhitBox;
    public GameObject player;
    public Animator animator;
    public bool isTutorialLevel = false;
    public GameObject transitionPanel;

    void Start()
    {
        player = GameObject.Find("Player");
    }

    void Update()
    {
        lvlCom = GetComponent<EnemyRespawn>().levelCleared;
        //Debug.Log("lvlCom: " + lvlCom + " carhitbox active: " + carhitBox.activeSelf);
        if (lvlCom == true)
        {
            carhitBox.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Trigger hit: " + other.name + " tag: " + other.tag + " lvlCom: " + lvlCom);
        if (other.CompareTag("hitbox") && lvlCom == true)
        {
            Debug.Log("Level Complete");
            player.GetComponent<Movement>().victory = true;
            StartCoroutine(TransitionToEndScene());
        }
    }

    private IEnumerator TransitionToEndScene()
    {
        if (transitionPanel != null) transitionPanel.SetActive(true);
        yield return new WaitForSeconds(1f);

        PlayerPrefs.SetInt("FinalScore", FindObjectOfType<ComboUI>().totalScore);
        PlayerPrefs.SetFloat("FinalTime", Time.timeSinceLevelLoad);
        PlayerPrefs.Save();

        if (isTutorialLevel)
            SceneManager.LoadScene("EndingSceneTutorial");
        else
            SceneManager.LoadScene("EndingSceneMain");
    }
}