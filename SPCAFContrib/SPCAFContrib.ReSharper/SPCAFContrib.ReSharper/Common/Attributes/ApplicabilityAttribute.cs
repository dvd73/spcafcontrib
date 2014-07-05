using System;

namespace SPCAFContrib.ReSharper.Common.Attributes
{
    /// <summary>
    /// Specify rule applicability depends on IDE project type 
    /// </summary>
    public class ApplicabilityAttribute : Attribute
    {
        public IDEProjectType ProjectType { get; private set; }

        public bool ForFarmSolution
        {
            get
            {
                return (ProjectType & IDEProjectType.SP2010FarmSolution) == IDEProjectType.SP2010FarmSolution ||
                       (ProjectType & IDEProjectType.SP2013FarmSolution) == IDEProjectType.SP2013FarmSolution;
            }
        }

        public bool For2010
        {
            get
            {
                return (ProjectType & IDEProjectType.SP2010FarmSolution) == IDEProjectType.SP2010FarmSolution ||
                       (ProjectType & IDEProjectType.SPSandbox) == IDEProjectType.SPSandbox;
            }
        }

        public bool For2013
        {
            get
            {
                return (ProjectType & IDEProjectType.SP2013FarmSolution) == IDEProjectType.SP2013FarmSolution ||
                       (ProjectType & IDEProjectType.SP2013App) == IDEProjectType.SP2013App;
            }
        }

        public ApplicabilityAttribute(IDEProjectType projectType)
        {
            this.ProjectType = projectType;
        }
    }
}
