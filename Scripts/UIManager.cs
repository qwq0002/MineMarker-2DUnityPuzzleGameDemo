using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {

    public void StartGame() {
        Debug.Log("开始游戏！");
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame() {
        Debug.Log("退出游戏");
        Application.Quit();
    }
    
}
