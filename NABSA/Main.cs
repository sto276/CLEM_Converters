namespace NABSA
{
    using System.IO;
    using System;

    class Terminal
    {
        public static void Main(string[] args)
        {
            string source = "Simulations";     

            // Checks that the source directory exists
            if (!Directory.Exists(source))
            {
                Console.WriteLine("Source directory not found.");
                return;
            }

            string[] dir = Directory.GetFiles(source, "*.nabsa");

            Console.WriteLine("NABSA to CLEM file conversion.");
            Console.WriteLine("Missing/Incorrect IAT data may produce incomplete .apsimx files.\n");

            do
            {
                int choice = ChooseFile(dir);
                if (choice == 0)
                {
                    foreach (string iat in dir) Simulation.Create(iat);
                    break; // Skips reset if all files are converted
                }
                else Simulation.Create(dir[choice - 1]);
            } while (Reset());

            Console.ReadKey();
            return;
        }

        /// <summary>
        /// Asks the user to choose a file to write a simulation for
        /// </summary>
        /// <param name="dir">The list of files to choose from (the directory)</param>
        private static int ChooseFile(string[] dir)
        {
            int output;
            do
            {
                Console.WriteLine("The following IAT files are available:\n");
                for (int i = 0; i < dir.Length; i++)
                    Console.WriteLine($"  {i + 1}. {dir[i]}");
                Console.WriteLine("\nEnter the number corresponding to the file you wish to convert, or '0' for all files.");

                string input = Console.ReadLine();
                if (int.TryParse(input, out output)) break;
                else Console.WriteLine("Invalid input.");
            }
            while (Reset());

            return output;
        }

        /// <summary>
        /// Asks the user if they want to attempt something again
        /// </summary>
        /// <returns>true if yes, false if no</returns>
        private static bool Reset()
        {
            Console.WriteLine("\nWould you like to try again? (y/n)");
            ConsoleKeyInfo key = Console.ReadKey();
            Console.WriteLine();
            if (key.KeyChar == 'y') return true;
            else return false;
        }
    }
}
