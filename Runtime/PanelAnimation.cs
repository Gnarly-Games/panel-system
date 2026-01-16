using PrimeTween;
using UnityEngine;

namespace Gnarly.PanelSystem
{
    [RequireComponent(typeof(PanelView))]
    public abstract class PanelAnimation : MonoBehaviour
    {
        public PanelView TargetPanel { get; private set; }
        
        private void Awake()
        {
            TargetPanel = GetComponent<PanelView>();
        }

        public abstract void SetInitialState();
        public abstract Sequence MakeVisible();
        public abstract Sequence MakeHidden(bool shouldDestroyAfterHide);
    }
}