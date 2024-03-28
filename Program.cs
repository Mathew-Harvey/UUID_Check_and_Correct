using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("Please provide a file path as an argument.");
            return;
        }

        string filePath = args[0];
        ProcessFile(filePath);
    }

    static void ProcessFile(string filePath)
    {
        string fileContent = File.ReadAllText(filePath);
        List<string> potentialGUIDs = ExtractPotentialGUIDs(fileContent);
        List<string> validatedGUIDs = ValidateAndReplaceGUIDs(potentialGUIDs);
        string modifiedContent = UpdateFileContent(fileContent, potentialGUIDs, validatedGUIDs);

        if (modifiedContent != fileContent)
        {
            File.WriteAllText(filePath, modifiedContent);
        }
        else
        {
            Console.WriteLine("No invalid GUIDs found in the file.");
        }
    }

    static List<string> ExtractPotentialGUIDs(string input)
    {
        string pattern = @"(?<=Id\s*=\s*"")[^""]+";
        Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

        MatchCollection matches = regex.Matches(input);
        return matches.Cast<Match>().Select(m => m.Value).ToList();
    }

    static List<string> ValidateAndReplaceGUIDs(List<string> potentialGUIDs)
    {
        List<string> validatedGUIDs = new List<string>();

        foreach (string potentialGUID in potentialGUIDs)
        {
            Console.WriteLine($"Checking GUID: {potentialGUID}");
            if (IsValidGuidFormat(potentialGUID))
            {
                validatedGUIDs.Add(potentialGUID);
            }
            else
            {
                string newGuid = Guid.NewGuid().ToString();
                Console.WriteLine($"Invalid GUID: {potentialGUID} --> {newGuid}");
                validatedGUIDs.Add(newGuid);
            }
        }

        return validatedGUIDs;
    }

    static string UpdateFileContent(string fileContent, List<string> potentialGUIDs, List<string> validatedGUIDs)
    {
        for (int i = 0; i < potentialGUIDs.Count; i++)
        {
            fileContent = fileContent.Replace($"Id = \"{potentialGUIDs[i]}\"", $"Id = \"{validatedGUIDs[i]}\"");
        }

        return fileContent;
    }

    static bool IsValidGuidFormat(string guid)
    {
        string pattern = @"^[A-F0-9]{8}-[A-F0-9]{4}-[A-F0-9]{4}-[A-F0-9]{4}-[A-F0-9]{12}$";
        Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
        return regex.IsMatch(guid);
    }
}