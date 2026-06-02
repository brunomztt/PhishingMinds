using Dapper;
using Microsoft.AspNetCore.Mvc;
using PhishingMinds.Server.Data;

namespace PhishingMinds.Server.Controllers
{
    [ApiController]
    [Route("track")]
    public class TrackingController : ControllerBase
    {
        private readonly DbConnectionFactory _dbFactory;

        public TrackingController(DbConnectionFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        [HttpGet]
        public IActionResult Track([FromQuery] int idTarget)
        {
            try
            {
                using var db = _dbFactory.CreateConnection();

                var sql = @"
                    UPDATE PhishingCampaignTarget
                    SET LinkClicked = 1
                    WHERE IdTarget = @IdTarget
                ";

                db.Execute(sql, new { IdTarget = idTarget });

                return Redirect("https://www.microsoft.com");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}