using UnityEngine;

public class SceneIntroTransition : MonoBehaviour
{
    public GameObject transitionPanel;
    public Animator transitionAnimator;

    void Awake()
    {
        transitionPanel.SetActive(false);
    }

    void Start()
    {
        transitionPanel.SetActive(true);
        transitionAnimator.Play(0, -1, 0f);
    }
}