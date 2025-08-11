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
    
    [SerializeField] List<GameObject> _objects;
    private bool _left;
    

    [SerializeField]private List<GameObject> _leftSteps = new();
    [SerializeField]private List<GameObject> _rightSteps = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(spawnStep());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) && _objects.Count > 0 && IsPartOfList(_objects, _leftSteps))
        {
            SuperDebug.Log("SUPER A!");
            var common = FindCommonObject(_objects, _leftSteps);
            _leftSteps.Remove(common);
            _objects.Remove(common);
            Destroy(common);
        }
        
        if (Input.GetKeyDown(KeyCode.D) && _objects.Count > 0 && IsPartOfList(_objects, _rightSteps))
        {
            SuperDebug.Log("SUPER D!");
            var common = FindCommonObject(_objects, _rightSteps);
            _rightSteps.Remove(common);
            _objects.Remove(common);
            Destroy(common);
        }
    }

    public void CollisionDetected(Collider2D other)
    {
        _objects.Add(other.gameObject);
    }

    public void CollisionEnded(Collider2D other)
    {
        if (_objects.Contains(other.gameObject))
        {
            _objects.Remove(other.gameObject);
            Destroy(other.gameObject);
        }
    }

    private int i;
    IEnumerator spawnStep()
    {
        while (true)
        {
            if(_left)
            {
                GameObject newstep = Instantiate(step, startPointLeft.transform);
                newstep.name = $"Step{i}";
                i++;
                _leftSteps.Add(newstep);
            }
            else
            {
                GameObject newstep = Instantiate(step, startPointRight.transform);
                newstep.name = $"Step{i}";
                i++;
                _rightSteps.Add(newstep);
            }

            _left = !_left;
            yield return new WaitForSeconds(interval);
        }
    }

    private bool IsPartOfList(List<GameObject> list1, List<GameObject> list2)
    {
        return list2.Any(list1.Contains);
    }

    private GameObject FindCommonObject(List<GameObject> list1, List<GameObject> list2)
    {
        var commonList = list1.Intersect(list2);
        return commonList.ToList()[0];
    }

}

