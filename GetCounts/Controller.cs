[PLCApiAuthorize(ClientActions.Read)]
[Route("client-counts/{clientId}"), HttpGet]
public async Task<HttpResponseMessage> GetClientCountsAsync(int clientId)
{
    if (clientId <= 0)
    {
        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, $"Client Id: {clientId} was not found");
    }

    ClientDetailsInfoDto clientCounts = await _clientService.GetClientCountsAsync(clientId);

    return CreateResponse(clientCounts);
}
