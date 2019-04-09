using System;

namespace SemanticVersioning
{
    public struct SemanticVersion
    {
        public int Major;

        public int Minor;

        public int Patch;

        public string Release;

        public SemanticVersion(int major, int minor, int patch) 
            : this()
        {
            Major = major;
            Minor = minor;
            Patch = patch;
        }

        public SemanticVersion(int major, int minor, int patch, string release) 
            : this(major, minor, patch)
        {
            if (!String.IsNullOrWhiteSpace(release))
            {
                Release = release;
            }
        }

        public static SemanticVersion Parse(string majorValue, string minorValue, string patchValue, string release)
        {
            if (!Int32.TryParse(majorValue, out int major)) throw new ArgumentException("invalid value for major field", nameof(majorValue));
            if (!Int32.TryParse(minorValue, out int minor)) throw new ArgumentException("invalid value for minor field", nameof(minorValue));
            if (!Int32.TryParse(patchValue, out int patch)) throw new ArgumentException("invalid value for patch field", nameof(patchValue));
            return new SemanticVersion(major, minor, patch, release);
        }

        public override string ToString()
        {
            return $"v{Major}.{Minor}.{Patch}{Release}";
        }
    }
}