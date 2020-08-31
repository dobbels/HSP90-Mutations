using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HSP90_Mutations
{
    class Program
    {
        static void Main(string[] args)
        {
            var originalProtein = "EEVETFAFQAEIAQLMSLIINTFY";
            var aminoAcids = new List<char>() { 'A', 'I', 'L', 'M', 'F', 'W', 'Y', 'V', 'S', 'T', 'N', 'Q', 'C', 'G', 'P', 'D', 'E', 'R', 'H', 'K' };

            var mutations = new List<string>();

            var randomGenerator = new Random();

            Console.WriteLine("What is the maximum number of amino acids that you would like to change for each generated mutation? (has to be a number between 3 and 24)");
            var maxNumberOfAAsToAdapt = Convert.ToInt32(Console.ReadLine());

            while (mutations.Count < 10000)
            {
                var numberOfAAsToAdapt = randomGenerator.Next(1, maxNumberOfAAsToAdapt + 1);

#if DEBUG
                Console.WriteLine($"numberOfAAsToAdapt: {numberOfAAsToAdapt}");
#endif

                var indicesToChange = new List<int>();

                while (indicesToChange.Count < numberOfAAsToAdapt)
                {
                    var indexToChange = randomGenerator.Next(originalProtein.Length);

                    indicesToChange.Add(indexToChange);
                    indicesToChange = indicesToChange.Distinct().ToList();
                }

                var generatedProtein = originalProtein;

                foreach (var index in indicesToChange)
                {
                    while (true)
                    {
                        var replacementAA = randomGenerator.Next(aminoAcids.Count);

                        if (!generatedProtein[index].Equals(aminoAcids[replacementAA]))
                        {
                            var firstSubstring = generatedProtein.Substring(0, index);
                            var insertedAA = aminoAcids[replacementAA].ToString();
                            var secondSubstring = generatedProtein.Substring(index + 1);
                            generatedProtein = firstSubstring + insertedAA + secondSubstring;
                            break;
                        }
                    }
                }
#if DEBUG
                var previousMutationsLength = mutations.Count;
#endif

                mutations.Add(generatedProtein);

                mutations = mutations.Distinct().ToList();
#if DEBUG
                if (mutations.Count > previousMutationsLength)
                {
                    if (generatedProtein.Length != originalProtein.Length)
                    {
                        Console.WriteLine("Something is wrong: lengths don't match");
                    }

                    Console.WriteLine(originalProtein);

                    var stringBuilder = new StringBuilder();
                    var numberOfChangedDigits = 0;
                    for (var i = 0; i < generatedProtein.Length; i++)
                    {
                        if (generatedProtein[i].Equals(originalProtein[i]))
                        {
                            stringBuilder.Append(" ");
                        }
                        else
                        {
                            stringBuilder.Append(generatedProtein[i]);
                            numberOfChangedDigits++;
                        }
                    }
                    Console.WriteLine(stringBuilder + $" with a total of {numberOfChangedDigits} changes");

                    Console.WriteLine($"Expected format: {calculateMutantSequence(originalProtein, generatedProtein)}");

                    Console.WriteLine();
                    Console.WriteLine($"Number of mutations at the moment: {mutations.Count}");
                    Console.WriteLine();
                }

                //Console.ReadKey();
#endif
            }

            if (mutations.Contains(originalProtein))
            {
                Console.WriteLine("ERROR: some mutations are identical to the initial protein.");
            }
            else
            {
                Console.WriteLine("SUCCESS: no mutations are identical to the initial protein.");
            }

            if (mutations.Distinct().ToList().Count < 10000)
            {
                Console.WriteLine("ERROR: some generated mutations are identical to each other.");
            }
            else
            {
                Console.WriteLine("SUCCESS: no generated mutations are identical to each other.");
            }

            Console.WriteLine($"Number of generated mutations: {mutations.Count}");

            var currentDirectory = Directory.GetCurrentDirectory();
            var fileName = $"mutations-max-{maxNumberOfAAsToAdapt}-AA-changes-per-mutation.txt";
            var filePath = Path.Combine(currentDirectory, fileName);
            using var file = new StreamWriter(fileName);
            foreach (var mutation in mutations)
            {
                file.WriteLine(CalculateMutantSequence(originalProtein, mutation));
            }

            Console.WriteLine($"All mutations have been written to the file with location {filePath}.");

            Console.WriteLine();
            Console.WriteLine("Press any key to close this window");
            Console.ReadKey();
        }

        private static string CalculateMutantSequence(string initialProtein, string generatedProtein)
        {
            var mutants = new List<string>();

            for (var i = 0; i < generatedProtein.Length; i++)
            {
                if (!generatedProtein[i].Equals(initialProtein[i]))
                {
                    var stringBuilder = new StringBuilder();
                    stringBuilder.Append(initialProtein[i]);
                    stringBuilder.Append("A");
                    var position = 10 + i;
                    stringBuilder.Append(position);
                    stringBuilder.Append(generatedProtein[i]);
                    mutants.Add(stringBuilder.ToString());
                }
            }

            return mutants.Aggregate((current, next) => current + "," + next) + ";";
        }
    }
}