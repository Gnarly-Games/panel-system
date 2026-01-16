using PrimeTween;
using UnityEngine;

namespace Gnarly.PanelSystem
{
    /// <summary>
    /// Abstract base class for defining show and hide animations for a PanelView.
    /// </summary>
    [RequireComponent(typeof(PanelView))]
    public abstract class PanelAnimation : MonoBehaviour
    {
        /// <summary>
        /// The <see cref="PanelView"/> instance this animation is attached to.
        /// </summary>
        protected abstract PanelView TargetPanel { get; set; }

        /// <summary>
        /// Initializes the panel's state immediately (without animation).
        /// Called when the panel is first created or reset.
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// Creates and returns a <see cref="Sequence"/> for the "Show" animation.
        /// </summary>
        /// <returns>A PrimeTween Sequence that handles showing the panel.</returns>
        public abstract Sequence MakeVisible();

        /// <summary>
        /// Creates and returns a <see cref="Sequence"/> for the "Hide" animation.
        /// </summary>
        /// <param name="shouldDestroyAfterHide">If true, the panel should be destroyed after the animation completes.</param>
        /// <returns>A PrimeTween Sequence that handles hiding the panel.</returns>
        public abstract Sequence MakeHidden(bool shouldDestroyAfterHide);
    }
}