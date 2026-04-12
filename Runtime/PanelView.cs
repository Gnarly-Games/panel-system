using System;
using NaughtyAttributes;
using PrimeTween;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Gnarly.PanelSystem
{
    [DefaultExecutionOrder(-100)]
    [RequireComponent(typeof(CanvasGroup))]
    public class PanelView : MonoBehaviour
    {
        [Tooltip("Duration in seconds for the show animation"), Space]
        public float showDuration = 0.2f;

        [Tooltip("Duration in seconds for the hide animation")]
        public float hideDuration = 0.2f;
        [Tooltip("Whether or not this panel is visible"), FormerlySerializedAs("initialState")]
        public bool isVisibleOnLoad;
        [Tooltip("Whether this panel can be dynamically loaded/unloaded")]
        public bool canLoad;
        [Tooltip("Whether to register this panel with the PanelManager"), FormerlySerializedAs("register")]
        public bool canAccessViaKey;
        [ShowIf(nameof(canAccessViaKey)), Tooltip("Key used to identify this panel in the PanelManager")]
        public string key;

        [Tooltip("vent invoked when a panel becomes visible")]
        public UnityEvent onShow;

        [Tooltip("Event invoked when a panel becomes hidden")]
        public UnityEvent onHide;

        private Sequence _showSequence;
        private Sequence _hideSequence;

        public CanvasGroup Group { get; private set; }

        private PanelAnimation _panelAnimation;
        private bool _lastAccessState;

        public bool IsInteractable
        {
            get => Group.blocksRaycasts;
            set => Group.blocksRaycasts = value;
        }

        public bool IsVisible { get; private set; }

        private void Awake()
        {
            Group = GetComponent<CanvasGroup>();
            Group.interactable = true;
            Group.blocksRaycasts = isVisibleOnLoad;

            if (TryGetComponent<PanelAnimation>(out var anim))
                _panelAnimation = anim;
            else
                _panelAnimation = gameObject.AddComponent<PanelFadeAnimation>();

            _panelAnimation.Initialize();

            IsVisible = isVisibleOnLoad;
            if (canAccessViaKey)
                PanelManager.Add(this);
        }

        private void OnEnable()
        {
            _lastAccessState = canAccessViaKey;
        }

        private void OnValidate()
        {
            if (canAccessViaKey && string.IsNullOrEmpty(key))
                key = gameObject.name;
        }

        private void OnDestroy()
        {
            _hideSequence.Stop();
            _showSequence.Stop();

            if (canAccessViaKey)
                PanelManager.Remove(this);
        }

        public void Show()
        {
            if (IsVisible) return;

            _hideSequence.Stop();
            _showSequence.Stop();

            _showSequence = _panelAnimation.MakeVisible();

            IsVisible = true;
            Group.interactable = true;
            Group.blocksRaycasts = true;
        }

        /// <summary>
        /// Sets this panel's sibling index to be one higher than the target
        /// </summary>
        public void SetFrontOf(Transform target)
        {
            var targetSiblingIndex = target.GetSiblingIndex() + 1;
            transform.SetSiblingIndex(targetSiblingIndex);
        }

        /// <summary>
        /// Sets this panel's sibling index to be one lower than the target
        /// </summary>
        public void SetBehindOf(Transform target)
        {
            var targetSiblingIndex = Mathf.Max(0, target.GetSiblingIndex() - 1);
            transform.SetSiblingIndex(targetSiblingIndex);
        }

        /// <summary>
        /// Sets this panel's sibling index as the last
        /// </summary>
        public void SetAsLastSibling()
        {
            transform.SetSiblingIndex(transform.parent.childCount - 1);
        }

        public int GetSiblingIndex()
        {
            return transform.GetSiblingIndex();
        }

        public void SetSiblingIndex(int value)
        {
            transform.SetSiblingIndex(value);
        }

        public void Hide(bool shouldDestroyAfterFinish = true)
        {
            if (!IsVisible) return;

            IsVisible = false;

            _hideSequence.Stop();
            _showSequence.Stop();

            _hideSequence = _panelAnimation.MakeHidden(shouldDestroyAfterFinish);
        }

        public void OnHide(bool shouldDestroyAfterFinish = true)
        {
            Group.blocksRaycasts = false;
            Group.interactable = false;
            onHide?.Invoke();

            if (!shouldDestroyAfterFinish) return;

            Destroy();
        }

        private void Destroy()
        {
            if (canLoad)
            {
                Destroy(gameObject);
            }
        }
    }
}