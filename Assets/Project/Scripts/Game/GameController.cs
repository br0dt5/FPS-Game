using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [Header("Game")]
    public Player player;
    public GameObject enemyContainer;

    [Header("UI")]
    public Text healthText;
    public Text ammoText;
    public Text enemyText;
    public Text infoText;

    private bool gameOver = false;
    private float resetTimer = 3f;

    void Start()
    {
        infoText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        healthText.text = $"Health: {player.Health}";
        ammoText.text = $"Ammo: {player.Ammo}";

        var aliveEnemies = 0;
        foreach (var enemy in enemyContainer.GetComponentsInChildren<Enemy>())
        {
            if (!enemy.Killed)
            {
                aliveEnemies++;
            }
        }
        enemyText.text = $"Enemies: {aliveEnemies}";

        if (aliveEnemies == 0)
        {
            gameOver = true;
            infoText.gameObject.SetActive(true);
            infoText.text = "You win!\nGood job!";
        }

        if (player.Killed) 
        {
            gameOver = true;
            infoText.gameObject.SetActive(true);
            infoText.text = "You lose :(\nTry again!";
        }

        if (gameOver) 
        {
            resetTimer -= Time.deltaTime;
            if (resetTimer <= 0) 
            {
                SceneManager.LoadScene("Menu");
            }
        }
    }
}
