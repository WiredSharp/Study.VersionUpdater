// //////////////////////////////////////////
// 
//  Created by BRUNEL Eric on 21.11.2018
// 
// //////////////////////////////////////////

namespace SemanticVersioning.Updater
{
    internal class Settings
    {
        public static Settings Current = new Settings();

        private Settings()
        {
            
        }

        public string NugetRepository { get; set; }
    }
}