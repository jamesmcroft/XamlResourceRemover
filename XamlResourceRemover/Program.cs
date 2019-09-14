namespace XamlResourceRemover
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;

    public class Program
    {
        private const string XamlPresentationNamespace = "def";

        private const string XamlNamespace = "x";

        private const string WindowsMapsNamespace = "maps";

        private static readonly string StyleXPath = $"//{XamlPresentationNamespace}:Style[@x:Key]";

        private static readonly string StyleTargetTypeXPath = $"//{XamlPresentationNamespace}:Style[@TargetType]";

        private static readonly string DataTemplateXPath = $"//{XamlPresentationNamespace}:DataTemplate[@x:Key]";

        private static readonly string ThicknessXPath = $"//{XamlPresentationNamespace}:Thickness[@x:Key]";

        private static readonly string DoubleXPath = $"//{XamlNamespace}:Double[@x:Key]";

        private static readonly string BooleanXPath = $"//{XamlNamespace}:Boolean[@x:Key]";

        private static readonly string FontFamilyXPath = $"//{XamlPresentationNamespace}:FontFamily[@x:Key]";

        private static readonly string FontWeightXPath = $"//{XamlPresentationNamespace}:FontWeight[@x:Key]";

        private static readonly string Int32XPath = $"//{XamlNamespace}:Int32[@x:Key]";

        private static readonly string StringXPath = $"//{XamlNamespace}:String[@x:Key]";

        private static readonly string ColorXPath = $"//{XamlPresentationNamespace}:Color[@x:Key]";

        private static readonly string SolidColorBrushXPath = $"//{XamlPresentationNamespace}:SolidColorBrush[@x:Key]";

        private static readonly string AcrylicBrushXPath = $"//{XamlPresentationNamespace}:AcrylicBrush[@x:Key]";

        private static readonly string StaticResourceXPath = $"//{XamlPresentationNamespace}:StaticResource[@x:Key]";

        private static readonly string GridLengthXPath = $"//{XamlPresentationNamespace}:GridLength[@x:Key]";

        private static readonly string ListViewItemPresenterCheckModeXPath =
            $"//{XamlPresentationNamespace}:ListViewItemPresenterCheckMode[@x:Key]";

        private static readonly string RevealBorderBrushXPath =
            $"//{XamlPresentationNamespace}:RevealBorderBrush[@x:Key]";

        private static readonly string RevealBackgroundBrushXPath =
            $"//{XamlPresentationNamespace}:RevealBackgroundBrush[@x:Key]";

        private static readonly string RatingItemFontInfoXPath =
            $"//{XamlPresentationNamespace}:RatingItemFontInfo[@x:Key]";

        private static readonly string TextCommandBarFlyoutXPath =
            $"//{XamlPresentationNamespace}:TextCommandBarFlyout[@x:Key]";

        public static void Main(string[] args)
        {
            ConsoleHelper.StartFileLogging();

            List<FileInfo> referenceXamlFiles = new List<FileInfo>();
            List<FileInfo> updateXamlFiles = new List<FileInfo>();

            string referenceDirectoryPath = Environment.CurrentDirectory + "\\Reference";
            string updateDirectoryPath = Environment.CurrentDirectory + "\\Update";

            if (args != null && args.Length > 1)
            {
                referenceDirectoryPath = args[0];

                if (args.Length == 2)
                {
                    updateDirectoryPath = args[1];
                }
            }

            GetDirectoryXamlFiles(referenceDirectoryPath, referenceXamlFiles);
            GetDirectoryXamlFiles(updateDirectoryPath, updateXamlFiles);

            Console.WriteLine($"Found {referenceXamlFiles.Count} XAML reference files to retrieve keys for");
            Console.WriteLine($"Found {updateXamlFiles.Count} XAML files to remove keys from");

            List<string> keyResources = new List<string>();
            List<string> genericResources = new List<string>();

            foreach (FileInfo referenceXamlFile in referenceXamlFiles)
            {
                XmlDocument xmlDocument = LoadXmlDocument(referenceXamlFile, out XmlNamespaceManager manager);

                RetrieveGenericResourcesToRemove(genericResources, xmlDocument, manager);
                RetrieveKeyResourcesToRemove(keyResources, xmlDocument, manager);
            }

            Console.WriteLine($"Found {genericResources.Count} generic XAML resources to remove");
            Console.WriteLine($"Found {keyResources.Count} keyed XAML resources to remove");

            foreach (FileInfo updateXamlFile in updateXamlFiles)
            {
                string fileName = updateXamlFile.FullName;
                XmlDocument xmlDocument = LoadXmlDocument(updateXamlFile, out XmlNamespaceManager manager);

                RemoveGenericResources(genericResources, xmlDocument, manager, fileName);
                RemoveKeyResources(keyResources, xmlDocument, manager, fileName);

                xmlDocument.Save(fileName);
            }

            ConsoleHelper.StopFileLogging();


            Console.WriteLine("Completed");
            Console.ReadLine();
        }

        private static void GetDirectoryXamlFiles(string directoryPath, List<FileInfo> fileInfos)
        {
            try
            {
                DirectoryInfo referenceDirectory = new DirectoryInfo(directoryPath);
                GetResourcesFromDirectory(referenceDirectory, fileInfos);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static void RetrieveGenericResourcesToRemove(
            List<string> resources,
            XmlDocument xmlDocument,
            XmlNamespaceManager manager)
        {
            try
            {
                ExtractResourcesFromNodes(StyleTargetTypeXPath, "TargetType", resources, xmlDocument, manager);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void RetrieveKeyResourcesToRemove(
            List<string> resources,
            XmlDocument xmlDocument,
            XmlNamespaceManager manager)
        {
            try
            {
                ExtractResourcesFromNodes(StyleXPath, "x:Key", resources, xmlDocument, manager);
                ExtractResourcesFromNodes(DataTemplateXPath, "x:Key", resources, xmlDocument, manager);
                ExtractResourcesFromNodes(ThicknessXPath, "x:Key", resources, xmlDocument, manager);
                ExtractResourcesFromNodes(DoubleXPath, "x:Key", resources, xmlDocument, manager);
                ExtractResourcesFromNodes(BooleanXPath, "x:Key", resources, xmlDocument, manager);
                ExtractResourcesFromNodes(FontFamilyXPath, "x:Key", resources, xmlDocument, manager);
                ExtractResourcesFromNodes(FontWeightXPath, "x:Key", resources, xmlDocument, manager);
                ExtractResourcesFromNodes(Int32XPath, "x:Key", resources, xmlDocument, manager);
                ExtractResourcesFromNodes(StringXPath, "x:Key", resources, xmlDocument, manager);
                ExtractResourcesFromNodes(ColorXPath, "x:Key", resources, xmlDocument, manager);
                ExtractResourcesFromNodes(SolidColorBrushXPath, "x:Key", resources, xmlDocument, manager);
                ExtractResourcesFromNodes(AcrylicBrushXPath, "x:Key", resources, xmlDocument, manager);
                ExtractResourcesFromNodes(StaticResourceXPath, "x:Key", resources, xmlDocument, manager);
                ExtractResourcesFromNodes(GridLengthXPath, "x:Key", resources, xmlDocument, manager);
                ExtractResourcesFromNodes(
                    ListViewItemPresenterCheckModeXPath,
                    "x:Key",
                    resources,
                    xmlDocument,
                    manager);
                ExtractResourcesFromNodes(RevealBorderBrushXPath, "x:Key", resources, xmlDocument, manager);
                ExtractResourcesFromNodes(RevealBackgroundBrushXPath, "x:Key", resources, xmlDocument, manager);
                ExtractResourcesFromNodes(RatingItemFontInfoXPath, "x:Key", resources, xmlDocument, manager);
                ExtractResourcesFromNodes(TextCommandBarFlyoutXPath, "x:Key", resources, xmlDocument, manager);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void RemoveGenericResources(
            List<string> resources,
            XmlDocument xmlDocument,
            XmlNamespaceManager manager,
            string fileName)
        {
            string[] attributeExcludes = { "x:Key", "x:Name" };

            try
            {
                RemoveResourcesFromNodes(
                    StyleTargetTypeXPath,
                    "TargetType",
                    resources,
                    xmlDocument,
                    manager,
                    fileName,
                    attributeExcludes);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void RemoveKeyResources(
            List<string> resources,
            XmlDocument xmlDocument,
            XmlNamespaceManager manager,
            string fileName)
        {
            string[] attributeExcludes = { "x:Name" };

            try
            {
                RemoveResourcesFromNodes(
                    StyleXPath,
                    "x:Key",
                    resources,
                    xmlDocument,
                    manager,
                    fileName,
                    attributeExcludes);
                RemoveResourcesFromNodes(
                    DataTemplateXPath,
                    "x:Key",
                    resources,
                    xmlDocument,
                    manager,
                    fileName,
                    attributeExcludes);
                RemoveResourcesFromNodes(
                    ThicknessXPath,
                    "x:Key",
                    resources,
                    xmlDocument,
                    manager,
                    fileName,
                    attributeExcludes);
                RemoveResourcesFromNodes(
                    DoubleXPath,
                    "x:Key",
                    resources,
                    xmlDocument,
                    manager,
                    fileName,
                    attributeExcludes);
                RemoveResourcesFromNodes(
                    BooleanXPath,
                    "x:Key",
                    resources,
                    xmlDocument,
                    manager,
                    fileName,
                    attributeExcludes);
                RemoveResourcesFromNodes(
                    FontFamilyXPath,
                    "x:Key",
                    resources,
                    xmlDocument,
                    manager,
                    fileName,
                    attributeExcludes);
                RemoveResourcesFromNodes(
                    FontWeightXPath,
                    "x:Key",
                    resources,
                    xmlDocument,
                    manager,
                    fileName,
                    attributeExcludes);
                RemoveResourcesFromNodes(
                    Int32XPath,
                    "x:Key",
                    resources,
                    xmlDocument,
                    manager,
                    fileName,
                    attributeExcludes);
                RemoveResourcesFromNodes(
                    StringXPath,
                    "x:Key",
                    resources,
                    xmlDocument,
                    manager,
                    fileName,
                    attributeExcludes);
                RemoveResourcesFromNodes(
                    ColorXPath,
                    "x:Key",
                    resources,
                    xmlDocument,
                    manager,
                    fileName,
                    attributeExcludes);
                RemoveResourcesFromNodes(
                    SolidColorBrushXPath,
                    "x:Key",
                    resources,
                    xmlDocument,
                    manager,
                    fileName,
                    attributeExcludes);
                RemoveResourcesFromNodes(
                    AcrylicBrushXPath,
                    "x:Key",
                    resources,
                    xmlDocument,
                    manager,
                    fileName,
                    attributeExcludes);
                RemoveResourcesFromNodes(
                    StaticResourceXPath,
                    "x:Key",
                    resources,
                    xmlDocument,
                    manager,
                    fileName,
                    attributeExcludes);
                RemoveResourcesFromNodes(
                    GridLengthXPath,
                    "x:Key",
                    resources,
                    xmlDocument,
                    manager,
                    fileName,
                    attributeExcludes);
                RemoveResourcesFromNodes(
                    ListViewItemPresenterCheckModeXPath,
                    "x:Key",
                    resources,
                    xmlDocument,
                    manager,
                    fileName,
                    attributeExcludes);
                RemoveResourcesFromNodes(
                    RevealBorderBrushXPath,
                    "x:Key",
                    resources,
                    xmlDocument,
                    manager,
                    fileName,
                    attributeExcludes);
                RemoveResourcesFromNodes(
                    RevealBackgroundBrushXPath,
                    "x:Key",
                    resources,
                    xmlDocument,
                    manager,
                    fileName,
                    attributeExcludes);
                RemoveResourcesFromNodes(
                    RatingItemFontInfoXPath,
                    "x:Key",
                    resources,
                    xmlDocument,
                    manager,
                    fileName,
                    attributeExcludes);
                RemoveResourcesFromNodes(
                    TextCommandBarFlyoutXPath,
                    "x:Key",
                    resources,
                    xmlDocument,
                    manager,
                    fileName,
                    attributeExcludes);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void RemoveResourcesFromNodes(
            string xpath,
            string attribute,
            List<string> resources,
            XmlDocument xmlDocument,
            XmlNamespaceManager manager,
            string fileName,
            params string[] attributeExcludes)
        {
            XmlNodeList nodes = xmlDocument.SelectNodes(xpath, manager);
            if (nodes == null)
            {
                return;
            }

            int idx = 0;
            int count = nodes.Count;

            foreach (XmlNode node in nodes)
            {
                idx++;

                Console.WriteLine($"Processing node {idx} of {count}");

                try
                {
                    if (node.ParentNode.Name.Contains("Setter.Value"))
                    {
                        Console.WriteLine($"Ignoring {node.Name} because it is the child of a Setter");
                        continue;
                    }

                    if (node.Attributes == null)
                    {
                        Console.WriteLine($"Ignoring {node.Name} because it has no attributes");
                        continue;
                    }

                    bool shouldIgnore = false;

                    if (attributeExcludes != null)
                    {
                        foreach (string attributeExclude in attributeExcludes)
                        {
                            if (shouldIgnore)
                            {
                                break;
                            }

                            if (!node.Attributes.Contains(attributeExclude))
                            {
                                continue;
                            }

                            Console.WriteLine(
                                $"Ignoring {node.Name} because it contains excluded attribute {attributeExclude}");
                            shouldIgnore = true;
                        }
                    }

                    if (shouldIgnore)
                    {
                        continue;
                    }

                    string keyedResource = node.Attributes[attribute].Value;

                    if (resources.Contains(keyedResource))
                    {
                        Console.WriteLine($"Removing {keyedResource} from {fileName}");

                        node.ParentNode?.RemoveChild(node);
                    }
                    else
                    {
                        Console.WriteLine(
                            $"Did not remove {keyedResource} from {fileName} as not in the list of resources.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        private static void ExtractResourcesFromNodes(
            string xpath,
            string attribute,
            List<string> resources,
            XmlDocument xmlDocument,
            XmlNamespaceManager manager)
        {
            // Finds all XAML elements and extracts their attributes.
            XmlNodeList nodes = xmlDocument.SelectNodes(xpath, manager);
            if (nodes == null)
            {
                return;
            }

            foreach (XmlNode node in nodes)
            {
                try
                {
                    if (node.Attributes == null)
                    {
                        Console.WriteLine($"Ignoring {node.Name} because it has no attributes");
                        continue;
                    }

                    string keyedResource = node.Attributes[attribute].Value;

                    if (resources.Contains(keyedResource))
                    {
                        continue;
                    }

                    Console.WriteLine($"Found {keyedResource}");
                    resources.Add(keyedResource);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        private static XmlDocument LoadXmlDocument(FileInfo fileInfo, out XmlNamespaceManager manager)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(fileInfo.FullName);

            manager = new XmlNamespaceManager(xmlDocument.NameTable);
            manager.AddNamespace(
                XamlPresentationNamespace,
                "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            manager.AddNamespace(XamlNamespace, "http://schemas.microsoft.com/winfx/2006/xaml");
            manager.AddNamespace(WindowsMapsNamespace, "using:Windows.UI.Xaml.Controls.Maps");

            return xmlDocument;
        }

        private static void GetResourcesFromDirectory(DirectoryInfo directoryInfo, List<FileInfo> resourceFiles)
        {
            if (resourceFiles == null)
            {
                resourceFiles = new List<FileInfo>();
            }

            try
            {
                List<DirectoryInfo> childDirectories = directoryInfo.GetDirectories().ToList();

                if (childDirectories.Count > 0)
                {
                    foreach (DirectoryInfo directory in childDirectories)
                    {
                        GetResourcesFromDirectory(directory, resourceFiles);
                    }
                }

                foreach (FileInfo file in directoryInfo.GetFiles("*.xaml"))
                {
                    Console.WriteLine($"Adding {file.Name} for processing");
                    resourceFiles.Add(file);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}