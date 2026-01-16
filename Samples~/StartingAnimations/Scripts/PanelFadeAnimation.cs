using PrimeTween;
using UnityEngine;

namespace Gnarly.PanelSystem
{
    [RequireComponent(typeof(PanelView))]
    public class PanelFadeAnimation : PanelAnimation
    {
        public override void SetInitialState()
        {
            if (TargetPanel.isVisibleOnLoad)
            {
                TargetPanel.Group.alpha = 1f;
                TargetPanel.Group.blocksRaycasts = true;
                TargetPanel.Group.interactable = true;
            }
            else
            {
                TargetPanel.Group.alpha = 0f;
                TargetPanel.Group.blocksRaycasts = false;
                TargetPanel.Group.interactable = false;
            }
        }

        public override Sequence MakeVisible()
        {
            var fadeInSequence = Sequence.Create(useUnscaledTime: true);
            fadeInSequence.Chain(Tween.Alpha(TargetPanel.Group, 1f, TargetPanel.showDuration));
            fadeInSequence.OnComplete(() => TargetPanel.onShow?.Invoke());

            return fadeInSequence;
        }

        public override Sequence MakeHidden(bool shouldDestroyAfterHide)
        {
            var fadeOutSequence = Sequence.Create(useUnscaledTime: true);
            fadeOutSequence.Chain(Tween.Alpha(TargetPanel.Group, 0f, TargetPanel.hideDuration));
            fadeOutSequence.OnComplete(() => TargetPanel.OnHide(shouldDestroyAfterHide));

            return fadeOutSequence;
        }
    }
}