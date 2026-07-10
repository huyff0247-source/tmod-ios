using Foundation;
using TMLiOS.Core.Diagnostics;
using TMLiOS.Core.Mods;
using TMLiOS.Core.Platform;
using TMLiOS.Core.Runtime;
using TMLiOS.Core.Storage;
using TMLiOS.Core.Terraria;
using UIKit;

namespace TMLiOS.App;

public sealed class LauncherViewController : UIViewController
{
    private readonly AppLog _log = new();
    private AppPaths? _paths;
    private UITextView? _logView;
    private UILabel? _statusLabel;

    public override void ViewDidLoad()
    {
        base.ViewDidLoad();
        Title = "tModLoader iOS";
        View!.BackgroundColor = UIColor.SystemBackground;

        var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        _paths = new AppPaths(documents, _log);
        _log.EntryAdded += _ => InvokeOnMainThread(RefreshLog);

        BuildUi();
        _log.Info("TMLiOS launcher booted.");
        _log.Info("v0.1 bootstrap: import, scan, runtime probe, DLL load.");
    }

    private void BuildUi()
    {
        var root = new UIStackView
        {
            Axis = UILayoutConstraintAxis.Vertical,
            Spacing = 8,
            TranslatesAutoresizingMaskIntoConstraints = false,
            LayoutMargins = new UIEdgeInsets(12, 12, 12, 12),
            LayoutMarginsRelativeArrangement = true
        };

        _statusLabel = new UILabel
        {
            Text = "Bootstrap ready",
            Lines = 0,
            Font = UIFont.SystemFontOfSize(13),
            TextColor = UIColor.SecondaryLabel
        };

        var buttonGrid = new UIStackView
        {
            Axis = UILayoutConstraintAxis.Vertical,
            Spacing = 8
        };

        buttonGrid.AddArrangedSubview(MakeButton("Run Runtime Probe", RunRuntimeProbe));
        buttonGrid.AddArrangedSubview(MakeButton("Import Mod/File", ImportFile));
        buttonGrid.AddArrangedSubview(MakeButton("Scan Mods", ScanMods));
        buttonGrid.AddArrangedSubview(MakeButton("Load Managed DLLs", LoadManagedDlls));
        buttonGrid.AddArrangedSubview(MakeButton("Scan Terraria Import", ScanTerrariaImport));
        buttonGrid.AddArrangedSubview(MakeButton("Save Log", SaveLog));

        _logView = new UITextView
        {
            Editable = false,
            Font = UIFont.MonospacedSystemFontOfSize(11, UIFontWeight.Regular),
            BackgroundColor = UIColor.SecondarySystemBackground,
            TextColor = UIColor.Label
        };
        _logView.Layer.CornerRadius = 8;

        root.AddArrangedSubview(_statusLabel);
        root.AddArrangedSubview(buttonGrid);
        root.AddArrangedSubview(_logView);

        View!.AddSubview(root);
        NSLayoutConstraint.ActivateConstraints(new[]
        {
            root.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor),
            root.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor),
            root.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor),
            root.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor),
            _logView.HeightAnchor.ConstraintGreaterThanOrEqualTo(260)
        });
    }

    private UIButton MakeButton(string title, Action action)
    {
        var button = UIButton.FromType(UIButtonType.System);
        button.SetTitle(title, UIControlState.Normal);
        button.TitleLabel!.Font = UIFont.BoldSystemFontOfSize(16);
        button.BackgroundColor = UIColor.TertiarySystemFill;
        button.Layer.CornerRadius = 8;
        button.HeightAnchor.ConstraintEqualTo(42).Active = true;
        button.TouchUpInside += (_, _) => action();
        return button;
    }

    private void RunRuntimeProbe()
    {
        foreach (var result in JitProbe.RunAll())
        {
            if (result.Success)
                _log.Info($"Probe OK: {result.Name} -> {result.Message}");
            else
                _log.Warning($"Probe failed: {result.Name} -> {result.Message}");
        }
    }

    private void ImportFile()
    {
        var picker = new UIDocumentPickerViewController(new[] { "public.data" }, UIDocumentPickerMode.Import)
        {
            AllowsMultipleSelection = true,
            Delegate = new ImportPickerDelegate(this)
        };
        PresentViewController(picker, true, null);
    }

    private void HandlePickedUrls(NSUrl[] urls)
    {
        if (_paths == null)
            return;

        var importer = new FileImporter(_paths, _log);
        foreach (var url in urls)
        {
            try
            {
                var path = url.Path;
                if (string.IsNullOrWhiteSpace(path))
                {
                    _log.Warning("Picked URL has no local path.");
                    continue;
                }

                importer.ImportToMods(path);
            }
            catch (Exception ex)
            {
                _log.Error("Import failed", ex);
            }
        }
    }

    private void ScanMods()
    {
        if (_paths == null)
            return;

        var scanner = new ModScanner(_paths, _log);
        var results = scanner.Scan();
        _statusLabel!.Text = $"Mods scanned: {results.Count}";
        foreach (var result in results)
        {
            foreach (var issue in result.Issues)
                _log.Warning($"{result.FileName}: {issue.Code} - {issue.Message}");
        }
    }

    private void LoadManagedDlls()
    {
        if (_paths == null)
            return;

        var loader = new AssemblyModLoader(_paths, _log);
        var loaded = loader.LoadManagedDlls();
        _statusLabel!.Text = $"Loaded DLL assemblies: {loaded.Count}";
    }

    private void ScanTerrariaImport()
    {
        if (_paths == null)
            return;

        var scanner = new TerrariaImportScanner(_paths, _log);
        var manifest = scanner.Scan();
        _statusLabel!.Text = manifest.HasMinimumFiles ? "Terraria import: content found" : "Terraria import: missing Content folder";
    }

    private void SaveLog()
    {
        if (_paths == null)
            return;

        var path = _paths.NewLogFilePath();
        _log.SaveTo(path);
        _log.Info("Saved log: " + path);
    }

    private void RefreshLog()
    {
        if (_logView == null)
            return;

        _logView.Text = _log.ToText();
        if (_logView.Text.Length > 0)
        {
            var range = new NSRange(_logView.Text.Length - 1, 1);
            _logView.ScrollRangeToVisible(range);
        }
    }

    private sealed class ImportPickerDelegate : UIDocumentPickerDelegate
    {
        private readonly LauncherViewController _owner;

        public ImportPickerDelegate(LauncherViewController owner)
        {
            _owner = owner;
        }

        public override void DidPickDocument(UIDocumentPickerViewController controller, NSUrl url)
        {
            _owner.HandlePickedUrls(new[] { url });
        }

        public override void DidPickDocument(UIDocumentPickerViewController controller, NSUrl[] urls)
        {
            _owner.HandlePickedUrls(urls);
        }

        public override void WasCancelled(UIDocumentPickerViewController controller)
        {
            _owner._log.Info("Import cancelled.");
        }
    }
}
