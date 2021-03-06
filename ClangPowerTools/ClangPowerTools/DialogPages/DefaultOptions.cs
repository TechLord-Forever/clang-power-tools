﻿namespace ClangPowerTools
{
  public class DefaultOptions
  {
    public static readonly string[] kClangFlags = new string[]
    {
      "-Wall",
      "-fms-compatibility-version=19.10",
      "-fms-compatibility",
      "-Wmicrosoft",
      "-Wno-invalid-token-paste",
      "-Wno-unknown-pragmas",
      "-Wno-unused-variable",
      "-Wno-unused-value"
    };

    public static readonly string[] kTidyChecks = new string[]
    {
      "modernize-use-equals-default",
      "modernize-use-equals-delete"
    };

  }
}
