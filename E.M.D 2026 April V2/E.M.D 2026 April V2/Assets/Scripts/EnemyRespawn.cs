using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI.Table;

public class EnemyRespawn : MonoBehaviour
{
    //this script controls enemy respawn and the activation for if the player clears all the enemies on a floor/level
    public List<GameObject> enemiesFloor1;
    public List<GameObject> enemiesFloor2;
    public List<GameObject> enemiesFloor3;
    public List<GameObject> enemiesFloor4;
    public GameObject arrowToFloor2;
    public List<GameObject> endLevelArrows;
    public GameObject floor1Barrier;
    public Text FloorLevelCleared;
    public bool floor1Cleared = false;
    public bool floor2Cleared = false;
    public bool floor3Cleared = false;
    public bool floor4Cleared = false;
    public bool levelCleared = false;
    public AudioSource Music;
    public GameObject LeaveArea;
    //idk how else to do this but i hate it
    bool functionInvoked1 = false;
    bool functionInvoked2 = false;
    bool functionInvoked3 = false;
    bool functionInvoked4 = false;
    bool functionInvoked5 = false;
    float counter = 0;
    public float numOfFloors;
    float floorCompleteCount;
    bool playerDead;
    private float deadEnemies = 0;
    private float maxEnemies;

    private void Start()
    {
        maxEnemies = enemiesFloor1.Count + enemiesFloor2.Count + enemiesFloor3.Count + enemiesFloor4.Count - 1;
    }

    void Update()
    {
        floorCleared();
        playerDead = GetComponent<Movement>().isDead;

        if (playerDead == true)
        {
            RespawnEnemies();
        }
    }

    //This function teleports enemies to their game start positions and
    //sets them active for that floor when the player respawn
    public void RespawnEnemies()
    {
        if (levelCleared == false)
        {
            if (floor1Cleared == false)
            {
                for (int i = 0; i < enemiesFloor1.Count; i++)
                {
                    if (enemiesFloor1[i].activeSelf == false)
                    {
                        enemiesFloor1[i].SetActive(true);
                        enemiesFloor1[i].GetComponent<Enemy>().health = enemiesFloor1[i].GetComponent<Enemy>().startHealth;
                        enemiesFloor1[i].transform.position = enemiesFloor1[i].GetComponent<Enemy>().enemyStartPos;
                    }
                    else
                    {
                        //do not set active as enemy is still alive
                        enemiesFloor1[i].transform.position = enemiesFloor1[i].GetComponent<Enemy>().enemyStartPos;
                    }
                }
            }

            if (floor1Cleared == true && enemiesFloor2.Count > 0)
            {
                for (int i = 0; i < enemiesFloor2.Count; i++)
                {
                    if (enemiesFloor2[i].activeSelf == false)
                    {
                        enemiesFloor2[i].SetActive(true);
                        enemiesFloor2[i].GetComponent<Enemy>().health = enemiesFloor2[i].GetComponent<Enemy>().startHealth;
                        enemiesFloor2[i].transform.position = enemiesFloor2[i].GetComponent<Enemy>().enemyStartPos;
                    }
                    else
                    {
                        //do not set active as enemy is still alive
                        enemiesFloor2[i].transform.position = enemiesFloor2[i].GetComponent<Enemy>().enemyStartPos;
                    }
                }
            }

            if (floor2Cleared == true && enemiesFloor3.Count > 0)
            {
                for (int i = 0; i < enemiesFloor3.Count; i++)
                {
                    if (enemiesFloor3[i].activeSelf == false)
                    {
                        enemiesFloor3[i].SetActive(true);
                        enemiesFloor3[i].GetComponent<Enemy>().health = enemiesFloor3[i].GetComponent<Enemy>().startHealth;
                        enemiesFloor3[i].transform.position = enemiesFloor3[i].GetComponent<Enemy>().enemyStartPos;
                    }
                    else
                    {
                        //do not set active as enemy is still alive
                        enemiesFloor3[i].transform.position = enemiesFloor3[i].GetComponent<Enemy>().enemyStartPos;
                    }
                }
            }

            if (floor3Cleared == true && enemiesFloor4.Count > 0)
            {
                for (int i = 0; i < enemiesFloor4.Count; i++)
                {
                    if (enemiesFloor4[i].activeSelf == false)
                    {
                        enemiesFloor4[i].SetActive(true);
                        enemiesFloor4[i].GetComponent<Enemy>().health = enemiesFloor4[i].GetComponent<Enemy>().startHealth;
                        enemiesFloor4[i].transform.position = enemiesFloor4[i].GetComponent<Enemy>().enemyStartPos;
                    }
                    else
                    {
                        //do not set active as enemy is still alive
                        enemiesFloor4[i].transform.position = enemiesFloor4[i].GetComponent<Enemy>().enemyStartPos;
                    }
                }
            }
        }
    }

    public void floorCleared()
    {
        if (enemiesFloor1.Count > 0 && floorCompleteCount != 1)
        {
            for (int i = 0; i < enemiesFloor1.Count; i++)
            {
                if (enemiesFloor1[i].activeSelf == false)
                {
                    deadEnemies += 1;
                    if (deadEnemies > maxEnemies)
                    {
                        deadEnemies = maxEnemies;
                    }
                }
                else
                {
                    deadEnemies = 0;
                }
                if (deadEnemies == enemiesFloor1.Count)
                {
                    floorCompleteCount = 1;
                    if (functionInvoked1 == false && numOfFloors != 1)
                    {
                        DialogueManager.Instance.ShowDialogue("This floor is clear, make your way upstairs. Good Luck.", 2.5f);
                        arrowToFloor2.SetActive(true);
                        counter = 1;
                        deadEnemies = 0;
                    }
                    floor1Cleared = true;
                    floor1Barrier.SetActive(false);
                    Invoke("TextReset", 5f);
                }
            }
        }
        else
        {

        }

        if (enemiesFloor2.Count > 0 && floorCompleteCount != 2)
        {
            for (int i = 0; i < enemiesFloor2.Count; i++)
            {
                if (enemiesFloor2[i].activeSelf == false)
                {
                    deadEnemies += 1;
                    if (deadEnemies > maxEnemies)
                    {
                        deadEnemies = maxEnemies;
                    }
                }
                else
                {
                    deadEnemies = 0;
                }
                if (deadEnemies == enemiesFloor2.Count)
                {
                    Debug.Log("Floor 2 cleared");
                    floorCompleteCount = 2;
                    if (functionInvoked2 == false && numOfFloors != 2)
                    {
                        counter = 2;
                        DialogueManager.Instance.ShowDialogue("Very impressive, it's time for you to get the hell out of there.", 2.5f);
                    }
                    Invoke("TextReset", 5f);
                    floor2Cleared = true;
                }
            }
        }
        else
        {

        }

        if (enemiesFloor3.Count > 0 && floorCompleteCount != 3)
        {

        }
        else
        {

        }

        if (enemiesFloor4.Count > 0 && floorCompleteCount != 4)
        {

        }
        else
        {

        }

        if (floorCompleteCount == numOfFloors)
        {
            if (functionInvoked5 == false)
            {
                functionInvoked5 = true;
                counter = 5;
                DialogueManager.Instance.ShowDialogue("\nVery impressive, now get the hell out of there.", 2.5f);
                Debug.Log("Level Cleared");
                Music.Stop();
                LeaveArea.SetActive(true);
                GetComponent<Movement>().weaponSwitchText.text = " ";
                levelCleared = true;
                for (int i = 0; i < endLevelArrows.Count; i++)
                {
                    endLevelArrows[i].SetActive(true);
                }
            }
        }
    }

    public void TextReset()
    {
        FloorLevelCleared.text = " ";
        if (counter == 1)
        {
            functionInvoked1 = true;
        }
        else if (counter == 2)
        {
            functionInvoked2 = true;
        }
        else if (counter == 3)
        {
            functionInvoked3 = true;
        }
        else if (counter == 4)
        {
            functionInvoked4 = true;
        }
        else if (counter == 5)
        {
            functionInvoked5 = true;
        }
    }
}