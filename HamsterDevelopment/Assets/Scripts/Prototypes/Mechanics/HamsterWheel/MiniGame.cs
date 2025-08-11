using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Prototypes.Mechanics.HamsterWheel
{
    public class MiniGame : MonoBehaviour
    {
        #region Exposed Variables

        [SerializeField] private float intervalSeconds = 1;
        [SerializeField] private int poolSize = 10;
        [SerializeField] private GameObject step;
        [SerializeField] private Transform startPointLeft;
        [SerializeField] private Transform startPointRight;

        #endregion

        #region Private Variables

        private readonly List<GameObject> _activeSteps = new();
        private readonly List<GameObject> _leftSteps = new();
        private readonly List<GameObject> _rightSteps = new();
        private bool _left;
        private int _i;

        #endregion

        #region Pooling Variables

            private readonly Queue<GameObject> _leftPool = new();
            private readonly Queue<GameObject> _rightPool = new();

        #endregion
        
    
        void Start()
        {
            CreatePool(_leftPool, startPointLeft);
            CreatePool(_rightPool, startPointRight);
        
            StartCoroutine(SpawnStep());
        }
        
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.A) && _activeSteps.Count > 0 && IsPartOfList(_activeSteps, _leftSteps))
            {
                HandleStepPressed(_leftSteps, _leftPool);
            }
        
            if (Input.GetKeyDown(KeyCode.D) && _activeSteps.Count > 0 && IsPartOfList(_activeSteps, _rightSteps))
            {
                HandleStepPressed(_rightSteps, _rightPool);
            }
        }
        
        /// <summary>
        /// Checks if the collision was detected.
        /// </summary>
        public void CollisionDetected(Collider2D other)
        {
            _activeSteps.Add(other.gameObject);
        }

        /// <summary>
        /// Checks if collision ended.
        /// </summary>
        public void CollisionEnded(Collider2D other)
        {
            if (!_activeSteps.Contains(other.gameObject)) return;
            if (_leftSteps.Contains(other.gameObject))
            {
                _leftSteps.Remove(other.gameObject);
                ReturnToPool(other.gameObject, _leftPool);
            }
            else if (_rightSteps.Contains(other.gameObject))
            {
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
            
                yield return new WaitForSeconds(intervalSeconds);
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

    }
}

