using AnimaSeek.iOS.UI.Accessibility;
using AnimaSeek.iOS.UI.App;
using AnimaSeek.iOS.UI.Components;
using Foundation;
using UIKit;

namespace AnimaSeek.iOS.UI.Screens.About;

/// <summary>
/// Shows AnimaSeek's product identity, version,
/// source, and permanent legal route.
/// </summary>
internal sealed class AboutViewController :
    UIViewController
{
    private const string SourceUrl =
        "https://github.com/alexandreteles/AnimaSeek";

    private readonly IAppRouter router;

    /// <summary>
    /// Creates the About destination.
    /// </summary>
    /// <param name="router">
    /// The application router used for legal navigation.
    /// </param>
    public AboutViewController(
        IAppRouter router)
    {
        this.router =
            router
            ?? throw new ArgumentNullException(
                nameof(router));

        Title = AppStrings.Get(
            "IosUiAbout");
    }

    /// <inheritdoc/>
    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        View!.BackgroundColor =
            UIColor.SystemBackground;

        View.AccessibilityIdentifier =
            AccessibilityIdentifiers.AboutScreen;

        NavigationItem.LargeTitleDisplayMode =
            UINavigationItemLargeTitleDisplayMode.Never;

        UIScrollView scrollView =
            new()
            {
                AlwaysBounceVertical = true,
                TranslatesAutoresizingMaskIntoConstraints =
                    false,
            };

        UIStackView content =
            UIKitFactory.VerticalStack(16);

        content.LayoutMargins =
            new UIEdgeInsets(
                24,
                0,
                40,
                0);

        content.LayoutMarginsRelativeArrangement =
            true;

        UIImageView logo =
            CreateLogoView();

        UILabel title =
            CreateTitleLabel();

        UILabel summary =
            CreateSummaryLabel();

        UILabel version =
            CreateVersionLabel();

        UIButton source =
            UIKitFactory.Button(
                AppStrings.Get(
                    "about_link_source_title"),
                UIButtonConfiguration
                    .TintedButtonConfiguration,
                OpenSource,
                "safari")
            .Identified(
                AccessibilityIdentifiers
                    .AboutSource);

        UIButton legal =
            UIKitFactory.Button(
                AppStrings.Get(
                    "IosUiLegal"),
                UIButtonConfiguration
                    .PlainButtonConfiguration,
                () =>
                    router.Navigate(
                        new AppRoute
                            .LegalNotices()),
                "doc.text");

        legal.AccessibilityIdentifier =
            AccessibilityIdentifiers.AboutLegal;

        UIView[] arrangedViews =
        [
            logo,
            title,
            summary,
            version,
            source,
            legal,
        ];

        foreach (UIView arrangedView
                 in arrangedViews)
        {
            content.AddArrangedSubview(
                arrangedView);
        }

        View.AddSubview(scrollView);
        scrollView.AddSubview(content);

        NSLayoutConstraint.ActivateConstraints(
        [
            scrollView.TopAnchor
                .ConstraintEqualTo(
                    View.TopAnchor),

            scrollView.BottomAnchor
                .ConstraintEqualTo(
                    View.BottomAnchor),

            scrollView.LeadingAnchor
                .ConstraintEqualTo(
                    View.LeadingAnchor),

            scrollView.TrailingAnchor
                .ConstraintEqualTo(
                    View.TrailingAnchor),

            content.TopAnchor
                .ConstraintEqualTo(
                    scrollView
                        .ContentLayoutGuide
                        .TopAnchor),

            content.BottomAnchor
                .ConstraintEqualTo(
                    scrollView
                        .ContentLayoutGuide
                        .BottomAnchor),

            content.LeadingAnchor
                .ConstraintEqualTo(
                    scrollView
                        .FrameLayoutGuide
                        .LeadingAnchor,
                    24),

            content.TrailingAnchor
                .ConstraintEqualTo(
                    scrollView
                        .FrameLayoutGuide
                        .TrailingAnchor,
                    -24),

            logo.HeightAnchor
                .ConstraintLessThanOrEqualTo(
                    160),

            logo.WidthAnchor
                .ConstraintEqualTo(
                    logo.HeightAnchor),
        ]);
    }

    private static UIImageView CreateLogoView()
    {
        UIImageView logo =
            new(
                UIImage.FromBundle(
                    "LaunchLogo"))
            {
                ContentMode =
                    UIViewContentMode
                        .ScaleAspectFit,

                TranslatesAutoresizingMaskIntoConstraints =
                    false,

                IsAccessibilityElement =
                    true,

                AccessibilityLabel =
                    AppStrings.Get(
                        "IosUiAppLogoAccessibilityLabel"),

                AccessibilityIgnoresInvertColors =
                    true,

                ClipsToBounds = true,
            };

        // Compatible avec iOS 15.
        // Remplace CornerConfiguration,
        // disponible seulement dans les SDK récents.
        logo.Layer.CornerRadius = 24;
        logo.Layer.MasksToBounds = true;

        logo.SetContentCompressionResistancePriority(
            (float)
                UILayoutPriority.DefaultLow,
            UILayoutConstraintAxis.Vertical);

        return logo;
    }

    private static UILabel CreateTitleLabel()
    {
        UILabel title =
            UIKitFactory.Label(
                UIFontTextStyle.Title1);

        title.Text =
            AppStrings.Get(
                "app_name");

        title.TextAlignment =
            UITextAlignment.Center;

        title.Font =
            UIFont.GetPreferredFontForTextStyle(
                UIFontTextStyle.LargeTitle)
            ?? UIFont.SystemFontOfSize(
                UIFont.LabelFontSize)!;

        return title;
    }

    private static UILabel CreateSummaryLabel()
    {
        UILabel summary =
            UIKitFactory.Label(
                UIFontTextStyle.Body,
                UIColor.SecondaryLabel);

        summary.Text =
            AppStrings.Get(
                "IosUiAboutBody");

        summary.TextAlignment =
            UITextAlignment.Center;

        return summary;
    }

    private static UILabel CreateVersionLabel()
    {
        UILabel version =
            UIKitFactory.Label(
                UIFontTextStyle.Footnote,
                UIColor.SecondaryLabel);

        string shortVersion =
            NSBundle.MainBundle
                .InfoDictionary?[
                    "CFBundleShortVersionString"]
                ?.ToString()
            ?? AppStrings.Get(
                "IosUiUnknown");

        string buildVersion =
            NSBundle.MainBundle
                .InfoDictionary?[
                    "CFBundleVersion"]
                ?.ToString()
            ?? AppStrings.Get(
                "IosUiUnknown");

        version.Text =
            AppStrings.Format(
                "IosUiVersionFormat",
                shortVersion,
                buildVersion);

        version.TextAlignment =
            UITextAlignment.Center;

        version.AccessibilityLabel =
            version.Text;

        return version;
    }

    /// <summary>
    /// Opens the canonical project source in
    /// the system-selected browser.
    /// </summary>
    private void OpenSource()
    {
        using NSUrl? url =
            NSUrl.FromString(SourceUrl);

        if (url is null)
        {
            return;
        }

        UIApplication.SharedApplication
            .OpenUrl(
                url,
                new UIApplicationOpenUrlOptions(),
                _ =>
                {
                });
    }
}
