using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    [Header("Scene")]
    public string gameSceneName = "Game";

    [Header("Buttons")]
    public TextMeshProUGUI playText;
    public TextMeshProUGUI quitText;

    [Header("Selector Arrow")]
    public TextMeshProUGUI selectorArrow;

    [Header("Selector Arrow Positions")]
    public RectTransform arrowPositionPlay;
    public RectTransform arrowPositionQuit;

    [Header("Colors")]
    public Color selectedColor = new Color(1f, 1f, 1f, 1f);
    public Color deselectedColor = new Color(0.45f, 0.45f, 0.45f, 1f);

    [Header("Transition")]
    public GameObject transitionPanel;

    private int currentIndex = 0;
    private int buttonCount = 2;
    private int hoverIndex = -1;
    private bool isTransitioning = false;
    private Animator arrowAnimator;

    void Start()
    {
        arrowAnimator = selectorArrow.GetComponent<Animator>();
        float animLength = arrowAnimator.GetCurrentAnimatorStateInfo(0).length;
        Invoke(nameof(EnableArrowControl), animLength);
        UpdateVisuals();
    }

    void Update()
    {
        if (isTransitioning) return;
        HandleKeyboard();
    }

    void HandleKeyboard()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentIndex = (currentIndex - 1 + buttonCount) % buttonCount;
            hoverIndex = -1;
            UpdateVisuals();
        }

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentIndex = (currentIndex + 1) % buttonCount;
            hoverIndex = -1;
            UpdateVisuals();
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            ActivateButton(currentIndex);
        }
    }

    public void SetHover(int index)
    {
        if (isTransitioning) return;
        hoverIndex = index;
        if (index >= 0) currentIndex = index;
        UpdateVisuals();
    }

    public void OnPlayHover() { SetHover(0); }
    public void OnQuitHover() { SetHover(1); }
    public void OnButtonExit() { SetHover(-1); }
    public void OnPlayClick() { ActivateButton(0); }
    public void OnQuitClick() { ActivateButton(1); }

    public void ActivateButton(int index)
    {
        if (isTransitioning) return;
        if (index == 0) StartCoroutine(TransitionToGame());
        else if (index == 1) QuitGame();
    }

    IEnumerator TransitionToGame()
    {
        isTransitioning = true;
        transitionPanel.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        yield return new WaitForSeconds(0.3f);
        SceneManager.LoadScene("RunNGun");
    }

    void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void UpdateVisuals()
    {
        SetButtonColor(playText, currentIndex == 0);
        SetButtonColor(quitText, currentIndex == 1);

        if (selectorArrow != null)
        {
            RectTransform arrowPos = currentIndex == 0 ? arrowPositionPlay : arrowPositionQuit;
            selectorArrow.rectTransform.localPosition = arrowPos.localPosition;
            selectorArrow.gameObject.SetActive(true);
        }
    }

    void SetButtonColor(TextMeshProUGUI text, bool selected)
    {
        if (text != null)
            text.color = selected ? selectedColor : deselectedColor;
    }

    public void EnableArrowControl()
    {
        if (arrowAnimator != null)
            arrowAnimator.enabled = false;
    }
}