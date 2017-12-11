﻿using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Shell;

namespace ClangPowerTools
{
  public abstract class ClangCommand : BasicCommand
  {
    #region Members

    protected static CommandsController mCommandsController = null;
    protected ItemsCollector mItemsCollector;
    protected static RunningProcesses mRunningProcesses = new RunningProcesses();
    protected List<string> mDirectoriesPath = new List<string>();
    protected static OutputManager mOutputManager;

    private ErrorsManager mErrorsManager;
    private GeneralOptions mGeneralOptions;
    private PowerShellWrapper mPowerShell = new PowerShellWrapper();
    private ScriptBuiler mScriptBuilder;
    private const string kVs15Version = "2017";
    private Dictionary<string, string> mVsVersions = new Dictionary<string, string>
    {
      {"11.0", "2010"},
      {"12.0", "2012"},
      {"13.0", "2013"},
      {"14.0", "2015"},
      {"15.0", "2017"}
    };

    #endregion

    #region Properties

    protected string VsEdition { get; set; }
    protected string VsVersion { get; set; }
    protected string WorkingDirectoryPath { get; set; }

    #endregion

    #region Constructor

    public ClangCommand(Package aPackage, Guid aGuid, int aId) : base(aPackage, aGuid, aId)
    {
      VsEdition = DTEObj.Edition;
      mVsVersions.TryGetValue(DTEObj.Version, out string version);
      VsVersion = version;

      if (null == mCommandsController)
        mCommandsController = new CommandsController(ServiceProvider, DTEObj);

      mErrorsManager = new ErrorsManager(Package, DTEObj);
      mGeneralOptions = (GeneralOptions)Package.GetDialogPage(typeof(GeneralOptions));
    }

    #endregion

    #region Protected methods

    protected void RunScript(string aCommandName, TidyOptions mTidyOptions = null, TidyChecks mTidyChecks = null)
    {
      mScriptBuilder = new ScriptBuiler();
      mScriptBuilder.ConstructParameters(mGeneralOptions, mTidyOptions, mTidyChecks,
        DTEObj, VsEdition, VsVersion);

      string solutionPath = DTEObj.Solution.FullName;

      mOutputManager = new OutputManager(DTEObj);
      InitPowerShell();
      ClearWindows(aCommandName);
      mOutputManager.AddMessage($"\n{OutputWindowConstants.kStart} {aCommandName}\n");
      foreach (var item in mItemsCollector.GetItems)
      {
        var blub = $"GetName: {item.GetName()}, ToString: {item.ToString()}, GetPath: {item.GetPath()}, GetType: {item.GetType().FullName}";
        if(item is SelectedProject sp)
        { 
          var pro = (sp.GetObject() as EnvDTE.Project);
          blub += $", Kind: {pro.Kind} FileName: {pro.FileName} FullName: {pro.FullName} IsDirty: {pro.IsDirty} Name: {pro.Name} ParentProjectItem: {pro.ParentProjectItem} UniqueName: {pro.UniqueName}";
        }
        mOutputManager.AddMessage(blub);
        try
        {
          var script = mScriptBuilder.GetScript(item, solutionPath);
          if (!mCommandsController.Running)
            break;

          var process = mPowerShell.Invoke(script);
          mRunningProcesses.Add(process);

          if (mOutputManager.MissingLlvm)
          {
            mOutputManager.AddMessage(ErrorParserConstants.kMissingLlvmMessage);
            break;
          }
        }
        catch(Exception ex)
        {
          mOutputManager.AddMessage($"{ex}, {ex.Message}");
        }
      }
      if (!mOutputManager.EmptyBuffer)
        mOutputManager.AddMessage(String.Join("\n", mOutputManager.Buffer));
      if (!mOutputManager.MissingLlvm)
      {
        mOutputManager.Show();
        mOutputManager.AddMessage($"\n{OutputWindowConstants.kDone} {aCommandName}\n");
      }
      if (mOutputManager.HasErrors)
        mErrorsManager.AddErrors(mOutputManager.Errors);
    }

    protected List<IItem> CollectSelectedItems()
    {
      mItemsCollector = new ItemsCollector(Package);
      mItemsCollector.CollectSelectedFiles(DTEObj, ActiveWindowProperties.GetProjectItemOfActiveWindow(DTEObj));
      return mItemsCollector.GetItems;
    }

    protected void SaveActiveDocuments() => DTEObj.Documents.SaveAll();

    #endregion

    #region Private Methods

    private void InitPowerShell()
    {
      mPowerShell = new PowerShellWrapper();
      mPowerShell.DataHandler += mOutputManager.OutputDataReceived;
      mPowerShell.DataErrorHandler += mOutputManager.OutputDataErrorReceived;
    }

    private void ClearWindows(string aCommandName)
    {
      mErrorsManager.Clear();
      mOutputManager.Clear();
      mOutputManager.Show();
    }

    #endregion

  }
}
