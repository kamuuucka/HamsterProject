using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Prototypes.Mechanics.HamsterWheel
{
    /// <summary>
    /// Mini game played when player interacts with the hamster wheel. 
    /// </summary>
    public class HwMiniGame : MonoBehaviour
    {
        #region Exposed Variables

        [Header("Mini Game Setup")]
        [Tooltip("Points required to finish the mini game.")]
        [SerializeField] private int requiredPoints = 1;
        [Tooltip("What will happen when player gets all the points.")]
        [SerializeField] private UnityEvent doAfterFinished;
        
        [Header("Spawning Steps")][Space(5)]
        [Tooltip("Time (in seconds) between each step.")]
        [SerializeField] private float intervalSeconds = 1;
        [Tooltip("How much should the interval decrease when the step is pressed correctly.")]
        [SerializeField] private float decreaseIntervalSeconds = 0.1f;
        [Tooltip("Size of the pool containing all the steps in the game. In short: how many steps will be in the game at the same time.")]
        [SerializeField] private int poolSize = 10;
        
        [Header("Necessary Objects")][Space(5)]
        [Tooltip("Steps to spawn.")]
        [SerializeField] private GameObject step;
        [Tooltip("Starting point for the left lane.")]
        [SerializeField] private Transform startPointLeft;
        [Tooltip("Starting point for the right lane.")]
        [SerializeField] private Transform startPointRight;
        
        [Space(10)]
        [SerializeField] private bool isDebug;

        #endregion

        #region Private Variables

        private readonly List<GameObject> _activeSteps = new();
        private readonly List<GameObject> _leftSteps = new();
        private readonly List<GameObject> _rightSteps = new();
        private bool _left;
        private int _i;
        private int _currentPoints;
        private float _actualInterval;

        #endregion

        #region Pooling Variables

            private readonly Queue<GameObject> _leftPool = new();
            private readonly Queue<GameObject> _rightPool = new();

        #endregion
        
    
        void Start()
        {
            _actualInterval =  intervalSeconds;
            
            CheckNecessaryObjects();
            
            CreatePool(_leftPool, startPointLeft);
            CreatePool(_rightPool, startPointRight);
        
            StartCoroutine(SpawnStep());
        }
        
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.A) && _activeSteps.Count > 0 && IsPartOfList(_activeSteps, _leftSteps))
            {
                HandleStepPressed(_leftSteps, _leftPool);
                CheckIfFinished();
            }
        
            if (Input.GetKeyDown(KeyCode.D) && _activeSteps.Count > 0 && IsPartOfList(_activeSteps, _rightSteps))
            {
                HandleStepPressed(_rightSteps, _rightPool);
                CheckIfFinished();
            }
        }

        private void CheckIfFinished()
        {
            if (_currentPoints >= requiredPoints)
            {
                doAfterFinished?.Invoke();
            }
        }
        
        /// <summary>
        /// Checks if the collision was detected.
        /// </summary>
        public void CollisionDetected(Collider2D other)
        {
            _activeSteps.Add(other.gameObject);
            if (isDebug)
            {
                SuperDebug.Log($"Interval: {_actualInterval}");
                other.gameObject.GetComponent<Image>().color = Color.blue;
            }
        }

        /// <summary>
        /// Checks if collision ended.
        /// </summary>
        public void CollisionEnded(Collider2D other)
        {
            if (!_activeSteps.Contains(other.gameObject)) return;
            if (isDebug) other.gameObject.GetComponent<Image>().color = Color.yellow;
            _actualInterval = Math.Min(intervalSeconds, _actualInterval + decreaseIntervalSeconds);
            if (_leftSteps.Contains(other.gameObject))
            {
                _activeSteps.Remove(other.gameObject);
                _leftSteps.Remove(other.gameObject);
                ReturnToPool(other.gameObject, _leftPool);
            }
            else if (_rightSteps.Contains(other.gameObject))
            {
                _activeSteps.Remove(other.gameObject);
                _rightSteps.Remove(other.gameObject);
                ReturnToPool(other.gameObject, _rightPool);
            }
        }

        /// <summary>
        /// When step is correctly pressed in the green zone: removes the object from the correct side list,
        /// removes the object from active steps,
        /// and returns object to the correct pool.
        /// </summary>
        /// <param name="steps">Left or Right list.</param>
        /// <param name="pool">Left or Right pool.</param>
        private void HandleStepPressed(List<GameObject> steps, Queue<GameObject> pool)
        {
            var common = FindCommonObject(_activeSteps, steps);
            steps.Remove(common);
            _activeSteps.Remove(common);
            ReturnToPool(common, pool);
            _actualInterval -= decreaseIntervalSeconds;
            _currentPoints++;
        }

        /// <summary>
        /// Populates the pool with the desired amount of objects.
        /// </summary>
        /// <param name="pool">Which pool should the objects be created in.</param>
        /// <param name="parent">The parent for the pool.</param>
        private void CreatePool(Queue<GameObject> pool, Transform parent)
        {
            for (int i = 0; i < poolSize; i++)
            {
                GameObject obj = Instantiate(step, parent);
                obj.SetActive(false);
                pool.Enqueue(obj);
            }
        }

        /// <summary>
        /// Deactivates the object and put it back in the pool's queue.
        /// </summary>
        /// <param name="obj">Object that needs to return to the pool.</param>
        /// <param name="pool">The pool that the objects belongs to.</param>
        private void ReturnToPool(GameObject obj, Queue<GameObject> pool)
        {
            obj.SetActive(false);
            pool.Enqueue(obj);
        }

        /// <summary>
        /// Activates steps in correct pools once in the left, once in the right based on the interval time.
        /// </summary>
        private IEnumerator SpawnStep()
        {
            while (true)
            {
                Queue<GameObject> pool = _left ? _leftPool : _rightPool;
                Transform startPoint = _left ? startPointLeft : startPointRight;
                List<GameObject> activeList = _left ? _leftSteps : _rightSteps;

                if (pool.Count > 0)
                {
                    GameObject newStep = pool.Dequeue();
                    newStep.transform.position = startPoint.position;
                    newStep.name = $"Step{_i}";
                    newStep.SetActive(true);
                    _i++;
                
                    activeList.Add(newStep);
                }
            
                _left = !_left;
            
                yield return new WaitForSeconds(_actualInterval);
            }
        }

        /// <summary>
        /// Checks if one list is a part of another one.
        /// </summary>
        /// <param name="list1">First list to check.</param>
        /// <param name="list2">Second list to check.</param>
        /// <returns>True if the list 1 is part of the list 2.</returns>
        private static bool IsPartOfList(List<GameObject> list1, List<GameObject> list2)
        {
            return list2.Any(list1.Contains);
        }

        /// <summary>
        /// Finds the first (or default) common object between two lists.
        /// </summary>
        /// <param name="list1">First list to check.</param>
        /// <param name="list2">Second list to check.</param>
        /// <returns>The first or default common object from the two lists.</returns>
        private static GameObject FindCommonObject(List<GameObject> list1, List<GameObject> list2)
        {
            var commonList = list1.Intersect(list2);
            return commonList.FirstOrDefault();
        }

        private void CheckNecessaryObjects()
        {
            if (step == null)
            {
                SuperDebug.LogError("Step assignment missing");
                return;
            }

            if (startPointLeft == null)
            {
                SuperDebug.LogError("No spawn point for the left lane!");
                return;
            }

            if (startPointRight == null)
            {
                SuperDebug.LogError("No spawn point for the right lane!");
                return;
            }
        }

    }
}

