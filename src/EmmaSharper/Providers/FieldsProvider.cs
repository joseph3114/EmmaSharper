using System.Collections.Generic;
using System.Threading.Tasks;
using EmmaSharper.Internals;

namespace EmmaSharper
{
    /// <inheritdoc/>
    internal class FieldsProvider : IEmmaFieldsProvider
    {
        private readonly IEmmaApiAdapter apiAdapter;

        /// <inheritdoc cref="object.Object"/>
        public FieldsProvider(IEmmaApiAdapter apiAdapter)
        {
            this.apiAdapter = apiAdapter;
        }

        /// <inheritdoc/>
        public async Task<int> ListFieldsCount(bool includeDeleted = false)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/fields"
            };
            request.AddParameter("count", "true");

            if (includeDeleted)
            {
                request.AddParameter("deleted", includeDeleted);
            }

            return await apiAdapter.MakeRequest<int>(request);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Field>> ListFields(bool includeDeleted = false, uint? start = null, uint? end = null)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/fields"
            };

            if (includeDeleted)
            {
                request.AddParameter("deleted", includeDeleted);
            }

            return await apiAdapter.MakeRequest<List<Field>>(request, start, end);
        }

        /// <inheritdoc/>
        public async Task<Field> GetField(string fieldId, bool includeDeleted = false)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/fields/{fieldId}"
            };
            request.AddUrlSegment("fieldId", fieldId);

            if (includeDeleted)
            {
                request.AddParameter("deleted", includeDeleted);
            }

            return await apiAdapter.MakeRequest<Field>(request);
        }

        /// <inheritdoc/>
        public async Task<int> CreateField(CreateField field)
        {
            EmmaRequest request = new EmmaRequest(Method.POST)
            {
                Resource = "/{accountId}/fields",
            };
            request.AddJsonBody(field);

            return await apiAdapter.MakeRequest<int>(request);
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteField(string fieldId)
        {
            EmmaRequest request = new EmmaRequest(Method.DELETE)
            {
                Resource = "/{accountId}/fields/{fieldId}"
            };
            request.AddUrlSegment("fieldId", fieldId);

            return await apiAdapter.MakeRequest<bool>(request);
        }

        /// <inheritdoc/>
        public async Task<bool> ClearField(string fieldId)
        {
            EmmaRequest request = new EmmaRequest(Method.POST)
            {
                Resource = "/{accountId}/fields/{fieldId}/clear"
            };
            request.AddUrlSegment("fieldId", fieldId);

            return await apiAdapter.MakeRequest<bool>(request);
        }

        /// <inheritdoc/>
        public async Task<int> UpdateField(string fieldId, UpdateField field)
        {
            EmmaRequest request = new EmmaRequest(Method.PUT)
            {
                Resource = "/{accountId}/fields/{fieldId}"
            };
            request.AddUrlSegment("fieldId", fieldId);
            request.AddJsonBody(field);

            return await apiAdapter.MakeRequest<int>(request);
        }
    }
}
