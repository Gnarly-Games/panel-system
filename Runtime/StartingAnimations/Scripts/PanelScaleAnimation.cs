using PrimeTween;
using UnityEngine;

namespace Gnarly.PanelSystem
{
    [RequireComponent(typeof(PanelView))]
    public class PanelScaleAnimation : PanelAnimation
    {
        protected override PanelView TargetPanel { get; set; }

        public override void Initialize()
        {
            TargetPanel = GetComponent<PanelView>();

            if (TargetPanel.isVisibleOnLoad)
            {
                TargetPanel.transform.localScale = Vector3.one;
                TargetPanel.Group.blocksRaycasts = true;
                TargetPanel.Group.interactable = true;
            }
            else
            {
                TargetPanel.transform.localScale = Vector3.zero;
                TargetPanel.Group.blocksRaycasts = false;
                TargetPanel.Group.interactable = false;
            }
        }

        public override Sequence MakeVisible()
        {
            var scaleInSequence = Sequence.Create(useUnscaledTime: true);
            scaleInSequence.Chain(Tween.Scale(TargetPanel.transform, 1f, TargetPanel.showDuration, Ease.OutBack));
            scaleInSequence.OnComplete(() => TargetPanel.onShow?.Invoke());

            return scaleInSequence;
        }

        public override Sequence MakeHidden(bool shouldDestroyAfterHide)
        {
            var scaleOutSequence = Sequence.Create(useUnscaledTime: true);
            scaleOutSequence.Chain(Tween.Scale(TargetPanel.transform, 0f, TargetPanel.hideDuration, Ease.InBack));
            scaleOutSequence.OnComplete(() => TargetPanel.OnHide(shouldDestroyAfterHide));
            return scaleOutSequence;
        }
    }
}