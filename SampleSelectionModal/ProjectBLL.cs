public async Task UpdateSampleSelectionNote(int projectId, string sampleNote)
{
    Project project = await GetAsync(projectId);

    project.SampleSelectionNote = sampleNote;

    base.Update(project);
}       




/// <summary>
/// Gets projects and product listed locations corresponding to clients in array and updates its sample selection note
/// Updates the standard's note as well 
/// </summary>
/// <param name="ssNoteDto">Dto holding sample selection note, standard id and array of clients' ids</param>
public void UpdateSampleSelectionNoteByClientIds(ProjectSampleSelectionNoteDto ssNoteDto)
{
    string newStandardSampleSelectionNote = ssNoteDto.SampleSelectionNote;

    Standard existingStandard = UnitOfWork.Standards.GetQueryable().First(x => x.Id == ssNoteDto.StandardId);

    existingStandard.SampleSelectionNote = newStandardSampleSelectionNote;

    foreach (int id in ssNoteDto.ClientIds)
    {
        //get list of projects for the client where the standardId matches the StandardId in the ssNoteDto
        List<Project> projects = GetQueryable().Where(x => x.ClientId == id &&
                                                           (x.ProductCode.Standards.Any(y => y.StandardId == ssNoteDto.StandardId) ||
                                                            x.ListedProducts.Any(z => z.AdditionalRecognitions.Any(a => a.AdditionalRecognition.StandardId == ssNoteDto.StandardId)))).ToList();

        foreach (Project project in projects)
        {
            project.SampleSelectionNote = newStandardSampleSelectionNote;

            //get productListedLocations for project, set SampleSelectionNote
            List<ProductListedLocation> productListedLocationsForProject = UnitOfWork.ProductListedLocations.GetQueryable().Where(x => x.ProjectId == project.Id).ToList();

            foreach (ProductListedLocation location in productListedLocationsForProject)
            {
                location.SampleSelectionNote = newStandardSampleSelectionNote;
            }
        }
    }
}
