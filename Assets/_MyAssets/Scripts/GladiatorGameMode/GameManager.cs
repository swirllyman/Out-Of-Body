using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public PlayerDissociation player;
    [SerializeField] TMP_Text victoryText;
    [SerializeField] GameObject victoryObject;

    [SerializeField] float distanceForVictory = 39.0f;

    Pose playerStartPose;
    bool victory = false;

    // Start is called before the first frame update
    void Start()
    {
        playerStartPose.position = player.transform.position;
        playerStartPose.rotation = player.transform.rotation;
        player.onPlayerHitCallback += PlayerHit;
    }

    void PlayerHit()
    {
        StartCoroutine(ResetPlayerAfterTime());
    }

    IEnumerator ResetPlayerAfterTime()
    {
        yield return new WaitForSeconds(player.reviveTimeInSeconds);
        ResetPlayer();
    }

    void ResetPlayer()
    {
        player.transform.position = playerStartPose.position;
        player.transform.rotation = playerStartPose.rotation;
    }

    private void Update()
    {
        if (player.transform.position.z > distanceForVictory &! victory)
        {
            player.invincible = true;
            victoryObject.SetActive(true);
            victory = true;

            victoryText.enabled = true;
            victoryText.text = "Victory!";
            StartCoroutine(ReloadScene());
        }
    }

    IEnumerator ReloadScene()
    {
        yield return new WaitForSeconds(5.0f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
