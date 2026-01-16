using PrimeTween;
using UnityEngine;

namespace Gnarly.PanelSystem
{
    [RequireComponent(typeof(PanelView))]
    public class PanelDelayAnimation : PanelAnimation
    {
        protected override PanelView TargetPanel { get; set; }

        public override void Initialize()
        {
            TargetPanel = GetComponent<PanelView>();
            
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
            var delayInSequence = Sequence.Create(useUnscaledTime: true);
            delayInSequence.ChainDelay(TargetPanel.showDuration);
            delayInSequence.ChainCallback(() =>
            {
                TargetPanel.Group.alpha = 1f;
                TargetPanel.Group.blocksRaycasts = true;
                TargetPanel.Group.interactable = true;
            });
            delayInSequence.OnComplete(() => TargetPanel.onShow?.Invoke());

            return delayInSequence;
        }

        public override Sequence MakeHidden(bool shouldDestroyAfterHide)
        {
            var delayOutSequence = Sequence.Create(useUnscaledTime: true);
            delayOutSequence.ChainDelay(TargetPanel.hideDuration);
            delayOutSequence.ChainCallback(() =>
            {
                TargetPanel.Group.alpha = 0f;
                TargetPanel.Group.blocksRaycasts = false;
                TargetPanel.Group.interactable = false;
            });
            delayOutSequence.OnComplete(() => TargetPanel.OnHide(shouldDestroyAfterHide));
            
            return delayOutSequence;
        }
    }
}