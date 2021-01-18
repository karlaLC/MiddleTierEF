 public string ConvertTemplateStringToPdf(string liquidTemplatePath, IDictionary<string, object> model, string fileName)
        {
            Document doc = new Document();
            string htmlTemplateWithData = DotLiquidHelper.RenderTemplate(liquidTemplatePath, model);

            Section sec = doc.AddSection();
            Paragraph para = sec.AddParagraph();
            string html = htmlTemplateWithData;
            para.AppendHTML(html);

            //Add Footer w/ReviewingEngineer + CreatedDate
            object reviewingEngineerValue = model.Where(x => x.Key == "reviewingEngineer").Select(y => y.Value).FirstOrDefault();
            object createdDateValue = model.Where(x => x.Key == "createdDate").Select(y => y.Value).FirstOrDefault();

            CharacterFormat format = new CharacterFormat(doc);
            format.FontName = "Roboto";
            format.FontSize = 10;
            format.Bold = true;

            HeaderFooter footer = sec.HeadersFooters.Footer;
            Paragraph footerParagraph = footer.AddParagraph();

            TextBox reviewingEngineerTextBox = footerParagraph.AppendTextBox(200, 30);
            reviewingEngineerTextBox.Format.VerticalOrigin = VerticalOrigin.Margin;
            reviewingEngineerTextBox.Format.VerticalPosition = 780;
            reviewingEngineerTextBox.Format.HorizontalOrigin = HorizontalOrigin.Margin;
            reviewingEngineerTextBox.Format.HorizontalPosition = 5;
            reviewingEngineerTextBox.Format.NoLine = true;
            Paragraph parForReviewingEngineer = reviewingEngineerTextBox.Body.AddParagraph();
            parForReviewingEngineer.AppendText($"Reviewing Engineer: {reviewingEngineerValue}").ApplyCharacterFormat(format);

            TextBox createdDateTextBox = footerParagraph.AppendTextBox(200, 30);
            createdDateTextBox.Format.VerticalOrigin = VerticalOrigin.Margin;
            createdDateTextBox.Format.VerticalPosition = 780;
            createdDateTextBox.Format.HorizontalOrigin = HorizontalOrigin.Margin;
            createdDateTextBox.Format.HorizontalPosition = 250;
            createdDateTextBox.Format.NoLine = true;
            Paragraph parForDate = createdDateTextBox.Body.AddParagraph();
            parForDate.AppendText($"Date: {createdDateValue}").ApplyCharacterFormat(format);

            doc.JPEGQuality = 100;

            // FileBasePath is set to "Uploads" in web config
            string fileBasePath = ConfigurationManager.AppSettings["FileBasePath"];
            string rootPath = HttpContext.Current.Server.MapPath($"~/{fileBasePath}/");
            string filePath = Path.Combine(rootPath, $"{fileName}");

            doc.SaveToFile(filePath, FileFormat.PDF);
            doc.Close();

            return filePath;
        }

        //https: //www.e-iceblue.com/Tutorials/Spire.PDF/Spire.PDF-Program-Guide/Conversion/Convert-the-PDF-to-word-HTML-SVG-XPS-and-save-them-to-stream.html
        public Stream GetAnalysisReportPdfStream(string reportFilePath)
        {
            PdfDocument pdf = new PdfDocument();
            pdf.LoadFromFile(reportFilePath); //path to source pdf file

            MemoryStream memoryStream = new MemoryStream();
            pdf.SaveToStream(memoryStream);

            return memoryStream;
        }
