        public async Task<ClientDetailsInfoDto> GetClientCountsAsync(int clientId, bool deleted)
        {
            if (clientId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(clientId), "ClientBLL > GetClientCountsAsync > clientId");
            }

            Client client = await GetQueryable(deleted).FirstOrDefaultAsync(x => x.Id == clientId);

            if (client == null)
            {
                throw new ArgumentNullException(nameof(client), "ClientBLL > GetClientCountsAsync > client");
            }

            ClientDetailsInfoDto clientCountsDto = new ClientDetailsInfoDto
            {
                ProjectsCount = client.Projects.Any() ? client.Projects.Count(x => x.Status.Name == PROJECT_STATUS_ACTIVE) : 0,
                OpenApplicationsCount = client.Applications.Any() ? client.Applications.Count(x => x.ApplicationStatus.Status != APPLICATION_STATUS_APPROVED || x.ApplicationStatus.Status != APPLICATION_STATUS_COMPLETE) : 0,
                ProductsCount = client.Applications.Any() ? client.Applications.SelectMany(x => x.ApplicationProducts.Where(y => y.Product.ProductStatus.Status == PRODUCT_STATUS_ACTIVE)).Count() : 0,
                UnpaidInvoicesCount = client.Invoices.Any() ? client.Invoices.Count(x => x.InvoiceStatus.Name == INVOICE_STATUS_OPEN || x.InvoiceStatus.Name == INVOICE_STATUS_OVERDUE || x.InvoiceStatus.Name == INVOICE_STATUS_PARTIAL_PAYMENT || x.InvoiceStatus.Name == INVOICE_STATUS_ON_HOLD) : 0,
                FactoryLocationsCount = client.ManufacturingLocations.Any() ? client.ManufacturingLocations.Count(x => x.ManufacturingLocationStatus.Name == FACTORY_STATUS_VERIFIED) : 0,
                UpcomingInspectionsCount = client.InspectionReports.Any() ? client.InspectionReports.Count(x => x.InspectionTripLineItem.IsApproved && x.InspectionTripLineItem.AssignedDate <= DateTime.Today.AddDays(60)) : 0
            };

            return clientCountsDto;
        }
