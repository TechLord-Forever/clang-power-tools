﻿using ClangPowerTools.Properties;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel;
using System.Linq;
using System.Web.UI.WebControls;

namespace ClangPowerTools
{
  [Serializable]
  public class GeneralOptions : DialogPage
  {
    #region Members

    private string[] mClangFlags = new string[] { };

    #endregion

    #region Properties

    [Category("General")]
    [DisplayName("Project to ignore")]
    [Description("Array of project(s) to ignore, from the matched ones. If empty, all already matched projects are compiled.")]
    [TypeConverter(typeof(StringArrayConverter))]
    public string[] ProjectsToIgnore { get; set; }

    [Category("General")]
    [DisplayName("File to ignore")]
    [Description("Array of file(s) to ignore, from the matched ones. If empty, all already matched files are compiled.")]
    [TypeConverter(typeof(StringArrayConverter))]
    public string[] FilesToIgnore { get; set; }

    [Category("General")]
    [DisplayName("Continue On Error")]
    [Description("Switch to continue project compilation even when errors occur.")]
    public bool Continue { get; set; }

    [Category("General")]
    [DisplayName("Treat Warnings As Errors")]
    [Description("Treats all compiler warnings as errors. For a new project, it may be best to use in all compilations; resolving all warnings will ensure the fewest possible hard to find code defects.")]
    public bool TreatWarningsAsErrors { get; set; } = true;

    [Category("General")]
    [DisplayName("Verbose Mode")]
    [Description("Enables verbose logging for diagnostic purposes.")]
    public bool VerboseMode { get; set; }

    [Category("General")]
    [DisplayName("Compile Flags")]
    [Description("Flags given to clang++ when compiling project, alongside project - specific defines. If empty the default flags will be loaded.")]
    [TypeConverter(typeof(StringArrayConverter))]
    public string[] ClangFlags
    {
      get => 0 == mClangFlags.Length ? DefaultOptions.kClangFlags : mClangFlags;
      set => mClangFlags = value;
    }

    [Browsable(false)]
    public ClangOptions SavedUserSettings
    {
      get { return Settings.Default.GeneralOptions; }
      set { Settings.Default.GeneralOptions = value; }
    }

    #endregion

    #region DialogPage Save and Load implementation 

    public override void SaveSettingsToStorage()
    {
      SavedUserSettings.ProjectsToIgnore = this.ProjectsToIgnore.ToList();
      SavedUserSettings.FilesToIgnore = this.FilesToIgnore.ToList();
      SavedUserSettings.Continue = this.Continue;
      SavedUserSettings.TreatWarningsAsErrors = this.TreatWarningsAsErrors;
      SavedUserSettings.VerboseMode = this.VerboseMode;
      SavedUserSettings.ClangFlags = this.ClangFlags.ToList();

      base.SaveSettingsToStorage();
      Settings.Default.Save();

    }

    public override void LoadSettingsFromStorage()
    {
      if (SavedUserSettings == null)
        SavedUserSettings = new ClangOptions();

      this.ProjectsToIgnore = SavedUserSettings.ProjectsToIgnore.ToArray();
      this.FilesToIgnore = SavedUserSettings.FilesToIgnore.ToArray();
      this.Continue = SavedUserSettings.Continue;
      this.TreatWarningsAsErrors = SavedUserSettings.TreatWarningsAsErrors;
      this.VerboseMode = SavedUserSettings.VerboseMode;
      this.ClangFlags = SavedUserSettings.ClangFlags.ToArray();

      base.LoadSettingsFromStorage();
    }

    #endregion

  }
}
