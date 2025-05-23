using System.Collections;
using UnityEngine;

public class Minigame : MonoBehaviour
{
    public float interval =1;
    [SerializeField] private GameObject step;
    [SerializeField] private GameObject startPoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(spawnStep());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator spawnStep()
    {
        while (true)
        {
            GameObject newstep = Instantiate(step, startPoint.transform);
            
            yield return new WaitForSeconds(interval);
        }
    }
}
