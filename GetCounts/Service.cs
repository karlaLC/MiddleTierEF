public async Task<ClientDetailsInfoDto> GetClientCountsAsync(int clientId, bool deleted = false)
{
    if (clientId <= 0)
    {
        throw new ArgumentOutOfRangeException(nameof(clientId), "ClientService > GetClientCountsAsync > clientId");
    }

    return await _clientBLL.GetClientCountsAsync(clientId, deleted);
}
