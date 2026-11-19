using System.Runtime.CompilerServices;
using UIKit;

namespace AnimaSeek.iOS.UI.Components;

/// <summary>
/// Describes an empty, loading, offline, or recoverable-error
/// presentation compatible with iOS 15.
/// </summary>
/// <param name="Title">
/// The localized state title.
/// </param>
/// <param name="Message">
/// The optional localized explanation and next-step guidance.
/// </param>
/// <param name="SymbolName">
/// The optional SF Symbol that reinforces the text.
/// </param>
/// <param name="ActionTitle">
/// The optional localized recovery action.
/// </param>
/// <param name="Action">
/// The recovery operation.
/// </param>
/// <param name="IsLoading">
/// Whether an activity indicator should be displayed.
/// </param>
internal sealed record ContentStatePresentation(
    string Title,
    string? Message = null,
    string? SymbolName = null,
    string? ActionTitle = null,
    Action? Action = null,
    bool IsLoading = false);

/// <summary>
/// Displays empty, loading, offline, and recoverable-error
/// states using UIKit components available on iOS 15.
/// </summary>
internal static class ContentStateView
{
    private const int ContainerTag = 764_215;

    private static readonly ConditionalWeakTable<
        UIViewController,
        UIView
    > PresentedViews = new();

    /// <summary>
    /// Shows a complete-screen state while no useful content exists.
    /// </summary>
    /// <param name="controller">
    /// The controller that owns the content plane.
    /// </param>
    /// <param name="presentation">
    /// The localized state presentation.
    /// </param>
    public static void Show(
        UIViewController controller,
        ContentStatePresentation presentation)
    {
        ArgumentNullException.ThrowIfNull(controller);
        ArgumentNullException.ThrowIfNull(presentation);

        Clear(controller);

        if (controller.View is null)
        {
            return;
        }

        UIView container = CreateContainer();
        UIStackView stack = CreateStack();

        container.Tag = ContainerTag;

        controller.View.AddSubview(container);
        container.AddSubview(stack);

        AddContainerConstraints(
            controller.View,
            container);

        AddStackConstraints(
            container,
            stack);

        if (presentation.IsLoading)
        {
            UIActivityIndicatorView indicator =
                new(
                    UIActivityIndicatorViewStyle.Medium)
                {
                    HidesWhenStopped = false,
                    TranslatesAutoresizingMaskIntoConstraints =
                        false,
                };

            indicator.StartAnimating();

            stack.AddArrangedSubview(indicator);
        }
        else if (!string.IsNullOrWhiteSpace(
                     presentation.SymbolName))
        {
            UIImageView imageView =
                CreateSymbolImageView(
                    presentation.SymbolName);

            stack.AddArrangedSubview(imageView);
        }

        UILabel titleLabel =
            CreateTitleLabel(
                presentation.Title);

        stack.AddArrangedSubview(titleLabel);

        if (!string.IsNullOrWhiteSpace(
                presentation.Message))
        {
            UILabel messageLabel =
                CreateMessageLabel(
                    presentation.Message);

            stack.AddArrangedSubview(messageLabel);
        }

        if (!string.IsNullOrWhiteSpace(
                presentation.ActionTitle) &&
            presentation.Action is not null)
        {
            UIButton actionButton =
                CreateActionButton(
                    presentation.ActionTitle,
                    presentation.Action);

            stack.AddArrangedSubview(actionButton);

            NSLayoutConstraint.ActivateConstraints(
            [
                actionButton.HeightAnchor
                    .ConstraintGreaterThanOrEqualTo(44),
            ]);
        }

        PresentedViews.Add(
            controller,
            container);
    }

    /// <summary>
    /// Removes the currently displayed state and restores
    /// the controller's normal content.
    /// </summary>
    /// <param name="controller">
    /// The controller whose state should clear.
    /// </param>
    public static void Clear(
        UIViewController controller)
    {
        ArgumentNullException.ThrowIfNull(controller);

        if (PresentedViews.TryGetValue(
                controller,
                out UIView? presentedView))
        {
            StopActivityIndicators(
                presentedView);

            presentedView.RemoveFromSuperview();

            PresentedViews.Remove(controller);

            return;
        }

        if (controller.View?
                .ViewWithTag(ContainerTag)
            is UIView fallbackView)
        {
            StopActivityIndicators(
                fallbackView);

            fallbackView.RemoveFromSuperview();
        }
    }

    private static UIView CreateContainer()
    {
        return new UIView
        {
            BackgroundColor =
                UIColor.SystemBackground,
            TranslatesAutoresizingMaskIntoConstraints =
                false,
            IsAccessibilityElement = false,
        };
    }

    private static UIStackView CreateStack()
    {
        return new UIStackView
        {
            Axis =
                UILayoutConstraintAxis.Vertical,
            Alignment =
                UIStackViewAlignment.Center,
            Distribution =
                UIStackViewDistribution.Fill,
            Spacing = 12,
            TranslatesAutoresizingMaskIntoConstraints =
                false,
            LayoutMarginsRelativeArrangement = true,
            DirectionalLayoutMargins =
                new NSDirectionalEdgeInsets(
                    24,
                    24,
                    24,
                    24),
        };
    }

    private static UIImageView CreateSymbolImageView(
        string symbolName)
    {
        UIImage? image =
            UIImage.GetSystemImage(symbolName);

        UIImageView imageView =
            new(image)
            {
                ContentMode =
                    UIViewContentMode.ScaleAspectFit,
                TintColor =
                    UIColor.SecondaryLabel,
                TranslatesAutoresizingMaskIntoConstraints =
                    false,
                IsAccessibilityElement = false,
            };

        UIImageSymbolConfiguration symbolConfiguration =
            UIImageSymbolConfiguration.Create(
                38,
                UIImageSymbolWeight.Regular);

        imageView.PreferredSymbolConfiguration =
            symbolConfiguration;

        NSLayoutConstraint.ActivateConstraints(
        [
            imageView.WidthAnchor
                .ConstraintEqualTo(52),

            imageView.HeightAnchor
                .ConstraintEqualTo(52),
        ]);

        return imageView;
    }

    private static UILabel CreateTitleLabel(
        string title)
    {
        return new UILabel
        {
            Text = title,
            Font =
                UIFont.GetPreferredFontForTextStyle(
                    UIFontTextStyle.Title2),
            TextColor =
                UIColor.Label,
            TextAlignment =
                UITextAlignment.Center,
            Lines = 0,
            AdjustsFontForContentSizeCategory =
                true,
            TranslatesAutoresizingMaskIntoConstraints =
                false,
            IsAccessibilityElement = true,
            AccessibilityTraits =
                UIAccessibilityTrait.Header,
        };
    }

    private static UILabel CreateMessageLabel(
        string message)
    {
        UILabel label =
            new()
            {
                Text = message,
                Font =
                    UIFont.GetPreferredFontForTextStyle(
                        UIFontTextStyle.Body),
                TextColor =
                    UIColor.SecondaryLabel,
                TextAlignment =
                    UITextAlignment.Center,
                Lines = 0,
                AdjustsFontForContentSizeCategory =
                    true,
                TranslatesAutoresizingMaskIntoConstraints =
                    false,
            };

        label.SetContentCompressionResistancePriority(
            (float)UILayoutPriority.Required,
            UILayoutConstraintAxis.Vertical);

        return label;
    }

    private static UIButton CreateActionButton(
        string title,
        Action action)
    {
        UIButton button =
            UIButton.FromType(
                UIButtonType.System);

        button.SetTitle(
            title,
            UIControlState.Normal);

        button.TitleLabel.Font =
            UIFont.GetPreferredFontForTextStyle(
                UIFontTextStyle.Headline);

        button.TitleLabel
            .AdjustsFontForContentSizeCategory =
            true;

        button.ContentEdgeInsets =
            new UIEdgeInsets(
                10,
                18,
                10,
                18);

        button.Layer.CornerRadius = 10;
        button.Layer.MasksToBounds = true;

       button.BackgroundColor =
    UIColor.SystemBlue;

        button.SetTitleColor(
            UIColor.White,
            UIControlState.Normal);

        button.TranslatesAutoresizingMaskIntoConstraints =
            false;

        button.AccessibilityLabel = title;

        button.TouchUpInside +=
            (_, _) => action();

        return button;
    }

    private static void AddContainerConstraints(
        UIView parent,
        UIView container)
    {
        NSLayoutConstraint.ActivateConstraints(
        [
            container.LeadingAnchor
                .ConstraintEqualTo(
                    parent.LeadingAnchor),

            container.TrailingAnchor
                .ConstraintEqualTo(
                    parent.TrailingAnchor),

            container.TopAnchor
                .ConstraintEqualTo(
                    parent.TopAnchor),

            container.BottomAnchor
                .ConstraintEqualTo(
                    parent.BottomAnchor),
        ]);
    }

    private static void AddStackConstraints(
        UIView container,
        UIStackView stack)
    {
        NSLayoutConstraint.ActivateConstraints(
        [
            stack.CenterXAnchor
                .ConstraintEqualTo(
                    container.CenterXAnchor),

            stack.CenterYAnchor
                .ConstraintEqualTo(
                    container.CenterYAnchor),

            stack.LeadingAnchor
                .ConstraintGreaterThanOrEqualTo(
                    container.SafeAreaLayoutGuide
                        .LeadingAnchor,
                    20),

            stack.TrailingAnchor
                .ConstraintLessThanOrEqualTo(
                    container.SafeAreaLayoutGuide
                        .TrailingAnchor,
                    -20),

            stack.WidthAnchor
                .ConstraintLessThanOrEqualTo(460),
        ]);
    }

    private static void StopActivityIndicators(
        UIView view)
    {
        if (view is UIActivityIndicatorView indicator)
        {
            indicator.StopAnimating();
        }

        foreach (UIView subview in view.Subviews)
        {
            StopActivityIndicators(subview);
        }
    }
}
