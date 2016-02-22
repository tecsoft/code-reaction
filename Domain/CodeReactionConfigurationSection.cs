using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace CodeReaction
{
    public class CodeReactionConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("svn")]
        public SvnElement Svn
        {
            get { return this["svn"] as SvnElement;  }
            set { this["svn"] = value; }
        }
    }

    public class SvnElement : ConfigurationElement
    {
        [ConfigurationProperty("server")]
        public string Server
        {
            get { return this["server"] as string; }
            set { this["server"] = value; }
        }

        [ConfigurationProperty("startRevision", DefaultValue = -1)]
        public int StartRevision
        {
            get { return (int)this["startRevision"]; }
            set { this["startRevision"] = value; }
        }

        [ConfigurationProperty("username")]
        public string Username
        {
            get { return this["username"] as string;  }
            set { this["username"] = value;  }
        }

        [ConfigurationProperty("password")]
        public string Password
        {
            get { return this["password"] as string; }
            set { this["password"] = value; }
        }
    }
}
