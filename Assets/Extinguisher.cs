using System.Collections;
using UnityEngine;

public class Extinguisher : MonoBehaviour
{

    public ParticleSystem foam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Extinguish() {
        StartCoroutine(Foam());
        GameObject.Find("Stove").GetComponent<Stove>().TurnOffServerRpc();
    }

    public IEnumerator Foam() {
        foam.Play();
        yield return new WaitForSeconds(2f);
        foam.Stop();
    }
}
