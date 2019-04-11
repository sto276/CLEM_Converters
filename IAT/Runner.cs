using Models;
using Models.Core;
using Models.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAT
{
    public static class Runner
    {
        public static void Run(IEnumerable<string> files)
        {
            Simulations simulations = new Simulations(null);
            DataStore store = new DataStore(simulations);

            foreach (string file in files)
            {
                IAT iat = PrepareIAT(file, Toolbox.OutDir);

                Simulation simulation = new Simulation(simulations)
                {
                    Name = iat.name,
                    Source = iat
                };                
            }
        }

        private static IAT PrepareIAT(string path, string folder)
        {
            // Create the object
            IAT iat = new IAT(path);

            // Prepare the directory
            Directory.CreateDirectory($"{folder}/{iat.name}");

            // Write the PRN files
            Grains.FileCrop(iat);
            Grains.FileResidue(iat);
            Forages.FileForage(iat);

            return iat;
        }
    }
}
