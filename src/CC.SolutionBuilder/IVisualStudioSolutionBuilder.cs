using CC.SolutionsAnalyzer;

namespace CC.SolutionBuilder;

public interface IVisualStudioSolutionBuilder
{
    /// <summary>
    /// Parses project references for all projects included in <paramref name="projectFilePaths"/>.
    /// </summary>
    /// <param name="projectFilePaths">List of project(.csproj) file paths.</param>
    /// <returns>Distinct list of referenced projects, including those in <paramref name="projectFilePaths"/>.</returns>
    IEnumerable<VisualStudioProject> GetProjects(string[] projectFilePaths);

    void Create(string outputSolutionPath, string[] projectFilePaths);
    void Create(Stream stream, IEnumerable<VisualStudioProject> projects);
}