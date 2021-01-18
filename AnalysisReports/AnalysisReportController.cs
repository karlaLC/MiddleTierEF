        [PLCApiAuthorize(AnalysisReportActions.Create)]
        [Route("pdf-analysis-report/{analysisReportId}"), HttpPost]
        public async Task<HttpResponseMessage> CreatePdfForAnalysisReport(int analysisReportId)
        {
            int pdfLaserficheQueueId = await Service.CreatePdfForAnalysisReport(analysisReportId);

            return Request.CreateResponse(HttpStatusCode.OK, pdfLaserficheQueueId);
        }
