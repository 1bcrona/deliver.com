using DeliverCom.Core.Context.Impl;
using DeliverCom.Core.Exception.Impl;
using DeliverCom.Core.Request.Model;
using NUnit.Framework;

namespace DeliverCom.Test.AmbientContext
{
    public class AmbientContextTests
    {
        [Test]
        public void Should_Create_AmbientContext()
        {
            var ambientContext = AmbientExecutionContext.Empty;
            Assert.That(ambientContext, Is.Not.Null);
            Assert.That(ambientContext.IsEmpty, Is.True);
        }

        [Test]
        public void Should_Create_New_AmbientContext()
        {
            var ambientContext = new AmbientExecutionContext();
            ambientContext.SetIdentity(new Identity("email", "companyId"));
            ambientContext.SetTraceId("TraceId");


            var serviceRequest = new ServiceRequest
            {
                Id = Guid.NewGuid().ToString("D"),
                Duration = 1,
                CallParameters = new CallParameters
                    { Return = 1, Inputs = new Dictionary<string, object> { { "Id", 1 } } },
                Exception = new UnknownException(),
                Request = new RequestProperties { Assembly = "Assembly", Method = "Method", Type = "Type" },
                CorrelationId = "CorrelationId",
                ResponseProperties = new ResponseProperties { Assembly = "Assembly", Type = "Type" },
                LocalIpAddress = "1.1.1.1",
                RemoteIpAddress = "1.1.1.1",
                RequestDateEpoch = 0,
                XFF = "XFF",
                RequestEndDateEpoch = 0,
            };

            ambientContext.SetRequest(serviceRequest);
            Assert.Multiple(() =>
            {
                Assert.That(serviceRequest.Id, Is.EqualTo(ambientContext.CurrentRequest.Id));
                Assert.That(ambientContext.CurrentRequest.Request.Assembly, Is.EqualTo("Assembly"));
                Assert.That(ambientContext.CurrentRequest.Request.Method, Is.EqualTo("Method"));
                Assert.That(ambientContext.CurrentRequest.Request.Type, Is.EqualTo("Type"));
                Assert.That(ambientContext.CurrentRequest.CorrelationId, Is.EqualTo("CorrelationId"));
                Assert.That(ambientContext.CurrentRequest.ResponseProperties.Assembly, Is.EqualTo("Assembly"));
                Assert.That(ambientContext.CurrentRequest.ResponseProperties.Type, Is.EqualTo("Type"));
                Assert.That(ambientContext.CurrentRequest.Duration, Is.EqualTo(1));
                Assert.That(ambientContext.CurrentRequest.CallParameters.Return, Is.EqualTo(1));
                Assert.That(ambientContext.CurrentRequest.CallParameters.Inputs["Id"], Is.EqualTo(1));
                Assert.That(ambientContext.CurrentRequest.Exception.Message,
                    Is.EqualTo("Unknown exception is occured"));
                Assert.That(ambientContext.CurrentRequest.LocalIpAddress, Is.EqualTo("1.1.1.1"));
                Assert.That(ambientContext.CurrentRequest.RemoteIpAddress, Is.EqualTo("1.1.1.1"));
                Assert.That(ambientContext.CurrentRequest.RequestDateEpoch, Is.EqualTo(0));
                Assert.That(ambientContext.CurrentRequest.XFF, Is.EqualTo("XFF"));
                Assert.That(ambientContext.CurrentRequest.RequestEndDateEpoch, Is.EqualTo(0));
            });
            Assert.Multiple(() =>
            {
                Assert.That(ambientContext.IsEmpty, Is.False);
                Assert.That(ambientContext, Is.Not.Null);
            });
            Assert.Multiple(() =>
            {
                Assert.That(ambientContext.Id, Is.Not.Null);

                Assert.That(ambientContext.Identity.Email, Is.EqualTo("email"));
                Assert.That(ambientContext.Identity.CompanyId, Is.EqualTo("companyId"));
                Assert.That(ambientContext.CurrentCulture.Name, Is.EqualTo("tr-TR"));
                Assert.That(ambientContext.TraceId, Is.EqualTo("TraceId"));
            });
            var identity = new Identity("email1", "companyId1");
            Assert.That(identity, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(identity, Is.Not.EqualTo(null));
                Assert.That(identity, Is.EqualTo(identity));
                Assert.That(identity, Is.Not.EqualTo(ambientContext.Identity));
            });
            var identity2 = new Identity("email1", "companyId1");
            Assert.That(identity2, Is.EqualTo(identity));

            var hashCode = identity.GetHashCode();
            Assert.That(hashCode, Is.EqualTo(HashCode.Combine(identity.Email, identity.CompanyId)));
        }
    }
}