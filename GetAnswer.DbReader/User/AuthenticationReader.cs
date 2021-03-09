using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GetAnswer.DbReader.Dtos.User.Authentication;
using GetAnswer.GraphDbSchema;
using Gremlin.Net.Process.Traversal;

namespace GetAnswer.DbReader.User
{
    public class AuthenticationReader
    {
        private GraphTraversalSource _g;

        public AuthenticationReader(GraphTraversalSource g)
        {
            _g = g;
        }


        public async Task<InfoForLoginPostHandler> GetInfoForLoginPostHandler(string email)
        {
            IDictionary<string, object> resultDictionary = await _g
                .V()
                .HasLabel(UserVertex.VERTEX_LABEL)
                .Has(UserVertex.EMAIL, email.ToLower())
                .Project<object>(
                    nameof(InfoForLoginPostHandler.UserId),
                    nameof(InfoForLoginPostHandler.HashedPassword),
                    nameof(InfoForLoginPostHandler.FirstName),
                    nameof(InfoForLoginPostHandler.AuthTicketInfoLastChangeUtcTime))
                    .By(T.Id)
                    .By(UserVertex.HASHED_PASSWORD)
                    .By(UserVertex.FIRST_NAME)
                    .By(UserVertex.AUTH_TICKET_INFO_LAST_CHANGE_UTC_TIME)
                .Promise(t => t.Next());

            InfoForLoginPostHandler infoForLoginPostHandler;

            if (resultDictionary is null)
            {
                infoForLoginPostHandler = null;
            }
            else
            {
                infoForLoginPostHandler = new InfoForLoginPostHandler
                {
                    UserId = resultDictionary[nameof(InfoForLoginPostHandler.UserId)].ToString(),
                    HashedPassword = (string)resultDictionary[nameof(InfoForLoginPostHandler.HashedPassword)],
                    FirstName = (string)resultDictionary[nameof(InfoForLoginPostHandler.FirstName)],
                    AuthTicketInfoLastChangeUtcTime = ((DateTimeOffset)resultDictionary[nameof(InfoForLoginPostHandler.AuthTicketInfoLastChangeUtcTime)]).DateTime
                };
            }

            return infoForLoginPostHandler;
        }

        public async Task<DateTime> GetAuthTicketInfoLastChangeUtcTimeAsync(string userId)
        {
            DateTimeOffset authTicketInfoLastChangeUtcDateTimeOffset = await _g
                .V(userId)
                .Values<DateTimeOffset>(UserVertex.AUTH_TICKET_INFO_LAST_CHANGE_UTC_TIME)
                .Promise(t => t.Next());

            return authTicketInfoLastChangeUtcDateTimeOffset.DateTime;
        }
    }
}