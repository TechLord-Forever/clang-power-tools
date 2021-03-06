﻿using System.Text.RegularExpressions;

namespace ClangPowerTools
{
  public class PCHParser
  {
    #region Members

    private const string kPCHRegex = @"(PCH:)(\t| )*(.\:\\[ \S+\\\/]*.clang.pch)";

    #endregion

    #region Public Methods

    public bool FindPath(string aMessage, out string aPath)
    {
      aPath = string.Empty;
      Regex regex = new Regex(kPCHRegex);
      Match matchResult = regex.Match(aMessage);
      if (!matchResult.Success)
        return false;
      aPath = matchResult.Groups[3].Value;
      return true;
    }

    #endregion

  }
}
