using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public MainMenu menu;
    public int index; // 0 = Play, 1 = Quit

    public void OnPointerEnter(PointerEventData e) { menu.SetHover(index); }
    public void OnPointerExit(PointerEventData e) { menu.SetHover(-1); }
    public void OnPointerClick(PointerEventData e) { menu.ActivateButton(index); }
}