using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{

    public List<GameObject> destructionBars;
    public TMP_Text timeText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int points = GameObject.Find("GameController").GetComponent<NetworkControl>().destructionPoints.Value;
        for (int i = 0; i< destructionBars.Count; i++) {
            if (i < points) {
                destructionBars[i].SetActive(true);
            } else {
                destructionBars[i].SetActive(false);
            }
        }

        if (GameObject.Find("GameController").GetComponent<GameController>().roundActive.Value) {
            timeText.text = GameObject.Find("GameController").GetComponent<GameController>().time.Value.ToString();
        }
        if (timeText.text == "0") {
            StartCoroutine(EndGame());
        }
    }

    public IEnumerator EndGame() {
        yield return new WaitForSeconds(1f);
        if (GameObject.Find("GameController").GetComponent<NetworkControl>().destructionPoints.Value > 2) {
            timeText.text = "Child wins!";
        } else {
            timeText.text = "Parent wins!";
        }
    }
}
