using UnityEngine;

public class ArrowAnim : MonoBehaviour
{
    private Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void EnableArrowControl()
    {
        anim.enabled = false;
    }

    void LateUpdate()
    {
        Vector3 pos = transform.localPosition;
        pos.z = 0f;
        transform.localPosition = pos;
    }

}