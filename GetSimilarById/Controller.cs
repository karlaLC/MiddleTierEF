        [PLCApiAuthorize(StandardActions.Read)]
        [Route("datatable/similar-standards/{standardId}"), HttpPost]
        public async Task<HttpResponseMessage> GetSimilarStandardsDataTableByStandardId(DataTableParametersDto<int> parametersDto, int standardId)
        {
            if (parametersDto == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, $"parametersDto is null");
            }

            DataTableResultsDto<object> dataTableResultsDto = await _standardService.GetSimilarStandardsDataTableByStandardId(parametersDto, standardId);

            if (dataTableResultsDto == null)
            {
                return Request.CreateResponse(HttpStatusCode.NoContent);
            }

            return Request.CreateResponse(HttpStatusCode.OK, dataTableResultsDto);
        }
