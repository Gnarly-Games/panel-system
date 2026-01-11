using System;
using PrimeTween;

namespace Gnarly.PanelSystem
{
    public enum PanelAnimationType
    {
        Scale,
        Fade
    }
    
    public static class PanelAnimator
    {
        public static Sequence MakeVisible(this PanelView targetPanel, PanelAnimationType animationType)
        {
            return animationType switch
            {
                PanelAnimationType.Scale => ShowScale(targetPanel),
                PanelAnimationType.Fade => ShowFade(targetPanel),
                _ => throw new ArgumentOutOfRangeException(nameof(animationType), animationType, null)
            };
        }
        
        public static Sequence MakeHidden(this PanelView panelView, PanelAnimationType animationType, bool shouldDestroyAfterHide)
        {
            return animationType switch
            {
                PanelAnimationType.Scale => HideScale(panelView, shouldDestroyAfterHide),
                PanelAnimationType.Fade => HideFade(panelView, shouldDestroyAfterHide),
                _ => throw new ArgumentOutOfRangeException(nameof(animationType), animationType, null)
            };
        }

        private static Sequence ShowScale(PanelView panelView)
        {
            var scaleInSequence = Sequence.Create(useUnscaledTime: true);
            scaleInSequence.Chain(Tween.Scale(panelView.transform, 1f, panelView.showDuration, Ease.OutBack));
            scaleInSequence.OnComplete(() => panelView.onShow?.Invoke());

            return scaleInSequence;
        }
        
        private static Sequence HideScale(PanelView panelView, bool shouldDestroyAfterHide)
        {
            var scaleOutSequence = Sequence.Create(useUnscaledTime: true);
            scaleOutSequence.Chain(Tween.Scale(panelView.transform, 0f, panelView.hideDuration, Ease.InBack));
            scaleOutSequence.OnComplete(() => panelView.OnHide(shouldDestroyAfterHide));
            
            return scaleOutSequence;
        }

        private static Sequence ShowFade(PanelView panelView)
        {
            var fadeInSequence = Sequence.Create(useUnscaledTime: true);
            fadeInSequence.Chain(Tween.Alpha(panelView.Group, 1f, panelView.showDuration));
            fadeInSequence.OnComplete(() => panelView.onShow?.Invoke());

            return fadeInSequence;
        }

        private static Sequence HideFade(PanelView panelView, bool shouldDestroyAfterHide)
        {
            var fadeOutSequence = Sequence.Create(useUnscaledTime: true);
            fadeOutSequence.Chain(Tween.Alpha(panelView.Group, 0f, panelView.hideDuration));
            fadeOutSequence.OnComplete(() => panelView.OnHide(shouldDestroyAfterHide));

            return fadeOutSequence;
        }
    }
}