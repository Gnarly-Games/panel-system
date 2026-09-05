using System.Collections;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Gnarly.PanelSystem
{

    using UnityEngine;

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
        [Tooltip("Whether this panel can be destroy on hide"), FormerlySerializedAs("canLoad")]
        public bool isDestroyOnHide;
        [Tooltip("Whether to register this panel with the PanelManager"), FormerlySerializedAs("register")]
        public bool canAccessViaKey;
        [ShowIf(nameof(canAccessViaKey))]
        public string key;

        [Tooltip("vent invoked when a panel becomes visible")]
        public UnityEvent onShow;

        [Tooltip("Event invoked when a panel becomes hidden")]
        public UnityEvent onHide;

        public enum PanelState
        {
            Hide,
            Showing,
            Showed,
            Hiding
        }


        public CanvasGroup Group { get; private set; }
        public bool IsVisible { get; private set; }

        private void Awake()
        {
            Group = GetComponent<CanvasGroup>();
            if (canAccessViaKey)
                PanelManager.Add(this);

            IsVisible = isVisibleOnLoad;

            if (IsVisible)
            {
                Group.alpha = 1;
                Group.blocksRaycasts = true;
                onShow?.Invoke();
            }
            else
            {
                Group.alpha = 0;
                Group.blocksRaycasts = false;
                onHide?.Invoke();
            }
        }

        private void OnValidate()
        {
            if (canAccessViaKey)
            {
                if (string.IsNullOrEmpty(key))
                    key = gameObject.name;
            }
            else
            {
                key = string.Empty;
            }

        }

        private void OnDestroy()
        {
            if (canAccessViaKey)
                PanelManager.Remove(this);
        }

        public void Show()
        {
            if (IsVisible) return;
            IsVisible = true;
            StopAllCoroutines();
            StartCoroutine(Progress(true));
        }

        private IEnumerator Progress(bool show)
        {
            Group.blocksRaycasts = show;
            var startTime = Time.realtimeSinceStartup;
            var startValue = show ? 0 : 1f;
            var endValue = show ? 1 : 0;

            while (true)
            {
                var passTime = Time.realtimeSinceStartup - startTime;
                var progress = passTime / hideDuration;
                Group.alpha = Mathf.Lerp(startValue, endValue, progress);
                if (progress >= 1)
                    break;

                yield return null;

            }

            if (show)
            {
                onShow?.Invoke();
            }
            else
            {
                onHide?.Invoke();
                if (isDestroyOnHide)
                    Destroy(gameObject);
            }
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

        public void Hide()
        {
            if (!IsVisible) return;
            IsVisible = false;
            StopAllCoroutines();
            StartCoroutine(Progress(false));
        }
    }
}