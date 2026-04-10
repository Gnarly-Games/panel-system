using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using PrimeTween;

namespace Gnarly.PanelSystem
{
    public static class PanelManager
    {
        private const string MainCanvasTag = "MainCanvas";
        private const string DuplicateKeyError = "PanelView Add Error!: Used same key {0}";
        private const string MissingKeyError = "PanelView Remove Error!: Missing key {0}";
        private const string PanelLoadError = "PanelView Get Error!: Missing key {0}";

        private static readonly Dictionary<string, PanelView> PanelViews = new();

        public static GameObject MainCanvas { get; set; }

        public static void Add(PanelView panelToAdd)
        {
            if (!panelToAdd) throw new ArgumentNullException(nameof(panelToAdd));

            var isSuccess = PanelViews.TryAdd(panelToAdd.key, panelToAdd);
            if (isSuccess) return;

            Debug.LogError(string.Format(DuplicateKeyError, panelToAdd.key));
        }

        public static void Remove(PanelView panelToRemove)
        {
            if (!panelToRemove) throw new ArgumentNullException(nameof(panelToRemove));

            var isSuccess = PanelViews.Remove(panelToRemove.key);
            if (isSuccess) return;

            Debug.LogWarning(string.Format(MissingKeyError, panelToRemove.key));
        }

        public static PanelView GetOrCreate(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));

            return PanelViews.TryGetValue(key, out var panel) ? panel : CreateAndRegisterPanel(key);
        }

        public static T GetOrCreate<T>(string key = null)
        {
            var panelKey = GetPanelKey<T>(key);
            var panel = GetOrCreate(panelKey);

            return panel ? panel.GetComponent<T>() : default;
        }

        public static bool IsLoaded<T>(string key = null)
        {
            var panelKey = GetPanelKey<T>(key);
            return PanelViews.ContainsKey(panelKey);
        }

        private static string GetPanelKey<T>(string key)
        {
            return string.IsNullOrEmpty(key)
                ? typeof(T).ToString().Split(".").Last()
                : key;
        }

        private static PanelView CreateAndRegisterPanel(string key)
        {
            var panelViewPrefab = Resources.Load<PanelView>(key);
            if (!panelViewPrefab)
            {
                Debug.LogError(string.Format(PanelLoadError, key));
                return null;
            }

            InitializeMainCanvasIfNeeded();

            var panel = UnityEngine.Object.Instantiate(panelViewPrefab, MainCanvas.transform);
            return panel;
        }

        private static void InitializeMainCanvasIfNeeded()
        {
            if (MainCanvas) return;

            MainCanvas = GameObject.FindWithTag(MainCanvasTag);
        }
    }
}