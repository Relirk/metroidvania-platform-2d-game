using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class DoorLvl : MonoBehaviour
{
    public int lvlIndex;
    void OnTriggerEnter2D(Collider2D collision) {
        if(collision.CompareTag("Player")){
            SceneManager.LoadScene(lvlIndex);
        }
    }
}