using EnvDTE;

namespace ClangPowerTools
{
  public static class ProjectConfiguration
  {
    public static string GetPlatform(Project aProjectItem) => 
      aProjectItem.ConfigurationManager.ActiveConfiguration.PlatformName;

        public static string GetConfiguration(Project aProjectItem)
        {
            if (aProjectItem == null)
                System.Windows.Forms.MessageBox.Show("aProjectItem");
            var cm = aProjectItem.ConfigurationManager;
            if(cm == null)
                System.Windows.Forms.MessageBox.Show("cm");
            var ac = cm.ActiveConfiguration;
            if (ac == null)
                System.Windows.Forms.MessageBox.Show("ac");
            var cn = ac.ConfigurationName;
            if(cn == null)
                System.Windows.Forms.MessageBox.Show("cn");
            return cn;
        }
  }
}
