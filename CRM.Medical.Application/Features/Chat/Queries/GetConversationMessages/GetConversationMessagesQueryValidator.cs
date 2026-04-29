using FluentValidation;

namespace CRM.Medical.Application.Features.Chat.Queries.GetConversationMessages;

public sealed class GetConversationMessagesQueryValidator : AbstractValidator<GetConversationMessagesQuery>
{
    public GetConversationMessagesQueryValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.ConversationId).NotEmpty();
        RuleFor(x => x.Take).InclusiveBetween(1, 200);
    }
}
