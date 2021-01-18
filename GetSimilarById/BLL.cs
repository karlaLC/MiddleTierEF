        public async Task<DataTableResultsDto<object>> GetSimilarStandardsDataTableByStandardId(DataTableParametersDto<int> parametersDto, int standardId)
        {
            if (parametersDto == null)
            {
                throw new ArgumentNullException(nameof(parametersDto), nameof(StandardBLL) + "." + nameof(GetSimilarStandardsDataTableByStandardId));
            }

            Standard standard = await GetAsync(standardId);
            string[] titleWords = standard.Title.Replace(",", "").ToLower().Split(' ');

            IQueryable<Standard> queryable = GetQueryable(parametersDto.ShowDeactivated).Where(s => s.Id != standardId && titleWords.Any(w => s.Title.ToLower().Contains(w)));

            return await GetDataTableResultsDtoAsync(queryable, parametersDto);
        }
