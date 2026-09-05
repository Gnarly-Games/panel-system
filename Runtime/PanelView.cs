using UnityEngine;
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
        [Tooltip("Whether this panel can be destroy on hide")]
        public bool isDestroyOnHide;
        [Tooltip("Whether to register this panel with the PanelManager"), FormerlySerializedAs("register")]
        public bool canAccessViaKey;
        [ShowIf(nameof(canAccessViaKey))]
        public string key;

        [Tooltip("vent invoked when a panel becomes visible")]
        public UnityEvent onShow;

        [Tooltip("Event invoked when a panel becomes hidden")]
        public UnityEvent onHide;
        private float _hideStartTime;
        private float _showStartTime;

        public enum State
        {
            Hide,
            Showing,
            Showed,
            Hiding
        }

        public State state { get; private set; }

        public CanvasGroup Group { get; private set; }


        public bool IsVisible { get; private set; }

        private void Awake()
        {
            Group = GetComponent<CanvasGroup>();
            Group.blocksRaycasts = isVisibleOnLoad;
            
            IsVisible = isVisibleOnLoad;
            if (canAccessViaKey)
                PanelManager.Add(this);
        }

        private void OnValidate()
        {
            if (canAccessViaKey && string.IsNullOrEmpty(key))
                key = gameObject.name;
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
            Group.blocksRaycasts = true;

            if (showDuration <= 0)
            {
                state = State.Showed;
                OnShow();
            }
            else
            {
                _showStartTime = Time.time;
                state = State.Showing;
            }
        }

        private void OnShow()
        {
            onShow?.Invoke();
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

            if (hideDuration <= 0)
            {
                state = State.Hide;
                OnHide();
            }
            else
            {
                _hideStartTime = Time.time;
                state = State.Hiding;
            }
        }

        public void OnHide(bool shouldDestroyAfterFinish = true)
        {
            Group.blocksRaycasts = false;
            onHide?.Invoke();

            if (!shouldDestroyAfterFinish) return;

            Destroy();
        }

        private void Destroy()
        {
            if (isDestroyOnHide)
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {

            if (state == State.Hiding)
            {
                var passTime = Time.time - _hideStartTime;
                var progress = passTime / hideDuration;
                Group.alpha = Mathf.Lerp(1, 0, progress);
                if (progress >= 1)
                {
                    state = State.Hide;
                    OnHide();
                }
            }
            else if (state == State.Showing)
            {
                var passTime = Time.time - _showStartTime;
                var progress = passTime / hideDuration;
                Group.alpha = Mathf.Lerp(0, 1, progress);
                if (progress >= 1)
                {
                    state = State.Showed;
                    OnShow();
                }
            }

        }
    }
}