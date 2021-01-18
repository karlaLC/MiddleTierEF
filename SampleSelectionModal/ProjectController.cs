[PLCApiAuthorize(ProjectActions.UpdateSampleSelectionNote)]
[Route("update-sample-notes/{projectId}"), HttpPut]
public async Task<HttpResponseMessage> UpdateSampleSelectionNote(int projectId, [FromBody]string sampleNote)
{
    int result = await _projectService.UpdateSampleSelectionNote(projectId, sampleNote);

    if (result > 0)
    {
        return Request.CreateResponse(HttpStatusCode.OK);
    }

    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Unable to update project's sample selection note");
}



[PLCApiAuthorize(ProjectActions.UpdateSampleSelectionNote)]
[Route("update-sample-notes-selected-only"), HttpPut]
public async Task<HttpResponseMessage> UpdateSelectedItemsSampleSelectionNote(ProjectSampleSelectionNoteDto itemsSampleNoteDto)
{
    int result = await _projectService.UpdateSelectedItemsSampleSelectionNote(itemsSampleNoteDto);

    if (result > 0)
    {
        return Request.CreateResponse(HttpStatusCode.OK);
    }

    return Request.CreateErrorResponse(HttpStatusCode.NoContent, "Unable to update Sample Selection Notes");
}
