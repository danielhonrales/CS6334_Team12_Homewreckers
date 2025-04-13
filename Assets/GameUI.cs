using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour
{

    public List<GameObject> destructionBars;

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
    }
}
