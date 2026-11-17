using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompletedLevel : MonoBehaviour
{
    bool lvlCom;
    public GameObject carhitBox;
    public GameObject player;
    public Animator animator;
    
        // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        lvlCom = GetComponent<EnemyRespawn>().levelCleared;
        if (lvlCom == true)
        {
            carhitBox.SetActive(true);

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("hitbox") && lvlCom == true)
        {
            Debug.Log("Level Complete");
            player.GetComponent<Movement>().victory = true;
            player.GetComponent<Movement>().playerCam.SetActive(false);
            player.GetComponent<Movement>().deathCam.SetActive(true);
            player.GetComponent<Movement>().deathCam.transform.position = player.transform.position + new Vector3(0, 10, 0);
            animator.SetBool("levelFinished", true);
        }
    }
}
