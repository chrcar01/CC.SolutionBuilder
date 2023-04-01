using CC.SolutionsAnalyzer;

namespace CC.SolutionBuilder
{
    public class VisualStudioSolutionBuilder : IVisualStudioSolutionBuilder
    {
        private readonly ISolutionParser _parser;

        public VisualStudioSolutionBuilder(ISolutionParser parser)
        {
            _parser = parser ?? throw new ArgumentNullException(nameof(parser));
        }

        /// <summary>
        /// Parses project references for all projects included in <paramref name="projectFilePaths"/>.
        /// </summary>
        /// <param name="projectFilePaths">List of project(.csproj) file paths.</param>
        /// <returns>Distinct list of referenced projects, including those in <paramref name="projectFilePaths"/>.</returns>
        public IEnumerable<VisualStudioProject> GetProjects(string[] projectFilePaths)
        {
            var solutionProjects = new List<VisualStudioProject>();
            foreach (var projectFilePath in projectFilePaths)
            {
                var project = _parser.GetProject(projectFilePath);
                solutionProjects.Add(project);
                Walk(project, solutionProjects);
            }

            return solutionProjects;
        }

        public void Create(string outputSolutionPath, string[] projectFilePaths)
        {

            var solutionProjects = GetProjects(projectFilePaths);

            using var fileStream = File.OpenWrite(outputSolutionPath);
            Create(fileStream, solutionProjects);
        }

        public void Create(Stream stream, IEnumerable<VisualStudioProject> projects)
        {
            using var writer = new StreamWriter(stream);
            writer.WriteLine("Microsoft Visual Studio Solution File, Format Version 12.00");
            writer.WriteLine("# Visual Studio Version 17");
            writer.WriteLine("VisualStudioVersion = 17.2.32602.215");
            writer.WriteLine("MinimumVisualStudioVersion = 10.0");
            foreach (var project in projects)
            {
                writer.WriteLine($"Project(\"{project.ProjectGuid}\") = \"{project.Name}\", \"{project.FilePath}\", \"{project.ProjectGuid}\"");
                writer.WriteLine("EndProject");
            }
        }

        private void Walk(VisualStudioProject project, List<VisualStudioProject> solutionProjects)
        {
            if (!solutionProjects.Any(sp => sp.Name.Equals(project.Name)))
            {
                solutionProjects.Add(project);
            }

            if (!project.ReferencedProjects.Any())
            {
                return;
            }

            if (project.ReferencedProjects.All(rf => solutionProjects.Any(sp => sp.Name.Equals(rf.Name))))
            {
                return;
            }

            foreach (var rp in project.ReferencedProjects)
            {
                var rpBasePath = new FileInfo(rp.FilePath).DirectoryName;
                var p = _parser.GetProject(rp.FilePath);
                Walk(p, solutionProjects);
            }

        }
    }
}