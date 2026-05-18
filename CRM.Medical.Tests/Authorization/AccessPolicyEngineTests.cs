using System.Text.Json;
using CRM.Medical.Application.Authorization;
using CRM.Medical.Domain.Entities;
using CRM.Medical.Infrastructure.Authorization;

namespace CRM.Medical.Tests.Authorization;

public sealed class AccessPolicyEngineTests
{
    [Fact]
    public void Parser_Should_Parse_All_Node()
    {
        var json = JsonDocument.Parse("""{"all":[{"field":"doctorId","operator":"eq","value":"@CurrentUserId"}]}""");
        var parser = new AccessPolicyConditionParser();

        var node = parser.Parse(json);

        var group = Assert.IsType<AccessConditionGroup>(node);
        Assert.Single(group.All);
    }

    [Fact]
    public void RuntimeTokenResolver_Should_Resolve_CurrentUserId()
    {
        var resolver = new AccessPolicyRuntimeTokenResolver();
        var subject = new CurrentSubjectContext("u1", ["Doctor"], ["r1"], "Cairo", "admin", true);

        var value = resolver.Resolve("@CurrentUserId", subject);

        Assert.Equal("u1", value);
    }

    [Fact]
    public void RuntimeTokenResolver_Should_Resolve_CurrentUserEmail()
    {
        var resolver = new AccessPolicyRuntimeTokenResolver();
        var subject = new CurrentSubjectContext("u1", ["Doctor"], ["r1"], null, null, true, "u1@test.com");

        var value = resolver.Resolve("@CurrentUserEmail", subject);

        Assert.Equal("u1@test.com", value);
    }

    [Fact]
    public void Validator_Should_Reject_Unknown_Field()
    {
        var validator = new AccessPolicyConditionValidator();
        var condition = new AccessConditionPredicate("missingField", "eq", JsonDocument.Parse("\"x\"").RootElement);

        var result = validator.Validate<TestRequest>(condition);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Compiler_Should_Filter_Doctor_Own_Requests()
    {
        var compiler = new AccessPolicyExpressionCompiler(new AccessPolicyRuntimeTokenResolver());
        var subject = new CurrentSubjectContext("doctor-1", [], [], null, null, true);
        var condition = new AccessConditionPredicate("doctorId", "eq", JsonDocument.Parse("\"@CurrentUserId\"").RootElement);
        var fn = compiler.Compile<TestRequest>(condition, subject).Compile();

        var own = new TestRequest { DoctorId = "doctor-1" };
        var other = new TestRequest { DoctorId = "doctor-2" };

        Assert.True(fn(own));
        Assert.False(fn(other));
    }

    [Fact]
    public void Compiler_Should_Filter_Conversation_Participant_CollectionAny()
    {
        var compiler = new AccessPolicyExpressionCompiler(new AccessPolicyRuntimeTokenResolver());
        var subject = new CurrentSubjectContext("u1", [], [], null, null, true);
        var condition = new AccessConditionCollectionAny(
            "participants",
            new AccessConditionPredicate("userId", "eq", JsonDocument.Parse("\"@CurrentUserId\"").RootElement));

        var fn = compiler.Compile<Conversation>(condition, subject).Compile();
        var conversation = new Conversation
        {
            Participants =
            [
                new ConversationParticipant { UserId = "u2" },
                new ConversationParticipant { UserId = "u1" }
            ]
        };

        Assert.True(fn(conversation));
    }
}
