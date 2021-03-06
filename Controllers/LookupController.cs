using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace binlookup.Controllers
{
    [Route("api/[controller]")]
    public class LookupController : ControllerBase
    {
        private readonly Data.IBINLookupResultRepository c_BINLookupResultRepository;
        private readonly Data.IBINRepository c_BINRepository;
        private readonly Data.ITestHelperRepository c_testHelperRepository;
        private readonly External.IReportingClient c_reportingClient;


        public LookupController(
            [FromServices] Data.IBINLookupResultRepository BINLookupResultRepository,
            [FromServices] Data.IBINRepository BINRepository,
            [FromServices] Data.ITestHelperRepository testHelperRepository,
            [FromServices] External.IReportingClient reportingClient)
        {
            this.c_BINLookupResultRepository = BINLookupResultRepository;
            this.c_BINRepository = BINRepository;
            this.c_testHelperRepository = testHelperRepository;
            this.c_reportingClient = reportingClient;
        }


        [HttpPost]
        public IActionResult Post(
            [FromBody] Models.LookupRequest lookupRequest)
        {
            var _requestId = Request.Headers["X-Request-Id"].ToString();
            var _correlationId = Request.Headers["X-Correlation-Id"].ToString();
            this.c_reportingClient.LogActivity(_requestId, _correlationId, "LookupController.Post", "Start");

            // For testing purposes only
            this.c_testHelperRepository.SeedDatabase();
            this.c_reportingClient.LogActivity(_requestId, _correlationId, "LookupController.Post", "Database seeded for testing purposes");

            // Idempotency check, if already there, return result
            var _previouslyCreatedBINLookupResult = this.c_BINLookupResultRepository.Get(_requestId);

            this.c_reportingClient.LogActivity(_requestId, _correlationId, "LookupController.Post", $"Idempotency check completed");

            if (_previouslyCreatedBINLookupResult != null)
            {
                var _lookupResponse = new binlookup.Models.LookupResponse(
                    _previouslyCreatedBINLookupResult.CardScheme,
                    _previouslyCreatedBINLookupResult.Country,
                    _previouslyCreatedBINLookupResult.Currency);

                this.c_reportingClient.LogActivity(_requestId, _correlationId, "LookupController.Post", $"Idempotency check found a previous BIN lookup result, 200 returned");
                return this.StatusCode(
                    StatusCodes.Status200OK,
                    _lookupResponse);
            }

            var _bins = this.c_BINRepository.GetAllBins();
            if (_bins.Count() == 0)
            {
                this.c_reportingClient.LogActivity(_requestId, _correlationId, "LookupController.Post", "BINs count = 0, 500 returned");
                return this.StatusCode(
                    StatusCodes.Status500InternalServerError);
            }

            var _matchedBin = _bins.SingleOrDefault(
                bin => lookupRequest.CardNumberBin >= bin.Low && lookupRequest.CardNumberBin <= bin.High);

            if (_matchedBin != null)
            {
                var _lookupResponse = new binlookup.Models.LookupResponse(
                    _matchedBin.CardScheme,
                    _matchedBin.Country,
                    _matchedBin.Currency);

                var _BINLookupResult = new Entity.BINLookupResult(
                    _requestId,
                    _correlationId,
                    _matchedBin.CardScheme,
                    _matchedBin.Country,
                    _matchedBin.Currency);
                this.c_BINLookupResultRepository.Save(_BINLookupResult);

                this.c_reportingClient.LogActivity(_requestId, _correlationId, "LookupController.Post", "Matching bin found");
                this.c_reportingClient.LogActivity(_requestId, _correlationId, "LookupController.Post", $"201 returned, response: {_lookupResponse.ToString()}");

                return this.StatusCode(
                    StatusCodes.Status201Created,
                    _lookupResponse);
            }
            else
            {
                this.c_reportingClient.LogActivity(_requestId, _correlationId, "LookupController.Post", "Matching bin not found, 404 returned");
                return this.StatusCode(
                    StatusCodes.Status404NotFound);
            }
        }
    }
}
