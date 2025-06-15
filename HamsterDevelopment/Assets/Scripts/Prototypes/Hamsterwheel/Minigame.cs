using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Minigame : MonoBehaviour
{
    public float interval =1;
    [SerializeField] private GameObject step;
    [SerializeField] private GameObject startPointLeft;
    [SerializeField] private GameObject startPointRight;
    [SerializeField] private GameObject target;

    private bool left;

    private float targetPosition;
    private float leway = 50;

    public List<GameObject> LeftSteps = new List<GameObject>();
    public List<GameObject> RightSteps = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(spawnStep());
        targetPosition = target.GetComponent<RectTransform>().localPosition.y;
        Debug.Log(targetPosition);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            CheckStep(LeftSteps);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            CheckStep(RightSteps);
        }
    }

    IEnumerator spawnStep()
    {
        while (true)
        {
            if(left)
            {
                GameObject newstep = Instantiate(step, startPointLeft.transform);
                LeftSteps.Add(newstep);
            }
            else
            {
                GameObject newstep = Instantiate(step, startPointRight.transform);
                RightSteps.Add(newstep);
            }

            left = !left;
            yield return new WaitForSeconds(interval);
        }
    }

    private void CheckStep(List<GameObject> StepList)
    {
        float posY = transform.InverseTransformPoint(StepList.First().transform.position).y;
        Debug.Log(StepList.First().GetComponent<RectTransform>().localPosition.y + targetPosition);
        if (StepList.First().GetComponent<RectTransform>().localPosition.y + targetPosition < leway && StepList.First().GetComponent<RectTransform>().localPosition.y + targetPosition > -leway)
        {
            Debug.Log("Success!");
        }
        Destroy(StepList.First());
        StepList.RemoveAt(0);
    }
}

