  public async Task<int> UpdateSampleSelectionNote(int projectId, string sampleNote)
  {
      if (projectId < 1)
      {
          throw new ArgumentNullException(nameof(projectId), "projectService>UpdateSampleSelectionNote>projectId");
      }

      await _projectBLL.UpdateSampleSelectionNote(projectId, sampleNote);

      int result = await SaveChangesAsync();
      return (result > 0) ? projectId : 0;
  }

  public async Task<int> UpdateSelectedItemsSampleSelectionNote(ProjectSampleSelectionNoteDto itemsSampleNoteDto)
  {
      if (itemsSampleNoteDto == null)
      {
          throw new ArgumentNullException(nameof(itemsSampleNoteDto), "projectService>UpdateSelectedItemsSampleSelectionNote>dto");
      }

      if (itemsSampleNoteDto.SampleSelectionNote == null)
      {
          throw new ArgumentNullException(nameof(itemsSampleNoteDto), "projectService>UpdateSelectedItemsSampleSelectionNote>sampleSelectionNote");
      }

      string newStandardSampleSelectionNote = itemsSampleNoteDto.SampleSelectionNote;

      Standard currentStandard = _standardBLL.Get(itemsSampleNoteDto.StandardId);
      currentStandard.SampleSelectionNote = newStandardSampleSelectionNote;

      if (itemsSampleNoteDto.ProjectIds.Any())
      {
          foreach (int projectId in itemsSampleNoteDto.ProjectIds)
          {
              await _projectBLL.UpdateSampleSelectionNote(projectId, newStandardSampleSelectionNote);
          }
      }

      if (itemsSampleNoteDto.ProductListedLocationsIds.Any())
      {
          foreach (int locationId in itemsSampleNoteDto.ProductListedLocationsIds)
          {
              await _productListedLocationBLL.UpdateSampleSelectionNote(locationId, newStandardSampleSelectionNote);
          }
      }

      if (itemsSampleNoteDto.ClientIds.Any())
      {
          //if any clients are selected, update all its related projects and productListedLocations' notes
          _projectBLL.UpdateSampleSelectionNoteByClientIds(itemsSampleNoteDto);
      }

      int result = await SaveChangesAsync();

      return result > 0 ? 1 : 0;
  }
