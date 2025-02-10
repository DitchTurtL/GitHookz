using GitHookz.Data;

namespace GitHookz.Services;

public interface IDatabaseService
{
    void AddProject(ProjectData projectData);
    ProjectData? GetProjectById(int projectId);
    ProjectData? GetProjectByRepositoryUrl(string repositoryUrl);
    IEnumerable<ProjectData> GetAllProjects();
    IEnumerable<ProjectData> GetProjectsByOwnerId(string ownerId);
    void UpdateProject(ProjectData projectData);
    

    int AddUser(UserData userData);
    UserData? GetUserById(int userId);
    UserData GetOrAddUser(UserData userData);
}