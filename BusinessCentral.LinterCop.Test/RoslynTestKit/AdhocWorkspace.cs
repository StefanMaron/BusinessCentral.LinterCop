using Microsoft.Dynamics.Nav.CodeAnalysis.Text;
using Microsoft.Dynamics.Nav.CodeAnalysis.Workspaces;
using Microsoft.Dynamics.Nav.CodeAnalysis.Workspaces.Host;

namespace BusinessCentral.LinterCop.Test.RoslynTestKit;

/// <summary>
/// A workspace that allows full manipulation of projects and documents,
/// but does not persist changes.
/// </summary>
public sealed class AdhocWorkspace(HostServices host, string workspaceKind = "Custom") : Workspace(host, workspaceKind)
{
    public AdhocWorkspace()
        : this(HostServices.DefaultHost)
    {
    }

    public override bool CanApplyChange(ApplyChangesKind feature)
    {
        // all kinds supported.
        return true;
    }

    /// <summary>
    /// Returns true, signifiying that you can call the open and close document APIs to add the document into the open document list.
    /// </summary>
    public override bool CanOpenDocuments => true;

    /// <summary>
    /// Clears all projects and documents from the workspace.
    /// </summary>
    public new void ClearSolution()
        => base.ClearSolution();

    /// <summary>
    /// Adds an entire solution to the workspace, replacing any existing solution.
    /// </summary>
    public Solution AddSolution(SolutionInfo solutionInfo)
    {
        if (solutionInfo is null)
        {
            throw new ArgumentNullException(nameof(solutionInfo));
        }

        OnSolutionAdded(solutionInfo);

        //this.UpdateReferencesAfterAdd();

        return CurrentSolution;
    }

    /// <summary>
    /// Adds a project to the workspace. All previous projects remain intact.
    /// </summary>
    public Project AddProject(string name, string language)
    {
        var info = ProjectInfo.Create(ProjectId.CreateNewId(), VersionStamp.Create(), name, name, language);
        return AddProject(info);
    }

    /// <summary>
    /// Adds a project to the workspace. All previous projects remain intact.
    /// </summary>
    public Project AddProject(ProjectInfo projectInfo)
    {
        if (projectInfo is null)
        {
            throw new ArgumentNullException(nameof(projectInfo));
        }

        OnProjectAdded(projectInfo);

        //this.UpdateReferencesAfterAdd();

        return CurrentSolution.GetProject(projectInfo.Id);
    }

    /// <summary>
    /// Adds multiple projects to the workspace at once. All existing projects remain intact.
    /// </summary>
    /// <param name="projectInfos"></param>
    public void AddProjects(IEnumerable<ProjectInfo> projectInfos)
    {
        if (projectInfos is null)
        {
            throw new ArgumentNullException(nameof(projectInfos));
        }

        foreach (var info in projectInfos)
        {
            OnProjectAdded(info);
        }

        //this.UpdateReferencesAfterAdd();
    }

    /// <summary>
    /// Adds a document to the workspace.
    /// </summary>
    public Document AddDocument(ProjectId projectId, string name, SourceText text)
    {
        if (projectId is null)
        {
            throw new ArgumentNullException(nameof(projectId));
        }

        if (name is null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        if (text is null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        var id = DocumentId.CreateNewId(projectId);
        var loader = TextLoader.From(TextAndVersion.Create(text, VersionStamp.Create()));

        return AddDocument(DocumentInfo.Create(id, name, loader: loader));
    }

    /// <summary>
    /// Adds a document to the workspace.
    /// </summary>
    public Document AddDocument(DocumentInfo documentInfo)
    {
        if (documentInfo is null)
        {
            throw new ArgumentNullException(nameof(documentInfo));
        }

        OnDocumentAdded(documentInfo);

        return CurrentSolution.GetDocument(documentInfo.Id);
    }

    /// <summary>
    /// Puts the specified document into the open state.
    /// </summary>
    public override void OpenDocument(DocumentId documentId, bool activate = true)
    {
        var doc = CurrentSolution.GetDocument(documentId);
        if (doc is not null)
        {
            var text = doc.GetTextAsync(CancellationToken.None).Result;
            OnDocumentOpened(documentId, text.Container, activate);
        }
    }

    /// <summary>
    /// Puts the specified document into the closed state.
    /// </summary>
    public override void CloseDocument(DocumentId documentId)
    {
        var doc = CurrentSolution.GetDocument(documentId);
        if (doc is not null)
        {
            var text = doc.GetTextAsync(CancellationToken.None).Result;
            var version = doc.GetTextVersionAsync(CancellationToken.None).Result;
            var loader = TextLoader.From(TextAndVersion.Create(text, version, doc.FilePath));
            OnDocumentClosed(documentId, loader);
        }
    }

    /// <summary>
    /// Puts the specified additional document into the open state.
    /// </summary>
    public override void OpenAdditionalDocument(DocumentId documentId, bool activate = true)
    {
        var doc = CurrentSolution.GetAdditionalDocument(documentId);
        if (doc is not null)
        {
            var text = doc.GetTextAsync(CancellationToken.None).Result;
            OnAdditionalDocumentOpened(documentId, text.Container, activate);
        }
    }

    /// <summary>
    /// Puts the specified additional document into the closed state
    /// </summary>
    public override void CloseAdditionalDocument(DocumentId documentId)
    {
        var doc = CurrentSolution.GetAdditionalDocument(documentId);
        if (doc is not null)
        {
            var text = doc.GetTextAsync(CancellationToken.None).Result;
            var version = doc.GetTextVersionAsync(CancellationToken.None).Result;
            var loader = TextLoader.From(TextAndVersion.Create(text, version, doc.FilePath));
            OnAdditionalDocumentClosed(documentId, loader);
        }
    }

    /// <summary>
    /// Puts the specified analyzer config document into the open state.
    /// </summary>
    //public override void OpenAnalyzerConfigDocument(DocumentId documentId, bool activate = true)
    //{
    //    var doc = this.CurrentSolution.GetAnalyzerConfigDocument(documentId);
    //    if (doc is not null)
    //    {
    //        var text = doc.GetTextSynchronously(CancellationToken.None);
    //        this.OnAnalyzerConfigDocumentOpened(documentId, text.Container, activate);
    //    }
    //}

    /// <summary>
    /// Puts the specified analyzer config document into the closed state
    /// </summary>
    //public override void CloseAnalyzerConfigDocument(DocumentId documentId)
    //{
    //    var doc = this.CurrentSolution.GetAnalyzerConfigDocument(documentId);
    //    if (doc is not null)
    //    {
    //        var text = doc.GetTextSynchronously(CancellationToken.None);
    //        var version = doc.GetTextVersionSynchronously(CancellationToken.None);
    //        var loader = TextLoader.From(TextAndVersion.Create(text, version, doc.FilePath));
    //        this.OnAnalyzerConfigDocumentClosed(documentId, loader);
    //    }
    //}
}