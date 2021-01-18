namespace PLC.Services
{
    public class ClientService : BaseComponentService<IClientBLL, Client, int, ClientDto>, IClientService
    {
        private const string DOCUMENT_TYPE_MISCELLANEOUS = "Miscellaneous";
        private const string DOCUMENT_CATEGORY_CLIENTS = "Clients";
        private const string ROLE_CLIENT_CONSULTANT = "Client Consultant";
        private const string ROLE_CLIENT_USER = "Client User";
        private const string ROLE_CLIENT_MANAGER = "Client Manager";

        #region Constructor
        private readonly IClientBLL _clientBLL;
        private readonly INotificationBLL _notificationBLL;
        private readonly ISignalRConnectionService _signalRService;
        private readonly IRoleBLL _roleBLL;
        private readonly IDocumentTypeBLL _documentTypeBLL;
        private readonly IAgreementBLL _agreementBLL;
        private readonly IClientAgreementBLL _clientAgreementBLL;

        public ClientService(IPLCUnitOfWork unitOfWork, IUserResolverService userResolverService,
            IClientBLL clientBLL, INotificationBLL notificationBLL, ISignalRConnectionService signalRService, IRoleBLL roleBLL,
            IActionItemBLL actionItemBLL, IEntityTypeBLL entityTypeBLL, IDocumentBLL documentBLL, ILocationBLL locationBLL,
            ICommentBLL commentBLL, IContactBLL contactBLL, IUserBLL userBLL, IDocumentTypeBLL documentTypeBLL, IAgreementBLL agreementBLL, IClientAgreementBLL clientAgreementBLL)
            : base(unitOfWork, userResolverService, clientBLL, actionItemBLL, entityTypeBLL, documentBLL, locationBLL, commentBLL, contactBLL, userBLL)
        {
            _clientBLL = clientBLL;
            _notificationBLL = notificationBLL;
            _signalRService = signalRService;
            _roleBLL = roleBLL;
            _documentTypeBLL = documentTypeBLL;
            _agreementBLL = agreementBLL;
            _clientAgreementBLL = clientAgreementBLL;
        }
        #endregion

        #region Custom Code



        public override async Task<int> CreateAsync(ClientDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto), "ClientService>CreateAsync>dto");
            }

            Client model = BLL.MapToModel(dto);
            Document logo = null;
            if (dto.Logo != null)
            {
                logo = await CreateLogoDocument(dto);
            }

            Location primaryLocation = _locationBLL.Create(dto.PrimaryLocation);
            Location billingLocation = _locationBLL.Create(dto.BillingLocation);

            BLL.Create(model);
            int result = await SaveChangesAsync(); //Saving changes to set location & document ids before creating the relationships

            if (result <= 0)
            {
                return 0;
            }

            bool addedDocument = false;
            if (logo != null)
            {
                dto.LogoId = model.LogoId = logo.Id; //needed to create the file
                addedDocument = await AddDocumentRelationshipForLogo(logo, model.Id);
            }

            bool addedPrimary = await AddLocationToEntity(primaryLocation, true, billingLocation == null, model.Id);
            bool addedBilling = await AddLocationToEntity(billingLocation, primaryLocation == null, true, model.Id);

            if (addedDocument || addedPrimary || addedBilling)
            {
                result = await SaveChangesAsync();
            }

            return result > 0 ? model.Id : 0;
        }
        
        private async Task<bool> AddDocumentRelationshipForLogo(Document logo, int clientId)
        {
            if (logo == null)
            {
                return false;
            }

            logo.Relationships.Add(new DocumentRelationship
                                   {
                                       Document = logo,
                                       EntityId = clientId.ToString(),
                                       EntityTypeId = await _entityTypeBLL.GetIdByName(nameof(Client))
                                   });

            return true;
        }

        private async Task<Document> CreateLogoDocument(ClientDto clientDto)
        {
            DocumentType documentType = await GetUnitOfWork().DocumentTypes.GetQueryable().FirstAsync(x => x.Name == DOCUMENT_TYPE_MISCELLANEOUS && x.Category == DOCUMENT_CATEGORY_CLIENTS);

            if (documentType == null)
            {
                throw new NullReferenceException("Document Type was not found");
            }

            clientDto.Logo.DocumentTypeId = documentType.Id;
            return _documentBLL.Create(clientDto.Logo);
        }
        
        public async Task<bool> AddLocationToEntity(Location location, bool isPrimary, bool isBilling, UId entityId)
        {
            if (location == null)
            {
                return false;
            }
            int entityTypeId = await _entityTypeBLL.GetIdByName(type.Name);

            await _locationBLL.AddLocationToEntity(location.Id, isPrimary, isBilling, entityId, entityTypeId);

            return true;
        }

        #endregion
    }
}
