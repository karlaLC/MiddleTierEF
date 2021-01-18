public async Task<int> CreatePdfForAnalysisReport(int analysisReportId)
        {
            AnalysisReport analysisReport = BLL.Get(analysisReportId);

            string timeStamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
            string fileName = $"{analysisReport.Type} Report-{analysisReportId}";

            Document reportDocument = new Document
            {
                Name = $"{fileName}-{timeStamp}.pdf"
            };

            _documentBLL.Create(reportDocument);
            reportDocument.DisplayPath = _pdfService.ConvertTemplateStringToPdf(GetHtmlFileForReportType(analysisReport.Type), ModelForReportType(analysisReport), reportDocument.Name);
            analysisReport.Document = reportDocument;

            int result = await SaveChangesAsync();

            return (result > 0) ? await AddAnalysisReportPdfToLaserfiche(reportDocument, fileName) : 0;
        }

        private IDictionary<string, object> ModelForReportType(AnalysisReport analysisReport)
        {
            dynamic model = new ExpandoObject();
            model.type = analysisReport.Type;
            model.finalAnalysis = analysisReport.FinalAnalysis;
            model.reviewingEngineer = $"{analysisReport.ReviewingEngineer?.FirstName} {analysisReport.ReviewingEngineer?.LastName}";
            model.createdDate = analysisReport.CreatedDate;
            model.laboratoryName = analysisReport.Laboratory?.LaboratoryName;
            model.laboratoryNumber = analysisReport.Laboratory?.LaboratoryNumber;

            switch (analysisReport.Type)
            {
                case "Pre-Audit Review":
                    model.expirationDate = analysisReport.Laboratory?.ExpiredDate;
                    model.auditor = $"{analysisReport.LaboratoryAuditor?.FirstName} {analysisReport.LaboratoryAuditor?.LastName}";

                    model.paperworkReceived = analysisReport.PaperworkReceived == null ? "N/A" : ((analysisReport.PaperworkReceived == true) ? "Yes" : "No");
                    model.paymentReceived = analysisReport.PaymentReceived == null ? "N/A" : ((analysisReport.PaymentReceived == true) ? "Yes" : "No");
                    model.newApplication = analysisReport.NewApplication == null ? "N/A" : ((analysisReport.NewApplication == true) ? "Yes" : "No");
                    model.renewalApplication = analysisReport.RenewalApplication == null ? "N/A" : ((analysisReport.RenewalApplication == true) ? "Yes" : "No");
                    model.secondaryReviewInLaserfiche = analysisReport.SecondaryReviewInLaserfiche;
                    model.applicationComplete = analysisReport.ApplicationComplete == null ? "N/A" : ((analysisReport.ApplicationComplete == true) ? "Yes" : "No");
                    model.applicationCompleteComments = analysisReport.ApplicationCompleteComments;
                    model.agreementSigned = analysisReport.AgreementSigned == null ? "N/A" : ((analysisReport.AgreementSigned == true) ? "Yes" : "No");
                    model.agreementSignedComments = analysisReport.AgreementSignedComments;

                    break;
                case "Test Report Review":
                    model.testedByLaboratory = analysisReport.TestedByLaboratory?.LaboratoryName;
                    model.iapmoProjectNumber = analysisReport.IapmoProject?.Number;
                    model.productReviewed = analysisReport.ProductReviewed?.Number;
                    model.productDescription = analysisReport.IapmoProject?.ProductCode?.ProductDescription;
                    model.engineerReviewDetail = analysisReport.EngineerReviewDetail;
                    model.comparisonResultsAcceptable = analysisReport.ComparisonResultsAcceptable == null ? "N/A" : ((analysisReport.ComparisonResultsAcceptable == true) ? "Yes" : "No");
                    model.comparisonResultsAcceptableComments = analysisReport.ComparisonResultsAcceptableComments;

                    break;
                case "Post-Audit Review":
                    model.inspectedBy = $"{analysisReport.LaboratoryAuditor?.FirstName} {analysisReport.LaboratoryAuditor?.LastName}";
                    model.inspectionDate = analysisReport.InspectionDate;
                    model.newApplication = analysisReport.NewApplication == null ? "N/A" : ((analysisReport.NewApplication == true) ? "Yes" : "No");
                    model.renewalApplication = analysisReport.RenewalApplication == null ? "N/A" : ((analysisReport.RenewalApplication == true) ? "Yes" : "No");
                    model.nonConformities = analysisReport.NonConformities == null ? "N/A" : ((analysisReport.NonConformities == true) ? "Yes" : "No");
                    model.nonConformitiesComments = analysisReport.NonConformitiesComments;
                    model.issueCertificate = analysisReport.IssueCertificate == null ? "N/A" : ((analysisReport.IssueCertificate == true) ? "Yes" : "No");
                    model.issueCertificateComments = analysisReport.IssueCertificateComments;
                    model.correctiveActionsRequired = analysisReport.CorrectiveActionsRequired == null ? "N/A" : ((analysisReport.CorrectiveActionsRequired == true) ? "Yes" : "No");
                    model.correctiveActionsRequiredComments = analysisReport.CorrectiveActionsRequiredComments;

                    break;
                case "Corrective Actions":
                    model.inspectedBy = $"{analysisReport.LaboratoryAuditor?.FirstName} {analysisReport.LaboratoryAuditor?.LastName}";
                    model.inspectionDate = analysisReport.InspectionDate;
                    model.issueCertificate = analysisReport.IssueCertificate == null ? "N/A" : ((analysisReport.IssueCertificate == true) ? "Yes" : "No");
                    model.issueCertificateComments = analysisReport.IssueCertificateComments;

                    break;
                case "Additional Capability":
                    model.paperworkReceived = analysisReport.PaperworkReceived == null ? "N/A" : ((analysisReport.PaperworkReceived == true) ? "Yes" : "No");
                    model.paymentReceived = analysisReport.PaymentReceived == null ? "N/A" : ((analysisReport.PaymentReceived == true) ? "Yes" : "No");
                    model.specialAuditRequired = analysisReport.SpecialAuditRequired == null ? "N/A" : ((analysisReport.SpecialAuditRequired == true) ? "Yes" : "No");
                    model.specialAuditRequiredComments = analysisReport.SpecialAuditRequiredComments;
                    model.Standards = analysisReport.AnalysisReportStandards?.Select(x => new
                    {
                        title = $"{x.Standard.StandardAgency} {x.Standard.StandardNumber} {x.Standard.StandardYear}{x.Standard.StandardYearSuffix}",
                        okToAddStandard = x.OkToAddStandardToListing ? "Yes" : "No",
                        okToAddStandardComments = x.OkToAddStandardToListingComments
                    });

                    break;
                default:
                    throw new Exception($"Model for report {analysisReport.Type} was not found");

            }

            return model;
        }

        private string GetHtmlFileForReportType(string reportType)
        {
            switch (reportType)
            {
                case PRE_AUDIT_REVIEW_REPORT:
                    return HttpContext.Current.Server.MapPath("~/LiquidTemplates/pre-audit-review-dotliquid-template.html");

                case ADDITIONAL_CAPABILITY_REPORT:
                    return HttpContext.Current.Server.MapPath("~/LiquidTemplates/additional-capability-dotliquid-template.html");

                case CORRECTIVE_ACTIONS_REPORT:
                    return HttpContext.Current.Server.MapPath("~/LiquidTemplates/corrective-actions-dotliquid-template.html");

                case TEST_REPORT_REVIEW_REPORT:
                    return HttpContext.Current.Server.MapPath("~/LiquidTemplates/test-report-review-dotliquid-template.html");

                case POST_AUDIT_REVIEW_REPORT:
                    return HttpContext.Current.Server.MapPath("~/LiquidTemplates/post-audit-review-dotliquid-template.html");

                default:
                    throw new Exception($"Template for type {reportType} was not found");
            }
        }

        private Task<int> AddAnalysisReportPdfToLaserfiche(Document reportDocument, string fileName)
        {
            //Get stream 
            Stream analysisReportStream = _pdfService.GetAnalysisReportPdfStream(reportDocument.DisplayPath);

            //Add to Laserfiche
            AddDocumentParamsDto addDocumentParams = new AddDocumentParamsDto
            {
                Name = reportDocument.Name,
                FolderPath = $"\\LaboratoryAnalysisReports\\{fileName}"
            };

            return _laserficheQueueService.AddDocument(reportDocument.Id, addDocumentParams, analysisReportStream, reportDocument.Name);
        }
