﻿using System;
using System.Collections.Generic;
using JetBrains.Application;
using JetBrains.DataFlow;
using JetBrains.ProjectModel;
using JetBrains.Util;
using JetBrains.ReSharper.Psi.Xml.Tree;

namespace SPCAFContrib.ReSharper.Common.Shell
{
    [ShellComponent]
    public class SPFileTypeDefinitionExtensionMapping : IFileExtensionMapping
    {
        private IEnumerable<string> xmlExtensions = new[] {".feature"};

        private readonly OneToSetMap<string, ProjectFileType> spExtensionsToTypes = new OneToSetMap<string, ProjectFileType>(StringComparer.OrdinalIgnoreCase);
        private readonly OneToSetMap<ProjectFileType, string> spTypesToExtensions = new OneToSetMap<ProjectFileType, string>();

        public ISimpleSignal Changed { get; private set; }

        public SPFileTypeDefinitionExtensionMapping(Lifetime lifetime, IProjectFileTypes fileTypes) 
        {
            Changed = new SimpleSignal(lifetime, GetType().Name + "::Changed");

            fileTypes.View(lifetime, (typeLifetime, fileType) =>
            {
                if (fileType.IsNullOrUnknown())
                    return;

                if (fileType.Is<XmlProjectFileType>() && fileType.Name == "XML")
                {
                    foreach (string extension in xmlExtensions)
                    {
                        spExtensionsToTypes.Add(typeLifetime, extension, fileType);
                        spTypesToExtensions.Add(typeLifetime, fileType, extension);
                    }
                }
            });
        }

        public IEnumerable<ProjectFileType> GetFileTypes(string extension)
        {
            return spExtensionsToTypes[extension];
        }

        public IEnumerable<string> GetExtensions(ProjectFileType projectFileType)
        {
            return spTypesToExtensions[projectFileType];
        }
    }
}