using System;
using System.Collections.Generic;
using EventBus;
using System.Diagnostics.Tracing;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using Object = System.Object;

    public class EventsReportWindow : EditorWindow
    {

        [MenuItem("Tools/Show Events Report Window")]
        public static void ShowWindow()
        {
            EventsReportWindow wnd = GetWindow<EventsReportWindow>();
            wnd.minSize = new Vector2(250, 300);
            wnd.maxSize = new Vector2(400, 700);
            wnd.titleContent = new GUIContent("Event report window");
        }
        private Vector2 scrollPosition;
        private Dictionary<string, List<GameObject>> _eventsReport =  new Dictionary<string, List<GameObject>>();
        private void OnGUI()
        {
            if (_eventsReport.Any(x => x.Value.Any(y => y == null)))
            {
                _eventsReport = new Dictionary<string, List<GameObject>>();
                GenerateReport();
            }
            GUILayout.Space(10);
            if (_eventsReport.Count == 0)
            {
                if (GUILayout.Button("Generate Events Report"))
                {
                    GenerateReport();
                }
            }
            else
            {
                if (GUILayout.Button("Refresh Events Report"))
                {
                    _eventsReport = new Dictionary<string, List<GameObject>>();
                    GenerateReport();
                }
            }

           
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            foreach (var kvp in _eventsReport)
            {
                
                GUILayout.Label(kvp.Key,EditorStyles.boldLabel);
                GuiLine(2);
                foreach (var evt in kvp.Value)
                {
                    var listener = evt.GetComponent<BaseEventBusListener>();
                    EditorGUILayout.BeginHorizontal(GUILayout.MinWidth(100), GUILayout.MaxWidth(500), GUILayout.ExpandWidth(true));
                    
                    
                    if (TryGetGenericEventType(listener, out var genericEvent))
                    {
                        EditorGUILayout.LabelField($"{evt.name} ({genericEvent.Name})", GUILayout.MinWidth(125), GUILayout.MaxWidth(250), GUILayout.ExpandWidth(true)); }
                    else
                    {
                        EditorGUILayout.LabelField(evt.name, GUILayout.MinWidth(125), GUILayout.MaxWidth(250), GUILayout.ExpandWidth(true));
                    }
                  
                    if (GUILayout.Button("Open", GUILayout.Height(25), GUILayout.Width(50)))
                    {
                        Selection.activeObject = evt;
                        EditorGUIUtility.PingObject(listener);
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space(5);
                    GuiLine();
                }
            }
            EditorGUILayout.EndScrollView();
        }
        void GuiLine( int i_height = 1 )
        {
            Rect rect = EditorGUILayout.GetControlRect(false, i_height );
            rect.height = i_height;
            EditorGUI.DrawRect(rect, new Color ( 0.5f,0.5f,0.5f, 1 ) );
        }
        private void OnDisable()
        {
            _eventsReport.Clear();
        }

        private void GenerateReport()
        {
            string[] events = FindObjectsOfType<EventBusPublisher>(true).Select(p => p.Eventname).ToArray();
            var listeners = FindObjectsOfType<BaseEventBusListener>(true);
           
            
            foreach (var eventName in events)
            {
                var listenersOfEventName = listeners.Where(l => l.PassedEventName == eventName);
                _eventsReport.Add(eventName, new List<GameObject>());
                foreach (var listener in listenersOfEventName)
                {
                    _eventsReport[eventName].Add(listener.gameObject);   
                }
               
            }
        }
        static bool TryGetGenericEventType(BaseEventBusListener listener, out Type eventType)
        {
            var type = listener.GetType();
            var baseType = type.BaseType;
            if (baseType != null && baseType.IsGenericType)
            {
                eventType = baseType.GenericTypeArguments[0];
                return true;
            }
            
            eventType = null;
            return false;
        }
    }


